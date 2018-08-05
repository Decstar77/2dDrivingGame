using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleLevelGenerator : MonoBehaviour {


	[SerializeField] GameObject PrefavStrRoad;
	[SerializeField] GameObject PrefavTurnRoad;
	[SerializeField] GameObject PrefavRoadFinish;
	[SerializeField] GameObject PrefavTree;
	[SerializeField] GameObject PrefavRock;
	[SerializeField] uint Height = 10;
	[SerializeField] uint Width = 10;
	[SerializeField] uint RoadLength = 10;

	VBuilds build = new VBuilds();
	RoadManager roadManager = new RoadManager();

	GameObject[] m_Trees;
	GameObject[] m_Rock;
	GameObject FinishRoad;
	RoadManager.Roads[] roads;
	private float pPixels = 50;
	private float sizeOfRoad = 128;
	private float inGameUnitsSize;
	private uint Size;


	void Start () {
		inGameUnitsSize = sizeOfRoad / pPixels;
		Size = Height * Width;		

		roads = new RoadManager.Roads[Size];

		build.Init(Height, Width, RoadLength);
		roadManager.InitRoadArray(ref roads);

		//Init the root vert/road//
		roads[0].m_GameObjectPrefabType = PrefavStrRoad;
		roads[0].pos = build.GetRootVert().pos * inGameUnitsSize;
		roads[0].m_GameObjectPrefabType.transform.localPosition = roads[0].pos;
		
		Debug.Log(roads[0].m_GameObjectPrefabType.transform.rotation);
		roads[0].m_GameObjectInstance = Instantiate(roads[0].m_GameObjectPrefabType);

		//Debug.Log(build.GetRealativePosition(new Vector2(3, 3), new Vector2(3, 6)));		
		for (int i = 0; i < build.GetPathLength(); i++)
		{
			build.NextVert(); 
		}
		CreateNextRoad();
		roads[0].m_GameObjectInstance.transform.rotation = roadManager.GetRoadDirection(Vector2.zero, build.GetRootVert().pos, build.GetVertPath(1).pos);
		//Create EndRoad
		

		//Fill the rest of the world - Tree
		int[] m_tree = build.GetStatusIntArr(false);
		int[] m_rock = build.GetStatusIntArr(true);
		m_Trees = new GameObject[m_tree.Length];
		m_Rock = new GameObject[m_rock.Length];
		for (int i = 0; i < m_rock.Length; i++)
		{
			VBuilds.Vertices temp = build.GetVert(m_rock[i]);
			if (temp.pathIndex == -1)
			{
				Vector2 pos = temp.pos * inGameUnitsSize;
				m_Rock[i] = PrefavRock;
				m_Rock[i].transform.position = pos;
				Instantiate(m_Rock[i]);
			}
		}
		for (int i = 0; i < m_tree.Length; i++)
		{			
			Vector2 pos = build.GetVert(m_tree[i]).pos * inGameUnitsSize;
			m_Trees[i] = PrefavTree;
			m_Trees[i].transform.position = pos;
			//PrefavTree.transform.position = pos;
			Instantiate(m_Trees[i]);
		}
	}

	private void CreateNextRoad()
	{
		int[] pathVert = build.GetIndexArr();
		RoadManager.RoadType type = RoadManager.RoadType.m_null;
		Quaternion angle;
		for (int i = 1; i < pathVert.Length; i++)
		{
			if (i + 1 == pathVert.Length)
				break;
			int index = pathVert[i];			
			int index0 = pathVert[i - 1];
			int index1 = pathVert[i + 1];
			Vector2 pos = build.GetVert(index).pos;
			Vector2 pos0 = build.GetVert(index0).pos;
			Vector2 pos1 = build.GetVert(index1).pos;			
			Vector2 Relative1 = build.GetRealativePosition(pos, pos0);
			Vector2 Relative2 = build.GetRealativePosition(pos, pos1);
			Vector2 ResultantDirection = Relative1 + Relative2;
			type = roadManager.GetRoadType(ResultantDirection);
			angle = roadManager.GetRoadDirection(ResultantDirection, pos, pos0);
			switch (type)
			{
				case RoadManager.RoadType.roadstraight: roads[i].m_GameObjectPrefabType = PrefavStrRoad; break;
				case RoadManager.RoadType.roadTurn: roads[i].m_GameObjectPrefabType = PrefavTurnRoad; break;
			}
			roads[i].pos = pos;
			roads[i].m_GameObjectPrefabType.transform.position = pos * inGameUnitsSize;
			roads[i].m_GameObjectPrefabType.transform.rotation = angle;
			roads[i].m_GameObjectInstance = Instantiate(roads[i].m_GameObjectPrefabType);
			//Debug.Log(ResultantDirection); 			
		}
		//Create Finsih Road
		int ind = pathVert.Length - 1;
		roads[ind].pos = build.GetLastVert().pos;
		roads[ind].m_GameObjectPrefabType = PrefavRoadFinish;
		roads[ind].m_GameObjectPrefabType.transform.position = roads[ind].pos * inGameUnitsSize;
		roads[ind].m_GameObjectPrefabType.transform.rotation = roadManager.GetRoadDirection(Vector2.zero, roads[ind].pos, roads[ind-1].pos);
		FinishRoad = roads[ind].m_GameObjectInstance = Instantiate(roads[ind].m_GameObjectPrefabType);

	}		
	public Vector2 GetStart(ref Quaternion angle)
	{
		if (roads[1].m_GameObjectInstance.transform.localPosition.x > roads[0].m_GameObjectInstance.transform.localPosition.x)
		{
			angle = Quaternion.Euler(0, 0, -90);
		}
		if (roads[1].m_GameObjectInstance.transform.localPosition.x < roads[0].m_GameObjectInstance.transform.localPosition.x)
		{
			angle = Quaternion.Euler(0, 0, 90); Debug.Log(roads[1].pos.x + "  " + roads[0].pos.x);
		}
		else if (roads[1].m_GameObjectInstance.transform.localPosition.y > roads[0].m_GameObjectInstance.transform.localPosition.y)
		{
			angle = Quaternion.Euler(0, 0, 0);
		}
		else if (roads[1].m_GameObjectInstance.transform.localPosition.y < roads[0].m_GameObjectInstance.transform.localPosition.y)
		{
			angle = Quaternion.Euler(0, 0, 180);
		}
		else
		{
			Debug.Log("Problem");
			angle = Quaternion.Euler(0, 0, 0);
		}
		return roads[0].pos;
	}
	public GameObject GetFinishRoad()
	{
		return FinishRoad;
	}
}
