using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class PingPong
{
	private string sentMessage;
	private string expectedResponse;
	private WebSocket websocketClient;

	public delegate void IsDoneCallback(bool isMatch);  
	private IsDoneCallback callback;

	public PingPong(WebSocket client, string ping, string pong)
	{
		websocketClient = client;
		sentMessage = ping;
		expectedResponse = pong;
	}

	public void Execute(IsDoneCallback externalCallback)
	{ 
		callback = externalCallback;
		websocketClient.OnMessage += ParseReponse;
		websocketClient.Send(sentMessage);
	}

	public void ParseReponse(object sender, WebSocketSharp.MessageEventArgs message)
	{
		websocketClient.OnMessage -= ParseReponse;
		Debug.LogFormat("MESSAGE {0}\n\nExpected: {1}\nGot: {2}", message.Data == expectedResponse ? "MATCHES" : "MISMATCHES!", expectedResponse, message.Data);
		callback(message.Data == expectedResponse);
	}
}

public class WebSocketServerTester : MonoBehaviour {

	public bool IsLocal = false;

	private WebSocket websocketClient;
	private Queue<PingPong> pingPongs = new Queue<PingPong>();

	void Start ()
	{
		websocketClient = new WebSocket(IsLocal ? "ws://localhost:5000/" : "ws://projectsimulatorsimulator.herokuapp.com/:39759");

		pingPongs.Enqueue(new PingPong(websocketClient, "{\"command\":\"createSession\"}", "{\"command\":\"sessionUpdate\",\"session\":{\"carPosition\":{\"x\":5,\"y\":6}}}"));
		pingPongs.Enqueue(new PingPong(websocketClient, "{\"command\":\"updateCarPosition\", \"sessionID\": 0, \"x\": 10, \"y\": 20}", "{\"command\":\"sessionUpdate\",\"session\":{\"carPosition\":{\"x\":10,\"y\":20}}}"));
	
		websocketClient.OnOpen += OnOpen;
		Debug.Log("CONNECTING TO " + (IsLocal ? "LOCAL" : "REMOTE"));
		websocketClient.Connect();
	}

	void OnFinishItem(bool wasMatch)
	{
		Debug.Log("PREVIOUS IS MATCH: " + (wasMatch ? "TRUE" : "FALSE"));
		if (wasMatch)
		{
			NextItem();
		}
		else
		{
			Debug.Log("Mismatch on last PingPong; halting execution");
		}
	}

	void NextItem()
	{
		if (pingPongs.Count > 0)
		{
			Debug.Log("NEXT ITEM");
			PingPong pingPong = pingPongs.Dequeue();
			pingPong.Execute(OnFinishItem);
		}
		else
		{
			Debug.Log("END QUEUE");
		}
	}

	void OnOpen(object sender, System.EventArgs e)
	{
		Debug.Log("CONNECTED");
		Debug.Log(pingPongs.Count);
		if (pingPongs.Count > 0)
		{
			Debug.Log("START QUEUE");
			NextItem();
		}
	}
	
}
