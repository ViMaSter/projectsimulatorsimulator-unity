using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHelper : MonoBehaviour {

	public GameObject MapItem;

	void Start ()
	{
		Transform rootTransform = GameObject.Find("Routes").GetComponent<Transform>();
		Transform thisTransform = gameObject.GetComponent<Transform>();
		foreach (Transform child in rootTransform)
		{
			Debug.Log("Created label for " + child.name);
			GameObject label = Instantiate(MapItem, child.Find("End").GetComponent<Transform>().position, Quaternion.identity, child);
			label.name = "Label";
			label.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = child.name;
		}
	}
}
