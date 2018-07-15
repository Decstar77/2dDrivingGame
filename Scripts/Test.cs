using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	SpriteRenderer spr;
	void Start () {
		spr = GetComponent<SpriteRenderer>();
		//Debug.Log(spr.bounds.size.x);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
