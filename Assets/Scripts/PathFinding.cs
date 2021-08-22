using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tile;
using System.Linq;
using UniRx;
using System;

public class PathFinding : MonoBehaviour
{
    private class Node
    {
        public Tile.Tile node;
        public Node parent;
        public int f;
        public Node(Tile.Tile node, Node parent, int f)
        {
            this.node = node;
            this.parent = parent;
            this.f = f;
        }
    }

    //NOTE : �ִ� ��θ� �м��ϱ� ���� ���°����� ��� ����
    private static List<Node> openList = new List<Node>();
    //NOTE : ó���� �Ϸ�� ��带 ��Ƶ�
    private static List<Node> closeList = new List<Node>();
    private static Node destinationNode;
    private static Tile.Tile destinationTile;

    private static List<Node> FindPath(Tile.Tile startTile, Tile.Tile destinationTile)
    {
        openList.Clear();
        closeList.Clear();
        destinationNode = null;

        var currentCloseNode = new Node(startTile, null, 0);
        PathFinding.destinationTile = destinationTile;
        CloseListLoop(currentCloseNode);

        List<Node> path = new List<Node>();
        AddToPath(path, destinationNode);

        return path;
    }

    private static void AddToPath(List<Node> path, Node node)
    {
        if (node.parent != null) {
            AddToPath(path, node.parent);
        }
        path.Add(node);
    }

    private static void CloseListLoop(Node currentCloseNode)
    {
        closeList.Add(currentCloseNode);

        var nearTiles = TileHelper.GetNearTiles(currentCloseNode.node);

        foreach (var currentTile in nearTiles)
        {
            if (!currentTile.IsMovable) continue;

            var parent = currentCloseNode;
            // NOTE : ���� ��忡�� �ش� �������� ���� �ҿ� ���
            var g = parent.f + currentTile.MoveCost;
            // NOTE : �޸���ƽ ���� ��.�ش� ��忡�� ���� ������������ ���� ��(�Ÿ�)
            var h = Mathf.Abs(destinationTile.IndexPair.x - currentTile.IndexPair.x) + Mathf.Abs(destinationTile.IndexPair.y - currentTile.IndexPair.y);
            var f = g + h;

            // NOTE : ���� ��尡 open list�� ������� fġ�� �� ���� ������ �߰���
            if (openList.Any(x => x.node == currentTile && x.f <= f)) continue;

            openList.Add(new Node(currentTile, parent, f));
        }

        var nextCloseNode = openList.MinBy(x => x.f).First();

        if(nextCloseNode.node == destinationTile) {
            destinationNode = nextCloseNode;
            return;
        }

        CloseListLoop(nextCloseNode);
    }
}

// NOTE : LINQȮ��
// TODO : ������ ���Ϸ� �ű��
public static class Linq
{
    public static IEnumerable<T> MinBy<T, U>(this IEnumerable<T> source, Func<T, U> selector)
    {
        return SelectBy(source, selector, (a, b) => Comparer<U>.Default.Compare(a, b) < 0);
    }

    public static IEnumerable<T> MaxBy<T, U>(this IEnumerable<T> source, Func<T, U> selector)
    {
        return SelectBy(source, selector, (a, b) => Comparer<U>.Default.Compare(a, b) > 0);
    }

    private static IEnumerable<T> SelectBy<T, U>(IEnumerable<T> source, Func<T, U> selector, Func<U, U, bool> comparer)
    {
        var list = new LinkedList<T>();
        U prevKey = default(U);
        foreach (var item in source)
        {
            var key = selector(item);
            if (list.Count == 0 || comparer(key, prevKey))
            {
                list.Clear();
                list.AddLast(item);
                prevKey = key;
            }
            else if (Comparer<U>.Default.Compare(key, prevKey) == 0)
            {
                list.AddLast(item);
            }
        }
        return list;
    }
}