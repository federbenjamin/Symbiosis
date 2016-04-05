using UnityEngine;
using System.Collections;
	
public class BulletBehavior : MonoBehaviour {

	public iAugment augment;
	public float bulletDamage;

	public AudioClip sound;
	public AudioClip sound2;
	private AudioPlacement AP;
	public void setAugment(iAugment aug){
		augment = aug;
	}

	// Use this for initialization
	void Start () {
		AP = GameObject.Find ("AudioListener").GetComponent<AudioPlacement> ();
		AP.PlayClip ("SFX/bullet_fire", 0.1f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider c) {
		GameObject other = c.gameObject;
		//If it hits a player don't disappear
		if (other.tag != "Player" && other.tag != "Bullet" && other.tag != "Room" && other.tag != "TutorialRoom" && other.tag != "Pickups" && other.tag != "Pipe") {
			AP.PlayClip ("SFX/bullet_die", 0.05f);
			Destroy (gameObject);
		}
		if (other.tag == "Bullet" || other.tag == "Pickups") {
			Physics.IgnoreCollision (other.GetComponent<Collider> (), GetComponent<Collider> ());
		}

		if (other.tag == "Enemy") {
			string damageType = "none";
			float force = 100;
			if (augment != null) {
				Debug.Log ("");
				//Destroy (collision.gameObject);
				augment.onHitEffect (other);
				damageType = augment.Element;
				if (augment.Element == "earth") {
					force = 400;
				}
			}

			Vector3 bulletDir = this.gameObject.transform.forward;
			bulletDir.y = 0;
			other.GetComponent<Rigidbody> ().AddForce (bulletDir.normalized * force);

			EnemyStats enemyHP = other.GetComponent<EnemyStats>();
			enemyHP.TakeDamage (bulletDamage, damageType);
		}
	}
}
