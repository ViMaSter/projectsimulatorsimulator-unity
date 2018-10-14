using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebSocketJoiner : MonoBehaviour {

	private WebSocket websocketClient;
	public new Transform transform;

	private float x = 0.0f, y = 0.0f;
	private int SessionID = -1;

	void Awake ()
	{
		websocketClient = new WebSocket(NetworkSettings.Instance.IsLocal ? "ws://localhost:5000/" : "ws://projectsimulatorsimulator.herokuapp.com/:22371");
		websocketClient.OnOpen += OnOpen;
		websocketClient.OnMessage += OnMessage;
		websocketClient.Connect();
	}

	void OnDisable()
	{
		websocketClient.Close();
	}

	void Update()
	{
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
			websocketClient.Send("{\"command\":\"updateCarPosition\", \"sessionID\": "+SessionID+", \"x\": "+x+", \"y\": "+y+"}");
		}
	}

	void OnMessage(object sender, WebSocketSharp.MessageEventArgs message)
	{
		Debug.Log("JOIN RESPONSE: "+message.Data);
		NetworkingDefinitions.Response r = JsonUtility.FromJson<NetworkingDefinitions.Response>(message.Data);
		x = r.session.carPosition.x;
		y = r.session.carPosition.y;

		if (r.command == "sessionJoin")
		{
			// TODO: GLOBALIZE THIS
			SessionID = r.sessionID;
		}
	}

	void OnOpen(object sender, System.EventArgs e)
	{
		Debug.Log("JOIN CONNECT");
	}
	
}
