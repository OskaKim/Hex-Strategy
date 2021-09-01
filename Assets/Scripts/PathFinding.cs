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

    //NOTE : �ִ� ��θ� �м��ϱ� ���� ���°����� ��� ����
    private static List<Node> openList = new List<Node>();
    //NOTE : ó���� �Ϸ�� ��带 ��Ƶ�
    private static List<Node> closeList = new List<Node>();
    private static Node destinationNode;
    private static Tile.Tile destinationTile;
    private static int calculCount = 0;
    // NOTE : ���� �ִ� ��. �̸� �Ѿ�� ���Ž�� ���з� ������
    private readonly static int MaxCalculCount = 1500;

    public static List<Tile.Tile> FindPath(Tile.Tile startTile, Tile.Tile destinationTile)
    {
        if (startTile == destinationTile || !startTile.IsMovable || !destinationTile.IsMovable) return new List<Tile.Tile>();

        openList.Clear();
        closeList.Clear();
        destinationNode = null;
        calculCount = 0;

        var currentCloseNode = new Node(startTile, null, 0);
        PathFinding.destinationTile = destinationTile;
        CloseListLoop(currentCloseNode);

        List<Tile.Tile> path = new List<Tile.Tile>();
        AddToPath(path, destinationNode);

        Debug.Log($"���ε� �н� ���� �� {calculCount}");
        return path;
    }

    private static void AddToPath(List<Tile.Tile> path, Node node)
    {
        if (node == null) return;

        if (node.parent != null)
        {
            AddToPath(path, node.parent);
        }

        path.Add(node.tile);
    }

    private static void CloseListLoop(Node currentCloseNode)
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
            return;
        }

        var nextCloseNode = openList.MinBy(x => x.f).First();

        if(nextCloseNode.tile == destinationTile) {
            destinationNode = nextCloseNode;
            return;
        }

        openList.Remove(nextCloseNode);
        CloseListLoop(nextCloseNode);
    }
}