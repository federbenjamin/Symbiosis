using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	Rigidbody playerRB;
	Transform playerTransform;
	public Animator animatorBody;
	public Animator animatorSlime;
	public GameObject playerBody;

	public GameObject room;
	public float baseSpeed = 8f;

	private string playerPrefix;
	private float horizMov, vertMov;
	private Vector3 playerMov;
	public bool isMoving;

	private StatsManager playerStats;
	private PlayerShooting playerShooting;

	private float speedModifier;


	void Awake () {

		//Get the StatsManager Script
		playerStats = GetComponent<StatsManager> ();
		playerShooting = GetComponent<PlayerShooting> ();
	}

	// Use this for initialization
	void Start () {
		playerRB = GetComponent<Rigidbody> ();
		playerTransform = GetComponent<Transform> ();
		playerPrefix = gameObject.name;

		foreach (Transform child in transform) {
			if (child.name == "Player_animated") {
				foreach (Transform bodypart in child) {
					if (bodypart.name == "Player_scientistonly") {
						animatorBody = bodypart.GetComponent<Animator> ();
					} else {
						animatorSlime = bodypart.GetComponent<Animator> ();
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {

		//Player Movement
		horizMov = Input.GetAxisRaw ("Horizontal" + playerPrefix);
		vertMov = Input.GetAxisRaw ("Vertical" + playerPrefix);
		playerMov = new Vector3 (horizMov, 0f, vertMov);

		isMoving = (horizMov != 0 || vertMov != 0);
		animatorBody.SetBool ("moving", isMoving);
		animatorSlime.SetBool ("moving", isMoving);

		bool playerBusy = (playerShooting.playerShooting || playerShooting.playerSwinging);

		if (!playerBusy) {
			if (horizMov != 0) {
				if (horizMov > 0) {
					if (vertMov > 0) {
						playerTransform.rotation = Quaternion.Euler (0, -135, 0);
					} else if (vertMov < 0) {
						playerTransform.rotation = Quaternion.Euler (0, -45, 0);
					} else {
						playerTransform.rotation = Quaternion.Euler (0, -90, 0);
					}
				} else if (horizMov < 0) {
					if (vertMov > 0) {
						playerTransform.rotation = Quaternion.Euler (0, 135, 0);
					} else if (vertMov < 0) {
						playerTransform.rotation = Quaternion.Euler (0, 45, 0);
					} else {
						playerTransform.rotation = Quaternion.Euler (0, 90, 0);
					}
				}
			} else if (vertMov != 0) {
				if (vertMov > 0) {
					playerTransform.rotation = Quaternion.Euler (0, -180, 0);
				}
			} else if (vertMov < 0) {
				playerTransform.rotation = Quaternion.Euler (0, 0, 0);
			}
		}

		//Get the speed stat for the player
		speedModifier = playerStats.GetSpeed ();

		//Apply Movement
		playerRB.AddForce (playerMov * ((baseSpeed + speedModifier) * 10) * Time.deltaTime, ForceMode.VelocityChange);

	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Room") {
			room = col.gameObject;
		}
	}
}
