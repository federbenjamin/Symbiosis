using UnityEngine;
using System.Collections;

public class PlayerSword : MonoBehaviour {

	Animator swordAnimator;
	public iAugment augment;
	public int swordDamage;

	public bool isSwinging;

	public void setAugment(iAugment aug){
		Debug.Log ("On-hit effects set");
		augment = aug;
	}

	// Use this for initialization
	void Start () {
		swordAnimator = transform.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StopSwinging() {
		isSwinging = false;
		swordAnimator.SetTrigger ("Stop");
	}

	public void Swing() {
		isSwinging = true;
		swordAnimator.SetTrigger ("Attack");
	}

	void OnTriggerEnter(Collider c) {
		GameObject other = c.gameObject;

		if (other.tag == "Enemy") {
			string damageType = "none";
			float force = 500;
			if (augment != null) {
				augment.onHitEffect (other);
				damageType = augment.Element;
				if (augment.Element == "earth") {
					force = 1000;
				}
			}

			//TODO: Add enemy Knockback

			EnemyStats enemyHP = other.GetComponent<EnemyStats> ();
			enemyHP.TakeDamage (swordDamage, damageType);
		}
	}
}

