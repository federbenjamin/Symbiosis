﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour {

	public GameObject player;

	private Vector3 newCameraPos;
	private Vector3 newRoomCameraPos;
	public Vector3 NewRoomCameraPos {
		set{newRoomCameraPos = value;}
	}
	private bool playersTogether = false;
	private float playersMiddle;

	private GameObject player2;

	private Camera camera;
	public GameObject playerSlime;
	public static bool followSlime = true;
	

	public GameObject cameraPointer;
	private float deltaDist;
	private bool movingRight;
	public bool MovingRight {
		set{movingRight = value;}
	}
	float oppositeOffset = 4.5f;
	float sameOffset = 0.3f;
	float camMoveSpeed = 3f;
	private float maxRight;
	private float maxLeft;
	public float xPosLimit;

	// Use this for initialization
	void Start () {
		camera = GameObject.Find ("CameraP2").GetComponent<Camera> ();
		player2 = GameObject.Find ("P2");
		newCameraPos = new Vector3 (player.transform.position.x, transform.position.y, transform.position.z);

		if (gameObject.name == "CameraParentP1") {
			float p1XPos = GameObject.Find ("P1").transform.position.x;
			setMaxX(p1XPos);
		} else {
			float p2XPos = player2.transform.position.x;
			setMaxX(p2XPos);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate () {
		if (followSlime == false) {

			if (playersTogether == true) {
				playersMiddle = (player.transform.position.x + player2.transform.position.x) * 0.5f;
				if ((playersMiddle - transform.position.x >= 1) || (playersMiddle - transform.position.x <= -1)) {
					newCameraPos = new Vector3 (playersMiddle, transform.position.y, transform.position.z);
					transform.position = Vector3.Lerp (transform.position, newCameraPos, 1f * Time.deltaTime);
				}
			} else {
				deltaDist = cameraPointer.transform.position.x - newCameraPos.x;
				if (movingRight == false) {
					//Far Right
					if (deltaDist > oppositeOffset) {
						movingRight = true;
					} else if (deltaDist <= -sameOffset) {
						newCameraPos = new Vector3 (Mathf.Max(cameraPointer.transform.position.x, maxLeft), transform.position.y, transform.position.z);
					}
				} else if (movingRight == true) {
					//Far Left
					if (deltaDist < -oppositeOffset) {
						movingRight = false;
					} else if (deltaDist >= sameOffset) {
						newCameraPos = new Vector3 (Mathf.Min(cameraPointer.transform.position.x, maxRight), transform.position.y, transform.position.z);
					}
				}

				if (newRoomCameraPos == Vector3.zero) {
					transform.position = Vector3.Lerp (transform.position, newCameraPos, camMoveSpeed * Time.deltaTime);
				} else {
					transform.position = newRoomCameraPos;
					newCameraPos = newRoomCameraPos;
					newRoomCameraPos = Vector3.zero;
				}
			}
		}
	}

	public void MergeCamera () {
		//Turn off P2 Camera and Divider, and extend P1 Camera to cover the screen
		playersTogether = true;
		GameObject.Find ("CameraParentP2").SetActive(false);
		GameObject.Find ("Divider").SetActive(false);
		GameObject.Find ("CameraP1").GetComponent<Camera> ().rect = new Rect (0, 0, 1, 1);
	}

	public void setMaxX(float roomCenterX) {
		maxRight = roomCenterX + xPosLimit;
		maxLeft = roomCenterX - xPosLimit;
	}
}
