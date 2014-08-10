#region Copyright
// ******************************************************************************************
//
// 							SimplePath, Copyright © 2011, Alex Kring
//
// ******************************************************************************************
#endregion

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using SimpleAI.Navigation;
using SimpleAI.Planning;
using SimpleAI;

public class PathManagerComponent_Batch : MonoBehaviour
{	
	#region Unity Editor Fields
	public PathTerrainComponent					m_pathTerrainComponent;
	public int									m_maxNumberOfNodesPerPlanner = 100;
	public int									m_maxNumberOfPlanners = 20;
	public int 									m_maxNumberOfCyclesPerFrame = 500;
	public int 									m_maxNumberOfCyclesPerPlanner = 50;
	#endregion

	#region Fields
	private Pool<PathPlanner>                   m_pathPlannerPool;
	private Queue<PathRequest_Batch>                  m_requestPool;
	private LinkedList<PathRequest_Batch>             _activeRequests;
	private LinkedList<PathRequest_Batch>             m_activeRequests{
		get{
			return _activeRequests;
		}
		set{
			_activeRequests = value;
			if(_activeRequests == null){
				UnityEngine.Debug.LogError("set m_activeRequests to null");
			}
		}
	}
	private IPathTerrain                  		m_terrain;
	private bool								m_bInitialized;
	#endregion
	
	#region Properties
	public IPathTerrain PathTerrain
	{
		get { return m_pathTerrainComponent.PathTerrain; }
	}
	#endregion
	
	#region MonoBehaviour Functions
	void Awake()
	{
		m_bInitialized = false;
		
		if (m_maxNumberOfNodesPerPlanner == 0)
		{
			UnityEngine.Debug.LogError(" 'Max Number Of Nodes Per Planner' must be set to a value greater than 0. Try 100.");
			return;
		}
		
		if (m_maxNumberOfPlanners == 0)
		{
			UnityEngine.Debug.LogError(" 'Max Number Of Planners' must be set to a value greater than 0. Try 20.");
			return;
		}
		
		m_pathPlannerPool = new Pool<PathPlanner>(m_maxNumberOfPlanners);
		foreach (Pool<PathPlanner>.Node planner in m_pathPlannerPool.AllNodes)
		{
			planner.Item.Awake(m_maxNumberOfNodesPerPlanner);
		}
		
		m_requestPool = new Queue<PathRequest_Batch>(m_maxNumberOfPlanners);
		for (int i = 0; i < m_maxNumberOfPlanners; i++)
		{
			m_requestPool.Enqueue(new PathRequest_Batch());
		}
		
		m_activeRequests = new LinkedList<PathRequest_Batch>();
	}
	
	void Start()
	{
		if ( m_pathTerrainComponent == null )
		{
			UnityEngine.Debug.LogError("Must give the PathManagerComponent a reference to the PathTerrainComponent. You can do this through the Unity Editor.");
			return;	
		}
		
		m_terrain = m_pathTerrainComponent.PathTerrain;
		
		if ( m_terrain == null )
		{
			UnityEngine.Debug.LogError("PathTerrain is NULL in PathTerrainComponent. Make sure you have instantiated a path terrain inside the Awake function" +
			                           "of your PathTerrainComponent");
			return;
		}
		
		foreach (Pool<PathPlanner>.Node planner in m_pathPlannerPool.AllNodes)
		{
			planner.Item.Start(m_terrain);
		}
		
		m_bInitialized = true;
	}
	
	public void Update()
	{
		if ( !m_bInitialized )
		{
			UnityEngine.Debug.LogError("PathManagerComponent failed to initialized. Can't call Update.");
			return;
		}
		
		// Update the path planners
		int numCyclesUsed = 0;
		LinkedList<PathRequest_Batch> requestsCompletedThisFrame = new LinkedList<PathRequest_Batch>();
		foreach (PathRequest_Batch request in m_activeRequests)
		{
			PathPlanner pathPlanner = request.PathPlanner.Item;
			int numCyclesUsedByPlanner = pathPlanner.Update(m_maxNumberOfCyclesPerPlanner);
			numCyclesUsed += numCyclesUsedByPlanner;
			
			if (pathPlanner.HasPlanFailed())
			{
				OnRequestCompleted(request, false);
				requestsCompletedThisFrame.AddFirst(request);
			}
			else if (pathPlanner.HasPlanSucceeded())
			{
				request.AccumulateSolution();
				if(request.HasSuccessfullyCompleted()){
					OnRequestCompleted(request, true);
					requestsCompletedThisFrame.AddFirst(request);
				}else{
					// Start the request
					pathPlanner.StartANewPlan( request.PlanParams.GetCurrentIndex(), request.PlanParams.GetCurrentGoalIndexAndMoveNext());
				}
			}
			
			if (numCyclesUsed >= m_maxNumberOfCyclesPerFrame)
			{
				break;
			}
		}
		
		// Remove all the completed requests from the active list
		foreach (PathRequest_Batch request in requestsCompletedThisFrame)
		{
			m_activeRequests.Remove(request);
			
			// If the request failed, then remove it completely
			if ( request.HasFailed() )
			{
				RemoveRequest(request);
			}
		}
	}
	#endregion
	
