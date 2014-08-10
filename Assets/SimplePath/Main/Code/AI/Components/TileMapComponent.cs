using UnityEngine;
using System.Collections;
using SimpleAI.Navigation;

[RequireComponent(typeof(PathGridComponent))]
public class TileMapComponent : MonoBehaviour
{

		PathGridComponent m_pathGridComponent;
		PathGrid m_pathGrid;
		// Use this for initialization
		void Start ()
		{
			m_pathGridComponent = gameObject.GetComponent<PathGridComponent>();
			m_pathGrid = m_pathGridComponent.PathGrid;
		}
	
		// Update is called once per frame
		void Update ()
		{
			
		}
}

