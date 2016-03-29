using UnityEngine;
using System.Collections;

public class FanSpinning : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!GamePause.isPaused) {
			transform.Rotate (0, 0, 2);
		}
	}
}
