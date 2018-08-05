using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSorter : MonoBehaviour {

	[SerializeField] RectTransform ButtonLeft;
	[SerializeField] RectTransform ButtonRight;
	[SerializeField] RectTransform ButtonAccel;
	[SerializeField] RectTransform ButtonBrake;
	[SerializeField] private float TurnButtonsHeight = 0.2f;
	[SerializeField] private float TurnButtonsWidth = 0.2f;
	[SerializeField] private float AccelerationgWidth = 0.2f;
	[SerializeField] private float AccelerationgHeight = 0.4f;
	[SerializeField] private float BrakeWidth = 0.2f;
	[SerializeField] private float BrakeHeight = 0.2f;
	private RectTransform canvas;


	void Start () {
		Screen.orientation = ScreenOrientation.Portrait;
		canvas = GetComponent<RectTransform>();
		Vector2 canvasOriigin = canvas.rect.position;
		ButtonLeft.sizeDelta = new Vector2(canvas.rect.width * TurnButtonsWidth, canvas.rect.height * TurnButtonsHeight);
		ButtonRight.sizeDelta = new Vector2(canvas.rect.width * TurnButtonsWidth, canvas.rect.height * TurnButtonsHeight);
		ButtonAccel.sizeDelta = new Vector2(canvas.rect.width * AccelerationgWidth, canvas.rect.height * AccelerationgHeight);
		ButtonBrake.sizeDelta = new Vector2(canvas.rect.width * BrakeWidth, canvas.rect.height * BrakeHeight);
		
		ButtonLeft.transform.SetPositionAndRotation(new Vector3(ButtonLeft.sizeDelta.x/2, ButtonLeft.sizeDelta.y/2, -2), Quaternion.Euler(0, 0, 0));
		ButtonRight.transform.SetPositionAndRotation(new Vector3(ButtonRight.sizeDelta.x * 3/2 + 5f, ButtonRight.sizeDelta.y/2, -2), Quaternion.Euler(0, 0, 0));
		ButtonAccel.transform.SetPositionAndRotation(new Vector3(canvas.sizeDelta.x - ButtonAccel.rect.width/2, ButtonAccel.sizeDelta.y/2, -2), Quaternion.Euler(0, 0, 0));
		ButtonBrake.transform.SetPositionAndRotation(new Vector3(canvas.sizeDelta.x - ButtonAccel.rect.width/2, ButtonAccel.sizeDelta.y + ButtonBrake.sizeDelta.y/2 , -2), Quaternion.Euler(0, 0, 0));		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
