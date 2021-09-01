using UnityEngine;
using UnityEngine.UI;
namespace Tile
{
    // TODO : 삭제
	public class HexGrid : MonoBehaviour
	{
		public Tile cellPrefab;
		public Text cellLabelPrefab;
		public Color defaultColor = Color.white;
		public Color touchedColor = Color.magenta;
		
        // TODO : 다른 클래스에서 터치 처리 정리하기
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                HandleInput();
            }
        }

        void HandleInput()
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                TouchCell(hit.point);
            }
        }
        public void TouchCell(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            var clickedTile = TileHelper.GetTile(coordinates);
            clickedTile.color = touchedColor;

            TileHelper.GetNearTiles(clickedTile).ForEach(x => x.color = Color.green);

            TileModel.hexMesh.Triangulate();
        }
    }
}