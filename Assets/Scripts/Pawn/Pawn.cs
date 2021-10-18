using System.Collections.Generic;
using Tile;
using UnityEngine;

// TODO : �� Ÿ�Կ� ���� �� Ŭ������ ����ϵ���
public class Pawn : MonoBehaviour {

    private Tile.Tile currentTile;
    private Tile.Tile destinationTile;
    private List<Tile.Tile> path;

    // NOTE : ���� ��ġ�� Ÿ��
    public Tile.Tile CurrentTile {
        get => currentTile;
        set {
            currentTile = value;
            transform.position = currentTile.transform.position + new Vector3(0, 10f);
        }
    }

    // NOTE : ������ Ÿ��
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

            // TODO : UI���� ǥ���ϵ���
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
