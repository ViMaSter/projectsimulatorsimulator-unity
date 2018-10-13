using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class SimpleReceiver : MonoBehaviour {

	public bool IsLocal = false;

	private WebSocket websocketClient;
	public Transform carTransform;

	private float x, y;

	void Start ()
	{
		StartCoroutine(DelayedStart());
	}

	IEnumerator DelayedStart()
	{
		yield return new WaitForSeconds(2.0f);
		Debug.Log("ATTEMPT!");
		websocketClient = new WebSocket(IsLocal ? "ws://localhost:5000/" : "ws://projectsimulatorsimulator.herokuapp.com/:39759");
		websocketClient.OnOpen += OnOpen;
		websocketClient.OnMessage += OnMessage;
		websocketClient.Connect();
	}

	void OnOpen(object sender, System.EventArgs e)
	{
		Debug.Log("Connected!");
		Connect(0);
	}

	void OnMessage(object sender, WebSocketSharp.MessageEventArgs message)
	{
		NetworkingDefinitions.Response r = JsonUtility.FromJson<NetworkingDefinitions.Response>(message.Data);
		// Debug.LogFormat("Position Update! [{0}, {1}]", r.session.carPosition.x, r.session.carPosition.y);
		x = r.session.carPosition.x;
		y = r.session.carPosition.y;
	}

	void Update()
	{
		carTransform.position = new Vector3(x, y, carTransform.position.z);
	}

	void Connect(int sessionID)
	{
		Debug.Log("Joining Session...");
		websocketClient.Send("{\"command\":\"joinSession\", \"sessionID\": "+sessionID+"}");
		Debug.Log("Joining Session...DONE!");
	}
	
}
