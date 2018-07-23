using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TestVerts : MonoBehaviour {

	[SerializeField] private GameObject m_vert;


	void Start () {
		LevelBuilder2 le = new LevelBuilder2();
		le.Init();
		for (int i = 0; i < le.GetRoadCountWidth() * le.GetRoadCountHeight(); i++)
		{
			GameObject obj = m_vert;
			Vector2 pos = le.GetVert(i).pos;
			obj.transform.position = pos;
			Instantiate(obj);
		}
	}
	

	void Update () {
		
	}
}
