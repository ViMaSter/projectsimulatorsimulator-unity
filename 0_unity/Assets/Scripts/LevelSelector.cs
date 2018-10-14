using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour {

	public GameObject taxiPlayer;

	void OnGUI ()
	{
		if (GUI.Button(new Rect(0, 0, 200, 20), "Organizer"))
		{
			SceneManager.LoadScene(2, LoadSceneMode.Additive);
			SceneManager.LoadScene(1, LoadSceneMode.Additive);
			Destroy(this.gameObject);
		}
		if (GUI.Button(new Rect(0, 30, 200, 20), "Taxi"))
		{
			SceneManager.LoadScene(3, LoadSceneMode.Additive);
			Destroy(this.gameObject);
		}
	}
}
