using UnityEngine;
using System.Collections;

public class BomberExplose : MonoBehaviour
{
		ExplosionManger m_ExplosionManger;
		// Use this for initialization
		void Start ()
		{
		m_ExplosionManger	= this.gameObject.GetComponent<ExplosionManger>();
		if(m_ExplosionManger == null)
			Debug.LogError("m_ExplosionManger == null");
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

	public void StartExplose(){
		Debug.Log("Explose");
		m_ExplosionManger.Explose();
	}

	public void ReturnToPool(){
		this.gameObject.transform.position = CommonHelper.HidenPos3D;
		BomberManager.Instance.RecycleBomber(this.gameObject);
	}
}

