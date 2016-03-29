using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class GamePause : MonoBehaviour {

	public static bool isPaused;
	private string startButton;
	public GameObject pausePanel;
	public GameObject firstSelected;
	private GameObject myEventSystem;
	private AudioPlacement gameAudio;


	// Use this for initialization
	void Start () {

		if ((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer)) {
			startButton = "StartMac";
		} else if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer)) {
			startButton = "StartPC";
		}

		isPaused = false;
		gameAudio = GameObject.Find("AudioListener").GetComponent<AudioPlacement> ();	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown(startButton)) {
			TogglePause ();
		}
	}

	public void TogglePause () {
		isPaused = !isPaused;

		if (isPaused) {
			pausePanel.SetActive(true);
			EventSystem.current.SetSelectedGameObject(firstSelected, null);
			Time.timeScale = 0.0f;
			gameAudio.changeMainSongVolume(0.07f);
			gameAudio.changeMainSongPitch(0.9f);
		} else {
			pausePanel.SetActive(false);
			Time.timeScale = 1.0f;
			gameAudio.changeMainSongVolume(0.264f);
			gameAudio.changeMainSongPitch(1f);
		}
	}
}
