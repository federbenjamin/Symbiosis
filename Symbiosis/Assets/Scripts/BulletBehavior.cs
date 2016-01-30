using UnityEngine;
using System.Collections;

public class BulletBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision) {

		//If it hits a player don't disappear
		if (collision.collider.tag != "Player") {
			Destroy (gameObject);
		}
	}
}
