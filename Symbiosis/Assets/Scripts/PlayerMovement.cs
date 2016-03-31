using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	Rigidbody playerRB;
	Transform playerTransform;
	public Animator animatorBody;
	public Animator animatorSlime;
	public GameObject playerBody;
	public GameObject cameraPointer;
	private CameraController camera;

	private GameObject room;
	public float baseSpeed = 8f;

	private string playerPrefix;
	private float horizMov, vertMov, horizLook, vertLook;
	private string moveButtonHoriz, moveButtonVert;
	private Vector3 playerMov, playerLook;
	public bool isMoving;

	private PlayerShooting playerShooting;

	private float speedModifier;


	void Awake () {
		//Get the StatsManager Script
		playerShooting = GetComponent<PlayerShooting> ();
	}

	// Use this for initialization
	void Start () {
		playerRB = GetComponent<Rigidbody> ();
		playerTransform = GetComponent<Transform> ();
		playerPrefix = gameObject.name;

		if ((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer)) {
			moveButtonHoriz = "HorizontalMac" + playerPrefix;
			moveButtonVert = "VerticalMac" + playerPrefix;
		} else if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer)) {
			moveButtonHoriz = "HorizontalPC" + playerPrefix;
			moveButtonVert = "VerticalPC" + playerPrefix;
		}

		if (playerPrefix == "P1") {
			camera = GameObject.Find ("CameraParentP1").GetComponent<CameraController>();
			animatorSlime = GameObject.Find("Player_blue_slime").GetComponent<Animator> ();
		} else {
			camera = GameObject.Find ("CameraParentP2").GetComponent<CameraController>();
			animatorSlime = GameObject.Find("Player_yellow_slime").GetComponent<Animator> ();
		}

		foreach (Transform child in transform) {
			if (child.name == "Player_animated") {
				animatorBody = child.GetChild(0).GetComponent<Animator> ();
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {
		if (!HealthManager.isGameOver && GameStats.gameStarted){
			//Player Movement
			horizMov = Input.GetAxisRaw (moveButtonHoriz);
			vertMov = Input.GetAxisRaw (moveButtonVert);
			playerMov = new Vector3 (horizMov, 0f, vertMov);
			if (playerMov.magnitude > 1) {
				playerMov = playerMov / playerMov.magnitude;
			}

			vertLook = Mathf.Round(Input.GetAxisRaw(moveButtonVert));
			horizLook  = Mathf.Round(Input.GetAxisRaw(moveButtonHoriz));
			playerLook = new Vector3 (horizLook, 0f, vertLook);

			//Debug.Log(horizLook);
			if (horizLook == 1) {
				camera.MovingRight = true;
			} else if (horizLook == -1) {
				camera.MovingRight = false;
			}

			isMoving = (horizMov != 0 || vertMov != 0);
			animatorBody.SetBool ("moving", isMoving);
			animatorSlime.SetBool ("moving", isMoving);

			bool playerBusy = (playerShooting.playerShooting || playerShooting.playerSwinging);


			if (!playerBusy) {
				if (playerLook.magnitude != 0) {
					transform.rotation = Quaternion.LookRotation (playerLook * -1);
				}
			}

			//Apply Movement
			playerRB.AddForce (playerMov * (baseSpeed * 10) * Time.deltaTime, ForceMode.VelocityChange);
		}
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Room" || col.gameObject.tag == "TutorialRoom") {
			room = col.gameObject;
		}
	}
}
