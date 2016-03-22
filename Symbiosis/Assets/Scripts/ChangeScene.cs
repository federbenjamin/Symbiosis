using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

	public string inputToChange;
	public string secondaryInputToChange;
	public string sceneName;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButton (inputToChange)){
			SceneManager.LoadScene(sceneName);
		} else if (Input.GetButton (secondaryInputToChange)) {
			SceneManager.LoadScene(sceneName);
		}
	}
}
