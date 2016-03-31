using UnityEngine;
using System.Collections;

public class ExplodingCrate : MonoBehaviour {
	private Collider P1Collider;
	private Collider P2Collider;
	private Renderer[] rendererObjects;
	private bool fadeOut;
	private bool setFadeRendering;
	private float alphaValue = 1f;

	// Use this for initialization
	void Start () {
		//rendererObjects = GetComponentsInChildren<Renderer>(); 
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (fadeOut) {
			alphaValue = alphaValue - 0.02f;
			Material material;
			//foreach (Renderer child in rendererObjects) {
			foreach (Transform child in transform) {
				material = child.GetComponent<Renderer>().material;
				if (!setFadeRendering) {
					material.SetFloat("_Mode", 2);
					material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
					material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					material.SetInt("_ZWrite", 0);
					material.DisableKeyword("_ALPHATEST_ON");
					material.EnableKeyword("_ALPHABLEND_ON");
					material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
					material.renderQueue = 3000;
	            }
	            
				Color newColor = material.color;
				newColor.a = alphaValue;
				material.SetColor("_Color", newColor);
			}
			setFadeRendering = true;
		}
		if (alphaValue <= 0) {
			Destroy(this.gameObject);
		}
	}

	void OnTriggerEnter (Collider other) {
		//If Player, or enemey, or bullets hit, or weapon hit, explode!
		if (other.tag == "Enemy" || other.tag == "Player" || other.tag == "Bullet" || other.tag == "Weapon") {
			P1Collider = GameObject.Find("P1").GetComponent<Collider>();
			P2Collider = GameObject.Find("P2").GetComponent<Collider>();
			foreach (Transform child in transform) {
				child.GetComponent<Rigidbody>().isKinematic = false;

				//Ignore physics with player so they don't get stuck
				Physics.IgnoreCollision(P1Collider, child.GetComponent<Collider>());
				Physics.IgnoreCollision(P2Collider, child.GetComponent<Collider>());
			}

			StartCoroutine("FadeOut");
		}
	}

	IEnumerator FadeOut() {
		yield return new WaitForSeconds (1f);
		fadeOut = true;
	}
}
