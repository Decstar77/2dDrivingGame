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
	[SerializeField] private LevelBuilder builder;

	private int SSteeringRealismInt;
	private Rigidbody2D ri;
	void Start () {
		builder = GetComponent<LevelBuilder>();
		ri = GetComponent<Rigidbody2D>();
		ri.gravityScale = 0;
		SSteeringRealismInt = 1;

	}

	private void FixedUpdate()
	{
		

		float vertInp = Input.GetAxis("Vertical");
		float hortInp = Input.GetAxis("Horizontal");
		ri.AddRelativeForce(Vector2.up * SpeedFactor * vertInp);
		//////////////////Do Steering //////////////
		vertInp = Mathf.Clamp(vertInp, -0.3f, 1);
		float tf = Mathf.Lerp(0, TorgueFactor, ri.velocity.magnitude/ UndersteerValue);
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
	private Vector2 ForwardVelocity()
	{
		return transform.up * Vector2.Dot(ri.velocity, transform.up);
	}
	private Vector2 SideWaysVelocity()
	{
		return transform.right * Vector2.Dot(ri.velocity, transform.right);
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		GameObject collider = collision.gameObject;
		
		builder.PassedRoad(collider);
	}
}
