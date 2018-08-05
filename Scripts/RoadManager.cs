using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager {


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
	public enum RoadType
	{
		m_null = -1,
		roadstraight = 0,
		roadTurn = 1,
		roadCheckerd = 2
	}
	public struct Roads
	{
		public RoadType type;
		public RoadDirection direction;
		public Quaternion rotation;
		public Vector2 pos;
		public GameObject m_GameObjectPrefabType;
		public GameObject m_GameObjectInstance;
		public SpriteRenderer m_renderer;
		public bool isActive;   //Gets passed to false when hit
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

	public void InitRoadArray(ref Roads[] roads)
	{
		for (int i = 0; i < roads.Length; i++)
		{
			roads[i] = new Roads();
		}
	}
	public RoadType GetRoadType(Vector2 RelativePostion)
	{
		if (RelativePostion == new Vector2(0, 0))
			return RoadType.roadstraight;
		return RoadType.roadTurn;
	}
	public Quaternion GetRoadDirection(Vector2 RelativePostion, Vector2 pos, Vector2 pos0)
	{
		if (RelativePostion == Vector2.zero)
		{
			if (pos.x == pos0.x)
				return Quaternion.Euler(0, 0, 0);
			return Quaternion.Euler(0, 0, -90);
		}											//0
		if (RelativePostion == new Vector2(1, 1))	//0-0
		{
			return Quaternion.Euler(0, 0, -90);		
		}
		if (RelativePostion == new Vector2(1,-1))   //0-0
		{                                           //0
			return Quaternion.Euler(0, 0, 180);
		}
		if (RelativePostion == new Vector2(-1, -1))	//0-0
		{											//  0
			return Quaternion.Euler(0, 0, 90);
		}											//  0
		if (RelativePostion == new Vector2(-1, 1))	//0-0
		{
			return Quaternion.Euler(0, 0, 0);
		}
		return Quaternion.Euler(-1, -1, -1);
	}
}
