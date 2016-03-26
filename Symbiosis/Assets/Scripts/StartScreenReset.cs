using UnityEngine;
using System.Collections;

public class StartScreenReset : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameStats.gameStarted = false;
		HealthManager.isGameOver = false;
	}
}
