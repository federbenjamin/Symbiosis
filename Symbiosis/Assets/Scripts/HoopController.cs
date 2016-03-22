using UnityEngine;
using System.Collections;

public class HoopController : MonoBehaviour {
	public GameObject lookAtObject;
	private Renderer hoopRenderer;
	private float alphaDelta;
	public float fadeTimer;

	// Use this for initialization
	void Start () {
		foreach (Transform child in transform) {
			if (child.tag == "Hoop") {
				hoopRenderer = child.gameObject.GetComponent<Renderer>();
			}
		}
		alphaDelta = 1f / fadeTimer;
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Material mat in hoopRenderer.materials) {
			mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, mat.color.a - alphaDelta);
		}
	}

	void FixedUpdate () {
		transform.LookAt (lookAtObject.transform);
	}

	public void Show() {
		foreach (Material mat in hoopRenderer.materials) {
			mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1);
		}
	}
}
