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

    //NOTE : 최단 경로를 분석하기 위한 상태값들이 계속 갱신
    private static List<Node> openList = new List<Node>();
    //NOTE : 처리가 완료된 노드를 담아둠
    private static List<Node> closeList = new List<Node>();
    private static Node destinationNode;
    private static Tile.Tile destinationTile;
    private static int calculCount = 0;
    // NOTE : 연산 최대 수. 이를 넘어가면 경로탐색 실패로 간주함
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

        Debug.Log($"파인딩 패스 연산 수 {calculCount}");
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