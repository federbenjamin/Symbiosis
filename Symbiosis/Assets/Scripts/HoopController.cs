using UnityEngine;
using System.Collections;

public class HoopController : MonoBehaviour {
	public GameObject roomSwitch;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		transform.LookAt (roomSwitch.transform);
	}
}
