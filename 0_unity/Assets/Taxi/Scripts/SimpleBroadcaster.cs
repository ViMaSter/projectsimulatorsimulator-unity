using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class SimpleBroadcaster : MonoBehaviour {

	public bool IsLocal = false;
	private bool IsReady = false;

	private WebSocket websocketClient;
	public Transform carTransform;

	void Start ()
	{
		Debug.Log("Broadcaster connecting...");
		websocketClient = new WebSocket(IsLocal ? "ws://localhost:5000/" : "ws://projectsimulatorsimulator.herokuapp.com/:39759");
		websocketClient.OnOpen += OnOpen;
		websocketClient.Connect();
	}

	void Update()
	{
		if (IsReady)
		{
			Debug.Log("Sending car position");
			Debug.Log("{\"command\":\"updateCarPosition\", \"sessionID\": 0, \"x\": "+carTransform.position.x+", \"y\": "+carTransform.position.y+"}");
			websocketClient.Send("{\"command\":\"updateCarPosition\", \"sessionID\": 0, \"x\": "+carTransform.position.x+", \"y\": "+carTransform.position.y+"}");
		}
	}

	void OnOpen(object sender, System.EventArgs e)
	{
		Debug.Log("Broadcaster connected!");
		websocketClient.Send("{\"command\":\"createSession\"}");
		IsReady = true;
	}

	void Connect(int sessionID)
	{
		Debug.Log("Connecting...");
		websocketClient.Send("{\"command\":\"joinSession\", \"sessionID\": "+sessionID+"}");
	}
	
}
