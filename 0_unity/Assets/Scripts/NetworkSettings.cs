using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSettings : MonoBehaviour {

	public static NetworkSettings Instance;
	
	public bool IsLocal;

	void Awake()
	{
		NetworkSettings.Instance = this;
	}
}
