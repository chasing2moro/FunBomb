using UnityEngine;
using System.Collections;
using SimpleAI.Navigation;

public interface ISteeringAgent{
	void SteerAlongPath (Vector3[] path, IPathTerrain pathTerrain);
	
	void StopSteering ();
}
