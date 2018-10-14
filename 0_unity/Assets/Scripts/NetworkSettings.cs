using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class NetworkSettings : MonoBehaviour {

	public static NetworkSettings Instance;
	
	public bool IsLocal;

	public static string WebSocketServer
	{
		get
		{
			return NetworkSettings.Instance.IsLocal ? "ws://localhost:5000/" : "wss://projectsimulatorsimulator.herokuapp.com";
		}
	}


	[DllImport("user32.dll", EntryPoint = "SetWindowText")]
	public static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);

	[DllImport("user32.dll", EntryPoint = "FindWindow")]
	public static extern System.IntPtr FindWindow(System.String className, System.String windowName);


	void Awake()
	{
		if (System.Environment.GetCommandLineArgs().Length > 1)
		{
			if (System.Environment.GetCommandLineArgs()[1] == "--local")
			{
				IsLocal = true;
				var windowPtr = FindWindow(null, "projectsimulatorsimulator");
				SetWindowText(windowPtr, "LOCAL");
			}
			if (System.Environment.GetCommandLineArgs()[1] == "--remote")
			{
				IsLocal = false;
				var windowPtr = FindWindow(null, "projectsimulatorsimulator");
				SetWindowText(windowPtr, "REMOTE");
			}
		}
		NetworkSettings.Instance = this;
	}
}
