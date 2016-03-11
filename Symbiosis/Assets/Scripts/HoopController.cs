using UnityEngine;
using System.Collections;

public class HoopController : MonoBehaviour {
	public GameObject otherPlayer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		transform.LookAt (otherPlayer.transform);
	}
}
