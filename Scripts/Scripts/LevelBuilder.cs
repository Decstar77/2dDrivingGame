using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelBuilder : MonoBehaviour {

	/*
	Notes/Todo
	Make it so that the less a roadtype is used that the probability of being selected of said roadtype increases
	StreamLine BuildStraightRoad() with SimplifyDirection():
		Created function SimplifyDirection which simplfies/normilzes the a direction. Usefull for creating staights. 
	Seems that the finish line is placed on top of prexisting road
		:Made a solution but was forgeting to change position. Thus this occurs. Rewrite with out lines 150. CreateSingleLevel() and make it change the postion instead. 
		:Will this now take into account when broken ??/ Seems to work when broken just fine.
	C# wat. Why you no like pointers!?!?
	lol what is happening ?
	*/

	[SerializeField] private GameObject rStraight;
	[SerializeField] private GameObject rTurn;
	[SerializeField] private GameObject rFinished;
	[SerializeField] private bool inFiniteLevel = false;
	[SerializeField] private const int RoadCount = 100;
	[SerializeField] private float padding = 0.01f;


	public enum RoadType
	{
		m_null = -1,
		roadstraight = 0,
		roadTurn = 1,
		roadCheckerd = 2
	}
	public enum RoadDirection
	{
		m_null = -1,
		up = 0,
		down = 1,
		left = 2,
		right = 3,
		right_to_up = 4,
		right_to_down = 5,
		left_to_up = 6,
		left_to_down = 7,
		up_to_right = 8,
		up_to_left = 9,
		down_to_right = 10,
		down_to_left = 11

	}
	private struct Roads
	{
		public RoadType type;
		public RoadDirection direction;
		public Quaternion rotation;
		public Vector2 pos;
		public GameObject m_GameObjectPrefabType;
		public GameObject m_GameObjectInstance;
		public SpriteRenderer m_renderer;
		public bool isActive;	//Gets passed to false when hit
		//Contructor
		public Roads(bool act)
		{
			type = RoadType.m_null;
			direction = RoadDirection.m_null;
			rotation = Quaternion.Euler(0, 0, 0);
			pos = new Vector2(0, 0);
			m_GameObjectPrefabType = null;
			m_GameObjectInstance = null;
			m_renderer = null;
			isActive = act;
		}

	}


	private Roads[] roads = new Roads[RoadCount];
	private Roads lastRoad;
	private Vector2 m_checked = new Vector2(0, 0);
	private Vector2[] roadPositions = new Vector2[RoadCount + 1];
	private MenuManager menu;

	private float sizeOfRoad = 128;
	private float ppOfRoad = 50;
	private float inGameUnitsSize;
	private float looserOfPicked = 0;
	private int PositionOfCheckeredFlagInArray = 0;
	private int LastAddRoad = 0;
	private bool CheckIfCompleted = false;


	void Start() {
		menu = GetComponent<MenuManager>();
		inGameUnitsSize = sizeOfRoad / ppOfRoad - padding;
		////RoadArray setup//////
		for (int i = 0; i < RoadCount; i++)
		{
			roads[i] = new Roads(false);
		}
		////LastRoad setup//////
		lastRoad = new Roads(true)
		{
			direction = RoadDirection.up,
			type = RoadType.roadstraight,
			pos = new Vector2(0, 0)
		};
		roadPositions[0] = lastRoad.pos;
		if (!inFiniteLevel)
		{
			CreateSingleLevel();
		}
		//StartCoroutine(CheckToDestoryRoad());
	}
	void FixedUpdate()
	{
		//Debug.Log("PositionOfCheckeredFlagInArray: " + PositionOfCheckeredFlagInArray);
		if (inFiniteLevel)
		{
			InFiniteLevelCreation();
		}
		if (!roads[PositionOfCheckeredFlagInArray].isActive && roads[PositionOfCheckeredFlagInArray].type == RoadType.roadCheckerd)
		{
			menu.RestartLevel();
		}
		

	}
	private bool CreateSingleLevel()
	{
		int tmp = 0;
		Random random = new Random();
		for (int i = 0; i < RoadCount; i++)
		{
			tmp = i;
			bool BuildSuccess = false;
			roads[i].type = (RoadType)Random.Range(0, 2);
			if (roads[i].type == RoadType.roadstraight)
			{
				BuildSuccess = BuildStraightRoad(i);
			}
			if (BuildSuccess)
				continue;

			BuildSuccess = BuildTurnRoad(i);
			if (!BuildSuccess)
			{
				CheckIfCompleted = FinishLevel(i);
				PositionOfCheckeredFlagInArray = i;
				Debug.Log("Broken " + i);
				return false;
			}						
		}
		PositionOfCheckeredFlagInArray = tmp;						//Was not changing the postion of the flag so this appeard to be the case. Not true, I think.
		lastRoad = roads[RoadCount - 2];							//This happens because 'lastRoad' is set to 'RoadCount - 1' on last intereration of the for loop
		CheckIfCompleted = FinishLevel(RoadCount -1);               //When checking for the last collion. Could be rewritten to take of such exception, in theory.	Debug.Log(roads[9].direction);Debug.Log(roads[8].direction);Debug.Log(lastRoad.direction);														
		return true;
	}
	private bool InFiniteLevelCreation()
	{
		DeleteRoad();
		for (int i = LastAddRoad; i < RoadCount; i++)
		{

			if (roads[i].isActive)
			{
				switch (roads[i].type)
				{
					case RoadType.roadstraight:
						if (roads[i].m_GameObjectInstance.GetComponent<SpriteRenderer>().isVisible == false)
						{
							LastAddRoad = i;
							return true;
						}
						break;
					case RoadType.roadTurn:
						if (roads[i].m_GameObjectInstance.GetComponentInChildren<SpriteRenderer>().isVisible == false)
						{
							LastAddRoad = i;
							return true;
						}
						break;
				}

			}

			bool done = false;
			roads[i].type = (RoadType)Random.Range(0, 2);
			if (roads[i].type == RoadType.roadstraight)
			{
				done = BuildStraightRoad(i);
			}
			if (done)
				continue;

			done = BuildTurnRoad(i);
			if (!done)
			{
				Debug.Log("Sheit");
				return false;
			}
		}
		LastAddRoad = 0;
		return true;

	}
	private bool CheckCollision(RoadDirection dir, Vector2 pos, Vector2[] roadsPos, ref Vector2 m_checked)
	{
		Vector2 CheckedPos;
		bool found;
		switch (dir)
		{
			case RoadDirection.up:
				CheckedPos = new Vector2(pos.x, pos.y + inGameUnitsSize); m_checked = CheckedPos;
				found = CheckVecArray(CheckedPos, roadsPos);
				return found;
			case RoadDirection.down:
				CheckedPos = new Vector2(pos.x, pos.y - inGameUnitsSize); m_checked = CheckedPos;
				found = CheckVecArray(CheckedPos, roadsPos);
				return found;
			case RoadDirection.right:
				CheckedPos = new Vector2(pos.x + inGameUnitsSize, pos.y); m_checked = CheckedPos;
				found = CheckVecArray(CheckedPos, roadsPos);
				return found;
			case RoadDirection.left:
				CheckedPos = new Vector2(pos.x - inGameUnitsSize, pos.y); m_checked = CheckedPos;
				found = CheckVecArray(CheckedPos, roadsPos);
				return found;
			case RoadDirection.left_to_up:
				CheckedPos = new Vector2(pos.x, pos.y + inGameUnitsSize); m_checked = CheckedPos;
				found = CheckVecArray(CheckedPos, roadsPos);
				return found;
			case RoadDirection.right_to_up:
				CheckedPos = new Vector2(pos.x, pos.y + inGameUnitsSize); m_checked = CheckedPos;
				found = CheckVecArray(CheckedPos, roadsPos);
				return found;
			case RoadDirection.left_to_down:
				CheckedPos = new Vector2(pos.x, pos.y - inGameUnitsSize); m_checked = CheckedPos;
				found = CheckVecArray(CheckedPos, roadsPos);
				return found;
			case RoadDirection.right_to_down:
				CheckedPos = new Vector2(pos.x, pos.y - inGameUnitsSize); m_checked = CheckedPos;
				found = CheckVecArray(CheckedPos, roadsPos);
				return found;
			case RoadDirection.down_to_right:
				CheckedPos = new Vector2(pos.x + inGameUnitsSize, pos.y); m_checked = CheckedPos;
				found = CheckVecArray(CheckedPos, roadsPos);
				return found;
			case RoadDirection.up_to_right:
				CheckedPos = new Vector2(pos.x + inGameUnitsSize, pos.y); m_checked = CheckedPos;
				found = CheckVecArray(CheckedPos, roadsPos);
				return found;
			case RoadDirection.down_to_left:
				CheckedPos = new Vector2(pos.x - inGameUnitsSize, pos.y); m_checked = CheckedPos;
				found = CheckVecArray(CheckedPos, roadsPos);
				return found;
			case RoadDirection.up_to_left:
				CheckedPos = new Vector2(pos.x - inGameUnitsSize, pos.y); m_checked = CheckedPos;
				found = CheckVecArray(CheckedPos, roadsPos);
				return found;
		}
		return false;
	}
	private bool BuildStraightRoad(int CurrentIncrement)
	{
		int i = CurrentIncrement;
		switch (lastRoad.direction)
		{
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			////////////////////////////////////////////From straights to straights//////////////////////////////////////////////////
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			case RoadDirection.up:
				roads[i].direction = lastRoad.direction;
				roads[i].pos = lastRoad.pos + new Vector2(0, inGameUnitsSize);
				break;
			case RoadDirection.down:
				roads[i].direction = lastRoad.direction;
				roads[i].pos = lastRoad.pos - new Vector2(0, inGameUnitsSize);
				break;
			case RoadDirection.left:
				roads[i].direction = lastRoad.direction;
				roads[i].pos = lastRoad.pos - new Vector2(inGameUnitsSize, 0);
				break;
			case RoadDirection.right:
				roads[i].direction = lastRoad.direction;
				roads[i].pos = lastRoad.pos + new Vector2(inGameUnitsSize, 0);
				break;
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			////////////////////////////////////////////From coners to straights/////////////////////////////////////////////////////
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			case RoadDirection.up_to_right:
				roads[i].direction = RoadDirection.right;
				roads[i].pos = lastRoad.pos + new Vector2(inGameUnitsSize, 0);
				break;
			case RoadDirection.down_to_right:
				roads[i].direction = RoadDirection.right;
				roads[i].pos = lastRoad.pos + new Vector2(inGameUnitsSize, 0);
				break;
			case RoadDirection.up_to_left:
				roads[i].direction = RoadDirection.left;
				roads[i].pos = lastRoad.pos - new Vector2(inGameUnitsSize, 0);
				break;
			case RoadDirection.down_to_left:
				roads[i].direction = RoadDirection.left;
				roads[i].pos = lastRoad.pos - new Vector2(inGameUnitsSize, 0);
				break;
			case RoadDirection.left_to_down:
				roads[i].direction = RoadDirection.down;
				roads[i].pos = lastRoad.pos - new Vector2(0, inGameUnitsSize);
				break;
			case RoadDirection.left_to_up:
				roads[i].direction = RoadDirection.up;
				roads[i].pos = lastRoad.pos + new Vector2(0, inGameUnitsSize);
				break;
			case RoadDirection.right_to_down:
				roads[i].direction = RoadDirection.down;
				roads[i].pos = lastRoad.pos - new Vector2(0, inGameUnitsSize);
				break;
			case RoadDirection.right_to_up:
				roads[i].direction = RoadDirection.up;
				roads[i].pos = lastRoad.pos + new Vector2(0, inGameUnitsSize);
				break;

		}
		if (!CheckCollision(roads[i].direction, roads[i].pos, roadPositions, ref m_checked))
		{
			lastRoad.type = RoadType.roadstraight;
			lastRoad.pos = roads[i].pos;
			lastRoad.direction = roads[i].direction;
			roadPositions[i] = roads[i].pos;
			//Debug.Log("Acutally pos str" + roads[i].pos + "CheckPos :" + m_checked + "I:" + i + "roadPosiotion: " +roadPositions[i]);
			MakeRoad(ref roads[i]);
			return true;
		}
		else
		{
			Debug.Log("Acutally pos " + roads[i].pos + "CheckPos :" + m_checked + "I:" + i + "Dir" + roads[i].direction);
			roads[i].type = RoadType.roadTurn;
			return false;
		}
	}                                                                   //Returns false if a collision is found. Else true: Sets dircetion and position as well as updates the lastRoad object
	private bool BuildTurnRoad(int CurrentIncrement)
	{
		int i = CurrentIncrement;
		switch (lastRoad.direction)
		{
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			////////////////////////////////////////////From straights to corners////////////////////////////////////////////////////
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			case RoadDirection.up:
				roads[i].direction = (RoadDirection)PickNumber((float)RoadDirection.down_to_right, (float)RoadDirection.down_to_left, ref looserOfPicked);
				roads[i].pos = lastRoad.pos + new Vector2(0, inGameUnitsSize);
				break;
			case RoadDirection.down:
				roads[i].direction = (RoadDirection)PickNumber((float)RoadDirection.up_to_right, (float)RoadDirection.up_to_left, ref looserOfPicked);
				roads[i].pos = lastRoad.pos - new Vector2(0, inGameUnitsSize);
				break;
			case RoadDirection.left:
				roads[i].direction = (RoadDirection)PickNumber((float)RoadDirection.right_to_up, (float)RoadDirection.right_to_down, ref looserOfPicked);
				roads[i].pos = lastRoad.pos - new Vector2(inGameUnitsSize, 0);
				break;
			case RoadDirection.right:
				roads[i].direction = (RoadDirection)PickNumber((float)RoadDirection.left_to_up, (float)RoadDirection.left_to_down, ref looserOfPicked);
				roads[i].pos = lastRoad.pos + new Vector2(inGameUnitsSize, 0);
				break;
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			////////////////////////////////////////////Corners to corners///////////////////////////////////////////////////////////
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			case RoadDirection.down_to_left:
				roads[i].direction = (RoadDirection)PickNumber((float)RoadDirection.right_to_up, (float)RoadDirection.right_to_down, ref looserOfPicked);
				roads[i].pos = lastRoad.pos - new Vector2(inGameUnitsSize, 0);
				break;
			case RoadDirection.down_to_right:
				roads[i].direction = (RoadDirection)PickNumber((float)RoadDirection.left_to_up, (float)RoadDirection.left_to_down, ref looserOfPicked);
				roads[i].pos = lastRoad.pos + new Vector2(inGameUnitsSize, 0);
				break;
			case RoadDirection.up_to_left:
				roads[i].direction = (RoadDirection)PickNumber((float)RoadDirection.right_to_up, (float)RoadDirection.right_to_down, ref looserOfPicked);
				roads[i].pos = lastRoad.pos - new Vector2(inGameUnitsSize, 0);
				break;
			case RoadDirection.up_to_right:
				roads[i].direction = (RoadDirection)PickNumber((float)RoadDirection.left_to_up, (float)RoadDirection.left_to_down, ref looserOfPicked);
				roads[i].pos = lastRoad.pos + new Vector2(inGameUnitsSize, 0);
				break;
			case RoadDirection.left_to_up:
				roads[i].direction = (RoadDirection)PickNumber((float)RoadDirection.down_to_left, (float)RoadDirection.down_to_right, ref looserOfPicked);
				roads[i].pos = lastRoad.pos + new Vector2(0, inGameUnitsSize);
				break;
			case RoadDirection.left_to_down:
				roads[i].direction = (RoadDirection)PickNumber((float)RoadDirection.up_to_left, (float)RoadDirection.up_to_right, ref looserOfPicked);
				roads[i].pos = lastRoad.pos - new Vector2(0, inGameUnitsSize);
				break;
			case RoadDirection.right_to_down:
				roads[i].direction = (RoadDirection)PickNumber((float)RoadDirection.up_to_left, (float)RoadDirection.up_to_right, ref looserOfPicked);
				roads[i].pos = lastRoad.pos - new Vector2(0, inGameUnitsSize);
				break;
			case RoadDirection.right_to_up:
				roads[i].direction = (RoadDirection)PickNumber((float)RoadDirection.down_to_left, (float)RoadDirection.down_to_right, ref looserOfPicked);
				roads[i].pos = lastRoad.pos + new Vector2(0, inGameUnitsSize);
				break;
		}
		if (!CheckCollision(roads[i].direction, roads[i].pos, roadPositions, ref m_checked))
		{
			lastRoad.type = RoadType.roadTurn;
			lastRoad.pos = roads[i].pos;
			lastRoad.direction = roads[i].direction;
			roadPositions[i] = roads[i].pos;
			//Debug.Log("Acutally pos str" + roads[i].pos + "CheckPos :" + m_checked + "I:" + i + "roadPosiotion: " + roadPositions[i]);
			MakeRoad(ref roads[i]);
			return true;
		}
		else
		{
			//Debug.Log("Collision on corner. Old direction:" + roads[i].direction + "Pos:" + roads[i].pos);
			//Debug.Log("Acutally pos " + roads[i].pos + "CheckPos :" + m_checked + "I:" + i + "Dir" + roads[i].direction);
			roads[i].direction = (RoadDirection)looserOfPicked;
			if (CheckCollision(roads[i].direction, roads[i].pos, roadPositions, ref m_checked))
				return false;
			lastRoad.type = RoadType.roadTurn;
			lastRoad.pos = roads[i].pos;
			lastRoad.direction = roads[i].direction;
			roadPositions[i] = roads[i].pos;
			MakeRoad(ref roads[i]);
			return true;
			//Debug.Log("Collision on corner. New direction:" + roads[i].direction + "Pos:" + roads[i].pos);
			//Debug.Break();
		}
	}                                                                       //Returns false if a collision is found. Else true: Sets dircetion and position as well as updates the lastRoad object
	private void MakeRoad(ref Roads road)
	{
		switch (road.type)
		{
			case RoadType.roadstraight: road.m_GameObjectPrefabType = rStraight; break;
			case RoadType.roadTurn: road.m_GameObjectPrefabType = rTurn; break;
			case RoadType.roadCheckerd: road.m_GameObjectPrefabType = rFinished; break;
			default: return;
		}
		switch (road.direction)
		{
			/////////////For Straights/////////////////
			case RoadDirection.up: road.rotation = Quaternion.Euler(0, 0, 0); break;
			case RoadDirection.down: road.rotation = Quaternion.Euler(0, 0, 180); break;
			case RoadDirection.right: road.rotation = Quaternion.Euler(0, 0, -90); break;
			case RoadDirection.left: road.rotation = Quaternion.Euler(0, 0, 90); break;
			case RoadDirection.left_to_up: road.rotation = Quaternion.Euler(0, 0, 0); break;
			case RoadDirection.left_to_down: road.rotation = Quaternion.Euler(0, 0, 90); break;
			case RoadDirection.right_to_up: road.rotation = Quaternion.Euler(0, 0, -90); break;
			case RoadDirection.right_to_down: road.rotation = Quaternion.Euler(0, 0, 180); break;
			case RoadDirection.up_to_left: road.rotation = Quaternion.Euler(0, 0, 0); break;
			case RoadDirection.up_to_right: road.rotation = Quaternion.Euler(0, 0, -90); break;
			case RoadDirection.down_to_left: road.rotation = Quaternion.Euler(0, 0, 90); break;
			case RoadDirection.down_to_right: road.rotation = Quaternion.Euler(0, 0, 180); break;
			default: return;
		}
		road.m_GameObjectPrefabType.transform.position = road.pos;
		road.m_GameObjectPrefabType.transform.rotation = road.rotation;
		road.isActive = true;
		road.m_GameObjectInstance = Instantiate(road.m_GameObjectPrefabType);
	}
	private bool FinishLevel(int CurrentIncrement)
	{
		int i = CurrentIncrement;
		roads[i].type = RoadType.roadCheckerd;
		roads[i].direction = SimplifyDirection(lastRoad.direction);
		if (roads[i].direction == RoadDirection.m_null)
			return false;
		//lastRoad = roads[i];
		MakeRoad(ref roads[i]);
		return true;
	}
	private RoadDirection SimplifyDirection(RoadDirection dir)
	{
		switch (dir)
		{
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			////////////////////////////////////////////From straights to straights//////////////////////////////////////////////////
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			case RoadDirection.up: return RoadDirection.up;
			case RoadDirection.down: return RoadDirection.down;
			case RoadDirection.left: return RoadDirection.left;
			case RoadDirection.right: return RoadDirection.right;
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			////////////////////////////////////////////From coners to straights/////////////////////////////////////////////////////
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			case RoadDirection.up_to_right: return RoadDirection.right;
			case RoadDirection.down_to_right: return RoadDirection.right;
			case RoadDirection.up_to_left: return RoadDirection.left;
			case RoadDirection.down_to_left: return RoadDirection.left;
			case RoadDirection.left_to_down: return RoadDirection.down;
			case RoadDirection.left_to_up: return RoadDirection.up;
			case RoadDirection.right_to_down: return RoadDirection.down;
			case RoadDirection.right_to_up: return RoadDirection.up;

		}
		return RoadDirection.m_null;

	}															  //Takes in a RoadDirection and returns the absolute last direction. Eg Returns RoadDirection.up from input RoadDirection.right_to_up  
	private void DeleteRoad()
	{
		bool endLoop = false;
		for (int i = 0; i < RoadCount; i++)
		{
			if (roads[i].m_GameObjectInstance == null)
			{
				continue;
			}
			switch (roads[i].type)
			{
				case RoadType.roadstraight:
					if (roads[i].isActive == false && roads[i].m_GameObjectInstance.GetComponent<SpriteRenderer>().isVisible == false)
					{
						Destroy(roads[i].m_GameObjectInstance);
						roads[i] = new Roads(false);
						roadPositions[i] = new Vector2(0, 0);
						endLoop = true;
						break;
					}
					break;
				case RoadType.roadTurn:
	
					if (roads[i].isActive == false && roads[i].m_GameObjectInstance.GetComponentInChildren<SpriteRenderer>().isVisible == false)
					{
						Destroy(roads[i].m_GameObjectInstance);
						roads[i] = new Roads(false);
						roadPositions[i] = new Vector2(0, 0);
						endLoop = true;
						break;
					}
					break;
				default: break;
			}
			if (endLoop)
				break;
		}
	}	
	public float PickNumber(float num1, float num2, ref float optLoseValue)
	{
		int ans = Random.Range(0, 2);
		if (ans == 1)
		{
			optLoseValue = num2;
			return num1;
		}
		optLoseValue = num1;
		return num2;
	}
	public bool CheckVecArray(Vector2 needsChecking, Vector2[] array)
	{
		for (int i = 0; i < RoadCount; i++)
		{
			if (needsChecking == array[i])
				return true;
		}
		return false;
	}
	public void PassedRoad(GameObject instObject)
	{
		for (int i = 0; i < RoadCount; i++)
		{
			if (instObject == roads[i].m_GameObjectInstance)
			{
				roads[i].isActive = false;
				Debug.Log(roads[i].type + "i:" + i);
				break;
			}
		}
	}	
	IEnumerator CheckToDestoryRoad()
	{
		DeleteRoad();
		Debug.Log("Ran, time" + Time.time);
		yield return new WaitForSeconds(2);
	}
}
