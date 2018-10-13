﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Acceleration
	[Range(0.0f, 1000.0f)]
	public float acceleration = 2.0f;
	[Range(0.0f, 1.0f)]
	public float accelerationDrag = 0.1f;

	private float currentSpeed = 0.0f;

	// Turning
	[Range(0.0f, 1000.0f)]
	public float turningSpeed = 1000.0f;
	[Range(0.0f, 1.0f)]
	public float steeringDrag = 0.01f;

	private float currentSteering = 0.0f;

	// Internals
	public Transform transform;

	void Update ()
	{
		DetectInput();

		ApplyRotation();
		ApplySpeed();

		ApplyDrag();
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

	void ApplyRotation()
	{
		transform.Rotate(0.0f, currentSteering, 0.0f);
	}

	void ApplySpeed()
	{
		transform.Translate(new Vector3(0, 0, currentSpeed) * Time.deltaTime);
	}
}