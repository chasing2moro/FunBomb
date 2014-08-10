#region Copyright
// ******************************************************************************************
//
// 							SimplePath, Copyright © 2011, Alex Kring
//
// ******************************************************************************************
#endregion

using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleAI.Navigation;
using SimpleAI.Planning;

namespace SimpleAI.Navigation
{
    /// <summary>
    ///The client making the request can ask for information about the request through this interface.
    /// </summary>
    public interface IPathRequestQuery
    {
        /// <summary>
        ///Retrieve the path solution of the request
        /// </summary>
        /// <returns></returns>
        LinkedList<Node> GetSolutionPath();
		
		/// <summary>
		///Retrieve the path solution of the request, as a list of points
		/// </summary>
		/// <param name="world">
		///PathTerrain where the request was made
		/// </param>
		/// <returns>
		/// A <see cref="Vector3[]"/>
		/// </returns>
		Vector3[] GetSolutionPath(IPathTerrain world);

		//int[] GetSolutionIndex(IPathTerrain world);
		
		/// <summary>
		///Get the start position of the path request 
		/// </summary>
		/// <returns>
		/// A <see cref="Vector3"/>
		/// </returns>
		Vector3 GetStartPos();
		
		/// <summary>
		///Get the goal position of the path request
		/// </summary>
		/// <returns>
		/// A <see cref="Vector3"/>
		/// </returns>
		Vector3 GetGoalPos();
		
		/// <summary>
		///Get the IPathAgent that originally made the path request 
		/// </summary>
		/// <returns>
		/// A <see cref="IPathAgent"/>
		/// </returns>
		IPathAgent GetPathAgent();
		
		/// <summary>
		///Determine if the path request has completed (regardless of it if failed or succeeded) 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
        bool HasCompleted();
		
		/// <summary>
		///Determine if the path request has successfully completed 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
        bool HasSuccessfullyCompleted();
		
		/// <summary>
		///Determine if the path request has failed to find a solution 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		bool HasFailed();
    };

    public class PathRequest : IComparable<PathRequest>, IPathRequestQuery
    {
        #region Fields
        private int                        	 					m_priority;
        private PathPlanParams              		m_pathPlanParams;
        private Pool<PathPlanner>.Node      	m_pathPlanner;
        public float                       					m_replanTimeRemaining;      // The number of seconds that must elapse before we re-plan this path request.
        private IPathAgent                  				m_agent;
        #endregion

        #region Properties
        public int Priority
        {
            get { return m_priority; }
        }

        public Pool<PathPlanner>.Node PathPlanner
        {
            get { return m_pathPlanner; }
        }

        public PathPlanParams PlanParams
        {
            get { return m_pathPlanParams; }
        }

        public IPathAgent Agent
        {
            get { return m_agent; }
        }
        #endregion

        #region Initialization
        public PathRequest()
        {
            m_priority = 0;
            m_replanTimeRemaining = 0.0f;
        }

        public void Set(PathPlanParams planParams, Pool<PathPlanner>.Node pathPlanner, IPathAgent agent)
        {
            m_pathPlanParams = planParams;
            m_pathPlanner = pathPlanner;
            m_replanTimeRemaining = planParams.ReplanInterval;
            m_agent = agent;
        }
        #endregion

        public void Update(float deltaTimeInSeconds)
        {
            m_replanTimeRemaining -= Convert.ToSingle(deltaTimeInSeconds);
            m_replanTimeRemaining = Math.Max(m_replanTimeRemaining, 0.0f);
        }

        public bool IsReadyToReplan()
        {
            return (m_replanTimeRemaining <= 0.0f);
        }

        public void RestartReplanTimer()
        {
            m_replanTimeRemaining = m_pathPlanParams.ReplanInterval;
        }

        #region IComparable<Request> Members
        public int CompareTo(PathRequest other)
        {
            if (m_priority > other.Priority)
            {
                return -1;
            }
            else if (m_priority < other.Priority)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region IPathRequestQuery Members
        public LinkedList<Node> GetSolutionPath()
        {
            return m_pathPlanner.Item.Solution;
        }
		
		public Vector3[] GetSolutionPath(IPathTerrain terrain)
        {
            if (!HasSuccessfullyCompleted())
            {
                return null;
            }
			
			LinkedList<Planning.Node> path = GetSolutionPath();
			
			if ( path == null || path.Count == 0 )
			{
				return null;
			}

            Vector3[] pathPoints = new Vector3[path.Count];
            int i = 0;
            foreach (Planning.Node node in path)
            {
                Vector3 nodePos = terrain.GetPathNodePos(node.Index);
                pathPoints[i] = nodePos;
                i++;
            }
			
			// Set the first position to be the start position
			pathPoints[0] = GetStartPos();
			
			// Set the last position to be the goal position.
			int lastIndex = Mathf.Clamp(i, 0, path.Count - 1);
			System.Diagnostics.Debug.Assert(lastIndex > 1 && lastIndex < path.Count);
			pathPoints[lastIndex] = GetGoalPos();

			return pathPoints;
        }
//		public int[] GetSolutionIndex(IPathTerrain world){
//			if (!HasSuccessfullyCompleted())
//			{
//				return null;
//			}
//			
//			LinkedList<Planning.Node> path = GetSolutionPath();
//			
//			if ( path == null || path.Count == 0 )
//			{
//				return null;
//			}
//			
//			int[] pathIndexs = new int[path.Count];
//			int i = 0;
//			foreach (Planning.Node node in path)
//			{
//				pathIndexs[i++] = node.Index;
//			}
//			return pathIndexs;
//		}
		
		public Vector3 GetStartPos()
		{
			return m_pathPlanParams.StartPos;
		}
		
		public Vector3 GetGoalPos()
		{
			return m_pathPlanParams.GoalPos;
		}
		
		public IPathAgent GetPathAgent()
		{
			return Agent;	
		}
		
        public bool HasCompleted()
        {
            return m_pathPlanner.Item.HasPlanCompleted();
        }

        public bool HasSuccessfullyCompleted()
        {
            return m_pathPlanner.Item.HasPlanSucceeded();
        }
		
		public bool HasFailed()
		{
			return m_pathPlanner.Item.HasPlanFailed();	
		}
        #endregion
    }

