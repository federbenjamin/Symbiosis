using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatsManager : MonoBehaviour {

	public float playerSpeedModifier;
	public float playerBulletSpeedModifier;
	public float playerFireRateModifier;
	public iAugment playerAugment;
	public string augmentName;

	public float invincibilityTime;
	private HealthManager playersHealth;
	private PlayerShooting playerShooting;
	private float nextHit = 0.0f;

	private float AugTrigger;
	private float WeapTrigger;
	private string playerPrefix;
	private string otherPlayerPrefix;
	private iAugment tempAug;
	private string tempWeap;
	private StatsManager otherPlayerStats;
	private PlayerShooting otherPlayerShooting;
	private GameObject playerAugSprite;
	private GameObject playerWeapSprite;
	private GameObject otherPlayerAugSprite;
	private GameObject otherPlayerWeapSprite;
	private Sprite tempSpr;
	private Sprite tempWeapSpr;
	private float nextSwap = 0.0f;


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
		playerPrefix = gameObject.name;

		if (playerPrefix == "P1") {
			otherPlayerPrefix = "P2";
		} else {
			otherPlayerPrefix = "P1";
		}

		otherPlayerStats = GameObject.Find (otherPlayerPrefix).GetComponent<StatsManager> ();
		playerAugSprite = GameObject.Find (playerPrefix + "Aug");
		otherPlayerAugSprite = GameObject.Find (otherPlayerPrefix + "Aug");
	
		playerShooting = GetComponent<PlayerShooting> ();
		otherPlayerShooting = GameObject.Find (otherPlayerPrefix).GetComponent<PlayerShooting> ();
		playerWeapSprite = GameObject.Find (playerPrefix + "Weap");;
		otherPlayerWeapSprite = GameObject.Find (otherPlayerPrefix + "Weap");;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		AugTrigger = Input.GetAxisRaw ("SwapAug" + playerPrefix);
		WeapTrigger = Input.GetAxisRaw ("SwapWeapon" + playerPrefix);

		if (AugTrigger > 0 && Time.time > nextSwap) {
			tempAug = GetAugment();
			tempSpr = playerAugSprite.GetComponent<Image> ().sprite;

			SetAugment(otherPlayerStats.GetAugment ());
			playerAugSprite.GetComponent<Image>().sprite = otherPlayerAugSprite.GetComponent<Image>().sprite;

			otherPlayerStats.SetAugment (tempAug);
			otherPlayerAugSprite.GetComponent<Image>().sprite = tempSpr;

			nextSwap = Time.time + 2;
		}

		if (WeapTrigger > 0 && Time.time > nextSwap) {
			tempWeap = playerShooting.curWeap;
			tempWeapSpr = playerWeapSprite.GetComponent<Image> ().sprite;

			playerShooting.ChangeWeapon (otherPlayerShooting.curWeap);
			playerWeapSprite.GetComponent<Image> ().sprite = otherPlayerWeapSprite.GetComponent<Image> ().sprite;
		
			otherPlayerShooting.ChangeWeapon (tempWeap);
			otherPlayerWeapSprite.GetComponent<Image> ().sprite = tempWeapSpr;

			nextSwap = Time.time + 2;
		}
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
		if (augment == null) {
			augmentName = "No Augment";
		} else {
			augmentName = augment.Element;
		}
	}

	//Health
	// void OnCollisionStay(Collision collision) {
	// 	if (collision.collider.tag == "Enemy" && Time.time > nextHit) {
	// 		playersHealth.DamageHealth (1);
	// 		nextHit = Time.time + invincibilityTime;
	// 	}
	// }
}
