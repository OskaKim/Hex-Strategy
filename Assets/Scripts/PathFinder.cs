using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tile;
using System.Linq;
using UniRx;
using System;

public class PathFinderManager : MonoBehaviour {
    private static PathFinderManager instance;
    private static PathFinderManager GetInstance() {
        if (!instance) {
            var gameObject = new GameObject("PathFinderManager");
            instance = gameObject.AddComponent<PathFinderManager>();
        }
        return instance;
    }

    public event Action<bool/*isPlayer*/, PathFinder> StartPathFindingEvent;
    public event Action<bool/*isPlayer*/, PathFinder> FinishPathFindingEvent;

    private readonly List<PathFinder> workingPathFinders = new List<PathFinder>();

    // NOTE : ������ �н� ���ε��� ���ÿ� ������ �� ���� ������ �ϳ��� ����
    private readonly PathFinder playerPathFinder = new PathFinder();

    private PathFinder GetNewPathFinder() {
        PathFinder newPathFinder = new PathFinder();
        workingPathFinders.Add(newPathFinder);
        return newPathFinder;
    }

    private void DeletePathFinder(PathFinder pathFinder) {
        workingPathFinders.Remove(pathFinder);
        Debug.Log(workingPathFinders.Count);
    }

    private bool isExistSamePathFinder(bool isPlayer, Tile.Tile startTile, Tile.Tile destinationTile) {
        if (isPlayer) {
            return playerPathFinder.StartTile == startTile && playerPathFinder.DestinationTile == destinationTile;
        }
        return workingPathFinders.Any(x => {
            return x.StartTile == startTile && x.DestinationTile == destinationTile;
        });
    }

    private IEnumerator PathFindCoroutine(Tile.Tile startTile, Tile.Tile destinationTile, Action<List<Tile.Tile>> callback, bool isPlayer, PathFinder pathFinder) {
        pathFinder.FindPath(startTile, destinationTile);
        while(true) {
            if (pathFinder.IsFinish) {
                if (callback != null) callback(pathFinder.Path);
                FinishPathFindingEvent(isPlayer, pathFinder);
                if (!isPlayer) DeletePathFinder(pathFinder);
                yield break;
            }

            yield return null;
        }
    }
    // NOTE : �н� ���ε� ����
    public static void StartPathFinding(bool isPlayer, Tile.Tile startTile, Tile.Tile destinationTile, Action<List<Tile.Tile>> callback) {
        var instance = GetInstance();

        // NOTE : �̹� ���� �������� �н� ���ε��� �������̶�� ����
        if (instance.isExistSamePathFinder(isPlayer, startTile, destinationTile)) {
            Debug.Log("path finding ignored");
            return;
        }

        var pathFinder = isPlayer ? instance.playerPathFinder : instance.GetNewPathFinder();
        instance.StartCoroutine(instance.PathFindCoroutine(startTile, destinationTile, callback, isPlayer, pathFinder));
        instance.StartPathFindingEvent(isPlayer, pathFinder);
    }
}

// NOTE : �н� ���δ�. PathFinderManager�� ���� ����
public class PathFinder
{
    private class Node
    {
        public Tile.Tile tile;
        public Node parent;
        public int f;
        public Node(Tile.Tile tile, Node parent, int f)
        {
            this.tile = tile;
            this.parent = parent;
            this.f = f;
        }
    }
    public Tile.Tile StartTile { get; private set; }
    public Tile.Tile DestinationTile { get; private set; }
    public bool IsFinish { get => isFinish; }
    private bool isFinish = false;
    public List<Tile.Tile> Path { get => path; }

    private readonly List<Tile.Tile> path = new List<Tile.Tile>();
    //NOTE : �ִ� ��θ� �м��ϱ� ���� ���°����� ��� ����
    private readonly List<Node> openList = new List<Node>();
    //NOTE : ó���� �Ϸ�� ��带 ��Ƶ�
    private readonly List<Node> closeList = new List<Node>();

