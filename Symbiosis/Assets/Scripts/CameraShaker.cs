using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour {

	public float shake = 0.0f;
	public float shakeAmount = 0.01f;
	public float decreaseFactor = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (shake > 0) {
			Vector3 offset = Random.insideUnitSphere * shakeAmount;
			offset.y = 0;
			offset.z = 0;
			transform.localPosition = transform.localPosition + offset;
			shake -= Time.deltaTime * decreaseFactor;

		} else {
			shake = 0.0f;
		}
	}
}
