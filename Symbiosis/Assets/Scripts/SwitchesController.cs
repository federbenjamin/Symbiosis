using UnityEngine;
using System.Collections;

public class SwitchesController : MonoBehaviour {

	private RoomController roomController;
	private string switchName;


	// Use this for initialization
	void Start () {
		roomController = transform.parent.GetComponent<RoomController> ();
		switchName = transform.name;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other) {
		if (switchName == "SwitchP1" && other.name == "P1") {
			roomController.Switch1Active = true;
			pressButton();
		} else if (switchName == "SwitchP2" && other.name == "P2") {
			roomController.Switch2Active = true;
			pressButton();
		}
	}

	void OnTriggerExit(Collider other) {
		if (switchName == "SwitchP1" && other.name == "P1") {
			roomController.Switch1Active = false;
			depressButton();
		} else if (switchName == "SwitchP2" && other.name == "P2") {
			roomController.Switch2Active = false;
			depressButton();
		}
	}

	void pressButton() {
		transform.Find("Button").transform.Translate(0,0,-0.1f);
	}

	void depressButton() {
		transform.Find("Button").transform.Translate(0, 0,0.1f);
	}
}