	public class PathRequest_Batch : IComparable<PathRequest_Batch>, IPathRequestQuery
	{
		#region Fields
		private int                        	 					m_priority;
		private PathPlanParams_Batch            m_pathPlanParams;
		private Pool<PathPlanner>.Node      	m_pathPlanner;
		private IPathAgent                  				m_agent;
		private List<int>                   m_solutionNodeList = new List<int>();
		#endregion
		
		#region Properties
		public int Priority
		{
			get { return m_priority; }
		}
		
		public Pool<PathPlanner>.Node PathPlanner
		{
			get { return m_pathPlanner; }
		}
		
		public PathPlanParams_Batch PlanParams
		{
			get { return m_pathPlanParams; }
		}
		
		public IPathAgent Agent
		{
			get { return m_agent; }
		}
		#endregion
		
		#region Initialization
		public PathRequest_Batch()
		{
			m_priority = 0;
		}
		
		public void Set(PathPlanParams_Batch planParams, Pool<PathPlanner>.Node pathPlanner, IPathAgent agent)
		{
			m_pathPlanParams = planParams;
			m_pathPlanner = pathPlanner;
			m_agent = agent;
			m_solutionNodeList.Clear();
		}
		#endregion

		#region IComparable<Request> Members
		public int CompareTo(PathRequest_Batch other)
		{
			if (m_priority > other.Priority)
			{
				return -1;
			}
			else if (m_priority < other.Priority)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}
		#endregion
		
		#region IPathRequestQuery Members
		public LinkedList<Node> GetSolutionPath()
		{
			Debug.LogError("Haven't implement");
			return null;
		}
		
		public Vector3[] GetSolutionPath(IPathTerrain terrain)
		{
			if (!HasSuccessfullyCompleted())
			{
				return null;
			}
			
		//	LinkedList<Planning.Node> path = GetSolutionPath();
			
			if ( m_solutionNodeList == null || m_solutionNodeList.Count == 0 )
			{
				return null;
			}

			List<int> PathCornerIndexList = null;
			if(terrain is PathGrid){
				PathCornerIndexList = (terrain as PathGrid).CornerIndexs(m_solutionNodeList.ToArray());
			}else{
				Debug.LogError("terrain is not PathGrid");
			}

			Vector3[] pathPoints = new Vector3[PathCornerIndexList.Count];
			int i = 0;
			foreach (int cornerIndex in PathCornerIndexList)
			{
				//Debug.Log("node.Index:" + node.Index);
				Vector3 nodePos = terrain.GetPathNodePos(cornerIndex);
				pathPoints[i] = nodePos;
				i++;
			}
			
			// Set the first position to be the start position
			pathPoints[0] = GetStartPos();
			
			// Set the last position to be the goal position.
			int lastIndex = Mathf.Clamp(i, 0, PathCornerIndexList.Count - 1);
			System.Diagnostics.Debug.Assert(lastIndex > 1 && lastIndex < PathCornerIndexList.Count);
			pathPoints[lastIndex] = GetGoalPos();
			
			return pathPoints;
		}
		
		public Vector3 GetStartPos()
		{
			return m_pathPlanParams.StartPos;
		}
		
		public Vector3 GetGoalPos()
		{
			return m_pathPlanParams.GoalPos;
		}
		
		public IPathAgent GetPathAgent()
		{
			return Agent;	
		}
		
		public bool HasCompleted()
		{
			Debug.LogError("You should not use this method");
			return m_pathPlanner.Item.HasPlanCompleted();
		}
		
		public bool HasSuccessfullyCompleted()
		{
			return m_pathPlanParams.FinishPlan;
		}
		
		public bool HasFailed()
		{
			 return m_pathPlanner.Item.HasPlanFailed();	
		}
		#endregion

		public void AccumulateSolution(){
			//m_solutionNodeList.AddLast(m_pathPlanner.Item.Solution);
		//	Debug.Log("Add node Index:");
			foreach (Node item in m_pathPlanner.Item.Solution) {
				m_solutionNodeList.Add(item.Index);
			//	Debug.Log(addItem.Index);
			}
		//	Debug.Log("---------------------");

		}
	}
}
