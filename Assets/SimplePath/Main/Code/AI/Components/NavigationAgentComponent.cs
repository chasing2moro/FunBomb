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

[RequireComponent(typeof(PathAgentComponent))]
//[RequireComponent(typeof(SteeringAgentComponent))]
//[RequireComponent(typeof(Rigidbody))]
public class NavigationAgentComponent : MonoBehaviour 
{	
	#region Unity Editor Fields
#if PathSmoothing 
	public bool								m_usePathSmoothing = true;
#endif
	#endregion
	
	#region Fields
	private PathAgentComponent 		m_pathAgent;
	private ISteeringAgent 			m_steeringAgent;
	PathPlanParams m_planParams;
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
		// Find a reference to the path and steering agent
		m_pathAgent = GetComponent<PathAgentComponent>();
		m_steeringAgent = GetComponent<SteeringAgentComponent2D> () as ISteeringAgent;
		if(m_steeringAgent == null)
			m_steeringAgent = GetComponent<SteeringAgentComponent> () as ISteeringAgent;
	}
	#endregion
	
	#region Movement Requests
#if false
	//add by bobo
	public bool ChangeTargetPos(int index){
		Vector3 targetPos = PathTerrain.GetPathNodePos (index);
		return ChangeTargetPos (targetPos);
	}
	//add by bobo
	public bool ChangeTargetPos(Vector3 targetPos){
		NavTargetPos navTargetPos = new NavTargetPos(targetPos, PathTerrain);
		if (m_planParams != null) {
			m_planParams.SetTarget (navTargetPos);
			return true;
		}
		return false;
	}
#endif

	//add by bobo
	public bool MoveToIndex(int index){
       Vector3 targetPos = PathTerrain.GetPathNodePos (index);
	   return MoveToPosition (targetPos);
	}

	public bool MoveToPosition(Vector3 targetPosition, float replanInterval = -1)
	{
		NavTargetPos navTargetPos = new NavTargetPos(targetPosition, PathTerrain);
		m_planParams = new PathPlanParams(transform.position, navTargetPos, replanInterval);
		return m_pathAgent.RequestPath(m_planParams);
	}
	
	public bool MoveToGameObject(GameObject gameObject, float replanInterval = -1)
	{
		NavTargetGameObject navTargetGameObject = new NavTargetGameObject(gameObject, PathTerrain);
		m_planParams = new PathPlanParams(transform.position, navTargetGameObject, replanInterval);
		return m_pathAgent.RequestPath(m_planParams);
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
		Vector3[] steeringPath = roughPath;

#if PathSmoothing 
		if ( m_usePathSmoothing )
		{
			// Smooth the path
			Vector3[] aLeftPortalEndPts;
			Vector3[] aRightPortalEndPts;
			m_pathAgent.PathManager.PathTerrain.ComputePortalsForPathSmoothing( roughPath, out aLeftPortalEndPts, out aRightPortalEndPts );
			steeringPath = PathSmoother.Smooth(roughPath, request.GetStartPos(), request.GetGoalPos(), aLeftPortalEndPts, aRightPortalEndPts);
		}
#endif

		// Begin steering along this path
		m_steeringAgent.SteerAlongPath( steeringPath, m_pathAgent.PathManager.PathTerrain );
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
