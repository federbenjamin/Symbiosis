using UnityEngine;
using System.Collections;




public class PlayerShooting : MonoBehaviour {

	public GameObject bullet;

	public Object fireEffect;
	public Object iceEffect;
	public Object earthEffect;

	public float baseBulletSpeed = 10f;
	public float baseFireRate = 0.5f;

	public iAugment aug = null;

	private float nextFire = 0.0f;
	private string playerPrefix;

	private StatsManager playerStats;
	private float bulletSpeedModifier;
	private float fireRateModifier;


	void Awake () {

		//Get the StatsManager Script
		playerStats = GetComponent<StatsManager> ();
	}

	// Use this for initialization
	void Start () {
		playerPrefix = gameObject.name;
	
	}
	
	// Update is called once per frame
	void Update () {

		//Get the stats for the player
		bulletSpeedModifier = playerStats.GetBulletSpeed ();
		fireRateModifier = playerStats.GetFireRate ();
	
		//Player Shooting
		if (Input.GetButton("FireRight" + playerPrefix) && Time.time > nextFire) 
		{
			Shoot(Vector3.right);
		}
		else if (Input.GetButton("FireDown" + playerPrefix) && Time.time > nextFire)
		{
			Shoot(Vector3.back);
		}
		else if (Input.GetButton("FireUp" + playerPrefix) && Time.time > nextFire) 
		{
			Shoot(Vector3.forward);
		}
		else if (Input.GetButton("FireLeft" + playerPrefix) && Time.time > nextFire) 
		{
			Shoot(Vector3.left);
		}
	}

	void Shoot (Vector3 shootDir) {

		aug = GetComponent<StatsManager> ().GetAugment ();

		//Create the bullet and launch it
		GameObject clone = Instantiate (bullet, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.rotation) as GameObject;
		clone.transform.rotation = Quaternion.LookRotation (shootDir);
		
		Physics.IgnoreCollision (clone.GetComponent<Collider> (), GetComponent<Collider> ());
		clone.GetComponent<Rigidbody> ().AddForce (clone.transform.forward * ((baseBulletSpeed + bulletSpeedModifier) * 100));

		Debug.Log ("Firing augged bullet");

		if (aug != null) {
			Debug.Log ("Applying on-hit effects");
			clone.GetComponent<BulletBehavior> ().setAugment(aug);
			GameObject augEffect;
			if (aug.Element == "fire") {
				Debug.Log ("Applying fire effects");
				augEffect = Instantiate (fireEffect, clone.transform.position, Quaternion.identity) as GameObject;
				augEffect.transform.parent = clone.transform;
			}
		}

		//Set when the next bullet can be fired
		nextFire = Time.time + (baseFireRate + fireRateModifier);
	}
}
