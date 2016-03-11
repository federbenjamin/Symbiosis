using UnityEngine;
using System.Collections;




public class PlayerShooting : MonoBehaviour {

	public GameObject Reg_bullet;
	public GameObject RayGun_bullet;
	public GameObject Red_bullet;
	public GameObject Blue_bullet;
	public GameObject Green_bullet;

	public GameObject rayGunTip;
	public LineRenderer line;

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
	public bool playerShooting;

	private GameObject cur_bullet;
	public GameObject hand;
	public GameObject RayGun;
	public GameObject Pistol;
	public string curWeap;

	void Awake () {

		//Get the StatsManager Script
		playerStats = GetComponent<StatsManager> ();
	}

	// Use this for initialization
	void Start () {
		playerPrefix = gameObject.name;
		cur_bullet = Reg_bullet;
		ChangeWeapon ("Pistol");
		curWeap = "Pistol";
	}
	
	// Update is called once per frame
	void Update () {

		//Get the stats for the player
		bulletSpeedModifier = playerStats.GetBulletSpeed ();
		fireRateModifier = playerStats.GetFireRate ();
		playerShooting = (Input.GetButton ("FireRight" + playerPrefix) ||
			Input.GetButton ("FireDown" + playerPrefix) ||
			Input.GetButton ("FireUp" + playerPrefix) ||
			Input.GetButton ("FireLeft" + playerPrefix));
		
		//Player Shooting
		if (curWeap == "RayGun") {
			if (Input.GetButton ("FireRight" + playerPrefix)) {
				transform.rotation = Quaternion.LookRotation (Vector3.left);
				StopCoroutine ("FireLaser");
				StartCoroutine ("FireLaser");
			} else if (Input.GetButton ("FireDown" + playerPrefix)) {
				transform.rotation = Quaternion.LookRotation (Vector3.forward);
				StopCoroutine ("FireLaser");
				StartCoroutine ("FireLaser");
			} else if (Input.GetButton ("FireUp" + playerPrefix)) {
				transform.rotation = Quaternion.LookRotation (Vector3.back);
				StopCoroutine ("FireLaser");
				StartCoroutine ("FireLaser");
			} else if (Input.GetButton ("FireLeft" + playerPrefix)) {
				transform.rotation = Quaternion.LookRotation (Vector3.right);
				StopCoroutine ("FireLaser");
				StartCoroutine ("FireLaser");
			}
		} else {
			if (Input.GetButton ("FireRight" + playerPrefix) && Time.time > nextFire) {
				transform.rotation = Quaternion.LookRotation (Vector3.left);
				Shoot (Vector3.right);
			} else if (Input.GetButton ("FireDown" + playerPrefix) && Time.time > nextFire) {
				transform.rotation = Quaternion.LookRotation (Vector3.forward);
				Shoot (Vector3.back);
			} else if (Input.GetButton ("FireUp" + playerPrefix) && Time.time > nextFire) {
				transform.rotation = Quaternion.LookRotation (Vector3.back);
				Shoot (Vector3.forward);
			} else if (Input.GetButton ("FireLeft" + playerPrefix) && Time.time > nextFire) {
				transform.rotation = Quaternion.LookRotation (Vector3.right);
				Shoot (Vector3.left);
			}
		}
	}

	void Shoot (Vector3 shootDir) {

		aug = GetComponent<StatsManager> ().GetAugment ();

		if (aug != null) {
			if (aug.Element == "fire") {
				cur_bullet = Red_bullet;
			} else if (aug.Element == "ice") {
				cur_bullet = Blue_bullet;
			} else if (aug.Element == "earth") {
				cur_bullet = Green_bullet;
			}
		}

		//Create the bullet and launch it
		GameObject clone = Instantiate (cur_bullet, hand.transform.position, hand.transform.rotation) as GameObject;
		clone.transform.rotation = Quaternion.LookRotation (shootDir);
		Physics.IgnoreCollision (clone.GetComponent<Collider> (), GetComponent<Collider> ());
		clone.GetComponent<Rigidbody> ().velocity = (clone.transform.forward * ((baseBulletSpeed + bulletSpeedModifier)));

//		Debug.Log ("Firing augged bullet");
//
//
//		if (aug != null) {
//			Debug.Log ("Applying on-hit effects");
//			clone.GetComponent<BulletBehavior> ().setAugment(aug);
//			GameObject augEffect;
//			if (aug.Element == "fire") {
//				Debug.Log ("Applying fire effects");
//				augEffect = Instantiate (fireEffect, clone.transform.position, Quaternion.identity) as GameObject;
//				augEffect.transform.parent = clone.transform;
//			}
//		}

		//Set when the next bullet can be fired
		nextFire = Time.time + (baseFireRate + fireRateModifier);
	}

	public void ChangeWeapon(string weapon) {
		curWeap = weapon;
		foreach (Transform child in hand.transform) {
			Destroy (child.gameObject);
		}
			
		switch (weapon) {

		case "Pistol":
			baseBulletSpeed = 10f;
			baseFireRate = 0.5f;
			cur_bullet = Reg_bullet;
			GameObject pistol = Instantiate (Pistol, hand.transform.position, hand.transform.rotation) as GameObject;
			pistol.transform.parent = hand.transform;
			break;

		case "RayGun":
			baseBulletSpeed = 20f;
			baseFireRate = 0.1f;
			GameObject rayGun = Instantiate (RayGun, hand.transform.position, hand.transform.rotation) as GameObject;
			rayGun.transform.parent = hand.transform;
			rayGunTip = rayGun.transform.GetChild (0).gameObject;
			line = rayGunTip.GetComponent<LineRenderer>();
			line.enabled = false;
			break;
		}
	}

	IEnumerator FireLaser() {
		line.enabled = true;

		while ((Input.GetButton ("FireRight" + playerPrefix)) ||
		      (Input.GetButton ("FireDown" + playerPrefix)) ||
		      (Input.GetButton ("FireUp" + playerPrefix)) ||
		      (Input.GetButton ("FireLeft" + playerPrefix))) 
		{
			Ray ray = new Ray (rayGunTip.transform.position, transform.forward * -1);
			RaycastHit hit;

			line.SetPosition (0, ray.origin);

			if (Physics.Raycast(ray, out hit, 50)) {
				line.SetPosition(1, hit.point);
				if (hit.transform.tag == "Enemy") {
					string damageType = "none";
					float force = 50;
					if (aug != null) {
						aug.onHitEffect (hit.transform.gameObject);
						damageType = aug.Element;
						if (aug.Element == "earth") {
							force = 100;
						}
					}

					//Add force to enemy
					hit.rigidbody.AddForceAtPosition(transform.forward * force * -1, hit.point);
					EnemyStats enemyHP = hit.transform.GetComponent<EnemyStats> ();
					enemyHP.TakeDamage (1, damageType);
				}
			} else {
				line.SetPosition (1, ray.GetPoint (50));
			}

			yield return null;
		}
		line.enabled = false;
	}
}
