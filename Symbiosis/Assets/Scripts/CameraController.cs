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

	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera> ();
		player2 = GameObject.Find ("P2");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {

		if (playersTogether == true) {
			playersMiddle = (player.transform.position.x + player2.transform.position.x) * 0.5f;
			if ((playersMiddle - transform.position.x >= 1) || (playersMiddle - transform.position.x <= -1)) {
				newCameraPos = new Vector3 (playersMiddle, transform.position.y, transform.position.z);
				transform.position = Vector3.Lerp (transform.position, newCameraPos, 1f * Time.deltaTime);
			}
		} else {
			if ((player.transform.position.x - transform.position.x >= 1) || (player.transform.position.x - transform.position.x <= -1)) {
				float camMoveSpeed = 1f;

				if ((player.transform.position.x - transform.position.x >= 2) || (player.transform.position.x - transform.position.x <= -2)) {
					camMoveSpeed = 2f;
				}

				newCameraPos = new Vector3 (player.transform.position.x, transform.position.y, transform.position.z);
				transform.position = Vector3.Lerp (transform.position, newCameraPos, camMoveSpeed * Time.deltaTime);
			} 
		}
	}

	public void MergeCamera () {
		//Turn off P2 Camera and Divider, and extend P1 Camera to cover the screen
		playersTogether = true;
		camera.rect = new Rect (0, 0, 1, 1);
		GameObject.Find ("CameraP2").SetActive(false);
		GameObject.Find ("Divider").SetActive(false);
	}
}
