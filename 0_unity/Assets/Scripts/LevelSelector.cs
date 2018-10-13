using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour {

	void OnGUI ()
	{
		if (GUI.Button(new Rect(0, 0, 200, 20), "Organizer"))
		{
			SceneManager.LoadScene(1);
		}
		if (GUI.Button(new Rect(0, 30, 200, 20), "Taxi"))
		{
			SceneManager.LoadScene(2);
		}
	}
}
