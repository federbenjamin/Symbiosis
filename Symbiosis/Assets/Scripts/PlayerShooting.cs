using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {

	private string playerPrefix;
	public GameObject bullet;
	public float bullet_speed;
	public float fire_rate = 0.5f;
	private float next_fire = 0.0f;

	// Use this for initialization
	void Start () {
		playerPrefix = gameObject.name;
	
	}
	
	// Update is called once per frame
	void Update () {
	
		//Player Shooting
		if (Input.GetButton("FireRight" + playerPrefix) && Time.time > next_fire) 
		{
			Shoot(Vector3.right);
		}
		else if (Input.GetButton("FireDown" + playerPrefix) && Time.time > next_fire)
		{
			Shoot(Vector3.back);
		}
		else if (Input.GetButton("FireUp" + playerPrefix) && Time.time > next_fire) 
		{
			Shoot(Vector3.forward);
		}
		else if (Input.GetButton("FireLeft" + playerPrefix) && Time.time > next_fire) 
		{
			Shoot(Vector3.left);
		}
	}

	void Shoot (Vector3 shoot_dir) {

		GameObject clone = Instantiate (bullet, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.rotation) as GameObject;
		clone.transform.rotation = Quaternion.LookRotation (shoot_dir);
		Physics.IgnoreCollision (clone.GetComponent<Collider> (), GetComponent<Collider> ());
		clone.GetComponent<Rigidbody> ().AddForce (clone.transform.forward * (bullet_speed * 100));

		next_fire = Time.time + fire_rate;
	}
}
