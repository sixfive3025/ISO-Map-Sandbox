using System;
using UnityEngine;
using System.Collections.Generic;
using Delaunay;
using Delaunay.Geo;

public class Main : MonoBehaviour {
	
	[SerializeField]
	private int m_pointCount = 0;
	private List<Vector2> m_points;
	private List<StarSystem> systems = null;
	private MapManager _mapManager = new MapManager();
	private float m_mapWidth = 0;
	private float m_mapHeight = 0;
	float minX = 0, maxX = 0, minY = 0, maxY = 0;
	private List<LineSegment> m_edges = null;
	//private List<LineSegment> m_spanningTree;
	//private List<LineSegment> m_delaunayTriangulation;

	void Awake ()
	{
		Demo ();
	}

	void Update ()
	{
		if (Input.anyKeyDown) {
			Demo ();
		}
	}

	private void Demo ()
	{	
		List<uint> colors = new List<uint> ();
		m_points = new List<Vector2> ();
		systems = new List<StarSystem> ();

		TextAsset systemAsset = Resources.Load("systems") as TextAsset;
		string[] systemList = systemAsset.text.Split('\n');

		for ( int sys = 0; sys < systemList.Length; sys++ )
		{
			string[] systemLine = systemList[sys].Split(',');
			string systemName = systemLine[0];
			float systemX = (Convert.ToSingle(systemLine[1])) * 3;
			float systemY = (Convert.ToSingle(systemLine[2])) * 3;
			string faction = systemLine[3];

			m_pointCount++;
			colors.Add (0);
			GameObject sysGO = new GameObject(systemName);
			StarSystem newSys = sysGO.AddComponent<StarSystem>();
			newSys.Setup(systemName, systemX, systemY, faction);
			systems.Add(newSys);

			if ( systemName.StartsWith( "zz" ) )
			{
				newSys.isDummy = true;
			}

			m_points.Add (new Vector2 ( systemX, systemY ) );
			if (systemX < minX) minX = systemX;
			if (systemX > maxX) maxX = systemX;
			if (systemY < minY) minY = systemY;
			if (systemY > maxY) maxY = systemY;
		}
		
		m_mapWidth = maxX-minX;
		m_mapHeight = maxY-minY;

		Delaunay.Voronoi v = new Delaunay.Voronoi (m_points, colors, new Rect (minX, minY, m_mapWidth, m_mapHeight));

		foreach( StarSystem sys in systems )
		{
			sys.border = v.Region(new Vector2(sys.x, sys.y));

			for ( int i = 0; i < sys.border.Count; i++ )
			{
				int nextCounter = i+1;
				if ( nextCounter == sys.border.Count ) nextCounter = 0;

				_mapManager.ClaimBoundary( sys, sys.border[i], sys.border[nextCounter]);
			}
		}
		
		_mapManager.Draw();
		// m_edges = v.VoronoiDiagram ();
		//m_spanningTree = v.SpanningTree (KruskalType.MINIMUM);
		//m_delaunayTriangulation = v.DelaunayTriangulation ();
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.red;
		if (m_points != null) {
			for (int i = 0; i < m_points.Count; i++) {
				Gizmos.DrawSphere (m_points [i], 0.2f);
			}
		}

		/*if (m_edges != null) {
			Gizmos.color = Color.white;
			for (int i = 0; i< m_edges.Count; i++) {
				Vector2 left = (Vector2)m_edges [i].p0;
				Vector2 right = (Vector2)m_edges [i].p1;
				Gizmos.DrawLine ((Vector3)left, (Vector3)right);
			}
		}

		Gizmos.color = Color.magenta;
		if (m_delaunayTriangulation != null) {
			for (int i = 0; i< m_delaunayTriangulation.Count; i++) {
				Vector2 left = (Vector2)m_delaunayTriangulation [i].p0;
				Vector2 right = (Vector2)m_delaunayTriangulation [i].p1;
				Gizmos.DrawLine ((Vector3)left, (Vector3)right);
			}
		}

		if (m_spanningTree != null) {
			Gizmos.color = Color.green;
			for (int i = 0; i< m_spanningTree.Count; i++) {
				LineSegment seg = m_spanningTree [i];				
				Vector2 left = (Vector2)seg.p0;
				Vector2 right = (Vector2)seg.p1;
				Gizmos.DrawLine ((Vector3)left, (Vector3)right);
			}
		}

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine (new Vector2 (minX, minY), new Vector2 (minX, maxY));
		Gizmos.DrawLine (new Vector2 (minX, minY), new Vector2 (maxX, minY));
		Gizmos.DrawLine (new Vector2 (maxX, maxY), new Vector2 (minX, maxY));
		Gizmos.DrawLine (new Vector2 (maxX, maxY), new Vector2 (maxX, minY));
		*/
	}
}