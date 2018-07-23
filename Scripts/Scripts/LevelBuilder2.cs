using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder2 {

	/*
	 Could have much better privacy status for structs. Made use funtions;
	 Better random choosing of next edge
	 Most of the class can never have a vert with the degree with more than 4. Espiecally FillCylce(); If the if of positions.x
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
		public Vertices(bool act, int amountOfedges, float x, float y, int m_index)
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
	private int activeVertAmount = 0; //Can used to get the amount of active verts or to get the next empty index in pathArray. HOWEVER BEFORE INC'ING IT, inc after activating another vert. eg Init()



	void Start()
	{

	}
	void Update()
	{

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
		verts[midIndex].pathIndex = activeVertAmount;
		activeVertAmount++;
		verts[midIndex].index = midIndex;
		ActivateConnectedEdges(ref verts[midIndex]);
		vertsPath[verts[midIndex].pathIndex] = verts[midIndex];
		lastVert = verts[midIndex];

		//Tests

	}
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
	public void FillCylce(int indexStart, int indexEnd)
	{
		int cycleLenth = (activeVertAmount + 1) - verts[indexEnd].pathIndex; //Haven't acrtivated the indexStart vert thus plus 1
		if (cycleLenth < 8)
			return;
		Vertices[] cycle = new Vertices[cycleLenth];
		for (int i = 0; i < cycleLenth; i++) // Prehaps ???
		{
			cycle[i] = verts[verts[indexStart].pathIndex - i];
		}
		bool Check = false; //False is the end vert is to the left or right; true is the end vert in above or below 
		if (verts[indexStart].pos.x == verts[indexEnd].pos.x)
			Check = true;

		if (Check)
		{
			//Check up
			
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
				Debug.Log("Degree: " + verts[i].degree + "Index: " + verts[i].index + "Pos: " + verts[i].pos);
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
	public int GetVertConnectedToEdgeIndex(bool statusVert, Vertices vertCase, Vertices vertIgnor)
	{
		for (int i = 0; i < vertCase.degree; i++)
		{
			int index = vertCase.edge[i].indexTo;
			if (index != vertIgnor.index && verts[index].active == statusVert)
			{
				return vertIgnor.index;
			}
		}
		return -1;
	}
	public Vertices ActivateVert(Vertices vert)
	{
		int index = vert.index;
		verts[index].active = true;
		verts[index].pathIndex = activeVertAmount;
		activeVertAmount++;
		ActivateConnectedEdges(ref verts[index]);
		vertsPath[verts[index].pathIndex] = verts[index];
		lastVert = verts[midIndex];
		return verts[index];
	}
	public Vertices ActivateRandomAdjacentVert(Vertices vert)
	{
		int dir = Random.Range(0, vert.degree);
		int indexTo = vert.edge[dir].indexTo;
		ActivateVert(indexTo);
		return GetVert(indexTo);
	}
	public int ActivateVert(int index)
	{
		verts[index].active = true;
		verts[index].pathIndex = activeVertAmount;
		activeVertAmount++;
		ActivateConnectedEdges(ref verts[index]);
		vertsPath[verts[index].pathIndex] = verts[index];
		lastVert = verts[midIndex];
		return verts[index].pathIndex;
	}
	public int ActivateVert(Vector2 pos)
	{
		int index = GetIndex((int)pos.x, (int)pos.y);
		verts[index].active = true;
		verts[index].pathIndex = activeVertAmount;
		activeVertAmount++;
		ActivateConnectedEdges(ref verts[index]);
		vertsPath[verts[index].pathIndex] = verts[index];
		lastVert = verts[midIndex];
		return verts[index].pathIndex;
	}
	public int GetIndex(int x, int y)
	{
		return x * (int)RoadCountWidth + y;
	}
	public int GetStatusVertAmount(bool activeState)
	{
		if (activeState)
			return activeVertAmount;
		else
			return (int)RoadCountHeight * (int)RoadCountWidth - activeVertAmount;
	}
	public Vertices GetVertPath(int index)
	{
		return vertsPath[index];
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
	public uint GetRoadCountWidth()
	{
		return RoadCountWidth;
	}
	public uint GetRoadCountHeight()
	{
		return RoadCountHeight;
	}



}
