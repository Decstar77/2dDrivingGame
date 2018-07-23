using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleLevelGenerator : MonoBehaviour {


	[SerializeField] GameObject PrefavStrRoad;
	[SerializeField] GameObject PrefavTurnRoad;

	LevelBuilder2 level;
	
	void Start () {
		LevelBuilder2 level = GetComponent<LevelBuilder2>();
		level.Init();
		LevelBuilder2.Vertices vert = level.GetRootVert();
		vert = level.ActivateRandomAdjacentVert(vert);


	}
	

	void Update () {
		
	}



}
