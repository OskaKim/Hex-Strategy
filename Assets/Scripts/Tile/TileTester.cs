using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

namespace Tile {
    public class TileTester : MonoBehaviour {
        [SerializeField] IndexPair getIndex;
        [SerializeField] MyCamera.CameraModel cameraModel;
        [SerializeField] public HexCoordinates findPathStart;
        [SerializeField] public HexCoordinates findPathEnd;
        [SerializeField] public List<Tile> path;
        [SerializeField] public Pawn pawnPrefab;
        void Start() {
            InputHandler.GetInstance().ClickOnceEvent += ClickOnce;
            InputHandler.GetInstance().ClickContinuingEvent += ClickContinuing;
        }

        void OnDestroy() {
            InputHandler.GetInstance().ClickOnceEvent -= ClickOnce;
            InputHandler.GetInstance().ClickContinuingEvent -= ClickContinuing;
        }

        private List<Pawn> pawns = new List<Pawn>();
        private Pawn selectedPawn;
        private void ClickOnce(int button, Tile clickedTile) {
            if (button == 0) {
                var currentTilePawn = pawns.FirstOrDefault(x => x.CurrentTile == clickedTile);
                if (currentTilePawn && !PawnCreater.isCreateMode) {
                    selectedPawn = currentTilePawn;
                }
                else if(!currentTilePawn && PawnCreater.isCreateMode) {
                    var pawnInstance = Instantiate(pawnPrefab);
                    pawnInstance.name = $"pawn{pawns.Count}";
                    pawnInstance.CurrentTile = clickedTile;
                    pawns.Add(pawnInstance);
                    PawnManager.GetInstance().AddPawn(pawnInstance, 0);
                }
            }
            else if (button == 1) {
                if(selectedPawn && !PawnCreater.isCreateMode) selectedPawn.DestinationTile = clickedTile;
            }
        }
        private void ClickContinuing(int button, Tile clickedTile) {
        }

        public void FindPath() {

            var startTile = TileHelper.GetTile(findPathStart);
            var endTile = TileHelper.GetTile(findPathEnd);

            PathFinderManager.StartPathFinding(true, startTile, endTile, (outPath) => {
                path = outPath;
                TileHelper.SetTilesColorToEnvironment();

                float strengthPerPath = 1.0f / outPath.Count;
                int cnt = 0;
                foreach (var path in path) {
                    path.color = new Color(strengthPerPath * ++cnt, 0, 0, 1);
                }
                TileHelper.ReDrawHexMesh();
            });
        }
    }
}