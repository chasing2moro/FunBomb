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

namespace SimpleAI.Navigation
{
	/// <summary>
	///Grid-based navigation terrain. This grid is 4-connect. In other words, each cell has a maximum
	///of 4 neighbors; there are no diagonals.
	/// </summary>
	public class PathGrid : SolidityGrid, IPathTerrain
	{
	    public enum eNeighborDirection
	    {
	        kNoNeighbor = -1,
	        kLeft,
	        kTop,
	        kRight,
	        kBottom,
	        kNumNeighbors
	    };
		
		#region Fields
		//private IHeightmap m_heightmap;
		#endregion
		
		public PathGrid()
		{
			//m_heightmap = null;
		}
		
		// Note: the heightmap is optional.
		public void Awake (Vector3 origin, int numRows, int numCols, float cellSize, bool show, IHeightmap heightmap)
		{
			base.Awake(origin, numRows, numCols, cellSize, show);
			//m_heightmap = heightmap;
		}
		
		public bool HasHeightMap()
		{
			return false;//( m_heightmap != null );
		}
	
	    #region IPathTerrain Members
	    public int GetNeighbors(int index, ref int[] neighbors)
	    {
	        neighbors = new int[(int)eNeighborDirection.kNumNeighbors];
	
	        for (int i = 0; i < (int)eNeighborDirection.kNumNeighbors; i++)
	        {
	            neighbors[i] = GetNeighbor(index, (eNeighborDirection)i);
	        }
	
	        return (int)eNeighborDirection.kNumNeighbors;
	    }
	
	    public int GetNumNodes()
	    {
	        return NumberOfCells;
	    }
	
	    public float GetGCost(int startIndex, int goalIndex)
	    {
	        Vector3 startPos = GetPathNodePos(startIndex);
	        Vector3 goalPos = GetPathNodePos(goalIndex);
	        float cost = Vector3.Distance(startPos, goalPos);
	        return cost;
	    }
		
		public float GetHCost(int startIndex, int goalIndex)
	    {
#if false
	        Vector3 startPos = GetPathNodePos(startIndex);
	        Vector3 goalPos = GetPathNodePos(goalIndex);
			float heuristicWeight = 2.0f;
			float cost = heuristicWeight * Vector3.Distance(startPos, goalPos);
			// Give extra cost to height difference
			cost = cost + Mathf.Abs(goalPos.y - startPos.y) * 1.0f;
	        return cost;
#else
			return 0;
#endif
	    }
	
	    public bool IsNodeBlocked(int index)
	    {
	        return IsBlocked(index);
	    }
	
	    public Vector3 GetPathNodePos(int index)
	    {
	        // Use the center of the grid cell, as the position of the planning node.
	       // Vector3 nodePos = GetCellPosition(index);
	       // nodePos += new Vector3(m_cellSize / 2.0f, 0.0f, m_cellSize / 2.0f);
			Vector3 nodePos = GetCellCenter (index);
			//nodePos.y = GetTerrainHeight(nodePos);
	        return nodePos;
	    }

		public bool GetPathNodePosByPos(Vector3 pos, out Vector3 outPos){
			int index = GetPathNodeIndex(pos);
			if(index == SimpleAI.Planning.Node.kInvalidIndex){
				outPos = Vector3.zero;
				return false;
			}

			outPos = GetPathNodePos(index);
			return true;
		}
	
	    public int GetPathNodeIndex(Vector3 pos)
	    {
	        int index = GetCellIndex( pos );
	        if ( !IsInBounds(index) )
			{
				index = SimpleAI.Planning.Node.kInvalidIndex;
			}
	        return index;
	    }
		
