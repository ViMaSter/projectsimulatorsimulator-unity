using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class SimpleReceiver : MonoBehaviour {

	private WebSocket websocketClient;
	public Transform carTransform;

	private float x, y;

	void Awake ()
	{
		websocketClient = new WebSocket(NetworkSettings.WebSocketServer);
		websocketClient.OnOpen += OnOpen;
		websocketClient.OnMessage += OnMessage;
		websocketClient.Connect();
	}

	void OnDisable()
	{
		websocketClient.Close();
	}

	void OnOpen(object sender, System.EventArgs e)
	{
		Debug.Log("Connected!");
		Connect(-1);
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
		carTransform.position = new Vector3(x, carTransform.position.y, y);
	}

	void Connect(int sessionID)
	{
		Debug.Log("Joining Session...");
		websocketClient.Send("{\"command\":\"joinSession\", \"sessionID\": "+sessionID+"}");
		Debug.Log("Joining Session...DONE!");
	}
	
}
