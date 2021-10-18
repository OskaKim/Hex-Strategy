using System.Collections.Generic;
using Tile;
using UnityEngine;

// TODO : 폰 타입에 따라 이 클래스를 계승하도록
public class Pawn : MonoBehaviour {

    private Tile.Tile currentTile;
    private Tile.Tile destinationTile;
    private List<Tile.Tile> path;

    // NOTE : 현재 위치한 타일
    public Tile.Tile CurrentTile {
        get => currentTile;
        set {
            currentTile = value;
            transform.position = currentTile.transform.position + new Vector3(0, 10f);
        }
    }

    // NOTE : 목적지 타일
    public Tile.Tile DestinationTile {
        set {
            destinationTile = value;
            StartPath();
        }
    }

    public void StartPath() {
        PathFinderManager.StartPathFinding(true, CurrentTile, destinationTile, (outPath) => {
            path = outPath;
            TileHelper.SetTilesColorToEnvironment();

            float strengthPerPath = 1.0f / outPath.Count;
            int cnt = 0;
            foreach (var path in path) {
                path.color = new Color(strengthPerPath * ++cnt, 0, 0, 1);
            }

            // TODO : UI에서 표현하도록
            TileHelper.ReDrawHexMesh();
        });
    }

    public void MoveForwardPath() {
        if (path == null) return;
        if (path.Count <= 1) {
            path = null;
            return;
        }

        currentTile = path[1];
        var nextTilePosition = currentTile.transform.position;
        transform.position = new Vector3(nextTilePosition.x, transform.position.y, nextTilePosition.z);
        StartPath();
    }
}
