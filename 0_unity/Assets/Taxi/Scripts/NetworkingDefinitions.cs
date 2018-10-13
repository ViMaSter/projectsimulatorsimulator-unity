namespace NetworkingDefinitions
{
	[System.Serializable]
	public class CarPosition
	{
	    public float x;
	    public float y;
	}

	[System.Serializable]
	public class Session
	{
	    public CarPosition carPosition;
	}

	[System.Serializable]
	public class Response
	{
	    public string command;
	    public Session session;
	}
}