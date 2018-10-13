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
		Debug.LogFormat("MESSAGE {0}\n\nSent:\n{1}\nExpected:\n{2}\nGot:\n{3}\n", message.Data == expectedResponse ? "MATCHES" : "MISMATCHES!", this.sentMessage, expectedResponse, message.Data);
		callback(message.Data == expectedResponse);
	}
}

public class WebSocketServerTester : MonoBehaviour {

	private WebSocket websocketClient;
	private Queue<PingPong> pingPongs = new Queue<PingPong>();

	void Start ()
	{
		websocketClient = new WebSocket(NetworkSettings.Instance.IsLocal ? "ws://localhost:5000/" : "ws://projectsimulatorsimulator.herokuapp.com/:22371");

		pingPongs.Enqueue(new PingPong(websocketClient,
			"{\"command\":\"createSession\", \"mapName\": \"TaxiScene\"}",
			"{\"command\":\"sessionJoin\",\"sessionID\":0,\"mapName\":\"TaxiScene\",\"session\":{\"carPosition\":{\"x\":5,\"y\":6},\"currentRoute\":{\"start\":\"\",\"end\":\"\"}}}"
		));
		pingPongs.Enqueue(new PingPong(websocketClient,
			"{\"command\":\"updateCarPosition\", \"sessionID\": 0, \"x\": 10, \"y\": 20}",
			"{\"command\":\"sessionUpdate\",\"session\":{\"carPosition\":{\"x\":10,\"y\":20},\"currentRoute\":{\"start\":\"\",\"end\":\"\"}}}"
		));
		pingPongs.Enqueue(new PingPong(websocketClient,
			"{\"command\":\"setCurrentRoute\", \"sessionID\": 0, \"start\": \"bahnhof\", \"end\": \"rathaus\"}",
			"{\"command\":\"sessionUpdate\",\"session\":{\"carPosition\":{\"x\":10,\"y\":20},\"currentRoute\":{\"start\":\"bahnhof\",\"end\":\"rathaus\"}}}"
		));
		pingPongs.Enqueue(new PingPong(websocketClient,
			"{\"command\":\"setCurrentRoute\", \"sessionID\": 0, \"start\": \"schule\", \"end\": \"krankenhaus\"}",
			"{\"command\":\"sessionUpdate\",\"session\":{\"carPosition\":{\"x\":10,\"y\":20},\"currentRoute\":{\"start\":\"bahnhof\",\"end\":\"rathaus\"}}}"
		));
		pingPongs.Enqueue(new PingPong(websocketClient,
			"{\"command\":\"finishRoute\", \"sessionID\": 0}",
			"{\"command\":\"sessionUpdate\",\"session\":{\"carPosition\":{\"x\":10,\"y\":20},\"currentRoute\":{\"start\":\"\",\"end\":\"\"}}}"
		));
		pingPongs.Enqueue(new PingPong(websocketClient,
			"{\"command\":\"setCurrentRoute\", \"sessionID\": 0, \"start\": \"schule\", \"end\": \"krankenhaus\"}",
			"{\"command\":\"sessionUpdate\",\"session\":{\"carPosition\":{\"x\":10,\"y\":20},\"currentRoute\":{\"start\":\"schule\",\"end\":\"krankenhaus\"}}}"
		));
		pingPongs.Enqueue(new PingPong(websocketClient,
			"{\"command\":\"leaveSession\", \"sessionID\": 0}",
			"{}"
		));



		pingPongs.Enqueue(new PingPong(websocketClient,
			NetworkingDefinitions.Generator.CreateSession("TaxiScene"),
			"{\"command\":\"sessionJoin\",\"sessionID\":1,\"mapName\":\"TaxiScene\",\"session\":{\"carPosition\":{\"x\":5,\"y\":6},\"currentRoute\":{\"start\":\"\",\"end\":\"\"}}}"
		));
		pingPongs.Enqueue(new PingPong(websocketClient,
			NetworkingDefinitions.Generator.UpdateCarPosition(1, 10, 20),
			"{\"command\":\"sessionUpdate\",\"session\":{\"carPosition\":{\"x\":10,\"y\":20},\"currentRoute\":{\"start\":\"\",\"end\":\"\"}}}"
		));
		pingPongs.Enqueue(new PingPong(websocketClient,
			NetworkingDefinitions.Generator.SetCurrentRoute(1, "bahnhof", "rathaus"),
			"{\"command\":\"sessionUpdate\",\"session\":{\"carPosition\":{\"x\":10,\"y\":20},\"currentRoute\":{\"start\":\"bahnhof\",\"end\":\"rathaus\"}}}"
		));
		pingPongs.Enqueue(new PingPong(websocketClient,
			NetworkingDefinitions.Generator.SetCurrentRoute(1, "schule", "krankenhaus"),
			"{\"command\":\"sessionUpdate\",\"session\":{\"carPosition\":{\"x\":10,\"y\":20},\"currentRoute\":{\"start\":\"bahnhof\",\"end\":\"rathaus\"}}}"
		));
		pingPongs.Enqueue(new PingPong(websocketClient,
			NetworkingDefinitions.Generator.FinishRoute(1),
			"{\"command\":\"sessionUpdate\",\"session\":{\"carPosition\":{\"x\":10,\"y\":20},\"currentRoute\":{\"start\":\"\",\"end\":\"\"}}}"
		));
		pingPongs.Enqueue(new PingPong(websocketClient,
			NetworkingDefinitions.Generator.SetCurrentRoute(1, "schule", "krankenhaus"),
			"{\"command\":\"sessionUpdate\",\"session\":{\"carPosition\":{\"x\":10,\"y\":20},\"currentRoute\":{\"start\":\"schule\",\"end\":\"krankenhaus\"}}}"
		));
		pingPongs.Enqueue(new PingPong(websocketClient,
			NetworkingDefinitions.Generator.LeaveSession(1),
			"{}"
		));
	
		websocketClient.OnOpen += OnOpen;
		Debug.Log("CONNECTING TO " + (NetworkSettings.Instance.IsLocal ? "LOCAL" : "REMOTE"));
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
