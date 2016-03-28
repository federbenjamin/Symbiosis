using UnityEngine;
using System.Collections;

public class LoadingCanvasSetup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		foreach (Transform ui in transform) {
			RectTransform uiTransform = (RectTransform) ui;
			float newScaleX, newScaleY = 0f;

			GameObject pauseUI = ui.gameObject;
			// Get scale for pause background and keep pause text in a locked ratio
			newScaleY = (Screen.height / 1080f);
			newScaleX = (Screen.width / 1920f);
			if (Mathf.Min(newScaleY, newScaleX) == newScaleX) {
				pauseUI.transform.GetChild(0).localScale = new Vector3(newScaleY / newScaleX, 1.0f, 1.0f);
			} else {
				pauseUI.transform.GetChild(0).localScale = new Vector3(1.0f, newScaleX / newScaleY, 1.0f);
			}

			// Rescale UI element
			uiTransform.localScale = new Vector3(newScaleX, newScaleY, 1.0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
