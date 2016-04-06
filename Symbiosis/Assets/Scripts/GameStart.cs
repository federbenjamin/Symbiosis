using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameStart : MonoBehaviour {
	private string startButton;
	public GameObject startPanel;
	public GameObject firstSelected;
	private GameObject myEventSystem;

	// Use this for initialization
	void Start () {
		startPanel.SetActive(false);

		if ((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer)) {
			startButton = "StartMac";
		} else if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer)) {
			startButton = "StartPC";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown(startButton)) {
			startPanel.SetActive(true);
			EventSystem.current.SetSelectedGameObject(firstSelected, null);
		}
	}

	public void SmallFloor() {
		LevelData.SetLevelSize(5);
		SceneManager.LoadScene("Generated_Level");
	}

	public void LargeFloor() {
		LevelData.SetLevelSize(8);
		SceneManager.LoadScene("Generated_Level");
	}
}
