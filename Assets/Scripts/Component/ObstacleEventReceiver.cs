
using UnityEngine;
using System.Collections;
using SimpleAI.Navigation;

[ExecuteInEditMode]
public class ObstacleEventReceiver : MonoBehaviour
{
	Player m_Player;
	void OnTap(TapGesture gesture) { 
		m_Player.MoveTo(gesture.Position);
		Debug.Log("tap Screen pos:" + gesture.Position);
		Debug.Log("tap Obj pos:" + CameraHelper.ScreenPosToBackgroundPos(gesture.Position));
	}
	#region ExecuteInEditMode
//	private PathGridComponent m_PathGridComponent;
//	public bool m_RepaintColliderBox = false;

	void Start(){
		GameObject PlayerObject = GameObject.Find("player");
		if(PlayerObject == null)
			Debug.LogError("PlayerObject == null");
		m_Player = PlayerObject.GetComponent<Player>();
	}

	void Update(){
//		if(m_RepaintColliderBox){
//			RepaintCollderBox();
//			m_RepaintColliderBox = false;
//			Debug.Log("Repaint ColliderBox Finish");
//		}
	}

//	private void RepaintCollderBox(){
//		m_PathGridComponent = this.transform.parent.gameObject.GetComponent<PathGridComponent>();
//		if(m_PathGridComponent == null)
//			Debug.LogError("m_PathGridComponent == null");
//		float cellSize = m_PathGridComponent.m_cellSize;
//		int column = m_PathGridComponent.m_numberOfColumns;
//		int row = m_PathGridComponent.m_numberOfRows;
//		Debug.Log("center:" + new Vector3(this.gameObject.transform.position.x + (cellSize * column) / 2, this.gameObject.transform.position.y  +  (cellSize * row) / 2, this.gameObject.transform.position.z));
//		Debug.Log("size:" + new Vector2(cellSize * column, cellSize * row));
//	}
	#endregion
}

