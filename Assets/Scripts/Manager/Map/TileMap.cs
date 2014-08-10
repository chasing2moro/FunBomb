using UnityEngine;
using System.Collections;
using SimpleAI.Navigation;

public class TileMap : Singleton<TileMap>
{
	PathGrid m_PathGrid;
	public PathGrid PathGrid
	{
		get{
			if(m_PathGrid == null)
				Debug.LogError("m_PathGrid == null");
			return m_PathGrid;
		}
	}
	public TileMap(){
		Init();
	}

	void Init(){
		GameObject  aPathGridWithObstacles =  GameObject.Find("PathGridWithObstacles");
		m_PathGrid = aPathGridWithObstacles.GetComponent<PathGridComponent>().PathGrid;
	}
}

