using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction { None, Davion, Kurita, Liao, Marik, Steiner, Comstar }; 
public class StarSystem : MonoBehaviour {

	public string systemName;
	public Faction faction;
	public float x;
	public float y;
	public bool isDummy = false;
	public List<Vector2> border;

	public void Setup( string sName, float xPos, float yPos, string sFaction)
	{
		systemName = sName;
		x = xPos;
		y = yPos;

		switch (sFaction)
		{
			case "s":
				faction = Faction.Steiner;
				break;
			case "m":
				faction = Faction.Marik;
				break;
			case "l":
				faction = Faction.Liao;
				break;
			case "d":
				faction = Faction.Davion;
				break;
			case "k":
				faction = Faction.Kurita;
				break;
			case "c":
				faction = Faction.Comstar;
				break;
			default:
				faction = Faction.None;
				break;
		}
	}

	void OnDrawGizmos ()
	{
		return;
		if (border != null) {
			
			switch (faction)
			{
				case Faction.Steiner:
					Gizmos.color = Color.blue;
					break;
				case Faction.Marik:
					Gizmos.color = Color.magenta;
					break;
				case Faction.Liao:
					Gizmos.color = Color.green;
					break;
				case Faction.Davion:
					Gizmos.color = Color.yellow;
					break;
				case Faction.Kurita:
					Gizmos.color = Color.red;
					break;
				case Faction.Comstar:
					Gizmos.color = Color.white;
					break;
				default:
					Gizmos.color = Color.gray;
					break;
			}

			if (isDummy) Gizmos.color = Color.clear;
			//else Gizmos.color = Color.gray;

			for (int i = 0; i< border.Count; i++) {
				Vector2 left = border[i];
				Vector2 right;
				if ( (i+1) == border.Count)
					right = border[0];
				else right = border[i+1];
				Gizmos.DrawLine ((Vector3)left, (Vector3)right);
			}
		}
	}
}
