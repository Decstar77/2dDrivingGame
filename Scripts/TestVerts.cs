using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TestVerts : MonoBehaviour {

	[SerializeField] private GameObject m_vert;
	[SerializeField] private Material on;
	[SerializeField] private Material off;
	[SerializeField] private Material path;
	[SerializeField] private bool isOn = true;
	VBuilds le = new VBuilds();
	GameObject[] vVerts = new GameObject[10 * 10]; int it = 11;
	void Start () {


		le.Init();
		if (!isOn)
			return;
		

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
		VBuilds.Vertices tempVert = le.GetRootVert();
		for (int i = 0; i < 12; i++)
		{
			le.NextVert();
		}
		ChangeColour(vVerts, le);
	}
	

	void Update () {
		bool inp = Input.GetButtonUp("Jump");
		bool inp2 = Input.GetButtonUp("Fire1");
		if (inp)
		{
			le.RemoveFromPath(it--);
			Debug.Log(le.GetVertPath(4).active);
			ChangeColour(vVerts, le);
		}
		if (inp2)
		{
			le.NextVert();
			ChangeColour(vVerts, le);
		}
	}

	private void ChangeColour(GameObject[] vVerts, VBuilds le)
	{
		// Change colour of game objects
		int[] inactArr = le.GetStatusIntArr(false);
		int[] actArr = le.GetStatusIntArr(true);
		int[] pathArr = le.GetIndexArr();
		for (int i = 0; i < inactArr.Length; i++)
		{
			vVerts[inactArr[i]].GetComponent<MeshRenderer>().material = off;
		}
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
