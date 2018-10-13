using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;

[System.Serializable]
public class Route
{
    public Vector2 start;
    public Vector2 end;
    public Route(Vector2 start, Vector2 end)
    {
		this.start = start;
		this.end = end;
    }
}

[System.Serializable]
public class Scene
{
	public List<Route> routes;
	public Scene(List<Route> routes)
	{
		this.routes = routes;
	}
}


public class GenerateRoutesJSON : EditorWindow
{
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/SimulatorSimulator/Generate Route JSON")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        GenerateRoutesJSON window = (GenerateRoutesJSON)EditorWindow.GetWindow(typeof(GenerateRoutesJSON));
        window.Show();
    }

	void OnGUI () {
		if(GUILayout.Button("Generate JSON for scene"))
        {
            var path = EditorUtility.SaveFilePanel(
                "Save Route JSON",
                Path.Combine(Path.Combine(Application.dataPath, "Scenes"), "MapData"),
                EditorSceneManager.GetActiveScene().name + ".json",
                "json"
            );

            if(path != string.Empty)
            {
	            string json = GeneratePathJSON();

	            File.WriteAllBytes(path, System.Text.Encoding.UTF8.GetBytes(json));
            }
        }
	}

	string GeneratePathJSON()
	{
		Transform rootTransform = GameObject.Find("Routes").GetComponent<Transform>();
		List<Route> routes = new List<Route>(rootTransform.childCount);
		foreach (Transform child in rootTransform)
		{
			Transform startTransform = child.Find("Start").GetComponent<Transform>();
			Transform endTransform = child.Find("End").GetComponent<Transform>();
			routes.Add(new Route(startTransform.position, endTransform.position));
		}
		Scene scene = new Scene(routes);
		return JsonUtility.ToJson(scene);
	}
}