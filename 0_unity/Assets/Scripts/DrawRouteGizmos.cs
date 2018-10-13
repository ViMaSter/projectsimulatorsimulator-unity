#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class DrawRouteGizmos : MonoBehaviour
{
	Transform tranform;
	void Awake()
	{
		tranform = GetComponent<Transform>();
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(tranform.position, 1);
		Handles.Label(tranform.position, gameObject.name);
	}

}
#endif