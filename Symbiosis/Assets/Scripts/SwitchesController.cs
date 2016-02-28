using UnityEngine;
using System.Collections;

public class SwitchesController : MonoBehaviour {

	private RoomController roomController;

	// Use this for initialization
	void Start () {
		roomController = transform.parent.GetComponent<RoomController> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			roomController.switchesActive += 1;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			roomController.switchesActive -= 1;
		}
	}
}