		public void ComputePortalsForPathSmoothing(Vector3[] roughPath, out Vector3[] aLeftPortalEndPts, out Vector3[] aRightPortalEndPts)
		{
			GridPortalComputer.ComputePortals(roughPath, this, out aLeftPortalEndPts, out aRightPortalEndPts);
		}
//#error This function is Wrong
		public Vector3 GetValidPathFloorPos(Vector3 position)
		{
			// Save this value off, in case we need to use it to search for a valid location further along in this function.
			Vector3 originalPosition = position;
			
			// Find a position that is within the grid, at the same height as the grid.
			float padding = m_cellSize / 4.0f;
			Bounds gridBounds = GetGridBounds();
			//Debug.Log("position1:" + position);
			//Debug.Log(" " + ConvertUtils.VerticalValue( position ) + " /" + (ConvertUtils.VerticalValue( gridBounds.min ) + padding )+ " /" + (ConvertUtils.VerticalValue( gridBounds.max ) - padding));
			position = ConvertUtils.PosByHV(Mathf.Clamp(ConvertUtils.HorizontalValue( position ), ConvertUtils.HorizontalValue( gridBounds.min ) + padding, ConvertUtils.HorizontalValue( gridBounds.max ) - padding),
			                                Mathf.Clamp(ConvertUtils.VerticalValue( position ),   ConvertUtils.VerticalValue( gridBounds.min ) + padding,   ConvertUtils.VerticalValue( gridBounds.max ) - padding),
			                     			ConvertUtils.ThirdValue( Origin ));
			//Debug.Log("position2:" + position);
			// If this position is blocked, then look at all of the neighbors of this cell, and see if one of those cells is
			// unblocked. If one of those neighbors is unblocked, then we return the position of that neighbor, to ensure that 
			// the agent is always pathing to and from valid positions.
			int cellIndex = GetCellIndex(position);
			if ( IsBlocked(cellIndex) )
			{
				// Find the closest unblocked neighbor, if one exists.
				int[] neighbors = null;
				int numNeighbors = GetNeighbors(cellIndex, ref neighbors);
				float closestDistSq = float.MaxValue;
				for ( int i = 0; i < numNeighbors; i++ )
				{
					int neighborIndex = neighbors[i];
					if ( !IsBlocked(neighborIndex) )
					{
						Vector3 neighborPos = GetCellCenter(neighborIndex);
						float distToCellSq = (neighborPos - originalPosition).sqrMagnitude;
						if ( distToCellSq < closestDistSq )
						{
							closestDistSq = distToCellSq;
							position = neighborPos;	
						}
					}
				}
			}
			
			return position;
		}
		
		public float GetTerrainHeight(Vector3 position)
		{
			return 0;
		//	if ( m_heightmap == null )
		//	{
		//		return Origin.y;	
		//	}
		//	else
		//	{
		//		return m_heightmap.SampleHeight(position);	
		//	}
		}
	    #endregion
	
		#region Neighbor Functions
		public eNeighborDirection GetNeighborDirection(int index, int neighborIndex)
		{
			for (int i = 0; i < (int)eNeighborDirection.kNumNeighbors; i++)
	        {
	            int testNeighborIndex = GetNeighbor(index, (eNeighborDirection)i);
				if ( testNeighborIndex	== neighborIndex )
				{
					return (eNeighborDirection)i;
				}
	        }
			
			return eNeighborDirection.kNoNeighbor;
		}
		
	    public int GetNeighbor(int index, eNeighborDirection neighborDirection)
	    {
			Vector3 neighborPos = GetCellCenter(index);
			
	        switch (neighborDirection)
	        {
	            case eNeighborDirection.kLeft:
					ConvertUtils.AccumulateH(ref neighborPos, -m_cellSize);
	                break;
	            case eNeighborDirection.kTop:
				ConvertUtils.AccumulateV(ref neighborPos, m_cellSize);
	                break;
	            case eNeighborDirection.kRight:
				ConvertUtils.AccumulateH(ref neighborPos, m_cellSize);
	                break;
	            case eNeighborDirection.kBottom:
				ConvertUtils.AccumulateV(ref neighborPos, -m_cellSize);
	                break;
	            default:
	                System.Diagnostics.Debug.Assert(false);
	                break;
	        };
			
			int neighborIndex = GetCellIndex(neighborPos);
			if ( !IsInBounds(neighborIndex) )
			{
				neighborIndex = (int)eNeighborDirection.kNoNeighbor;
			}
	
	        return neighborIndex;
	    }
		#endregion
	}
	
