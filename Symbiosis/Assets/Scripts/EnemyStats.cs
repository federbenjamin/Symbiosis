﻿using UnityEngine;
using System.Collections;

public class EnemyStats : MonoBehaviour {

	public float currentHP;
	public float maxHP;
	public int moveSpeed;
	public string elementType;
	private bool onFire;
	private bool frozen;
	private float ongoingDamage;
	private int ongoingTimer;
	public Animator enemyAnimator;
	public GameObject spawnParticles;
	float halfDmg, doubleDmg;
	public Object hitSpark;

	// Use this for initialization
	void Start () {
		onFire = false;
		frozen = false;
		currentHP = maxHP;
		ongoingTimer = 30;

		foreach (Transform child in transform) {
			if (child.name == "SpawnParticles") {
				spawnParticles = child.gameObject;
			} else {
				enemyAnimator = child.GetComponent<Animator> ();
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (onFire) {
			if (ongoingTimer <= 0) {
				currentHP = currentHP - ongoingDamage;
				ongoingTimer = 30;
			} else {
				ongoingTimer = ongoingTimer - 1;
			}
		}
		if (currentHP <= 0) {
			Die ();
		}
	}

	void Die(){
		//Play dying animation, and destroy
		// enemyAnimator.SetTrigger("Dead");
		enemyAnimator.Play("Player_Death");
		StartCoroutine ("Wait");
	}

	public void TakeDamage(float incomingDamage, string damageType){
		if (currentHP > 0) {
			GameObject spark = Instantiate (hitSpark, transform.position, transform.rotation) as GameObject;
		}

		DamageMultiplier(incomingDamage, damageType);
		if (!(damageType == "fire" && elementType == "ice")) {
			StatusEffect(damageType);
		} else {
			onFire = false;
			frozen = false;
		}
	}

	void DamageMultiplier(float incomingDamage, string damageType) {
		doubleDmg = (incomingDamage * 2);
		halfDmg = Mathf.Floor(incomingDamage / 2);
		halfDmg = (halfDmg == 0) ? 0.5f : halfDmg;

		if (damageType == elementType) {
			currentHP = currentHP - incomingDamage;
		} else if (damageType == "fire") {
			if (elementType == "ice") {
				currentHP = currentHP - halfDmg;
			} else if (elementType == "earth") {
				currentHP = currentHP - doubleDmg;
			}
		} else if (damageType == "ice") {
			if (elementType == "earth") {
				currentHP = currentHP - halfDmg;
			} else if (elementType == "fire") {
				currentHP = currentHP - doubleDmg;
			}
		} else if (damageType == "earth") {
			if (elementType == "fire") {
				currentHP = currentHP - halfDmg;
			} else if (elementType == "ice") {
				currentHP = currentHP - doubleDmg;
			}
		} else {
			currentHP = currentHP - halfDmg;
		}
	}

	void StatusEffect(string damageType) {
		if (damageType == "fire" && elementType != "fire" && !onFire) {
			Ignite ();
		}
		if (damageType == "ice" && elementType != "ice" && !frozen) {
			Freeze ();
		}
	}

	void Freeze(){
		frozen = true;
		spawnParticles.SetActive(true);
		spawnParticles.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture>("particle-blue");
		EnemyBehavior mover = GetComponent<EnemyBehavior> ();
		mover.setMoveSpeed(moveSpeed / 2);
	}

	void Ignite(){
		onFire = true;
		spawnParticles.SetActive(true);
		spawnParticles.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture>("particle-red");
		ongoingDamage = 0.25f;
	}


	IEnumerator Wait() {
		yield return new WaitForSeconds (0.75f);
		Destroy (gameObject);
	}
}
