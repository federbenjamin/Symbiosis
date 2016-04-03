using UnityEngine;
using System.Collections;

public class SwapPrompt : MonoBehaviour {

	private GameObject player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = player.transform.position;
	}

	public void FocusPlayer(string playerName) {
		player = GameObject.Find(playerName);
	}
}
