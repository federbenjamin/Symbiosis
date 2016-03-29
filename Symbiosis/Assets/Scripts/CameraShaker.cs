using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour {

	public float shake = 0.0f;
	public float shakeAmount = 100.01f;
	public float decreaseFactor = 1.0f;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (!GamePause.isPaused) {
			if (shake > 0) {
				Vector3 offset = Random.insideUnitSphere * shakeAmount;
				transform.localPosition = offset;
				shake -= Time.deltaTime * decreaseFactor;
				if (shake <= 0) {
					transform.localPosition = new Vector3 (0, 0, 0);
				}

			} else {
				shake = 0.0f;
			}
		}
	}
}
