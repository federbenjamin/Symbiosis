﻿using UnityEngine;
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
		Color newColor = new Color ();

		if (augment != null) {
			if (augment.Element == "fire") {
				ColorUtility.TryParseHtmlString ("#C70A09", out newColor);
			} else if (augment.Element == "ice") {
				Debug.Log ("Ice AUgment");
				ColorUtility.TryParseHtmlString ("#033EC7", out newColor);
			} else if (augment.Element == "earth") {
				ColorUtility.TryParseHtmlString ("#0E5910", out newColor);
			}
		}else {
			ColorUtility.TryParseHtmlString ("#CCCCCC", out newColor);
		}

		Renderer renderer = transform.GetComponent<Renderer> ();
		foreach (Material matt in renderer.materials) {
			if (matt.name == "RedGlow (Instance)") {
				matt.SetColor("_Color", newColor);
			}
		}
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

