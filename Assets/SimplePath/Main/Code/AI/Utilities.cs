#define UseIn2D
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
using SystemDebug = System.Diagnostics.Debug;

namespace SimpleAI
{
	public static class FlagUtils
	{
		public static bool TestFlag(int flags, int flag)
		{
			return ((flags & flag) != 0);
		}
		
		public static int SetFlag(int flags, int flag)
		{
			return (flags |= flag);
		}
		
		public static int ClearFlag(int flags, int flag)
		{
			return (flags & ~flag);
		}
	}
	
	public static class PathUtils
	{
		public static bool AreVertsTheSame(Vector3 v1, Vector3 v2)
		{
			const float eps = 0.001f * 0.001f;
			return ((v1 - v2).sqrMagnitude < eps);
		}
		
		/// <summary>
		/// Get the clockwise angle from dir1 to dir2, in radians. This assumes +y is the up dir.
		/// </summary>
		/// <param name="dir1">
		/// Start vector. Must be normalized.
		/// </param>
		/// <param name="dir2">
		/// End vector. Rotate toward this vector from v1. Must be normalized.
		/// </param>
		/// <returns>
		/// Clockwise angle in radians
		/// </returns>
		public static float CalcClockwiseAngle(Vector3 dir1, Vector3 dir2)
		{
			dir1.y = 0.0f;
			dir2.y = 0.0f;
			dir1.Normalize();
			dir2.Normalize();
			
			// Find the clockwise angle
			Vector3 dir1Normal = Vector3.Cross(dir1, Vector3.up);
			dir1Normal.Normalize();
			float checkDirectionDot = Vector3.Dot(dir2, dir1Normal);    // dot for checking the direction of rotation (CW or CCW)
			float cwAngle = 0.0f;
			float dot = Vector3.Dot(dir1, dir2);                        // actual dot between the two vectors
			if (checkDirectionDot > 0.0f)
			{
				cwAngle = Mathf.PI * 2.0f - Mathf.Acos(dot);
			}
			else
			{
				cwAngle = Mathf.Acos(dot);
			}
			
			return cwAngle;
		}	
	}
	
	public static class ConvertUtils{
		public static float HorizontalValue(Vector3 vec){
			return vec.x;
		}
		
		public static float VerticalValue(Vector3 vec){
#if UseIn2D
			return vec.y;
#else
			return vec.z;
#endif
		}
		public static float ThirdValue(Vector3 vec){
			#if UseIn2D
			return vec.z;//Except Horizon &  Vertical Value
			#else
			return vec.y;
			#endif
		}

		public static Vector3 PosByHV(Vector3 HorizonVec, Vector3 VerticalVec, Vector3 thridValueVec){
			float Horizon = HorizontalValue(HorizonVec);
			float Vertical = VerticalValue(VerticalVec);
			float thirdValue = ThirdValue(thridValueVec);
			return PosByHV(Horizon, Vertical, thirdValue);
		}
		public static Vector3 PosByHV(float Horizon, float Vertical, float thirdValue = 0.0f){
#if UseIn2D
			return new Vector3(Horizon, Vertical, thirdValue);
#else
			return new Vector3(Horizon, thirdValue, Vertical);
#endif
		}
		public static void Accumulate(ref Vector3 origin, float Horizon, float Vertical){
			origin.x += Horizon;
#if UseIn2D
			origin.y += Vertical;
#else
			origin.z += Vertical;
#endif
		}
		public static void AccumulateH(ref Vector3 origin, float Horizon){
			Accumulate (ref origin, Horizon, 0);
		}
		public static void AccumulateV(ref Vector3 origin, float Vertical){
			Accumulate (ref origin, 0, Vertical);
		}
		public static void SetHorizontalValue(ref Vector3 origin, float Horizon){
			origin.x = Horizon;
		}
		public static void SetVerticalValue(ref Vector3 origin, float Vertical){
#if UseIn2D
			origin.y = Vertical;
#else
			origin.z = Vertical;
#endif
		}
		public static void SetThirdValue(ref Vector3 origin, float thirdValue){
			#if UseIn2D
			origin.z = thirdValue;
			#else
			origin.y = thirdValue;
			#endif
		}

	}
}

