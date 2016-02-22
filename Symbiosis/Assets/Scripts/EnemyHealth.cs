using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

	public int currentHP;
	public int maxHP;


	// Use this for initialization
	void Start () {
		currentHP = maxHP;

	}
	
	// Update is called once per frame
	void Update () {
		if (currentHP <= 0) {
			Die ();
		}
	}

	void Die(){
		Destroy (gameObject);
	}

	public void TakeDamage(int incomingDamage, string damageType){
		currentHP = currentHP - incomingDamage;
	}
}
