using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Pickups : MonoBehaviour {

	public float rotateSpeed;
	private StatsManager playerStats;
	private PlayerShooting playerShooting;
	private PlayerMovement playerMovement;
	private GameObject playerAugSprite;
	private GameObject playerWeapSprite;
	private string playerPrefix;
	public string powerupType;

	public GameObject pistolPickup;
	public GameObject rayGunPickup;
	public GameObject swordPickup;

	public GameObject redAugmentPickup;
	public GameObject blueAugmentPickup;
	public GameObject greenAugmentPickup;

	private string oldWeapon;
	private GameObject oldWeaponPickup;
	private string oldAugment;
	private GameObject oldAugmentPickup;
	public bool notUsed;
	private float nextPickup = 0.0f;
	static float nextPickupP1 = 0.0f;
	static float nextPickupP2 = 0.0f;

	// Use this for initialization
	void Start () {
		notUsed = false;
		oldWeapon = null;
		oldAugment = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {

		//Spin the Pickup
		if (powerupType == "SpeedUp" || powerupType == "FireRateUp") {
			transform.Rotate (0,0,rotateSpeed);
		} else {
			transform.Rotate (Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
		}
	}

	void OnTriggerEnter(Collider other) {

		//Check if it is the player
		if (other.tag == "Player") {
			playerStats = other.GetComponent<StatsManager> ();
			playerShooting = other.GetComponent<PlayerShooting> ();
			playerMovement = other.GetComponent<PlayerMovement> ();
			playerPrefix = other.name;
			playerAugSprite = GameObject.Find (playerPrefix + "Aug");
			playerWeapSprite = GameObject.Find (playerPrefix + "Weap");
			if (other.name == "P1") {
				nextPickup = nextPickupP1;
			} else {
				nextPickup = nextPickupP2;
			}

			if (Time.time > nextPickup) {

				//Check which pickup it is and apply effects
				switch (powerupType) 
				{
				// case "BeepAugment":
				// 	Augment temp = new Augment ("Sounds/beep/beep_1");
				// 	Debug.Log (temp);
				// 	playerStats.SetAugment (temp);
				// 	playerAugSprite.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("SpeedUpSprite");

				// 	Debug.Log (playerStats.GetAugment());

				// 	break;
				// case "GrowAugment":
				// 	GrowAugment g = new GrowAugment();
				// 	Debug.Log (g);
				// 	playerStats.SetAugment (g);
				// 	playerAugSprite.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("BulletSpeedUpSprite");

				// 	Debug.Log (playerStats.GetAugment());

				// 	break;
				case "FireAugment":
					if (playerStats.GetAugment() != null) {
						oldAugment = playerStats.GetAugment().Element;
					}
					FireAugment f = new FireAugment();
					Debug.Log (f);
					playerStats.SetAugment (f);
					playerAugSprite.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Interface/Augment-Red-Blank");

					Debug.Log (playerStats.GetAugment());

					break;
				case "IceAugment":
					if (playerStats.GetAugment() != null) {
						oldAugment = playerStats.GetAugment().Element;
					}
					IceAugment i = new IceAugment();
					Debug.Log (i);
					playerStats.SetAugment (i);
					playerAugSprite.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Interface/Augment-Blue-Blank");

					Debug.Log (playerStats.GetAugment());

					break;
				case "EarthAugment":
					if (playerStats.GetAugment() != null) {
						oldAugment = playerStats.GetAugment().Element;
					}
					EarthAugment e = new EarthAugment ();
					Debug.Log (e);
					playerStats.SetAugment (e);
					playerAugSprite.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Interface/Augment-Green-Blank");

					Debug.Log (playerStats.GetAugment ());

					break;

				case "Pistol":
					oldWeapon = playerShooting.curWeap;
					playerShooting.ChangeWeapon(powerupType);
					break;

				case "RayGun":
					oldWeapon = playerShooting.curWeap;
					playerShooting.ChangeWeapon(powerupType);
					//playerWeapSprite.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("raygunsprite");
					break;

				case "Sword":
					oldWeapon = playerShooting.curWeap;
					playerShooting.ChangeWeapon (powerupType);
					//playerWeapSprite.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("swordsprite");
					break;

				case "FullHealth":
					if (HealthManager.currentHealth < 10) {
						HealthManager.HealHealth (2);
					} else {
						notUsed = true;
					}
					break;
				
				case "HalfHealth":
					if (HealthManager.currentHealth < 10) {
						HealthManager.HealHealth (1);
					} else {
						notUsed = true;
					}
					break;
				}

				if (oldWeapon != null) {
					if (oldWeapon == "Pistol") {
						oldWeaponPickup = Instantiate (pistolPickup, transform.position, pistolPickup.transform.rotation) as GameObject;
					} else if (oldWeapon == "RayGun") {
						oldWeaponPickup = Instantiate (rayGunPickup, transform.position, rayGunPickup.transform.rotation) as GameObject;
					} else if (oldWeapon == "Sword") {
						oldWeaponPickup = Instantiate (swordPickup, transform.position, swordPickup.transform.rotation) as GameObject;
					}

					oldWeaponPickup.transform.parent = playerMovement.room.transform;
					oldWeapon = null;
				}

				if (oldAugment != null) {
					if (oldAugment == "fire") {
						oldAugmentPickup = Instantiate (redAugmentPickup, transform.position, redAugmentPickup.transform.rotation) as GameObject;
					} else if (oldAugment == "ice") {
						oldAugmentPickup = Instantiate (blueAugmentPickup, transform.position, blueAugmentPickup.transform.rotation) as GameObject;
					} else if (oldAugment == "earth") {
						oldAugmentPickup = Instantiate (greenAugmentPickup, transform.position, greenAugmentPickup.transform.rotation) as GameObject;
					}

					oldAugmentPickup.transform.parent = playerMovement.room.transform;
					oldAugment = null;
				}

				//Get rid of the pickup if it has been used
				if (!notUsed){
					Destroy (gameObject);
				}
				
				//Reset the variable for next switch statement
				notUsed = false;

				//Check if P1 or P2 picked it up, update nextPickup time respectively
				if (other.name == "P1") {
					nextPickupP1 = Time.time + 1f;
				} else {
					nextPickupP2 = Time.time + 1f;
				}
			}
		}
	}
}
