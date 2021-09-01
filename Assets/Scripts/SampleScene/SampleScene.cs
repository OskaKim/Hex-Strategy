using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 리소스 반영 확인용
public class SampleScene : MonoBehaviour
{
    [SerializeField] Tile.Tile tilePrefab;
    [SerializeField] List<string> tileNames;
    [SerializeField] List<Tile.Tile> tiles;
    [SerializeField] float offset = 3.0f;

    private void Start()
    {
        var pos = new Vector3();

        foreach(var tileName in tileNames) {
            var newTile = Instantiate(tilePrefab);
            newTile.attachResourceFromResourcePath(tileName);
            newTile.transform.position = pos;
            pos += new Vector3(offset, 0);
            tiles.Add(newTile);
        }
    }
}
