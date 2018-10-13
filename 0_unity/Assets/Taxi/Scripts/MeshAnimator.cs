using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAnimator : MonoBehaviour {

	public new Material materialH;
	public new Material materialV;
	public Vector2 speed;

	void Update () {
		materialH.mainTextureOffset = new Vector2(1.0f, (Time.time * speed.x) % 1.0f);
		materialV.mainTextureOffset = new Vector2(1.0f, (Time.time * speed.y) % 1.0f);
	}
}
