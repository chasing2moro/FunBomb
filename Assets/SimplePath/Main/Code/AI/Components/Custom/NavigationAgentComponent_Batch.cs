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
using SimpleAI.Planning;
using SimpleAI.Steering;

public class NavigationAgentComponent_Batch : MonoBehaviour 
{	
	#region Fields
	private PathAgentComponent_Batch  		m_pathAgent;
	private ISteeringAgent 			       			m_steeringAgent;
	PathPlanParams_Batch 							m_planParams;
	#endregion
	
	
	
	#region Properties
	public IPathTerrain PathTerrain
	{
		get { return m_pathAgent.PathManager.PathTerrain; }
	}
	#endregion
	
	#region MonoBehaviour Functions
	void Awake()
	{
		m_pathAgent       =       GetComponent<PathAgentComponent_Batch>();
		m_steeringAgent = GetComponent<SteeringAgentComponent2D> () as ISteeringAgent;
	}
	#endregion
	
	#region Movement Requests
	public bool MoveToIndex(int[] goalIndexArray){
		int startIndex =  PathTerrain.GetPathNodeIndex(transform.position);
		m_planParams = new PathPlanParams_Batch(startIndex, goalIndexArray, PathTerrain);
		return m_pathAgent.RequestPath(m_planParams);
	}
	public bool MoveToIndex(int index){
		int[] goalIndexArray = new int[]{index};
		return MoveToIndex(goalIndexArray);
	}
	
	public void CancelActiveRequest()
	{
		m_steeringAgent.StopSteering();
		m_pathAgent.CancelActiveRequest();
	}
	#endregion
	
	#region Messages
	private void OnPathRequestSucceeded(IPathRequestQuery request)
	{
		Vector3[] roughPath = request.GetSolutionPath(m_pathAgent.PathManager.PathTerrain);
		m_steeringAgent.SteerAlongPath( roughPath, PathTerrain);
	}
	
	private void OnPathRequestFailed()
	{
		m_steeringAgent.StopSteering();
		SendMessageUpwards("OnNavigationRequestFailed", SendMessageOptions.DontRequireReceiver);
	}
	
	private void OnSteeringRequestSucceeded()
	{
		SendMessageUpwards("OnNavigationRequestSucceeded", SendMessageOptions.DontRequireReceiver);
	}
	
	#endregion
}
