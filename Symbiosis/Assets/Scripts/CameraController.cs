using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject player;

	private Vector3 newCameraPos;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("P1");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		if (player.transform.position.x - transform.position.x >= 1) {
			newCameraPos = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
			transform.position = Vector3.Lerp (transform.position, newCameraPos, 4.5f * Time.deltaTime);
		} 
		else if (player.transform.position.x - transform.position.x <= -1) {
			newCameraPos = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
			transform.position = Vector3.Lerp (transform.position, newCameraPos, 4.5f * Time.deltaTime);
		} 
	}
}
