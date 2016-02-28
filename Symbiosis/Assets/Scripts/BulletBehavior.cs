using UnityEngine;
using System.Collections;
	
public class BulletBehavior : MonoBehaviour {

	public iAugment augment;
	public int bulletDamage;

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
		if (other.tag != "Player" && other.tag != "Bullet") {
			Destroy (gameObject);
		}
		if (other.tag == "Bullet") {
			Physics.IgnoreCollision (other.GetComponent<Collider> (), GetComponent<Collider> ());
		}

		if (other.tag == "Enemy") {
			string damageType = "none";
			if (augment != null) {
				Debug.Log ("");
				//Destroy (collision.gameObject);
				augment.onHitEffect (other);
				damageType = augment.Element;
			}
			EnemyHealth enemyHP = other.GetComponent<EnemyHealth>();
			enemyHP.TakeDamage (bulletDamage, damageType);
		}
	}
}
