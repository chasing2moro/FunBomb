#region Copyright
// ******************************************************************************************
//
// 							SimplePath, Copyright © 2011, Alex Kring
//
// ******************************************************************************************
#endregion

using UnityEngine;
using System.Collections;
using SimpleAI.Navigation;

[RequireComponent(typeof(NavigationAgentComponent_Batch))]
public class Wander : MonoBehaviour 
{
	
	#region Fields
	private NavigationAgentComponent_Batch 		m_navigationAgent;
	private bool							m_bNavRequestCompleted;
	private PathGrid						m_pathGrid;
	#endregion
	
	#region MonoBehaviour Functions
	void Awake()
	{
		m_bNavRequestCompleted = true;
		m_navigationAgent = GetComponent<NavigationAgentComponent_Batch>();
	}
	
	// Use this for initialization
	void Start () 
	{
		if ( m_navigationAgent.PathTerrain == null || !(m_navigationAgent.PathTerrain is PathGrid) )
		{
			Debug.LogError("Interaction_Wander was built to work with a PathGrid terrain; can't use it on other terrain types.");
		}
		else
		{
			m_pathGrid = m_navigationAgent.PathTerrain as PathGrid;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (  m_bNavRequestCompleted )
		{
			int destIndex = ChooseRandomDestination();
			if ( m_navigationAgent.MoveToIndex(destIndex) )
			{
				m_bNavRequestCompleted = false;
			}
		}
	}
	#endregion
	
	private int ChooseRandomDestination()
	{
		// Pick a random grid cell.
		int randomGridCell = Random.Range(0, m_pathGrid.NumberOfCells - 1);
		if(m_pathGrid.IsNodeBlocked(randomGridCell)){
			randomGridCell = ChooseRandomDestination();
		}

		return randomGridCell;
	}
	
	#region Messages
	private void OnNavigationRequestSucceeded()
	{
		m_bNavRequestCompleted = true;
	}
	
	private void OnNavigationRequestFailed()
	{
		m_bNavRequestCompleted = true;
	}
	#endregion
}
