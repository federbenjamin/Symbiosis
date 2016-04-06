using UnityEngine;
using System.Collections;


public class HitSpark : MonoBehaviour {
	public float lifespan;
	public float startSize;
	public float endSize;
	public float speed;
	private float time;

	public float alphaValue = 0.7f;
	// Use this for initialization
	void Start () {
		time = 0;
	}

	// Update is called once per frame
	void Update () {
		time += 1;
		transform.Rotate(Vector3.left, speed * Time.deltaTime);

		alphaValue = 1 - (time / lifespan);

		Material material;
		material = GetComponent<Renderer>().material;

		material.SetFloat("_Mode", 2);
		material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		material.SetInt("_ZWrite", 0);
		material.DisableKeyword("_ALPHATEST_ON");
		material.EnableKeyword("_ALPHABLEND_ON");
		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = 3000;
		Color newColor = material.color;
		newColor.a = alphaValue;
		material.SetColor("_Color", newColor);

		transform.localScale = new Vector3(1, 1, 1) * (startSize + ((time / lifespan) * (endSize - startSize)));
		if (time >= lifespan) {
			Destroy (this.gameObject);
		}
	}
}
