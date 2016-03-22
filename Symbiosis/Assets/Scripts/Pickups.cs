using UnityEngine;
using UnityEngine.UI;
using System.Collections;




public class Pickups : MonoBehaviour {

	public float rotateSpeed;
	private StatsManager playerStats;
	private PlayerShooting playerShooting;
	private GameObject playerAugSprite;
	private GameObject playerWeapSprite;
	private string playerPrefix;
	public string powerupType;

	public GameObject rayGunPickup;
	public GameObject swordPickup;

	public GameObject redAugmentPickup;
	public GameObject blueAugmentPickup;
	public GameObject greenAugmentPickup;

	private string oldWeapon;
	private string oldAugment;
	public bool notUsed;
	private float nextPickup = 0.0f;

	// Use this for initialization
	void Start () {
		notUsed = false;
		oldWeapon = null;
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
			playerPrefix = other.name;
			playerAugSprite = GameObject.Find (playerPrefix + "Aug");
			playerWeapSprite = GameObject.Find (playerPrefix + "Weap");

			if (Time.time > nextPickup) {

				//Check which pickup it is and apply effects
				switch (powerupType) 
				{
				case "SpeedUp":
					playerStats.SetSpeed (10f);
					break;
				case "FireRateUp":
					playerStats.SetFireRate (0.4f);
					break;
				case "BulletSpeedUp":
					playerStats.SetBulletSpeed (15f);
					break;
				case "BeepAugment":
					Augment temp = new Augment ("Sounds/beep/beep_1");
					Debug.Log (temp);
					playerStats.SetAugment (temp);
					playerAugSprite.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("SpeedUpSprite");

					Debug.Log (playerStats.GetAugment());

					break;
				case "GrowAugment":
					GrowAugment g = new GrowAugment();
					Debug.Log (g);
					playerStats.SetAugment (g);
					playerAugSprite.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("BulletSpeedUpSprite");

					Debug.Log (playerStats.GetAugment());

					break;
				case "FireAugment":
					if (playerStats.GetAugment() != null) {
						oldAugment = playerStats.GetAugment().Element;
					}
					FireAugment f = new FireAugment();
					Debug.Log (f);
					playerStats.SetAugment (f);
					playerAugSprite.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("redaugmentsprite");

					Debug.Log (playerStats.GetAugment());

					break;
				case "IceAugment":
					if (playerStats.GetAugment() != null) {
						oldAugment = playerStats.GetAugment().Element;
					}
					IceAugment i = new IceAugment();
					Debug.Log (i);
					playerStats.SetAugment (i);
					playerAugSprite.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("blueaugmentsprite");

					Debug.Log (playerStats.GetAugment());

					break;
				case "EarthAugment":
					if (playerStats.GetAugment() != null) {
						oldAugment = playerStats.GetAugment().Element;
					}
					EarthAugment e = new EarthAugment ();
					Debug.Log (e);
					playerStats.SetAugment (e);
					playerAugSprite.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("greenaugmentsprite");

					Debug.Log (playerStats.GetAugment ());

					break;
				case "RayGun":
					oldWeapon = playerShooting.curWeap;
					playerShooting.ChangeWeapon(powerupType);
					playerWeapSprite.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("raygunsprite");
					break;

				case "Sword":
					oldWeapon = playerShooting.curWeap;
					Debug.Log("Picked Sword");
					playerShooting.ChangeWeapon (powerupType);
					playerWeapSprite.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("swordsprite");
					break;

				case "FullHealth":
					if (playerStats.playersHealth.currentHealth < 10) {
						playerStats.playersHealth.HealHealth (2);
					} else {
						notUsed = true;
					}
					break;
				
				case "HalfHealth":
					if (playerStats.playersHealth.currentHealth < 10) {
						playerStats.playersHealth.HealHealth (1);
					} else {
						notUsed = true;
					}
					break;
				}

				if (oldWeapon != null) {
					if (oldWeapon == "RayGun") {
						Instantiate (rayGunPickup, transform.position, rayGunPickup.transform.rotation);
					} else if (oldWeapon == "Sword") {
						Instantiate (swordPickup, transform.position, swordPickup.transform.rotation);
					}

					oldWeapon = null;
				}

				if (oldAugment != null) {
					if (oldAugment == "fire") {
						Instantiate (redAugmentPickup, transform.position, redAugmentPickup.transform.rotation);
					} else if (oldAugment == "ice") {
						Instantiate (blueAugmentPickup, transform.position, blueAugmentPickup.transform.rotation);
					} else if (oldAugment == "earth") {
						Instantiate (greenAugmentPickup, transform.position, greenAugmentPickup.transform.rotation);
					}

					oldAugment = null;
				}

				//Get rid of the pickup if it has been used
				if (!notUsed){
					Destroy (gameObject);
				}
				
				//Reset the variable for next switch statement
				notUsed = false;
				nextPickup = Time.time + 2f;
			}
		}
	}
}
