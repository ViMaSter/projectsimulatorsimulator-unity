using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebSocketServerTester : MonoBehaviour {

	private WebSocket websocketClient;

	void Start ()
	{
		websocketClient = new WebSocket("ws://projectsimulatorsimulator.herokuapp.com/:39759");
		websocketClient.OnOpen += OnOpen;
		websocketClient.Connect ();
		websocketClient.Send ("{\"command\":\"createSession\"}");
	}

	void OnOpen(object sender, System.EventArgs e)
	{
		websocketClient.Send ("{\"command\":\"updateCarPosition\", \"sessionDI\": 0, \"x\": 10, \"y\": 20}");
	}
	
}
