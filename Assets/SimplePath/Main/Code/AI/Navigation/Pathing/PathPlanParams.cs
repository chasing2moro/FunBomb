#region Copyright
// ******************************************************************************************
//
// 							SimplePath, Copyright © 2011, Alex Kring
//
// ******************************************************************************************
#endregion

using UnityEngine;
using System.Collections.Generic;
using SimpleAI.Navigation;

namespace SimpleAI.Navigation
{
	public interface IPathPlanParams{
		Vector3 StartPos{get;}
		Vector3 GoalPos{get;}
	}

	public class PathPlanParams_Batch : IPathPlanParams{
		int m_startIndex;
		int[] m_goalIndexArray;
		int m_goalIndexIdx;
		IPathTerrain m_terrain;

		public PathPlanParams_Batch(int startIndex, int[] goalIndexArray, IPathTerrain terrain)
		{
			m_startIndex = startIndex;
			m_goalIndexArray = goalIndexArray;
			m_goalIndexIdx = 0;
			m_terrain = terrain;
		}

		public Vector3 StartPos{
			get{
				return m_terrain.GetPathNodePos(m_startIndex);
			}
		}
		public Vector3 GoalPos{
			get{
				return m_terrain.GetPathNodePos(m_goalIndexArray[m_goalIndexArray.Length - 1]);
			}
		}

		public bool FinishPlan{
			get{
				return m_goalIndexIdx >= m_goalIndexArray.Length;
			}
		}

		public void MoveNext(){
			m_goalIndexIdx++;
		}

		public int CurrentGoalIndex() {
			return m_goalIndexArray[m_goalIndexIdx];
		}

		public int GetCurrentIndex(){
			if(m_goalIndexIdx == 0)
				return m_startIndex;
			else
				return m_goalIndexArray[m_goalIndexIdx - 1];
		}
		public int GetCurrentGoalIndexAndMoveNext(){
			if(FinishPlan){
				Debug.LogError("Plan has Finish");
				return m_goalIndexArray[m_goalIndexArray.Length - 1];
			}
			int pos = CurrentGoalIndex();
			MoveNext();
			return pos;
		}

	}
    /// <summary>
    ///Defines the characteristics of a path request
    /// </summary>
	public class PathPlanParams  : IPathPlanParams
    {
        #region Fields
        Vector3         m_startPos;
        Vector3         m_goalPos;
        INavTarget      m_target;           // ex: a position, or a GameObject.
        float           m_replanInterval;   // number of seconds between each replan
        #endregion

        #region Properties
        public Vector3 StartPos 
        {
            get { return m_startPos; }
        }

        public Vector3 GoalPos
        {
            get { return m_goalPos; }
        }

        public float ReplanInterval
        {
            get { return m_replanInterval; }
        }
        #endregion

        public PathPlanParams(Vector3 startPos, INavTarget target, float replanInterval)
        {
            System.Diagnostics.Debug.Assert(replanInterval > 0.0f);
            m_startPos = startPos;
            m_target = target;
            m_goalPos = target.GetNavTargetPosition();
            m_replanInterval = replanInterval;
        }

		public void SetTarget(INavTarget target){
			m_target = target;
		}
		
		/// <summary>
		///Paths are replanned. When they are replanned, we need to recompute the new start and goal position of the path,
		///since the world has changed since the path was last planned. This function handles recomputing those values.
		/// </summary>
		/// <param name="newStartPos">
		///The new start position of the path plan (likely the agent's current position)
		/// </param>
        public void UpdateStartAndGoalPos(Vector3 newStartPos)
        {
            m_startPos = newStartPos;
            m_goalPos = m_target.GetNavTargetPosition();
        }
    }
}
