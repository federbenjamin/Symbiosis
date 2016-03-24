using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour {

	public GameObject player;

	private Vector3 newCameraPos;
	private bool playersTogether = false;
	private float playersMiddle;

	private GameObject player2;

	private Camera camera;

	public GameObject cameraPointer;
	private float deltaDist;
	private bool facingRight;
	float oppositeOffset = 4.5f;
	float sameOffset = 0.3f;
	float camMoveSpeed = 3f;

	// Use this for initialization
	void Start () {
		camera = GameObject.Find ("CameraP2").GetComponent<Camera> ();
		player2 = GameObject.Find ("P2");
		newCameraPos = new Vector3 (player.transform.position.x, transform.position.y, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate () {

		if (playersTogether == true) {
			playersMiddle = (player.transform.position.x + player2.transform.position.x) * 0.5f;
			if ((playersMiddle - transform.position.x >= 1) || (playersMiddle - transform.position.x <= -1)) {
				newCameraPos = new Vector3 (playersMiddle, transform.position.y, transform.position.z);
				transform.position = Vector3.Lerp (transform.position, newCameraPos, 1f * Time.deltaTime);
			}
		} else {
			deltaDist = cameraPointer.transform.position.x - newCameraPos.x;
			if (facingRight == false) {
				//Far Right
				if (deltaDist > oppositeOffset) {
					facingRight = true;
					newCameraPos = new Vector3 (cameraPointer.transform.position.x, transform.position.y, transform.position.z);
				} else if (deltaDist <= -sameOffset) { 
					newCameraPos = new Vector3 (cameraPointer.transform.position.x, transform.position.y, transform.position.z);
				}
				transform.position = Vector3.Lerp (transform.position, newCameraPos, camMoveSpeed * Time.deltaTime);
			} else if (facingRight == true) {
				//Far Left
				if (deltaDist < -oppositeOffset) {
					facingRight = false;
					newCameraPos = new Vector3 (cameraPointer.transform.position.x, transform.position.y, transform.position.z);
				} else if (deltaDist >= sameOffset) {
					newCameraPos = new Vector3 (cameraPointer.transform.position.x, transform.position.y, transform.position.z);
				}
				transform.position = Vector3.Lerp (transform.position, newCameraPos, camMoveSpeed * Time.deltaTime);
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
}
