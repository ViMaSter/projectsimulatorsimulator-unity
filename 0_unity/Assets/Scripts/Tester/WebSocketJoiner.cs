using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebSocketJoiner : MonoBehaviour {

	public bool IsLocal = false;

	private WebSocket websocketClient;
	public Transform transform;

	private float x = 0.0f, y = 0.0f;

	void Start ()
	{
		websocketClient = new WebSocket(IsLocal ? "ws://localhost:5000/" : "ws://projectsimulatorsimulator.herokuapp.com/:39759");
		websocketClient.OnOpen += OnOpen;
		websocketClient.OnMessage += OnMessage;
		websocketClient.Connect();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.D))
		{
			Connect(0);
		}
		transform.position = new Vector3(x, y, 0);

		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			x--;
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			x++;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			y--;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			y++;
		}

		if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
		{
			websocketClient.Send("{\"command\":\"updateCarPosition\", \"sessionID\": 0, \"x\": "+x+", \"y\": "+y+"}");
		}
	}

	void OnMessage(object sender, WebSocketSharp.MessageEventArgs message)
	{
		Debug.Log("JOIN RESPONSE: "+message.Data);
		NetworkingDefinitions.Response r = JsonUtility.FromJson<NetworkingDefinitions.Response>(message.Data);
		x = r.session.carPosition.x;
		y = r.session.carPosition.y;
	}

	void OnOpen(object sender, System.EventArgs e)
	{
		Debug.Log("JOIN CONNECT");
	}

	void Connect(int sessionID)
	{
		Debug.Log("Connecting...");
		websocketClient.Send("{\"command\":\"joinSession\", \"sessionID\": "+sessionID+"}");
	}
	
}
