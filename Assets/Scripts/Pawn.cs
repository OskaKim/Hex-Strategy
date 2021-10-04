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
            TileHelper.ReDrawHexMesh();
        });
        //var next = Path[0].transform.position;
        //Observable.EveryUpdate()
        //        .Subscribe(_ => {
        //            transform.position = Vector3.MoveTowards(transform.position, next, 10 * Time.deltaTime);

        //            if ((next - transform.position).sqrMagnitude < 1) {
        //                Path.RemoveAt(0);
        //                DeleteWhenNoPath();
        //                next = Path[0].transform.position;
        //            }
        //        });
    }
}
