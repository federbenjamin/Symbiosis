using UnityEngine;
using System.Collections;

public class FadeOut : MonoBehaviour {

	private Renderer renderer;
	private float alphaValue;
	private bool startFadeOut = false;

	// Use this for initialization
	void Start () {
		StartCoroutine("WaitForFadeOut");
		renderer = transform.GetComponent<Renderer>();
		alphaValue = renderer.material.color.a;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (startFadeOut) {
			alphaValue = alphaValue - 0.005f;
			Color newColor = renderer.material.color;
			newColor.a = alphaValue;
			renderer.material.SetColor("_Color", newColor);
		}

		if (alphaValue <= 0) {
			Destroy(this.gameObject);
		}
	}

	IEnumerator WaitForFadeOut() {
		yield return new WaitForSeconds (4f);
		startFadeOut = true;
	}
}
