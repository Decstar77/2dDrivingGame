using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TestVerts : MonoBehaviour {

	[SerializeField] private GameObject m_vert;
	[SerializeField] private Material on;
	[SerializeField] private Material path;

	void Start () {
		LevelBuilder2 le = new LevelBuilder2();
		
		le.Init();
		GameObject[] vVerts = new GameObject[le.GetRoadCountWidth() * le.GetRoadCountHeight()];


		for (int i = 0; i < le.GetRoadCountWidth() * le.GetRoadCountHeight(); i++)
		{
			GameObject obj = m_vert;
			Vector2 pos = le.GetVert(i).pos;
			bool act = le.GetVert(i).active;

			obj.transform.position = pos;
			vVerts[i] = Instantiate(obj);
			if (act)
			{
				vVerts[i].GetComponent<MeshRenderer>().material = on;
			}
		}
		LevelBuilder2.Vertices tempVert = le.GetRootVert();
		Vector2 pos1 = tempVert.pos + new Vector2(0, 1);
		Vector2 pos2 = tempVert.pos + new Vector2(0, 2);
		Vector2 pos3 = new Vector2(4, 7);
		Vector2 pos4 = new Vector2(3 ,7);
		Vector2 pos5 = new Vector2(3, 6);
		Vector2 pos6 = new Vector2(3, 5);
		le.ActivateVert(pos1);
		le.ActivateVert(pos2);
		le.ActivateVert(pos3);
		le.ActivateVert(pos4);
		le.ActivateVert(pos5);
		le.ActivateVert(pos6);

		/*tempVert = le.GetVert(pos7);
		tempVert.active = true;
		le.ChangeVert(tempVert.index, tempVert);

		tempVert = le.GetVert(pos8);
		tempVert.active = true;
		le.ChangeVert(tempVert.index, tempVert);*/

		le.NextVert();
		le.NextVert();
		le.NextVert();
		le.NextVert();
		le.NextVert();
		le.NextVert();
		le.NextVert();
		le.NextVert();
		ChangeColour(vVerts, le);
	}
	

	void Update () {
		
	}

	private void ChangeColour(GameObject[] vVerts, LevelBuilder2 le)
	{
		// Change colour of game objects
		int[] actArr = le.GetStatusIntArr(true);
		int[] pathArr = le.GetIntPathArr();
		for (int i = 0; i < actArr.Length; i++)
		{
			vVerts[actArr[i]].GetComponent<MeshRenderer>().material = on;
		}
		for (int i = 0; i < pathArr.Length; i++)
		{
			vVerts[pathArr[i]].GetComponent<MeshRenderer>().material = path;
		}
	}
}
