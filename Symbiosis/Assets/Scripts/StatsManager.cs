using UnityEngine;
using System.Collections;

public class StatsManager : MonoBehaviour {

	public float playerSpeedModifier;
	public float playerBulletSpeedModifier;
	public float playerFireRateModifier;
	public iAugment playerAugment;
	public string augmentName;

	public float invincibilityTime;
	private HealthManager playersHealth;
	private float nextHit = 0.0f;

	void Awake () {
		//Get the HealthManager Script
		playersHealth = GameObject.Find("Health").GetComponent<HealthManager> ();
	}

	// Use this for initialization
	void Start () {
		playerSpeedModifier = 0f;
		playerBulletSpeedModifier = 0f;
		playerFireRateModifier = 0f;
		playerAugment = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Gets Player's Speed
	public float GetSpeed() {
		return playerSpeedModifier;
	}

	//Sets Player's Speed
	public void SetSpeed(float modifier) {
		playerSpeedModifier += modifier;
	}

	//Gets Player's FireRate
	public float GetFireRate() {
		return playerFireRateModifier;
	}

	//Sets Player's FireRate
	public void SetFireRate(float modifier) {
		playerFireRateModifier -= modifier;
	}

	//Gets Player's BulletSpeed
	public float GetBulletSpeed() {
		return playerBulletSpeedModifier;
	}

	//Sets Player's BulletSpeed
	public void SetBulletSpeed(float modifier) {
		playerBulletSpeedModifier += modifier;
	}
		
	public iAugment GetAugment() {
		return playerAugment;
	}

	public void SetAugment (iAugment augment){
		playerAugment = augment;
		augmentName = augment.Element;
	}
	//Health
	void OnCollisionStay(Collision collision) {
		if (collision.collider.name == "Enemy1" && Time.time > nextHit) {
			playersHealth.DamageHealth (1);
			nextHit = Time.time + invincibilityTime;
		}
	}
}
