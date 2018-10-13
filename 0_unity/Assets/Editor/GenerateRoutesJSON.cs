using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;


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
            Debug.Log(Application.dataPath + "/../../../../0_server/0_git/maps");
            var path = EditorUtility.SaveFilePanel(
                "Save Route JSON",
                Application.dataPath + "/../../../../0_server/0_git/maps",
                EditorSceneManager.GetActiveScene().name + ".json",
                "json"
            );

            if(path != string.Empty)
            {
	            string json = GeneratePathJSON();

                File.WriteAllBytes(
                    path,
                    System.Text.Encoding.UTF8.GetBytes(json)
                );
                File.WriteAllBytes(
                    Application.dataPath+"/Resources/MapData/"+EditorSceneManager.GetActiveScene().name + ".json",
                    System.Text.Encoding.UTF8.GetBytes(json)
                );
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