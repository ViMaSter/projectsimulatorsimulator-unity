using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraMovement : MonoBehaviour {

	public PlayerController Car;
	public Vector3 cameraOffset;
	[Range(0.0f, 2.0f)]
	public float peakAhead = 5.0f;
	[Range(0.0f, 5.0f)]
	public float peakAheadLerp = 0.5f;
	private float currentPeakAhead = 0.0f;

	private Transform thisTransform;
	private Transform carTransform;

	void Awake()
	{
		thisTransform = GetComponent<Transform>();
		carTransform = Car.GetComponent<Transform>();
	}

	void LateUpdate ()
	{
		currentPeakAhead = Mathf.Lerp(currentPeakAhead, Car.CurrentSpeed * peakAhead, peakAheadLerp * Time.deltaTime);
		thisTransform.position = carTransform.position + cameraOffset + (carTransform.forward * currentPeakAhead);
	}
}
