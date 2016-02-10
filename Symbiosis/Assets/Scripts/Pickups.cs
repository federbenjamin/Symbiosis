using UnityEngine;
using UnityEngine.UI;
using System.Collections;




public class Pickups : MonoBehaviour {

	public float rotateSpeed;
	private StatsManager playerStats;
	private GameObject playerAugSprite;
	private string playerPrefix;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {

		//Spin the Pickup
		transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
	}

	void OnTriggerEnter(Collider other) {

		//Check if it is the player
		if (other.tag == "Player") {
			playerStats = other.GetComponent<StatsManager> ();
			playerPrefix = other.name;
			playerAugSprite = GameObject.Find (playerPrefix + "Aug");

			//Check which pickup it is and apply effects
			switch (gameObject.name) 
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
			}

			//Get rid of the pickup since it has been used
			Destroy (gameObject);
		}
	}
}
