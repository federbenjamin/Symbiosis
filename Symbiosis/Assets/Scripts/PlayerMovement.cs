using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	Rigidbody playerRB;

	public float baseSpeed = 8f;
	public float speedModifier = 0f;

	private string playerPrefix;
	private float horizMov, vertMov;
	private Vector3 playerMov;


	// Use this for initialization
	void Start () {
		playerRB = GetComponent<Rigidbody> ();
		playerPrefix = gameObject.name;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {

		//Player Movement
		horizMov = Input.GetAxisRaw ("Horizontal" + playerPrefix);
		vertMov = Input.GetAxisRaw ("Vertical" + playerPrefix);
		playerMov = new Vector3 (horizMov, 0f, vertMov);
		playerRB.AddForce (playerMov * ((baseSpeed + speedModifier) * 10) * Time.deltaTime, ForceMode.Impulse);

	}
}
