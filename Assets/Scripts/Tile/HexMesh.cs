using UnityEngine;
using System.Collections.Generic;

namespace Tile
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class HexMesh : MonoBehaviour
	{
		[SerializeField] private Mesh hexMesh;
		[SerializeField] private MeshCollider meshCollider;
		private List<Vector3> vertices = new List<Vector3>();
		private List<int> triangles = new List<int>();
		private List<Color> colors = new List<Color>();

		public void Clear()
		{
			if (hexMesh != null)
			{
				hexMesh.Clear();
				hexMesh = null;
			}
			vertices = new List<Vector3>();
			triangles = new List<int>();
			colors = new List<Color>();
		}

		public void Triangulate()
		{
			Clear();

			if (hexMesh == null) GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
			if (TileModel.hexMesh == null) TileModel.hexMesh = this;

			foreach (var tile in TileModel.tiles) { Triangulate(tile); }
            hexMesh.vertices = vertices.ToArray();
			hexMesh.triangles = triangles.ToArray();
			hexMesh.colors = colors.ToArray();
			hexMesh.RecalculateNormals();
			meshCollider.sharedMesh = hexMesh;
		}

		void Triangulate(Tile cell)
		{
			Vector3 center = cell.transform.localPosition;
			for (int i = 0; i < 6; i++)
			{
				AddTriangle(
					center,
					center + HexMetrics.corners[i],
					center + HexMetrics.corners[i + 1]
				);
				AddTriangleColor(cell.color);
			}
		}
		void AddTriangleColor(Color color)
		{
			colors.Add(color);
			colors.Add(color);
			colors.Add(color);
		}

		void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
		{
			int vertexIndex = vertices.Count;
			vertices.Add(v1);
			vertices.Add(v2);
			vertices.Add(v3);
			triangles.Add(vertexIndex);
			triangles.Add(vertexIndex + 1);
			triangles.Add(vertexIndex + 2);
		}
	}
}