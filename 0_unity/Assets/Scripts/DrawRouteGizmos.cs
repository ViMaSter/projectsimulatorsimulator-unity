using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class DrawRouteGizmos : MonoBehaviour
{
	Transform startTransform;
	Transform endTransform;
	void Awake()
	{
		Transform thisTransform = GetComponent<Transform>();
		startTransform = thisTransform.Find("Start").GetComponent<Transform>();
		endTransform = thisTransform.Find("End").GetComponent<Transform>();
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(startTransform.position, 1);
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(endTransform.position, 1);
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(startTransform.position, endTransform.position);
		Handles.Label(endTransform.position, gameObject.name);
	}

}
