using UnityEngine;
using System.Collections;
	
public class EnemyBulletBehavior : MonoBehaviour {

	public iAugment augment;
	public int bulletDamage;

	public void setAugment(iAugment aug){
		augment = aug;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider c) {
		GameObject other = c.gameObject;
		//If it hits a player don't disappear
		if (other.tag != "Enemy" && other.tag != "Bullet" && other.tag != "Room") {
			Destroy (gameObject);
		}
		if (other.tag == "Bullet") {
			Physics.IgnoreCollision (other.GetComponent<Collider> (), GetComponent<Collider> ());
		}
		if (other.tag == "Player") {
			string damageType = "none";

			bool playersTogether = GameObject.FindWithTag("Canvas").GetComponent<GameStats>().PlayersTogether;
			if (!playersTogether) {
				GameObject.Find ("Camera"+ other.name).GetComponent<CameraShaker> ().shake = 0.1f;
			} else {
				GameObject.Find ("CameraP1").GetComponent<CameraShaker> ().shake = 0.1f;
			}

			if (augment != null) {
				Debug.Log ("");
				//Destroy (collision.gameObject);
				augment.onHitEffect (other);
				damageType = augment.Element;
			}
			HealthManager playerHP = GameObject.Find ("Health").GetComponent<HealthManager>();
			playerHP.DamageHealth (bulletDamage);
		}
	}
}
