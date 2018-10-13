using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class SimpleBroadcaster : MonoBehaviour {

	private bool IsReady = false;

	private WebSocket websocketClient;
	public Transform carTransform;

	private int sessionID;

	void Start ()
	{
		Debug.Log("[BROAD] Broadcaster connecting...");
		websocketClient = new WebSocket(NetworkSettings.WebSocketServer);
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
		if (IsReady)
		{
			websocketClient.Send("{\"command\":\"updateCarPosition\", \"sessionID\": "+sessionID+", \"x\": "+carTransform.position.x+", \"y\": "+carTransform.position.y+"}");
		}
	}

	void OnMessage(object sender, WebSocketSharp.MessageEventArgs message)
	{
		NetworkingDefinitions.Response response = JsonUtility.FromJson<NetworkingDefinitions.Response>(message.Data);
		if (response.command == "sessionJoin")
		{
			// TODO: GLOBALIZE THIS
			sessionID = response.sessionID;
			Debug.LogFormat("[BROAD] Joined ID {0}", sessionID);
			IsReady = true;
			websocketClient.Send(NetworkingDefinitions.Generator.SetCurrentRoute(sessionID, "Wochenmarkt", "Stadtgrenze"));
		}
	}

	void OnOpen(object sender, System.EventArgs e)
	{
		Debug.Log("[BROAD] Broadcaster connected!");
		websocketClient.Send("{\"command\":\"createSession\", \"mapName\": \"TaxiScene\"}");
		IsReady = true;
	}
	
}
