using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleAI.Navigation;

public class ExplosionManger : MonoBehaviour
{

	//#error CompareBaseObjectsInternal can only be called from the main thread.   
	private ExplosionPool m_ExplosionPoolInstance;

	//
//	private  LinkedList< GameObject >[] m_ExplosionListArray;
//	private void InitExplosionListArray(){
//		for (int i = 0; i < (int)PathGrid.eNeighborDirection.kNumNeighbors; i++) {
//			m_ExplosionListArray[i] = new LinkedList<GameObject>();
//		}
//	}
//	private LinkedList<GameObject> ExplosionListByDirection(PathGrid.eNeighborDirection direction){
//		return m_ExplosionListArray[(int)direction];
//	}

	private  List< GameObject > m_ExplosionList;
	//
	private PathGrid m_PathGrid;

	int m_ExplosionIndex = 69;
	public int m_length = 3;
	public string m_ExplosionPrefabPath = "Prefab/Effect/Explosion";

	void Awake(){

		m_ExplosionPoolInstance = ExplosionPool.Instance;
		if(m_ExplosionPoolInstance == null)
			Debug.LogError("m_ExplosionPoolInstance == null");
		m_ExplosionPoolInstance.Init(m_ExplosionPrefabPath);

//		m_ExplosionListArray = new LinkedList<GameObject>[(int)PathGrid.eNeighborDirection.kNumNeighbors];
	//	InitExplosionListArray();

		m_ExplosionList = new List<GameObject>();



	}

	// Use this for initialization
	void Start ()
	{
		m_PathGrid = GameObject.Find("PathGridWithObstacles").GetComponent<PathGridComponent>().PathGrid;
		if(m_PathGrid == null)
			UnityEngine.Debug.LogError("m_PathGrid == null");

	//	Explose();
	}

	public void Explose(){
		Debug.Log("Explose");

		m_ExplosionIndex = m_PathGrid.GetPathNodeIndex( this.gameObject.transform.position );
		
		m_ExplosionList.Clear();
		m_Index2PathGridPosList.Clear();

		//add the first one
		List<Vector3> firstPosList = new List<Vector3>(1);
		firstPosList.Add( m_PathGrid.GetPathNodePos(m_ExplosionIndex) ) ;
		m_Index2PathGridPosList.Add(firstPosList);
		//add others
		for (int i = 0; i < (int)PathGrid.eNeighborDirection.kNumNeighbors; i++) {
			List<Vector3> positions = CommonHelper.PositionsByDirection(m_ExplosionIndex, m_length, (PathGrid.eNeighborDirection) i, m_PathGrid);
			m_Index2PathGridPosList.Add(positions);
		}

		foreach (List<Vector3> PathGridPosList in m_Index2PathGridPosList) {
			foreach (Vector3 PathGridPos in PathGridPosList) {
				GameObject gameObject = m_ExplosionPoolInstance.DequeueFromPool();
				gameObject.transform.position = PathGridPos;
				gameObject.GetComponent<Animator>().SetTrigger("Explosion");
				m_ExplosionList.Add(gameObject);
			}
		}

//
//		//add the first one
//		GameObject gameObject = m_ExplosionPoolInstance.DequeueFromPool();
//		gameObject.transform.position = m_PathGrid.GetPathNodePos(m_ExplosionIndex);
//		gameObject.GetComponent<Animator>().SetTrigger("Explosion");
//		m_ExplosionList.Add(gameObject);
//
//		//add others
//		for (int i = 0; i < (int)PathGrid.eNeighborDirection.kNumNeighbors; i++) {
//			Vector3[] positions = CommonHelper.PositionsByDirection(m_ExplosionIndex, m_length, (PathGrid.eNeighborDirection) i, m_PathGrid);
//			for (int j = 0; j < m_length; j++) {
//				gameObject = m_ExplosionPoolInstance.DequeueFromPool();
//				gameObject.transform.position = positions[j];
//				gameObject.GetComponent<Animator>().SetTrigger("Explosion");
//				//Debug.Log("Set Explosion: pos" + gameObject.transform.position );
//				m_ExplosionList.Add(gameObject);
//			}
//		}

	}

	//Return Object To Pool
//	void ReturnExplosion(){
//		foreach (GameObject item in m_ExplosionList) {
//			m_ExplosionPoolInstance.EnqueueToPool(item);
//		}
//		m_ExplosionList.Clear();
//	}


	private List<List<Vector3>> m_Index2PathGridPosList = new List<List<Vector3>>();

	// Update is called once per frame
	void Update ()
	{
		
	}
}

