using UnityEngine;
using System.Collections;

public class Augment : iAugment{

	public Augment(string a){
		ac = (AudioClip) Resources.Load(a);
	}

	public AudioClip ac;

	private string element = "TEST";
	public string Element{ 
		get{return element;}
		set{ element = value;}
	}

	private int onHitChance;
	public int OnHitChance{ 
		get{return onHitChance;}
		set{ onHitChance = value;}
	}

	public void onHitEffect(GameObject other){
		other.GetComponent<AudioSource> ().PlayOneShot (ac);
	}
}


public class Pickups : MonoBehaviour {

	public float rotateSpeed;

	private StatsManager playerStats;

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

				Debug.Log (playerStats.GetAugment());
				
				break;
			}

			//Get rid of the pickup since it has been used
			Destroy (gameObject);
		}
	}
}
