using System.Collections;
using System.Collections.Generic;
using Tile;
using UniRx;
using UnityEngine;

public class Pawn : MonoBehaviour {
    public Tile.Tile CurrentTile {
        get => currentTile;
        set {
            currentTile = value;
            transform.position = currentTile.transform.position + new Vector3(0, 10f);
            Debug.Log(currentTile.transform.position);
        }
    }
    public Tile.Tile DestinationTile {
        set {
            destinationTile = value;
            StartPath();
        }
    }

    private Tile.Tile currentTile;
    private Tile.Tile destinationTile;
    private List<Tile.Tile> path;

    public void StartPath() {
        PathFinderManager.StartPlayerPathFinding(CurrentTile, destinationTile, (outPath) => {
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
