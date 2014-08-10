using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectPool 
{
	
	protected Queue<GameObject>                  m_GameObjectPool;
	protected string											m_GameObjectPath;
	protected GameObject 								m_StuffGameObject;


	//Initialize
	public GameObjectPool(string gameObjectPath){
		Init(gameObjectPath);
	}
	public GameObjectPool(){
	}
//	~ GameObjectPool(){
//		Debug.Log("~GameObjectPool()");
//		Release();
//	}
	void Dispose() {
		Debug.Log("Dispose()");
		Release();
	}

	public virtual void Init(string gameObjectPath){
		m_GameObjectPath = gameObjectPath;
		m_GameObjectPool = new Queue<GameObject> ();
		m_StuffGameObject = Resources.Load<GameObject>(m_GameObjectPath);
		if(m_StuffGameObject == null)
			UnityEngine.Debug.LogError("m_StuffGameObject == null");
	}
	public void Release(){
		foreach (GameObject item in m_GameObjectPool) {
			GameObject.Destroy(item);
		}
		m_GameObjectPool.Clear();

	//	if(m_StuffGameObject != null)
		//	Resources.UnloadAsset(m_StuffGameObject);
	}

	//Take object
	public GameObject DequeueFromPool(){
		if (m_GameObjectPool.Count == 0) {
			GameObject tempGameObj = GameObject.Instantiate(m_StuffGameObject) as GameObject;
			if(tempGameObj == null)
				UnityEngine.Debug.LogError("tempGameObj == null");
			return tempGameObj;
		}
		return m_GameObjectPool.Dequeue ();
	}
	
	public bool EnqueueToPool(GameObject gameObject){
		if(m_GameObjectPool.Contains(gameObject)){
			Debug.LogError(gameObject + "is already in queue");
			return false;
		}
		m_GameObjectPool.Enqueue(gameObject);
		return true;
	}

}

