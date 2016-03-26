using UnityEngine;
using System.Collections;

public class SwitchesController : MonoBehaviour {

	private RoomController roomController;
	private Light lightObject;
	private Material buttonShader;
	private string switchName;
	private bool permanentlySwitchedOff = false;
	public bool PermanentlySwitchedOff {
		get{return permanentlySwitchedOff;}
		set
		{
			permanentlySwitchedOff = value;
			if (value == true) turningOff = true;
		}
	}
	private bool turningOff = false;
	private int timer1 = -1;
	private int timer2 = -1;
	private int timerMax = 100;

	public float inverseShutoffSpeed;
	private Color basicColor;
	private Color emissionColor;
	private float intensityDelta;
	private float colorRDelta;
	private float colorGDelta;
	private float colorBDelta;
	// private float colorADelta;
	private float emissionColorRDelta;
	private float emissionColorGDelta;
	private float emissionColorBDelta;

	public GameObject ButtonWire;
	private Animator animator;

	// Use this for initialization
	void Start () {
		roomController = transform.parent.GetComponent<RoomController> ();
		switchName = transform.name;
		foreach (Transform child in transform) {
			if (child.CompareTag("Light")) {
				lightObject = child.GetComponent<Light> ();
			} else if (child.name == "Button") {
				buttonShader = child.GetComponent<Renderer>().material;
			}
		}

		animator = ButtonWire.GetComponent<Animator> ();

		basicColor = buttonShader.GetColor("_Color");
		emissionColor = buttonShader.GetColor("_EmissionColor");
		intensityDelta = lightObject.intensity / inverseShutoffSpeed;
		colorRDelta = basicColor.r / inverseShutoffSpeed;
		colorGDelta = basicColor.g / inverseShutoffSpeed;
		colorBDelta = basicColor.b / inverseShutoffSpeed;
		//colorADelta = basicColor.a / inverseShutoffSpeed;
		emissionColorRDelta = emissionColor.r / inverseShutoffSpeed;
		emissionColorGDelta = emissionColor.g / inverseShutoffSpeed;
		emissionColorBDelta = emissionColor.b / inverseShutoffSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		if (turningOff) {
			basicColor = buttonShader.GetColor("_Color");
			emissionColor = buttonShader.GetColor("_EmissionColor");
			Color newColor = new Color (
				basicColor.r - colorRDelta,
				basicColor.g - colorGDelta,
				basicColor.b - colorBDelta,
				basicColor.a// - colorADelta
			);
			Color newEmissionColor = new Color (
				emissionColor.r - emissionColorRDelta,
				emissionColor.g - emissionColorGDelta,
				emissionColor.b - emissionColorBDelta
			);

			if (lightObject.intensity > 0) {
				lightObject.intensity -= intensityDelta;
				buttonShader.SetColor("_Color", newColor);
				buttonShader.SetColor("_EmissionColor", newEmissionColor);
			} else {
				turningOff = false;
			}
		}
	}

	void FixedUpdate () {
		if (timer1 > 0) {
			timer1--;
		} else if (timer1 == 0) {
			roomController.Switch1Active = true;
		}

		if (timer2 > 0) {
			timer2--;
		} else if (timer2 == 0) {
			roomController.Switch2Active = true;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (switchName == "SwitchP1" && other.name == "P1") {
			timer1 = timerMax;
			pressButton();
		} else if (switchName == "SwitchP2" && other.name == "P2") {
			timer2 = timerMax;
			pressButton();
		}
	}

	void pressButton() {
		transform.Find("Button").transform.Translate(0,0,-0.1f);
		animator.SetTrigger ("Triggered");
	}

	void OnTriggerExit(Collider other) {
		if (!permanentlySwitchedOff) {
			if (switchName == "SwitchP1" && other.name == "P1") {
				timer1 = -1;
				roomController.Switch1Active = false;
				depressButton();
			} else if (switchName == "SwitchP2" && other.name == "P2") {
				timer2 = -1;
				roomController.Switch2Active = false;
				depressButton();
			}
		}
	}

	void depressButton() {
		transform.Find("Button").transform.Translate(0, 0,0.1f);
		animator.SetTrigger ("Revert");
	}

}
