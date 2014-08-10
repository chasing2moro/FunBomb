using UnityEngine;
using System.Collections;

public class CameraHelper 
{
	public  static float DistanceToBackground(Camera camera, GameObject backgroound = null){
		if (backgroound == null)
			return Mathf.Abs (camera.transform.position.z);
		return Mathf.Abs(camera.transform.position.z - backgroound.transform.position.z);
	}

	public static Vector3 ScreenPosToBackgroundPos(Vector2 screenPos){
		Ray ray = Camera.main.ScreenPointToRay(screenPos);
		Vector3 interactPos = ray.GetPoint(CameraHelper.DistanceToBackground(Camera.main));
		return interactPos;
	}
}

