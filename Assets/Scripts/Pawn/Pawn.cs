using System.Collections.Generic;
using Tile;
using UnityEngine;

public interface IMovablePawn {
    public void StartPath();
    public void MoveForwardPath();
};

// TODO : �� Ÿ�Կ� ���� �� Ŭ������ ����ϵ���
public class Pawn : MonoBehaviour, IMovablePawn {

    private Tile.Tile currentTile;
    private Tile.Tile destinationTile;
    private List<Tile.Tile> path;
    private int movePoint = 2;
    private List<Tile.Tile> pathPerMovePoint = new List<Tile.Tile>();

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

            //// TODO : UI���� ǥ���ϵ���
            TileHelper.SetTilesColorToEnvironment();

            for (int i = 0, moveTick = movePoint; i < path.Count; ++i) {
                var currentTile = path[i];

                if(moveTick <= 0 || i == path.Count - 1) {
                    moveTick = movePoint;
                    pathPerMovePoint.Add(currentTile);
                    currentTile.color = new Color(1, 0, 0, 1);
                    Debug.Log(pathPerMovePoint.Count);
                }

                moveTick -= currentTile.MoveCost;
            }
            
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
