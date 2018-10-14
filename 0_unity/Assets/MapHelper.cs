using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHelper : MonoBehaviour {

	public GameObject MapItem;

	void Awake ()
	{
		Transform rootTransform = GameObject.Find("Routes").GetComponent<Transform>();
		foreach (Transform child in rootTransform)
		{
			// Debug.Log("Created label for " + child.name);
			GameObject label = Instantiate(MapItem, child.position, Quaternion.identity, child);
			label.name = "Label";
			label.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = child.name;
		}
	}
}
