using UnityEngine;
using UnityEngine.UI;
namespace Tile
{
	public class HexGrid : MonoBehaviour
	{
		public Tile cellPrefab;
		public Text cellLabelPrefab;
		public Color defaultColor = Color.white;
		public Color touchedColor = Color.magenta;
		Canvas gridCanvas;

		Tile[] cells;
		HexMesh hexMesh;

		void CreateCell(int x, int y, int i)
        {
            Vector3 position = new Vector3(
                (x + y * 0.5f - y / 2) * (HexMetrics.innerRadius * 2f),
				0f,
				y * (HexMetrics.outerRadius * 1.5f) );

			Tile cell = cells[i] = Instantiate<Tile>(cellPrefab);
			TileModel.tiles.Add(cell);
			cell.transform.SetParent(transform, false);
			cell.transform.localPosition = position;
			cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, y);
			cell.color = defaultColor;

			Text label = Instantiate<Text>(cellLabelPrefab);
			label.rectTransform.SetParent(gridCanvas.transform, false);
			label.rectTransform.anchoredPosition =
				new Vector2(position.x, position.z);
			label.text = cell.coordinates.ToStringOnSeparateLines();
		}

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