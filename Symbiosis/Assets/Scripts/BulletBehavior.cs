using UnityEngine;
using System.Collections;
	
public class BulletBehavior : MonoBehaviour {

	public iAugment augment;
	public int bulletDamage;

	public AudioClip sound;

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

	void OnTriggerEnter(Collider c) {
		GameObject other = c.gameObject;
		//If it hits a player don't disappear
		if (other.tag != "Player" && other.tag != "Bullet" && other.tag != "Room") {
			AudioSource.PlayClipAtPoint (sound, transform.position);
			Destroy (gameObject);
		}
		if (other.tag == "Bullet") {
			Physics.IgnoreCollision (other.GetComponent<Collider> (), GetComponent<Collider> ());
		}

		if (other.tag == "Enemy") {
			string damageType = "none";
			float force = 500;
			if (augment != null) {
				Debug.Log ("");
				//Destroy (collision.gameObject);
				augment.onHitEffect (other);
				damageType = augment.Element;
				if (augment.Element == "earth") {
					force = 1000;
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
