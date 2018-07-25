using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder2 {

	/*
	 Could have much better privacy status for structs. Made use funtions;
	 Better random choosing of next edge
	 Most of the class can never have a vert with the degree with more than 4. Espiecally FillCylce(); If the if of positions.x
	 Ativevert() TEMPLATE PLZ!!!

	*/


	[SerializeField] private uint RoadCountWidth = 10;
	[SerializeField] private uint RoadCountHeight = 10;
	public struct Edges
	{
		public int indexFrom;
		public int indexTo;
	}

	public struct Vertices
	{
		public Edges[] edge;
		public Vector2 pos;
		public bool active;
		public int degree;
		public int index;
		public int pathIndex;
		public int activeEdges;
		public Vertices(bool act, int amountOfedges, float x, float y, int m_index = -1)
		{
			edge = new Edges[amountOfedges];
			degree = amountOfedges;
			pos.x = x;
			pos.y = y;
			active = act;
			index = m_index;
			pathIndex = -1;
			activeEdges = 0;
		}
	}

	private Vertices[] verts;
	private Vertices[] vertsPath;
	private Vertices lastVert;
	private float sizeOfUnit;
	private int midIndex = 0;
	private int pathVertAmount = 0; //Can used to get the amount of active verts or to get the next empty index in pathArray. HOWEVER BEFORE INC'ING IT, inc after activating another vert. eg Init()
	private int activeVertAmount = 0;

	private void InitializeVerts()
	{

		for (int x = 0; x < RoadCountWidth; x++)
		{
			for (int y = 0; y < RoadCountHeight; y++)
			{
				bool left = true;
				bool right = true;
				bool up = true;
				bool down = true;
				int amountOfEdge = 4;
				int index = GetIndex(x, y);
				if (x - 1 < 0)
				{
					amountOfEdge--;
					left = false;
				}
				if (x + 1 == RoadCountWidth)
				{
					amountOfEdge--;
					right = false;
				}
				if (y - 1 < 0)
				{
					amountOfEdge--;
					down = false;
				}
				if (y + 1 == RoadCountHeight)
				{
					amountOfEdge--;
					up = false;
				}
				verts[index] = new Vertices(false, amountOfEdge, x, y, index);
				for (int i = 0; i < amountOfEdge; i++)
				{
					verts[index].edge[i].indexFrom = index;
					if (left)
					{
						verts[index].edge[i].indexTo = GetIndex(x - 1, y);
						left = false;
						continue;
					}
					if (right)
					{
						verts[index].edge[i].indexTo = GetIndex(x + 1, y);
						right = false;
						continue;
					}
					if (up)
					{
						verts[index].edge[i].indexTo = GetIndex(x, y + 1);
						up = false;
						continue;
					}
					if (down)
					{
						verts[index].edge[i].indexTo = GetIndex(x, y - 1);
						down = false;
						continue;
					}
				}

			}
		}
	}
	void Start()
	{

	}
	void Update()
	{

	}
	public bool CompareVerts(Vertices vert1, Vertices vert2)
	{
		if (vert1.active == vert2.active &&
			vert1.index == vert2.index &&
			vert1.pos == vert2.pos)
			return true;
		return false;
	}
	public bool CheckVecArray(Vector2 needsChecking, Vector2[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (needsChecking == array[i])
				return true;
		}
		return false;
	}
	public bool CheckVertArray(Vertices needChecking, Vertices[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (CompareVerts(needChecking, array[i]))
				return true;
		}
		return false;
	}
	public void Init()
	{
		sizeOfUnit = 128;
		verts = new Vertices[RoadCountWidth * RoadCountHeight];
		vertsPath = new Vertices[25];
		InitializeVerts();
		Vector2 middle = new Vector2(RoadCountWidth / 2, RoadCountHeight / 2);
		midIndex = GetIndex((int)middle.x, (int)middle.y);
		verts[midIndex].active = true;
		activeVertAmount++;
		verts[midIndex].pathIndex = pathVertAmount;
		pathVertAmount++;
		verts[midIndex].index = midIndex;
		ActivateConnectedEdges(ref verts[midIndex]);
		vertsPath[verts[midIndex].pathIndex] = verts[midIndex];
		lastVert = verts[midIndex];

		//Tests

	}	
 	public void ActivateInbetweens(Vertices vert1, Vertices vert2, bool AddToPath = true)
	{
		if (vert1.pos.x == vert2.pos.x && vert1.pos.y == vert2.pos.y)
		{
			Debug.Log("Verts have no Inbetweens");
			return;
		}
		bool dir = false;//False is the end vert is to the left or right; true is the end vert in above or below 
		int incDir = 0;
		if (vert1.pos.x == vert2.pos.x)
			dir = true;
			
		if (dir)
		{
			int amounts = Mathf.Abs((int)vert1.pos.y - (int)vert2.pos.y) -1;
			if (amounts == 0)
				return;	

			if (vert1.pos.y > vert2.pos.y)
			{
				incDir = 1; 
			}
			else
			{
				incDir = -1;
			}
			for (int i = 1; i <= amounts; i++)
			{
				Vector2 vec = new Vector2(vert2.pos.x, vert2.pos.y + incDir*i);
				int index = GetIndex(vec);
				if (verts[index].active)
					continue;
				ActivateVert(index, false, AddToPath);
			}
		}
		else 
		{
			int amounts = Mathf.Abs((int)vert1.pos.x - (int)vert2.pos.x) -1 ;
			Debug.Log("sd" + amounts);
			if (amounts == 0)
				return;
			
			if (vert1.pos.x > vert2.pos.x)
			{
				incDir = 1; 
			}
			else
			{
				incDir = -1;
			}
			for (int i = 1; i <= amounts; i++)
			{
				Vector2 vec = new Vector2(vert2.pos.x + incDir * i, vert2.pos.y);
				int index = GetIndex(vec);
				if (verts[index].active)
					continue;
				ActivateVert(index, false, AddToPath);
			}
		}
	}
	public void FillCylce(int indexStart, int indexEnd)
	{
		int cycleLenth = Mathf.Abs(verts[indexStart].pathIndex - verts[indexEnd].pathIndex) + 1; //Haven't acrtivated the indexStart vert thus plus 1
		Debug.Log("CycleLength: " + cycleLenth);
		if (cycleLenth < 8)
			return;
		Vertices[] cycle = new Vertices[cycleLenth];
		for (int i = 0; i < cycleLenth; i++) // Prehaps ???
		{
			cycle[i] = GetVertPath(verts[indexStart].pathIndex - i);
		}
		
		Vertices vertStart = GetVert(indexStart);
		Vertices vertEnd = GetVert(indexEnd);
		if (vertStart.pos.y == vertEnd.pos.y)
		{
			Debug.Log("Starting fill");
			for (int i = 0; i < cycleLenth; i++)
			{
				for (int ii = 0; ii < cycleLenth; ii++)
				{
					Vector2 posUp = new Vector2(cycle[i].pos.x, cycle[i].pos.y + 1 + ii);
					Vector2 posDown = cycle[i].pos + new Vector2(0, -1 - ii);
					if (posUp.y >= RoadCountHeight)
					{
						Debug.Log("BreakingRoadCountHeight");
						break;
					}
					if (posDown.y < 0)
					{
						Debug.Log("BreakingRoadCountHeight");
						break;
					}
					if (CheckVertArray(GetVert(posUp), cycle))
					{
						if (ii == 0)
							break;
						Vertices vert = GetVert(GetIndex(posUp));
						ActivateInbetweens(vert, cycle[i], false);
						Debug.Log("FoundUp: pos ");
						break;
					}
					if (CheckVertArray(GetVert(posDown), cycle))
					{
						if (ii == 0)
							break;
						Vertices vert = GetVert(GetIndex(posDown));
						ActivateInbetweens(vert, cycle[i], false);
						Debug.Log("FoundDown");	
						break;
					}
					
				}
					
			}
		}
		else
		{

		}


	}
	public void NextVert()
	{
		int dir = Random.Range(0, lastVert.degree);
		int index = lastVert.edge[dir].indexTo;
		for (int i = 0; verts[index].active == true; i++)
		{
			if (i == 4)
			{ 
				Debug.Log("Probs");
				return;
			}
			dir = i;
			index = lastVert.edge[dir].indexTo;
		}// Redo somehow


		if (verts[index].activeEdges > 1)
		{
			int indexEnd = GetVertConnectedToEdgeIndex(true, verts[index], lastVert);
			Debug.Log("Cycle, index");
			ActivateVert(index);
			FillCylce(index, indexEnd);	
		}
		else
		{
			ActivateVert(index);
		}
	}
	public void DisplayVertsArray(Vertices[] Verts = null)
	{
		if (Verts != null)
		{
			for (int i = 0; i < RoadCountWidth * RoadCountHeight; i++)
			{
				Debug.Log(Verts[i].pos);
				Debug.Log(Verts[i].degree);
			}
		}
		else
		{
			for (int i = 0; i < verts.Length; i++)
			{
				Debug.Log(verts[i].pos);
				Debug.Log(verts[i].degree);
			}
		}
	}
	public void DisplayActiveVerts(bool activeStatus)
	{
		for (int i = 0; i < RoadCountWidth * RoadCountHeight; i++)
		{
			if (verts[i].active == activeStatus)
			{
				DisplayVert(verts[i]);
			}
		}
	}
	public void DisplayVert(Vertices vert)
	{
		Debug.Log("//////////////////////START////////////////////////////////");
		Debug.Log("PathIndex: " + vert.pathIndex + " TrueIndex: " + vert.index);
		Debug.Log("Position: " + vert.pos + " Active: " + vert.active);
		Debug.Log("Degree: " + vert.degree + " ActiveEdge: " + vert.activeEdges);
		Debug.Log("//////////////////////END//////////////////////////////////");
	}
	public void DisplayEdge(Vertices vert)
	{
		for (int i = 0; i < vert.degree; i++)
		{
			Debug.Log("Edge indexfrom: " + vert.edge[i].indexFrom + " Edge indexTo: " + vert.edge[i].indexTo + " i:" + i);
		}
	}
	public void ActivateConnectedEdges(ref Vertices vert)
	{
		for (int i = 0; i < vert.degree; i++)
		{
			int indexTo = vert.edge[i].indexTo;
			if (verts[indexTo].activeEdges < verts[indexTo].degree) // To stop active edge overlapping the max degree of 4
				verts[indexTo].activeEdges++;
			//Debug.Log("TEstsss" + indexTo);
		}
		vert.activeEdges = vert.degree;
	}
	public void ChangeVert(int index, Vertices vert)
	{
		verts[index] = vert;
	}
	public int GetVertConnectedToEdgeIndex(bool statusVert, Vertices vertCase, Vertices vertIgnor)
	{
		for (int i = 0; i < vertCase.degree; i++)
		{
			int index = vertCase.edge[i].indexTo;
			if (index != vertIgnor.index && verts[index].active == statusVert)
			{
				return index;
			}
		}
		return -1;
	}
	public int ActivateVert(Vertices vert, bool SetLastVert = true, bool AddToPath = true)
	{
		int index = vert.index;
		verts[index].active = true;
		activeVertAmount++;
		if (AddToPath)
		{
			verts[index].pathIndex = pathVertAmount;
			vertsPath[verts[index].pathIndex] = verts[index];
			pathVertAmount++;
		}
		ActivateConnectedEdges(ref verts[index]);		
		if (SetLastVert)
			lastVert = verts[index];
		return verts[index].pathIndex;
	}
	public int ActivateVert(int index, bool SetLastVert = true, bool AddToPath = true)
	{
		verts[index].active = true;
		activeVertAmount++;
		if (AddToPath)
		{
			verts[index].pathIndex = pathVertAmount;
			vertsPath[verts[index].pathIndex] = verts[index];
			pathVertAmount++;
		}
		ActivateConnectedEdges(ref verts[index]);		
		if (SetLastVert)
			lastVert = verts[index];
		return verts[index].pathIndex;
	}
	public int ActivateVert(Vector2 pos, bool SetLastVert = true, bool AddToPath = true)
	{
		int index = GetIndex((int)pos.x, (int)pos.y);
		verts[index].active = true;
		activeVertAmount++;
		if (AddToPath)
		{
			verts[index].pathIndex = pathVertAmount;
			vertsPath[verts[index].pathIndex] = verts[index];
			pathVertAmount++;
		}
		ActivateConnectedEdges(ref verts[index]);		
		if (SetLastVert)
			lastVert = verts[index];
		return verts[index].pathIndex;
	}
	public int GetIndex(int x, int y)
	{
		if (x >= RoadCountWidth || y >= RoadCountHeight)
			return -1;
		return x * (int)RoadCountWidth + y;
	}
	public int GetIndex(Vector2 vec)
	{
		if (vec.x >= RoadCountWidth || vec.y >= RoadCountHeight)
			return -1;
		return (int)vec.x * (int)RoadCountWidth + (int)vec.y;
	}
	public int GetStatusVertAmount(bool activeState)
	{
		if (activeState)
			return pathVertAmount;
		else
			return (int)RoadCountHeight * (int)RoadCountWidth - pathVertAmount;
	}
	public Vertices GetVertConnectedToEdge(bool statusVert, Vertices vertCase, Vertices vertIgnor)
	{
		for (int i = 0; i < vertCase.degree; i++)
		{
			int index = vertCase.edge[i].indexTo;
			if (index != vertIgnor.index && verts[index].active == statusVert)
			{
				return vertIgnor;
			}
		}
		return new Vertices();
	}
	public Vertices ActivateRandomAdjacentVert(Vertices vert)
	{
		int dir = Random.Range(0, vert.degree);
		int indexTo = vert.edge[dir].indexTo;
		ActivateVert(indexTo);
		return GetVert(indexTo);
	}
	public Vertices GetVertPath(int pIndex)
	{
		if (pIndex < 0 || pIndex > pathVertAmount)
			return new Vertices();
		return vertsPath[pIndex];
	}
	public Vertices GetVert(int index)
	{
		return verts[index];
	} //No checks
	public Vertices GetVert(Vector2 pos)
	{
		return verts[GetIndex((int)pos.x, (int)pos.y)];
	}	//No checks
	public Vertices GetRootVert()
	{
		return verts[midIndex];
	}
	public Vertices[] GetStatusVertsArr(bool activeState)
	{
		Vertices[] rVerts;
		int lastCount = 0;
		if (activeState)
			rVerts = new Vertices[activeVertAmount];
		else
			rVerts = new Vertices[RoadCountHeight * RoadCountWidth - activeVertAmount];

		for (int i = 0; i < activeVertAmount; i++)
		{
			for (int ii = lastCount; ii < RoadCountHeight * RoadCountWidth; ii++)
			{
				if (verts[ii].active == activeState)
				{
					rVerts[i] = verts[ii];
					lastCount = ii + 1;
					break;
				}
			}
		}
		return rVerts;
	}
	public int[] GetStatusIntArr(bool activeState)
	{
		int[] arr;
		int lastCount = 0;
		if (activeState)
			arr = new int[activeVertAmount];
		else
			arr = new int[RoadCountHeight * RoadCountWidth - activeVertAmount];

		for (int i = 0; i < activeVertAmount; i++)
		{
			for (int ii = lastCount; ii < RoadCountHeight * RoadCountWidth; ii++)
			{
				if (verts[ii].active == activeState)
				{
					arr[i] = verts[ii].index;
					lastCount = ii + 1;
					break;
				}
			}
		}
		return arr;
	}
	public int[] GetIntPathArr()
	{
		int []arr = new int[pathVertAmount];
		for (int i = 0; i < pathVertAmount; i++)
		{
			arr[i] = vertsPath[i].index; 
		}
		return arr;
	}
	public uint GetRoadCountWidth()
	{
		return RoadCountWidth;
	}
	public uint GetRoadCountHeight()
	{
		return RoadCountHeight;
	}



}
