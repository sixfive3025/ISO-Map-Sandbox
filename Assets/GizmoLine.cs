using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoLine : MonoBehaviour {

	public Vector2 A;
	public Vector2 B;
	public bool drawMe = false;

	void OnDrawGizmos ()
	{
		if (drawMe)
		{
			Gizmos.color = Color.cyan;

			Gizmos.DrawLine ((Vector3)A, (Vector3)B);
		}
	}
}
