using UnityEngine;
using System.Collections;

public class EnemyStats : MonoBehaviour {

	public int currentHP;
	public int maxHP;
	public int moveSpeed;
	private bool onFire;
	private bool frozen;
	private int ongoingDamage;
	private int ongoingTimer;


	// Use this for initialization
	void Start () {
		onFire = false;
		frozen = false;
		currentHP = maxHP;
		ongoingTimer = 30;
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
		Destroy (gameObject);
	}

	public void TakeDamage(int incomingDamage, string damageType){
		currentHP = currentHP - incomingDamage;
		if (damageType == "fire" && !onFire) {
			onFire = true;
			ongoingDamage = 4;
		}
		if (damageType == "ice" && !frozen) {
			Freeze ();
		}
	}

	void Freeze(){
		frozen = true;
		EnemyBehavior mover = GetComponent<EnemyBehavior> ();
		mover.setMoveSpeed(moveSpeed / 2);
	}
}