    private Node destinationNode;
    private Tile.Tile destinationTile;
    private int calculCount = 0;
    // NOTE : ���� �ִ� ��. �̸� �Ѿ�� ���Ž�� ���з� ������
    private readonly static int MaxCalculCount = 1500;
    // NOTE : ���� �н� ������ �ڽ�Ʈ�� ���� ������ �� �����ӿ� �ش� Ƚ����ŭ�� ó��
    private readonly static int MaxNearPathLoopInOneCicle = 10;

    private void Clear() {
        isFinish = false;
        path.Clear();
        openList.Clear();
        closeList.Clear();
        destinationNode = null;
        calculCount = 0;
    }

    public void FindPath(Tile.Tile startTile, Tile.Tile destinationTile) {
        Clear();
        StartTile = startTile;
        DestinationTile = destinationTile;

        if (startTile == destinationTile || !startTile.IsMovable || !destinationTile.IsMovable) return;

        var currentCloseNode = new Node(startTile, null, 0);
        this.destinationTile = destinationTile;

        Observable.FromCoroutine<bool>(observer => FindNearPathLoopCoroutine(currentCloseNode, observer))
            .Subscribe(x => {
                if (x) {
                    Debug.Log("finish finding path");
                    AddToPath(path, destinationNode);
                    isFinish = true;
                }
            });
    }

    private void AddToPath(List<Tile.Tile> path, Node node)
    {
        if (node == null) return;

        if (node.parent != null)
        {
            AddToPath(path, node.parent);
        }

        path.Add(node.tile);
    }
    
    int count;

    private IEnumerator FindNearPathLoopCoroutine(Node firstCloseNode, IObserver<bool> observer) {

        Node currentCloseNode = firstCloseNode;

        for (int i = 0; ; ++i) {
            currentCloseNode = FindNearPathFrom(currentCloseNode);

            // NOTE : Ž�� ����
            if (currentCloseNode == null) {
                observer.OnNext(true);
                yield break;
            }

            // NOTE : �� ������ �� ó�� Ƚ���� �ʰ��ϸ� ���� ���������� ����
            if (i >= MaxNearPathLoopInOneCicle) {
                i = 0;
                yield return null;
            }
        }
    }

    private Node FindNearPathFrom(Node currentCloseNode)
    {
        closeList.Add(currentCloseNode);

        var nearTiles = TileHelper.GetNearTiles(currentCloseNode.tile);

        foreach (var currentTile in nearTiles)
        {
            if (!currentTile.IsMovable) continue;

            var parent = currentCloseNode;
            // NOTE : ���� ��忡�� �ش� �������� ���� �ҿ� ���
            var g = parent.f + currentTile.MoveCost;
            // NOTE : �޸���ƽ ���� ��.�ش� ��忡�� ���� ������������ ���� ��(�Ÿ�)
            var h = Mathf.Abs(destinationTile.IndexPair.X - currentTile.IndexPair.X) + Mathf.Abs(destinationTile.IndexPair.Y - currentTile.IndexPair.Y);
            var f = g + h;

            // NOTE : close list�� ���� ��쿣 open list�� �߰� ����
            if (closeList.Any(x => x.tile == currentTile)) continue;

            var sameNodeInOpenList = openList.FirstOrDefault(x => x.tile == currentTile);
            if (sameNodeInOpenList != null)
            {
                // NOTE : ���� ��尡 open list�� ���� ���, fġ�� �� ���� ��쿡 ���� �� ����
                if (sameNodeInOpenList.f > f)
                {
                    sameNodeInOpenList.f = f;
                    sameNodeInOpenList.parent = parent;
                }
                continue;
            }
            openList.Add(new Node(currentTile, parent, f));
            ++calculCount;
        }

        if(openList.Count == 0 || calculCount >= MaxCalculCount)
        {
            // NOTE : ��� Ž�� ����
            Debug.Log("failed to find path");
            return null;
        }

        var nextCloseNode = openList.MinBy(x => x.f).First();

        if(nextCloseNode.tile == destinationTile) {
            destinationNode = nextCloseNode;
            return null;
        }

        openList.Remove(nextCloseNode);

        return nextCloseNode;
    }
}