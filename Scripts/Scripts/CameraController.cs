using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	[SerializeField] private Transform target;

	void Start () {
			
	}
	

	void Update () {
		transform.position = new Vector3(target.position.x, target.position.y, -100);
	}
}
