using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Tile
{
    // TODO : ����Ƽ ����� ������� Ȯ���� �� �ֵ��� ����
    public class TileTester : MonoBehaviour
    {
        [SerializeField] IndexPair getIndex;

        private List<Tile> cacheTiles = new List<Tile>();

        private void Start()
        {
            var clickStream = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ =>
            {
                var clickedObject = InputHelper.GetGameObjectFromScreenPointToRay(Input.mousePosition);

                if (clickedObject)
                {
                    Debug.Log(clickedObject.name);
                }
            });

            var showNearTile = Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.F))
            .Subscribe(_ =>
            {
                foreach (var cacheTile in cacheTiles)
                {
                    cacheTile.DebugTextToIndex();
                }
                cacheTiles.Clear();

                var tile = TileHelper.GetTile(getIndex);
                tile.CustomDebugText("target");
                cacheTiles.Add(tile);

                foreach (var nearTile in TileHelper.GetNearTiles(tile))
                {
                    Debug.Log($"{nearTile.IndexPair.x} / {nearTile.IndexPair.y})");
                    nearTile.CustomDebugText("near tile");
                    cacheTiles.Add(nearTile);
                }
            });
        }
    }
}