	/// <summary>
	/// Given a list of points, this class computes portals on a grid terrain. Portals are used for path smoothing. 
	/// </summary>
	public static class GridPortalComputer
	{
		public static void ComputePortals(Vector3[] roughPath, PathGrid grid, out Vector3[] aLeftEndPts, out Vector3[] aRightEndPts)
		{
			aLeftEndPts = null;
			aRightEndPts = null;
			
			if ( roughPath.Length < 2 )
			{
				return;
			}
			
			aLeftEndPts = new Vector3[roughPath.Length-1];
			aRightEndPts = new Vector3[roughPath.Length-1];
			
			for ( int i = 0; i < roughPath.Length - 1; i++ )
			{
				Vector3 currentPos = roughPath[i];
				Vector3 nextPos = roughPath[i+1];
				Vector3 leftPoint, rightPoint;
				ComputePortal(currentPos, nextPos, grid, out leftPoint, out rightPoint);
				aLeftEndPts[i] = leftPoint;
				aRightEndPts[i] = rightPoint;
			}
		}
		
		private static void ComputePortal(Vector3 startPos, Vector3 endPos, PathGrid grid, out Vector3 leftPoint, out Vector3 rightPoint)
		{
			leftPoint = Vector3.zero;
			rightPoint = Vector3.zero;
			
			int startCellIndex = grid.GetCellIndex(startPos);
			int endCellIndex = grid.GetCellIndex(endPos);
			
			Bounds startCellBounds = grid.GetCellBounds(startCellIndex);
			PathGrid.eNeighborDirection neighborDirection = grid.GetNeighborDirection(startCellIndex, endCellIndex);
			float thirdValue = ConvertUtils.ThirdValue( grid.Origin );
			switch (neighborDirection)
			{
			case PathGrid.eNeighborDirection.kTop:
				leftPoint = ConvertUtils.PosByHV(ConvertUtils.HorizontalValue(startCellBounds.min),
				                                 ConvertUtils.VerticalValue(startCellBounds.max),
				                                 thirdValue);//new Vector3(startCellBounds.min.x, grid.Origin.y, startCellBounds.max.z);
				rightPoint =ConvertUtils.PosByHV(ConvertUtils.HorizontalValue(startCellBounds.max),
				                                 ConvertUtils.VerticalValue(startCellBounds.max),
				                                 thirdValue);//new Vector3(startCellBounds.max.x, grid.Origin.y, startCellBounds.max. z);
				break;
			case PathGrid.eNeighborDirection.kRight:
				leftPoint = ConvertUtils.PosByHV(ConvertUtils.HorizontalValue(startCellBounds.max),
				                                 ConvertUtils.VerticalValue(startCellBounds.max),
				                                 thirdValue);//new Vector3(startCellBounds.max.x, grid.Origin.y, startCellBounds.max. z);
				rightPoint = ConvertUtils.PosByHV(ConvertUtils.HorizontalValue(startCellBounds.max),
				                                  ConvertUtils.VerticalValue(startCellBounds.min),
				                                  thirdValue);//new Vector3(startCellBounds.max.x, grid.Origin.y, startCellBounds.min. z);
				break;
			case PathGrid.eNeighborDirection.kBottom:
				leftPoint = ConvertUtils.PosByHV(ConvertUtils.HorizontalValue(startCellBounds.max),
				                                 ConvertUtils.VerticalValue(startCellBounds.min),
				                                 thirdValue);//new Vector3(startCellBounds.max.x, grid.Origin.y, startCellBounds.min. z);
				rightPoint = ConvertUtils.PosByHV(ConvertUtils.HorizontalValue(startCellBounds.min),
				                                  ConvertUtils.VerticalValue(startCellBounds.min),
				                                  thirdValue);//new Vector3(startCellBounds.min.x, grid.Origin.y, startCellBounds.min. z);
				break;
			case PathGrid.eNeighborDirection.kLeft:
				leftPoint = ConvertUtils.PosByHV(ConvertUtils.HorizontalValue(startCellBounds.min),
				                                 ConvertUtils.VerticalValue(startCellBounds.min),
				                                 thirdValue);//new Vector3(startCellBounds.min.x, grid.Origin.y, startCellBounds.min. z);
				rightPoint = ConvertUtils.PosByHV(ConvertUtils.HorizontalValue(startCellBounds.min),
				                                  ConvertUtils.VerticalValue(startCellBounds.max),
				                                  thirdValue);//new Vector3(startCellBounds.min.x, grid.Origin.y, startCellBounds.max. z);
				break;
			default:
				UnityEngine.Debug.LogError("ComputePortal failed to find a neighbor");
				break;
			};
		}
	};

}