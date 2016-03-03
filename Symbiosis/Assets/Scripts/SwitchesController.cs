using UnityEngine;
using System.Collections;

public class SwitchesController : MonoBehaviour {

	private RoomController roomController;
	private GameObject button;

	// Use this for initialization
	void Start () {
		roomController = transform.parent.GetComponent<RoomController> ();
		foreach (Transform child in transform) {
			if (child.name == "Curve_001") {
				button = child.gameObject;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			roomController.switchesActive += 1;
			button.transform.Translate(0,0,-0.1f);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			roomController.switchesActive -= 1;
			button.transform.Translate(0, 0,0.1f);
		}
	}
}
