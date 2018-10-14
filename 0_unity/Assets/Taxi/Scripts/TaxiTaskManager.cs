using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class TaxiTaskManager : MonoBehaviour {

	private WebSocket websocketClient;

	private string start, end;
	private bool startDone = false, endDone = false;

	private string CurrentCheckpoint = "";

	private int SessionID = -1;

	public AudioSource audioSource;
	public AudioClip checkpointSfx;
	public AudioClip failSfx;
	public AudioClip doneSfx;
	public AudioClip newTaskSfx;
	private bool newTaskFlag = false;


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
		if (r.session.currentRoute.start != start || r.session.currentRoute.end != end)
		{
			Debug.LogFormat("Task Update! [from '{0}' to '{1}']", r.session.currentRoute.start, r.session.currentRoute.end);
			start = r.session.currentRoute.start;
			end = r.session.currentRoute.end;
			startDone = false;
			endDone = false;
			if (r.session.currentRoute.start != "" && r.session.currentRoute.end != "")
			{
				newTaskFlag = true;
			}
		}
		if (r.command == "sessionJoin")
		{
			SessionID = r.sessionID;
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		Debug.LogFormat("Entered checkpoint {0}", collider.transform.parent.parent.name);
		if (collider.tag == "checkpoint")
		{
			CurrentCheckpoint = collider.transform.parent.parent.name;
		}
	}

	void OnTriggerExit(Collider collider)
	{
		Debug.LogFormat("Left checkpoint {0}", collider.transform.parent.parent.name);
		if (collider.tag == "checkpoint")
		{
			CurrentCheckpoint = "";
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			SignalCheckpoint(CurrentCheckpoint);
		}

		if (newTaskFlag)
		{
			audioSource.PlayOneShot(newTaskSfx, 0.7f);
			newTaskFlag = false;
		}
	}

	void Connect(int sessionID)
	{
		Debug.Log("Joining Session...");
		websocketClient.Send("{\"command\":\"joinSession\", \"sessionID\": "+sessionID+"}");
		Debug.Log("Joining Session...DONE!");
	}

	void SignalCheckpoint(string checkpoint)
	{
		if (!startDone)
		{
			Debug.LogFormat("Attempt to commit start {0}: {1}", checkpoint, checkpoint == start);
			if (checkpoint == start)
			{
				startDone = true;
				audioSource.PlayOneShot(checkpointSfx, 1.0f);
				return;
			}
			else
			{
				audioSource.PlayOneShot(failSfx, 1.0f);
			}
		}
		if (startDone)
		{
			if (!endDone)
			{
				Debug.LogFormat("Attempt to commit end {0}: {1}", checkpoint, checkpoint == end);
				if (checkpoint == end)
				{
					endDone = true;
					audioSource.PlayOneShot(doneSfx, 0.6f);
				}
				else
				{
					audioSource.PlayOneShot(failSfx, 1.0f);
				}
			}
		}
		if (startDone && endDone)
		{
			Debug.LogFormat("Attemptin to complete task...");
			websocketClient.Send(NetworkingDefinitions.Generator.FinishRoute(SessionID));
		}
	}
	
	void OnGUI()
	{
		if (start == "" && end == "")
		{
			GUI.Label(new Rect(0, Screen.height/2, 600, 30), "Warte auf neuen Auftrag...");
		}
		else if (!startDone)
		{
			GUI.Label(new Rect(0, Screen.height/2, 600, 30), String.Format("Gast abholen - Haltestelle: {0}", start));
		}
		else if (!endDone)
		{
			GUI.Label(new Rect(0, Screen.height/2, 600, 30), String.Format("Gast absetzen - Haltestelle: {0}", end));
		}
	}
}
