using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {

	public GameObject bullet;
	public float bulletSpeed = 10f;
	public float fireRate = 0.5f;

	private float nextFire = 0.0f;
	private string playerPrefix;

	// Use this for initialization
	void Start () {
		playerPrefix = gameObject.name;
	
	}
	
	// Update is called once per frame
	void Update () {
	
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

		GameObject clone = Instantiate (bullet, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.rotation) as GameObject;
		clone.transform.rotation = Quaternion.LookRotation (shootDir);
		Physics.IgnoreCollision (clone.GetComponent<Collider> (), GetComponent<Collider> ());
		clone.GetComponent<Rigidbody> ().AddForce (clone.transform.forward * (bulletSpeed * 100));

		nextFire = Time.time + fireRate;
	}
}