	#region Requests
	public IPathRequestQuery RequestPathPlan(PathPlanParams_Batch pathPlanParams, IPathAgent agent)
	{
		if ( !m_bInitialized )
		{
			UnityEngine.Debug.LogError("PathManagerComponent isn't yet fully intialized. Wait until Start() has been called. Can't call RequestPathPlan.");
			return null;
		}
		
		if ( m_requestPool.Count == 0 )
		{
			UnityEngine.Debug.Log("RequestPathPlan failed because it is already servicing the maximum number of requests: " +
			                      m_maxNumberOfPlanners.ToString());
			return null;
		}
		
		if ( m_pathPlannerPool.AvailableCount == 0 )
		{
			UnityEngine.Debug.Log("RequestPathPlan failed because it is already servicing the maximum number of path requests: " +
			                      m_maxNumberOfPlanners.ToString());
			return null;
		}
		
		// Clamp the start and goal positions within the terrain space, and make sure they are on the floor.
		//pathPlanParams.UpdateStartAndGoalPos( m_terrain.GetValidPathFloorPos(pathPlanParams.StartPos) );
		
		// Make sure this agent does not have an active request
		if ( m_activeRequests.Count	 > 0 )
		{
			foreach ( PathRequest_Batch pathRequest in m_activeRequests )
			{
				if ( pathRequest.Agent.GetHashCode() == agent.GetHashCode() )
				{
					System.Diagnostics.Debug.Assert(false, "Each agent can only have one path request at a time.");
					return null;
				}
			}
		}
		
		// Create the new request
		Pool<PathPlanner>.Node pathPlanner = m_pathPlannerPool.Get();
		PathRequest_Batch request = m_requestPool.Dequeue();
		request.Set(pathPlanParams, pathPlanner, agent);
		m_activeRequests.AddFirst(request);
		
		// Start the request
		pathPlanner.Item.StartANewPlan(request.PlanParams.GetCurrentIndex(), request.PlanParams.GetCurrentGoalIndexAndMoveNext());
		
		return request;
	}
	
	public void RemoveRequest(IPathRequestQuery requestQuery)
	{
		if ( !m_bInitialized )
		{
			UnityEngine.Debug.LogError("PathManagerComponent isn't yet fully intialized. Wait until Start() has been called. Can't call ConsumeRequest.");
			return;
		}
		
		if ( requestQuery == null )
		{
			return;
		}
		
		PathRequest_Batch request = GetPathRequest_Batch(requestQuery);
		
		if ( request == null )
		{
			return;
		}
		
		// In case the request is active, remove it.
		m_activeRequests.Remove(request);
		
		// Return the request back to the pool so that it can be used again.
		m_pathPlannerPool.Return(request.PathPlanner);
		m_requestPool.Enqueue(request);
	}
	#endregion
	
	#region Notifications
	private void OnRequestCompleted(PathRequest_Batch request, bool bSucceeded)
	{
		if (bSucceeded)
		{
			OnRequestSucceeded(request);
		}
		else
		{
			OnRequestFailed(request);
		}
	}
	
	private void OnRequestFailed(PathRequest_Batch request)
	{
		request.Agent.OnPathAgentRequestFailed();
	}
	
	private void OnRequestSucceeded(PathRequest_Batch request)
	{
		request.Agent.OnPathAgentRequestSucceeded(request);
	}
	#endregion
	
	#region Misc Helpers
	private PathRequest_Batch GetPathRequest_Batch(IPathRequestQuery requestQuery)
	{
		PathRequest_Batch foundRequest = null;
		System.Diagnostics.Debug.Assert(requestQuery is PathRequest_Batch);
		if (requestQuery is PathRequest_Batch)
		{
			foundRequest = requestQuery as PathRequest_Batch;
		}
		return foundRequest;
	}
	#endregion
}