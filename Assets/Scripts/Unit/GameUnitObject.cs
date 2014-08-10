using UnityEngine;
using System.Collections;
using SimpleAI.Navigation;

public class GameUnitObject : MonoBehaviour
{
	public  float 																m_Speed;
	protected PathGrid														m_pathGrid;
	protected NavigationAgentComponent_Batch 			m_navigationAgent;



	// Use this for initialization
	void Start () 
	{
		m_navigationAgent = GetComponent<NavigationAgentComponent_Batch>();
		if ( m_navigationAgent.PathTerrain == null || !(m_navigationAgent.PathTerrain is PathGrid) )
		{
			Debug.LogError("Interaction_Touch was built to work with a PathGrid terrain; can't use it on other terrain types.");
		}
		else
		{
			m_pathGrid = m_navigationAgent.PathTerrain as PathGrid;
		}
	}
}

