using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;

[System.Serializable]
public class Route
{
    public string name;
    public Vector2 location;
    public Route(string name, Vector2 location)
    {
		this.name = name;
		this.location = location;
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
			routes.Add(new Route(child.gameObject.name, child.position));
		}
		Scene scene = new Scene(routes);
		return JsonUtility.ToJson(scene);
	}
}