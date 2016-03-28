using UnityEngine;
using System.Collections;

public class HoopController : MonoBehaviour {
	private GameObject lookAtObject;
	private Renderer hoopRenderer;
	private float alphaDelta;
	private float fadeTimer = 20;
	private int showTimer;

	// Use this for initialization
	void Start () {
		if (transform.parent.name == "P1") {
			lookAtObject = GameObject.Find("P2");
		} else {
			lookAtObject = GameObject.Find("P1");
		}
		foreach (Transform child in transform) {
			if (child.tag == "Hoop") {
				hoopRenderer = child.gameObject.GetComponent<Renderer>();
			}
		}
		alphaDelta = 1f / fadeTimer;
	}
	
	// Update is called once per frame
	void Update () {
		if (showTimer == 0) {
			foreach (Material mat in hoopRenderer.materials) {
				mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, mat.color.a - alphaDelta);
			}
		}
	}

	void FixedUpdate () {
		transform.LookAt (lookAtObject.transform);
		if (showTimer > 0) {
			showTimer--;
		}
	}

	public void Show() {
		foreach (Material mat in hoopRenderer.materials) {
			mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1);
		}
		showTimer = 50;
	}
}
