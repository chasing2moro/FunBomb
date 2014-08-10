using UnityEngine;
using System.Collections;
using SimpleAI.Navigation;
using System.Collections.Generic;

public class CommonHelper : Singleton<CommonHelper>
{
	public static List<Vector3> PositionsByDirection(int gridIndex, int length, PathGrid.eNeighborDirection dirction,PathGrid grid ){
	//	UnityEngine.Debug.Log("Origin PosIndex:" + gridIndex);
		List<Vector3> Positions = new List<Vector3>();
		for (int i = 0; i < length; i++) {
			gridIndex = grid.GetNeighbor(gridIndex, dirction);
			if(grid.IsBlocked(gridIndex))
				return Positions;
			Positions.Add( grid.GetPathNodePos(gridIndex) );
		//	UnityEngine.Debug.Log("Search PosIndex:" + gridIndex);
		}
		return Positions;
	}
	public static Vector3 HidenPos3D = new Vector3(-100, -100, 0);
	public static Vector2 HidenPos2D = new Vector2(-100, -100);
}

