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

    // NOTE : 유저의 패스 파인딩은 동시에 존재할 수 없기 때문에 하나만 존재
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
    // NOTE : 패스 파인딩 개시
    public static void StartPathFinding(bool isPlayer, Tile.Tile startTile, Tile.Tile destinationTile, Action<List<Tile.Tile>> callback) {
        var instance = GetInstance();

        // NOTE : 이미 같은 조건으로 패스 파인딩이 진행중이라면 무시
        if (instance.isExistSamePathFinder(isPlayer, startTile, destinationTile)) {
            Debug.Log("path finding ignored");
            return;
        }

        var pathFinder = isPlayer ? instance.playerPathFinder : instance.GetNewPathFinder();
        instance.StartCoroutine(instance.PathFindCoroutine(startTile, destinationTile, callback, isPlayer, pathFinder));
        instance.StartPathFindingEvent(isPlayer, pathFinder);
    }
}

// NOTE : 패스 파인더. PathFinderManager를 통해 사용됨
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
    //NOTE : 최단 경로를 분석하기 위한 상태값들이 계속 갱신
    private readonly List<Node> openList = new List<Node>();
    //NOTE : 처리가 완료된 노드를 담아둠
    private readonly List<Node> closeList = new List<Node>();

    private Node destinationNode;
    private Tile.Tile destinationTile;
    private int calculCount = 0;
    // NOTE : 연산 최대 수. 이를 넘어가면 경로탐색 실패로 간주함
    private readonly static int MaxCalculCount = 1500;
    // NOTE : 인접 패스 연산은 코스트가 높기 때문에 한 프레임에 해당 횟수만큼만 처리
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

            // NOTE : 탐색 종료
            if (currentCloseNode == null) {
                observer.OnNext(true);
                yield break;
            }

            // NOTE : 한 프레임 내 처리 횟수를 초과하면 다음 프레임으로 연기
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
            // NOTE : 시작 노드에서 해당 노드까지의 실제 소요 경비값
            var g = parent.f + currentTile.MoveCost;
            // NOTE : 휴리스틱 수정 값.해당 노드에서 최종 목적지까지의 추정 값(거리)
            var h = Mathf.Abs(destinationTile.IndexPair.X - currentTile.IndexPair.X) + Mathf.Abs(destinationTile.IndexPair.Y - currentTile.IndexPair.Y);
            var f = g + h;

            // NOTE : close list에 있을 경우엔 open list에 추가 안함
            if (closeList.Any(x => x.tile == currentTile)) continue;

            var sameNodeInOpenList = openList.FirstOrDefault(x => x.tile == currentTile);
            if (sameNodeInOpenList != null)
            {
                // NOTE : 동일 노드가 open list에 있을 경우, f치가 더 낮은 경우에 한해 값 갱신
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
            // NOTE : 경로 탐색 실패
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