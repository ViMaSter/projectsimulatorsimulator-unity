using UnityEngine;

namespace NetworkingDefinitions
{
	[System.Serializable]
	public class CarPosition
	{
	    public float x;
	    public float y;
	}

	[System.Serializable]
	public class Route
	{
	    public float start;
	    public float end;
	}

	[System.Serializable]
	public class Session
	{
	    public CarPosition carPosition;
	    public Route currentRoute;
	}

	[System.Serializable]
	public class Response
	{
	    public string command;
	    public Session session;
	    public int sessionID;
	    public float x;
	    public float y;
	    public string start;
	    public string end;
	}

	public class Generator
	{
		public static string CreateSession()
		{
			Response response = new Response();
			response.command = "createSession";
			return JsonUtility.ToJson(response);
		}

		public static string LeaveSession(int sessionID)
		{
			Response response = new Response();
			response.command = "leaveSession";
			return JsonUtility.ToJson(response);
		}
		
		public static string JoinSession(int sessionID)
		{
			Response response = new Response();
			response.command = "joinSession";
			response.sessionID = sessionID;
			return JsonUtility.ToJson(response);
		}
		
		public static string UpdateCarPosition(int sessionID, float x, float y)
		{
			Response response = new Response();
			response.command = "updateCarPosition";
			response.sessionID = sessionID;
			response.x = x;
			response.y = y;
			return JsonUtility.ToJson(response);
		}
		
		public static string SetCurrentRoute(int sessionID, string start, string end)
		{
			Response response = new Response();
			response.command = "setCurrentRoute";
			response.sessionID = sessionID;
			response.start = start;
			response.end = end;
			return JsonUtility.ToJson(response);
		}
		
		public static string FinishRoute(int sessionID)
		{
			Response response = new Response();
			response.command = "finishRoute";
			response.sessionID = sessionID;
			return JsonUtility.ToJson(response);
		}

		public Response ParseResponse(string rawJSON)
		{
			return JsonUtility.FromJson<NetworkingDefinitions.Response>(rawJSON);
		}
	}
}