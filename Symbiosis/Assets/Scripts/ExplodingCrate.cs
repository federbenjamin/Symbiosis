using UnityEngine;
using System.Collections;

public class ExplodingCrate : MonoBehaviour {
	private Collider P1Collider;
	private Collider P2Collider;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
		}
	}
}
