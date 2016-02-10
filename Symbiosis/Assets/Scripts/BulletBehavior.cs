using UnityEngine;
using System.Collections;

// interface for Bullet Augments
public interface iAugment {
	// Name of bullet's element
	string Element{ 
		get; 
		set;
	}
	// chance out of 100 of effect occuring
	int OnHitChance{ 
		get; 
		set;
	}
	// effect to trigger on hit
	void onHitEffect(GameObject other);
}
	
public class BulletBehavior : MonoBehaviour {

	public iAugment augment;

	public void setAugment(iAugment aug){
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
				//Destroy (collision.gameObject);
				augment.onHitEffect (collision.gameObject);
			}
		}
	}
}
