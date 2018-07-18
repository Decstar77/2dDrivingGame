using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSorter : MonoBehaviour {

	[SerializeField] RectTransform ButtonLeft;
	[SerializeField] RectTransform ButtonRight;
	[SerializeField] RectTransform ButtonAccel;
	[SerializeField] RectTransform ButtonBrake;

	private RectTransform canvas;


	void Start () {
		Screen.orientation = ScreenOrientation.Portrait;
		canvas = GetComponent<RectTransform>();
		Vector2 canvasOriigin = canvas.rect.position;
		
		//ButtonLeft.rect.position = new Vector2(50f, 50f);
		ButtonLeft.transform.SetPositionAndRotation(new Vector3(ButtonLeft.rect.width/2, ButtonLeft.rect.height/2,-2), Quaternion.Euler(0, 0, 0));
		ButtonRight.transform.SetPositionAndRotation(new Vector3(ButtonRight.rect.width*1.5f + 50f, ButtonRight.rect.height / 2, -2), Quaternion.Euler(0, 0, 0));
		ButtonAccel.transform.SetPositionAndRotation(new Vector3(canvas.rect.width - ButtonAccel.rect.width/2, ButtonAccel.rect.height/2, -2), Quaternion.Euler(0, 0, 0));
		ButtonBrake.transform.SetPositionAndRotation(new Vector3(canvas.rect.width - ButtonBrake.rect.width / 2, ButtonBrake.rect.height *2, -2), Quaternion.Euler(0, 0, 0));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
