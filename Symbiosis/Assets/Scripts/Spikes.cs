using UnityEngine;
using System.Collections;

public class Spikes : MonoBehaviour {

	private HealthManager healthManager;
	private Vector3 dir;

	// Use this for initialization
	void Start () {
		healthManager = GameObject.FindWithTag("Health").GetComponent<HealthManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider other) {
		if (other.tag == "Player") {
			healthManager.DamageHealth(1);
			dir = other.transform.position - transform.position;
			other.GetComponent<Rigidbody>().AddForce(dir * 400);
		}
	}
}
