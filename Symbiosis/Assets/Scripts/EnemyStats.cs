using UnityEngine;
using System.Collections;

public class EnemyStats : MonoBehaviour {

	public int currentHP;
	public int maxHP;
	public int moveSpeed;
	public string elementType;
	private bool onFire;
	private bool frozen;
	private int ongoingDamage;
	private int ongoingTimer;
	public Animator enemyAnimator;
	public GameObject spawnParticles;


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
	void Update () {
		if (onFire) {
			if (ongoingTimer <= 0) {
				currentHP = currentHP - ongoingDamage;
				ongoingTimer = 60;
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

	public void TakeDamage(int incomingDamage, string damageType){
		DamageMultiplier(incomingDamage, damageType);
		if (!(damageType == "fire" && elementType == "ice")) {
			StatusEffect(damageType);
		}
	}

	void DamageMultiplier(int incomingDamage, string damageType) {
		if (damageType == elementType) {
			currentHP = currentHP - incomingDamage;
		} else if (damageType == "fire") {
			if (elementType == "ice") {
				currentHP = currentHP - (int)Mathf.Floor(incomingDamage / 2);
			} else if (elementType == "earth") {
				currentHP = currentHP - (int)(incomingDamage * 2);
			}
		} else if (damageType == "ice") {
			if (elementType == "earth") {
				currentHP = currentHP - (int)Mathf.Floor(incomingDamage / 2);
			} else if (elementType == "fire") {
				currentHP = currentHP - (int)(incomingDamage * 2);
			}
		} else if (damageType == "earth") {
			if (elementType == "fire") {
				currentHP = currentHP - (int)Mathf.Floor(incomingDamage / 2);
			} else if (elementType == "ice") {
				currentHP = currentHP - (int)(incomingDamage * 2);
			}
		} else {
			currentHP = currentHP - incomingDamage;
		}
	}

	void StatusEffect(string damageType) {
		if (damageType == "fire" && !onFire) {
			Ignite ();
		}
		if (damageType == "ice" && !frozen) {
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
		ongoingDamage = 4;
	}


	IEnumerator Wait() {
		yield return new WaitForSeconds (0.75f);
		Destroy (gameObject);
	}
}
