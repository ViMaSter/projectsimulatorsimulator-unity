using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Acceleration
	[Range(0.0f, 1000.0f)]
	public float acceleration = 2.0f;
	[Range(0.0f, 1.0f)]
	public float accelerationDrag = 0.1f;

	private float currentSpeed = 0.0f;
	public float CurrentSpeed
	{
		get 
		{
			return currentSpeed;
		}
	}

	// Turning
	[Range(0.0f, 1000.0f)]
	public float turningSpeed = 1000.0f;
	[Range(0.0f, 1.0f)]
	public float steeringDrag = 0.01f;

	private float currentSteering = 0.0f;
	private float currentDirection = 0.0f;

	public ParticleSystem exhaustParticles;
	public AudioSource hornAudioSource;
	public AudioClip hornClip;
	public Vector2 hornPitchRange;


	// Internals
	public new Transform transform;
	public new Rigidbody rigidbody;
	public Camera playerCamera;
	public Camera mapCamera;

	void Update ()
	{
		if (ShowMapCamera())
		{
			DetectInput();
		}

		ApplyDrag();

		CalculateCurrentDirection();

		ApplyHorn();

		UpdateParticleSystem();
	}

	void FixedUpdate()
	{
		ApplyRotation();
		ApplySpeed();
	}

	void ApplyHorn()
	{
		if(Input.GetKeyDown(KeyCode.H))
		{
			hornAudioSource.pitch = Random.Range(hornPitchRange[0], hornPitchRange[1]);
			hornAudioSource.PlayOneShot(hornClip, 0.5f);
		}
	}

	void UpdateParticleSystem()
	{
		var module = exhaustParticles.emission;
		module.rateOverTime = (currentSpeed / 9.0f) * 5000f;
	}

	bool ShowMapCamera()
	{
		playerCamera.enabled = !Input.GetKey(KeyCode.M);
		mapCamera.enabled = Input.GetKey(KeyCode.M);
		return !Input.GetKey(KeyCode.M);
	}

	void DetectInput()
	{
		if (Input.GetKey(KeyCode.UpArrow))
		{
			currentSpeed += acceleration;
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			currentSpeed -= acceleration;
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			currentSteering -= turningSpeed;
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			currentSteering += turningSpeed;
		}
	}

	void ApplyDrag()
	{
		currentSpeed = currentSpeed * (1.0f - accelerationDrag);
		currentSteering = currentSteering * (1.0f - steeringDrag);
	}

	void CalculateCurrentDirection()
	{
		currentDirection -= currentSteering;
	}

	void ApplyRotation()
	{
		rigidbody.rotation = Quaternion.Euler(new Vector3(currentDirection, -90, 90));
	}

	void ApplySpeed()
	{
		rigidbody.velocity = transform.forward * currentSpeed;
	}
}
