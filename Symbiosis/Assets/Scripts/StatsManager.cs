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
	public HealthManager playersHealth;
	private PlayerShooting playerShooting;
	private float nextHit = 0.0f;

	private float AugTrigger;
	//private float WeapTrigger;
	//string swapButtonWeap;
	string swapButtonAug;
	private string playerPrefix;
	private string otherPlayerPrefix;
	private iAugment tempAug;
	private string tempWeap;
	private StatsManager otherPlayerStats;
	private PlayerShooting otherPlayerShooting;
	private GameObject playerAugSprite;
	//private GameObject playerWeapSprite;
	private GameObject otherPlayerAugSprite;
	//private GameObject otherPlayerWeapSprite;
	private Image otherPlayerHudImage;
	private Sprite hudDefault;
	private Sprite hudReq;
	// private Sprite hud1;
	// private Sprite hud2;
	// private Sprite hud3;
	private Sprite tempSpr;
	//private Sprite tempWeapSpr;
	private static float nextAugSwap = 0.0f;
	private static float nextWeapSwap = 0.0f;

	private bool requestAugSwap;
	public bool RequestAugSwap {
		get{return requestAugSwap;}
		set{requestAugSwap = value;}
	}
	private bool requestWeapSwap;
	public bool RequestWeapSwap {
		get{return requestWeapSwap;}
		set{requestWeapSwap = value;}
	}
	private int swapAugTimeout;
	private int swapWeapTimeout;
	public AudioClip swapCooldownSound;
	private bool nextWeapSwapFailedSound = false;
	private bool nextAugSwapFailedSound = false;

	private HoopController hoopController;
	private string pingButton;
	private float nextHoopShow = 0.0f;

	private AudioPlacement audioPlacement;

	void Awake () {
		//Get the HealthManager Script
		playersHealth = GameObject.Find("Health").GetComponent<HealthManager> ();
		audioPlacement = GameObject.Find("AudioListener").GetComponent<AudioPlacement> ();
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

		if ((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer)) {
			swapButtonAug = "SwapAugMac" + playerPrefix;
			//swapButtonWeap = "SwapWeaponMac" + playerPrefix;
		} else if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer)) {
			swapButtonAug = "SwapAugPC" + playerPrefix;
			//swapButtonWeap = "SwapWeaponPC" + playerPrefix;
		}

		otherPlayerStats = GameObject.Find (otherPlayerPrefix).GetComponent<StatsManager> ();
		playerAugSprite = GameObject.Find (playerPrefix + "Aug");
		otherPlayerAugSprite = GameObject.Find (otherPlayerPrefix + "Aug");
		otherPlayerHudImage = GameObject.Find(otherPlayerPrefix + "Hud").GetComponent<Image>();
		if (playerPrefix == "P1") {
			hudDefault = Resources.Load<Sprite> ("Interface/P2-slots-blank");
			hudReq = Resources.Load<Sprite> ("Interface/P2-slots-prompt");
			// hud1 = Resources.Load<Sprite> ("YellowSlots/YellowSlots1");
			// hud2 = Resources.Load<Sprite> ("YellowSlots/YellowSlots2");
			// hud3 = Resources.Load<Sprite> ("YellowSlots/YellowSlots3");
		} else if (playerPrefix == "P2") {
			hudDefault = Resources.Load<Sprite> ("Interface/P1-slots-blank");
			hudReq = Resources.Load<Sprite> ("Interface/P1-slots-prompt");
			// hud1 = Resources.Load<Sprite> ("BlueSlots/BlueSlots1");
			// hud2 = Resources.Load<Sprite> ("BlueSlots/BlueSlots2");
			// hud3 = Resources.Load<Sprite> ("BlueSlots/BlueSlots3");
		}
		requestAugSwap = false;
		//requestWeapSwap = false;
	
		playerShooting = GetComponent<PlayerShooting> ();
		otherPlayerShooting = GameObject.Find (otherPlayerPrefix).GetComponent<PlayerShooting> ();
		//playerWeapSprite = GameObject.Find (playerPrefix + "Weap");
		//otherPlayerWeapSprite = GameObject.Find (otherPlayerPrefix + "Weap");

		foreach (Transform child in transform) {
			if (child.tag == "Hoop") {
				hoopController = child.gameObject.GetComponent<HoopController>();
			}
		}
		//Determine shooting buttons for OS and Player
		if ((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer)) {
			pingButton = "PingMac" + playerPrefix;
		} else if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer)) {
			pingButton = "PingPC" + playerPrefix;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		if (GameStats.playersCanMove) {
			if (Input.GetButton(pingButton) && Time.time > nextHoopShow) {
				hoopController.Show();
				nextHoopShow = Time.time + 3f;
			}

			// Check the swap request timeout, reset the request bool when it hits 0
			if (swapAugTimeout > 0) {
				swapAugTimeout--;
			} else {
				requestAugSwap = false;
			}
			// if (swapWeapTimeout > 0) {
			// 	swapWeapTimeout--;
			// } else {
			// 	requestWeapSwap = false;
			// }

			// If swap cooldown time has passed, request a aug/weap swap when the trigger is pressed
			// Otherwise play swap failed sound
			AugTrigger = Input.GetAxisRaw (swapButtonAug);
			//WeapTrigger = Input.GetAxisRaw (swapButtonWeap);
			if (AugTrigger > 0 && Time.time > nextAugSwap) {
				requestSwapAugments();
				nextAugSwapFailedSound = false;
			} else if (AugTrigger <= 0) {
				nextAugSwapFailedSound = true;
			} else if (AugTrigger > 0 && nextAugSwapFailedSound) {
				audioPlacement.PlayClip (swapCooldownSound, 0.05f);
				nextAugSwapFailedSound = false;
			}
			// if (WeapTrigger > 0 && Time.time > nextWeapSwap) {
			// 	requestSwapWeapons();
			// 	nextWeapSwapFailedSound = false;
			// } else if (WeapTrigger <= 0) {
			// 	nextWeapSwapFailedSound = true;
			// } else if (WeapTrigger > 0 && nextWeapSwapFailedSound) {
			// 	//audioPlacement.PlayClip (swapCooldownSound, 0.05f);
			// 	nextWeapSwapFailedSound = false;
			// }

			// If swap request currently sent, check for a response from other player
			if (requestAugSwap) {
				checkRequestSwapAugments();
			} else {
				otherPlayerHudImage.sprite = hudDefault;
			}
			// else if (requestWeapSwap) {
			// 	checkRequestSwapWeapons();
			// }

			// If no requests, reset other player hud
			// if (!requestAugSwap && !requestWeapSwap) {
			// 	otherPlayerHudImage.sprite = hudDefault;
			// }
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

	void requestSwapAugments() {
		requestAugSwap = true;
		swapAugTimeout = 60;
	}

	// void requestSwapWeapons() {
	// 	requestWeapSwap = true;
	// 	swapWeapTimeout = 60;
	// }

	void checkRequestSwapAugments() {
		if (otherPlayerStats.RequestAugSwap) {
			SwapAugments();
		} else {
			string myElement = (GetAugment() == null ? null : GetAugment().Element);
			string otherElement = (otherPlayerStats.GetAugment() == null ? null : otherPlayerStats.GetAugment().Element);
			// Activate Swap Request for Other Player
			// if (requestWeapSwap) {
			// 	otherPlayerHudImage.sprite = hud3;
			// } else {
			otherPlayerHudImage.sprite = hudReq;
			if (otherElement == "fire") {
				if (myElement == "ice") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Red-Blue");
				} else if (myElement == "earth") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Red-Green");
				}
			} else if (otherElement == "ice") {
				if (myElement == "fire") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Blue-Red");
				} else if (myElement == "earth") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Blue-Green");
				}
			} else if (otherElement == "earth") {
				if (myElement == "ice") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Green-Blue");
				} else if (myElement == "fire") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Green-Red");
				}
			}

			// }

			// Vibrate?
		}
	}

	// void checkRequestSwapWeapons() {
	// 	if (otherPlayerStats.RequestWeapSwap) {
	// 		SwapWeapons();
	// 	} else {
	// 		// Activate Swap Request for Other Player
	// 		if (requestAugSwap) {
	// 			otherPlayerHudImage.sprite = hud3;
	// 		} else {
	// 			otherPlayerHudImage.sprite = hud1;
	// 		}

	// 		// Vibrate?
	// 	}
	// }

	public void SwapAugments() {
		otherPlayerStats.RequestAugSwap = false;
		requestAugSwap = false;

		tempAug = GetAugment();
		tempSpr = playerAugSprite.GetComponent<Image> ().sprite;

		SetAugment(otherPlayerStats.GetAugment ());
		playerAugSprite.GetComponent<Image>().sprite = otherPlayerAugSprite.GetComponent<Image>().sprite;

		otherPlayerStats.SetAugment (tempAug);
		otherPlayerAugSprite.GetComponent<Image>().sprite = tempSpr;

		nextAugSwap = Time.time + 2;
		swapAugTimeout = 0;
	}

	// public void SwapWeapons() {
	// 	otherPlayerStats.RequestWeapSwap = false;
	// 	requestWeapSwap = false;

	// 	tempWeap = playerShooting.curWeap;
	// 	tempWeapSpr = playerWeapSprite.GetComponent<Image> ().sprite;

	// 	playerShooting.ChangeWeapon (otherPlayerShooting.curWeap);
	// 	playerWeapSprite.GetComponent<Image> ().sprite = otherPlayerWeapSprite.GetComponent<Image> ().sprite;
	
	// 	otherPlayerShooting.ChangeWeapon (tempWeap);
	// 	otherPlayerWeapSprite.GetComponent<Image> ().sprite = tempWeapSpr;

	// 	nextWeapSwap = Time.time + 2;
	// 	swapWeapTimeout = 0;
	// }


}
