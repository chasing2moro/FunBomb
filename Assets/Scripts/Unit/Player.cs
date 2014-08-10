using UnityEngine;
using System.Collections;

public class Player : GameUnitObject
{
	public void MoveTo(Vector2 ScreenPos){
		Vector3 interactPos = CameraHelper.ScreenPosToBackgroundPos (ScreenPos);
		int index = m_pathGrid.GetCellIndex (interactPos);
		if(index == SimpleAI.Planning.Node.kInvalidIndex)
			return;
		int[] indexArray = new int[]{ index };
		m_navigationAgent.MoveToIndex(indexArray);
	}

	void OnGUI() {
		if (GUI.Button(new Rect(10, 10, 150, 100), "Lay Bomb")){
			Vector3 LayBombPos;
			if( m_pathGrid.GetPathNodePosByPos(this.gameObject.transform.position, out LayBombPos) ){
				//(Instantiate( Resources.Load<GameObject>("Prefab/Props/Bomber") ) as GameObject).transform.position = LayBombPos;
				BomberManager.Instance.LayBomber(new Vector2(LayBombPos.x, LayBombPos.y));
			}
			
			
		}
	}

}

