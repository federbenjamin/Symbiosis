using UnityEngine;
using System.Collections;

public class Spikes : MonoBehaviour {

	private Vector3 dir;
	private float nextHit = 0.0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider other) {
		if (other.tag == "Player" && Time.time > nextHit) {
			if (!HealthManager.isGameOver) {
				bool playersTogether = GameObject.FindWithTag("Canvas").GetComponent<GameStats>().PlayersTogether;
				if (!playersTogether) {
					GameObject.Find ("Camera"+ other.name).GetComponent<CameraShaker> ().shake = 0.15f;
				} else {
					GameObject.Find ("CameraP1").GetComponent<CameraShaker> ().shake = 0.15f;
				}

				HealthManager.DamageHealth(1);
				nextHit = Time.time + 1;
				dir = other.transform.position - transform.position;
				other.GetComponent<Rigidbody>().AddForce(dir * 50);
			}
		}
	}
}
