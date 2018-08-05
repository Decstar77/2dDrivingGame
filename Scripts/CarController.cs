using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarController : MonoBehaviour {

	[SerializeField] private float SpeedFactor = 8f;
	[SerializeField] private float TorgueFactor = 200f;
	[SerializeField] private float DriftFactorStick = 0.9f;
	[SerializeField] private float DriftFactorSlip = 1f;
	[SerializeField] private float MaxStick = 2.5f;
	[SerializeField] private float UndersteerValue = 5;
	[SerializeField] private bool SteeringRealism = false;

	//Platform
	private bool android = false;
	////Turning Shit. Buttons are a nightmare btw
	private int SSteeringRealismInt;
	private float TurningRight;
	private float TurningLeft;
	private float TurnDirection;
	private float TurnAmount;
	////Acceleration Shit.Buttons are a nightmare btw
	private float Acceleration;
	private float Brake;
	private float VelocityDirection;
	private float VelocityAmount;

	float vertInp = 0;
	float hortInp = 0;
	private Rigidbody2D ri;
	private SingleLevelGenerator level;

	void Start() {
#if UNITY_ANDROID
		android = true;
#endif
		TurnDirection = VelocityDirection = 0;
		TurnAmount = VelocityAmount = 0;
		ri = GetComponent<Rigidbody2D>();
		level = GetComponent<SingleLevelGenerator>();
		ri.gravityScale = 0;
		SSteeringRealismInt = 1;
		Quaternion angle = new Quaternion();
		transform.position = level.GetStart(ref angle);
		transform.rotation = angle;
	}

	private void FixedUpdate()
	{

		if (android)
		{
			hortInp = DoSteeringInput();
			vertInp = DoAccelerationInput();
		}
		else
		{
			vertInp = Input.GetAxis("Vertical");
			hortInp = Input.GetAxis("Horizontal");
		}

		ri.AddRelativeForce(Vector2.up * SpeedFactor * vertInp);
		//////////////////Do Steering //////////////
		vertInp = Mathf.Clamp(vertInp, -0.3f, 1);
		float tf = Mathf.Lerp(0, TorgueFactor, ri.velocity.magnitude / UndersteerValue);
		//Debug.Log(vertInp);
		if (vertInp < 0 && SteeringRealism == true)
			ri.angularVelocity = (tf * hortInp * SSteeringRealismInt);
		else
			ri.angularVelocity = (tf * hortInp * -1);
		////////////////////Do drift cals///////////////
		float driftFactor = DriftFactorStick;
		if (SideWaysVelocity().magnitude > MaxStick)
			driftFactor = DriftFactorSlip;
		ri.velocity = ForwardVelocity() + SideWaysVelocity() * driftFactor;
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		GameObject collider = collision.gameObject;
		if (collider == level.GetFinishRoad())
		{
			Debug.Log("Finsihed");
		}
	}
	private Vector2 ForwardVelocity()
	{
		//Debug.Log("Velo " + ri.velocity + "Tran " + transform.up + "Vec " + Vector2.Dot(ri.velocity, transform.up) * transform.up);
		return transform.up * Vector2.Dot(ri.velocity, transform.up);		
	}
	private Vector2 SideWaysVelocity()
	{
		return transform.right * Vector2.Dot(ri.velocity, transform.right);
	}
	private float DoSteeringInput()
	{
		TurnDirection = TurningRight - TurningLeft;
		TurnAmount = Mathf.Lerp(TurnAmount, TurnDirection, 0.2f);
		if (TurnAmount == -1 || TurnAmount == 1)
			return TurnAmount;
		return TurnAmount;
	}
	private float DoAccelerationInput()
	{
		VelocityDirection = Acceleration - Brake;
		VelocityAmount = Mathf.Lerp(VelocityAmount, VelocityDirection, 0.2f);
		if (VelocityAmount == -1 || VelocityAmount == 1)
			return VelocityAmount;
		return VelocityAmount;
	}












	//Buttons
	public void TurnRightButtonDown()
	{

		TurningRight = 1;
	}
	public void TurnRightButtonUp()
	{

		TurningRight = 0;
	}
	public void TurnLeftButtionDown()
	{
		TurningLeft = 1;
	}
	public void TurnLeftButtionUp()
	{
		TurningLeft = 0;
	}
	public void AccelerateButtonDown()
	{
		Acceleration = 1;
	}
	public void AccelerateButtonUp()
	{
		Acceleration = 0;
	}
	public void BrakeButtionDown()
	{
		Brake = 1;
	}
	public void BrakeButtionUp()
	{
		Brake = 0;
	}


}
