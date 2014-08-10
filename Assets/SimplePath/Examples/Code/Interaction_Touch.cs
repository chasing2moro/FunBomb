using UnityEngine;
using System.Collections;
using SimpleAI.Navigation;
using System.Collections.Generic;

[RequireComponent(typeof(NavigationAgentComponent_Batch))]
[RequireComponent(typeof(PathAgentComponent_Batch))]
public class Interaction_Touch : MonoBehaviour
{
	private NavigationAgentComponent_Batch 		m_navigationAgent;
	private PathGrid										m_pathGrid;


	private Vector2										m_swipeLastRecordVec;
	private Vector2										m_swipeCurrentVec;
	private Vector2										m_swipeNextVec;
	private int													swipeTempColumn;
	private List<int> 										swipeIndexListRaw;
	private List<int> 										swipeIndexList;
	private int													swipeIndexListIndex;
	private SearchPathType 							searchType = SearchPathType.TapToWalk;

	//Debug show in Scene
	public bool m_debugShowPath = true;



	void Awake()
	{
		m_navigationAgent = GetComponent<NavigationAgentComponent_Batch>();
	}

	// Use this for initialization
	void Start () 
	{
		if ( m_navigationAgent.PathTerrain == null || !(m_navigationAgent.PathTerrain is PathGrid) )
		{
			Debug.LogError("Interaction_Touch was built to work with a PathGrid terrain; can't use it on other terrain types.");
		}
		else
		{
			m_pathGrid = m_navigationAgent.PathTerrain as PathGrid;
		}

			swipeIndexList = new List<int> ();
			swipeIndexListRaw = new List<int>();
			swipeIndexListIndex = 0;
	
	}

	void OnTap(TapGesture gesture) {
		searchType = SearchPathType.TapToWalk;

		Vector3 interactPos = CameraHelper.ScreenPosToBackgroundPos (gesture.StartPosition);
		int index = m_pathGrid.GetCellIndex (interactPos);
		if(index == SimpleAI.Planning.Node.kInvalidIndex)
			return;
		int[] indexArray = new int[]{ index };
		m_navigationAgent.MoveToIndex(indexArray);
	}

	void OnSwipe(SwipeGesture gesture){
		searchType = SearchPathType.SwipeToWalk;
		swipeIndexList.Clear ();
		swipeIndexListRaw.Clear();

		foreach (Vector2 item in gesture.VectorsInProgress) {
			Vector3 bgPos = CameraHelper.ScreenPosToBackgroundPos (item);
			int index =	m_pathGrid.GetCellIndexClamped(bgPos);
			if(m_pathGrid.IsBlocked(index))
				continue;

			if(swipeIndexListRaw.Count == 0){
				swipeIndexListRaw.Add(index);
				continue;
			}

			if(swipeIndexListRaw[swipeIndexListRaw.Count - 1] != index){
				swipeIndexListRaw.Add(index);
			}
		}

		swipeIndexList =m_pathGrid.CornerIndexs(swipeIndexListRaw);

		swipeIndexListIndex = 0;
		m_navigationAgent.MoveToIndex (swipeIndexList.ToArray());
	}

	private void OnNavigationRequestSucceeded()
	{
		//Debug.Log("---------OnNavigationRequestSucceeded----------");
	}
	
	void OnDrawGizmos()
	{
		//Debug show in Scene
		if (m_debugShowPath) {
			if (swipeIndexList != null && swipeIndexList.Count > 0) {
				Gizmos.color = Color.red;
				for (int i = 1; i < swipeIndexList.Count; i++) {
					Vector3 start = m_pathGrid.GetCellCenter (swipeIndexList [i - 1]);
					Vector3 end = m_pathGrid.GetCellCenter (swipeIndexList [i]);
					Gizmos.DrawLine (start, end);
					if(swipeIndexList [i - 1] == swipeIndexList [i]){
						Debug.LogError("i =" + i + "  swipeIndexList [i - 1]  =" + swipeIndexList [i - 1]  + " swipeIndexList [i ]  " + swipeIndexList [i]  + " is the same");
					}
					Gizmos.DrawCube (start, new Vector3 (0.2f, 0.2f, 0.2f));
				}
			}
		}
	}
	
}

