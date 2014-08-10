using UnityEngine;
using System.Collections;

public class BomberPool : GameObjectPool
{
	internal static BomberPool _instance = new BomberPool();
	public static BomberPool Instance 
	{ 
		get {
			if( null == _instance ) _instance = new BomberPool();
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

