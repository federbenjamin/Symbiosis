using UnityEngine;
using System.Collections;
	
public class BulletBehavior : MonoBehaviour {

	public iAugment augment;

	public void setAugment(iAugment aug){
		Debug.Log ("On-hit effects set");
		augment = aug;
	}

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
		if (collision.collider.tag == "Enemy") {
			if (augment != null) {
				Debug.Log ("");
				//Destroy (collision.gameObject);
				augment.onHitEffect (collision.gameObject);
			}
		}
	}
}
