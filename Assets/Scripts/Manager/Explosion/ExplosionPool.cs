using UnityEngine;
using System.Collections;

public class ExplosionPool : GameObjectPool
{
	internal static ExplosionPool _instance = new ExplosionPool();
	public static ExplosionPool Instance 
	{ 
		get {
			if( null == _instance ) _instance = new ExplosionPool();
			return _instance;
		}
	}
	
	public static void PurgeInstance()
	{
		_instance = null;
	}

	public override void Init(string gameObjectPath){
		if(m_StuffGameObject == null)
			base.Init(gameObjectPath);
	}
}

