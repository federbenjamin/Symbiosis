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
			transform.Find("Button").transform.Translate(0,0,-0.1f);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			roomController.switchesActive -= 1;
			transform.Find("Button").transform.Translate(0, 0,0.1f);
		}
	}
}
