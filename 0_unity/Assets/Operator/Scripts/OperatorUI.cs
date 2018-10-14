using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

class Helper
{
	public static List<string> GenerateRoute(System.Random random, List<string> list, List<string> except)
	{
		int startIndex = random.Next(0, list.Count);
		int endIndex = random.Next(0, list.Count);
		
		while (except.Contains(list[startIndex]))
		{
			startIndex = (startIndex+1) % list.Count;
		}

		while (except.Contains(list[endIndex]) || startIndex == endIndex)
		{
			endIndex = (endIndex+1) % list.Count;
		}

		return new List<string>{list[startIndex], list[endIndex]};
	}
}

public class OperatorUI : MonoBehaviour {

	private WebSocket websocketClient;

	private List<string> availableLandmarks;
	private List<List<string>> offeredRoutes = new List<List<string>>(3);

	private string currentStart;
	private string currentEnd;
	private string lastMapName = "";
	private string mapName = "";
	private int SessionID = -1;

	private bool UIDirtyFlag = false;

	public AudioSource audioSource;
	public AudioClip newTaskSfx;
	public bool audioDirtyFlag;

	System.Random randomness = new System.Random();

	void Awake()
	{
		Debug.Log("[OPERA] ATTEMPT!");
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
		Connect(-1);
	}

	void RefillList(List<string> disqualifiedLandmarks)
	{
		Debug.LogFormat("Refilling available list...");
		offeredRoutes.Clear();
		for (int i = 0; i < 3; i++)
		{
			offeredRoutes.Add(Helper.GenerateRoute(randomness, this.availableLandmarks, disqualifiedLandmarks));
			disqualifiedLandmarks.Add(offeredRoutes[i][0]);
			disqualifiedLandmarks.Add(offeredRoutes[i][1]);
			Debug.LogFormat("Adding {0}->{1}", offeredRoutes[i][0], offeredRoutes[i][1]);
		}	
	}

	void OnMessage(object sender, WebSocketSharp.MessageEventArgs message)
	{
		NetworkingDefinitions.Response r = JsonUtility.FromJson<NetworkingDefinitions.Response>(message.Data);
		Debug.Log(r);
		if (r.command == "sessionJoin")
		{
			mapName = r.mapName;
			currentStart = r.session.currentRoute.start;
			currentEnd = r.session.currentRoute.end;
			SessionID = r.sessionID;
		}

		if (r.session.currentRoute.start != currentStart || r.session.currentRoute.end != currentEnd)
		{
			Debug.LogFormat("[OPERA] Task Update! [from '{0}' to '{1}']", r.session.currentRoute.start, r.session.currentRoute.end);
			
			currentStart = r.session.currentRoute.start;
			currentEnd = r.session.currentRoute.end;
			UIDirtyFlag = false;

			if (currentStart == "" && currentEnd == "")
			{
				audioDirtyFlag = true;
			}
		}
	}

	void CheckAudioDirty()
	{
		if (audioDirtyFlag)
		{
			audioSource.PlayOneShot(newTaskSfx, 1.0f);
			audioDirtyFlag = false;
		}
	}

	void Connect(int sessionID)
	{
		Debug.Log("[OPERA] Joining Session...");
		websocketClient.Send("{\"command\":\"joinSession\", \"sessionID\": "+sessionID+"}");
		Debug.Log("[OPERA] Joining Session...DONE!");
	}

	void SelectRoute(List<string> route)
	{
		Debug.LogFormat("[OPERA] Selecting from '{0}' to '{1}'", route[0], route[1]);
		websocketClient.Send(NetworkingDefinitions.Generator.SetCurrentRoute(SessionID, route[0], route[1]));
	}

	void CheckMapChange()
	{
		if (lastMapName != mapName)
		{
			availableLandmarks = new List<string>();
			Debug.LogFormat("Filling landmarks from map '{0}'", mapName);
			lastMapName = mapName;
			Debug.Log("MapData/" + mapName + ".json");
			TextAsset targetFile = Resources.Load<TextAsset>("MapData/" + mapName) as TextAsset;
			Debug.Log(targetFile);
			Scene loadedData = JsonUtility.FromJson<Scene>(targetFile.text);
			foreach (Route location in loadedData.routes)
			{
				availableLandmarks.Add(location.name);
			}
			RefillList(new List<string>{currentStart, currentEnd});
		}
	}

	void Update()
	{
		CheckMapChange();
		CheckAudioDirty();
	}

	void OnGUI()
	{
		GUI.enabled = (currentStart == "" && currentEnd == "" && !UIDirtyFlag);

		float height = 20;
		float margin = 10;
		for (int i = 0; i < offeredRoutes.Count; i++)
		{
			if (GUI.Button(new Rect(0, (height + margin) * i, (int)(Screen.width * 0.4), 20), string.Format("'{0}' -> '{1}'", offeredRoutes[i][0], offeredRoutes[i][1])))
			{
				websocketClient.Send(NetworkingDefinitions.Generator.SetCurrentRoute(SessionID, offeredRoutes[i][0], offeredRoutes[i][1]));
				RefillList(offeredRoutes[i]);
				UIDirtyFlag = true;
			}
		}
	}
}
