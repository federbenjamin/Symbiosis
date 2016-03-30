using UnityEngine;
using System.Collections;

public class AudioPlacement : MonoBehaviour {

	private Transform cameraP1;
	private Transform cameraP2;

	// Use this for initialization
	void Start () {
		cameraP1 = GameObject.Find("CameraParentP1").transform;
		cameraP2 = GameObject.Find("CameraParentP2").transform;

		GameObject.Find("P1").GetComponent<StatsManager>().AudioPlacement = this;
		GameObject.Find("P2").GetComponent<StatsManager>().AudioPlacement = this;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = (cameraP1.position + cameraP2.position) / 2.0f;
	}

	public void PlayClip (string clip) {
		clip = "Sounds/" + clip;
		AudioClip soundClip = Resources.Load (clip) as AudioClip;
		AudioSource.PlayClipAtPoint (soundClip, transform.position);
	}

	public void PlayClip (string clip, float volume) {
		clip = "Sounds/" + clip;
		AudioClip soundClip = Resources.Load (clip) as AudioClip;
		AudioSource.PlayClipAtPoint (soundClip, transform.position, volume);
	}

	public void PlayClip (AudioClip clip) {
		AudioSource.PlayClipAtPoint (clip, transform.position);
	}

	public void PlayClip (AudioClip clip, float volume) {
		AudioSource.PlayClipAtPoint (clip, transform.position, volume);
	}

	public void changeMainSongPitch(float newPitch) {
		foreach (AudioSource audioSource in GetComponents<AudioSource>()) {
			if (audioSource.clip.name == "creeping2.0") {
				audioSource.pitch = newPitch;
				return;
			}
		}
	}

	public void changeMainSongVolume(float newVolume) {
		foreach (AudioSource audioSource in GetComponents<AudioSource>()) {
			if (audioSource.clip.name == "creeping2.0") {
				audioSource.volume = newVolume;
				return;
			}
		}
	}
}
