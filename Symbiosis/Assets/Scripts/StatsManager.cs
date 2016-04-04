using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatsManager : MonoBehaviour {

	private GameObject roomIn;
	public GameObject RoomIn {
		get{return roomIn;}
		set{roomIn = value;}
	}
	public string startRoom;

	public iAugment playerAugment;
	public string augmentName;

	public float invincibilityTime;
	private PlayerShooting playerShooting;
	private float nextHit = 0.0f;

	private float AugTriggerRight;
	private float AugTriggerLeft;
	string swapButtonAug;
	private string playerPrefix;
	private string otherPlayerPrefix;
	private iAugment tempAug;
	private StatsManager otherPlayerStats;
	private PlayerShooting otherPlayerShooting;

	private GameObject playerAugSprite;
	private GameObject otherPlayerAugSprite;
	private Image otherPlayerHudImage;
	private bool hudReset = true;
	private Sprite hudDefault;
	private Sprite hudReq;
	private Sprite tempSpr;
	private static float nextAugSwap = 0.0f;
	public GameObject playerPrompt;
	public GameObject swapPrompt;

	private bool requestAugSwap = false;
	public bool RequestAugSwap {
		get{return requestAugSwap;}
		set{requestAugSwap = value;}
	}

	private int swapAugTimeout;
	public AudioClip swapCooldownSound;
	private bool nextAugSwapFailedSound = false;

	private HoopController hoopController;
	private string pingButton;
	private float nextHoopShow = 0.0f;

	private AudioPlacement audioPlacement;
	public AudioPlacement AudioPlacement {
		set{audioPlacement = value;}
	}
	private static bool swapLock = false;
	public bool inGeneratedLevel;

	void Awake () {
		playerPrefix = gameObject.name.Substring(0, 2);
		string startRoomName;
		if (inGeneratedLevel) {
			startRoom = (startRoom == "" ? "Tutorial1" : "-" + startRoom);
			startRoomName = "Room" + playerPrefix + startRoom;
		} else {
			if (startRoom == "") {
				startRoom = (playerPrefix == "P1" ? "1" : "2");
			}
			startRoomName = "Room" + startRoom;
		}
		roomIn = GameObject.Find(startRoomName);
		//audioPlacement = GameObject.Find("AudioListener").GetComponent<AudioPlacement> ();
	}

	// Use this for initialization
	void Start () {
		playerAugment = null;

		if (playerPrefix == "P1") {
			otherPlayerPrefix = "P2";
		} else {
			otherPlayerPrefix = "P1";
		}

		if ((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer)) {
			swapButtonAug = "SwapAugMac" + playerPrefix;
		} else if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer)) {
			swapButtonAug = "SwapAugPC" + playerPrefix;
		}

		otherPlayerStats = GameObject.Find (otherPlayerPrefix).GetComponent<StatsManager> ();
		playerAugSprite = GameObject.Find (playerPrefix + "Aug");
		otherPlayerAugSprite = GameObject.Find (otherPlayerPrefix + "Aug");
		otherPlayerHudImage = GameObject.Find(otherPlayerPrefix + "Hud").GetComponent<Image>();
		if (playerPrefix == "P1") {
			hudDefault = Resources.Load<Sprite> ("Interface/P2-slots-blank");
			hudReq = Resources.Load<Sprite> ("Interface/P2-slots-prompt");
		} else if (playerPrefix == "P2") {
			hudDefault = Resources.Load<Sprite> ("Interface/P1-slots-blank");
			hudReq = Resources.Load<Sprite> ("Interface/P1-slots-prompt");
		}

		if (roomIn != null) {
			transform.position = new Vector3(roomIn.transform.position.x, 0f, roomIn.transform.position.z);
		}

		playerShooting = GetComponent<PlayerShooting> ();
		otherPlayerShooting = GameObject.Find (otherPlayerPrefix).GetComponent<PlayerShooting> ();

		foreach (Transform child in transform) {
			if (child.tag == "Hoop") {
				hoopController = child.gameObject.GetComponent<HoopController>();
			}
		}
		//Determine shooting buttons for OS and Player
		if ((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer)) {
			pingButton = "PingMac" + playerPrefix;
		} else if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer)) {
			pingButton = "PingPC" + playerPrefix;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		if (GameStats.gameStarted) {
			if (Input.GetButton(pingButton) && Time.time > nextHoopShow) {
				hoopController.Show();
				nextHoopShow = Time.time + 3f;
			}

			// Check the swap request timeout, reset the request bool when it hits 0
			if (swapAugTimeout > 0) {
				swapAugTimeout--;
			} else {
				requestAugSwap = false;
				otherPlayerStats.DestroySwapPrompt();
			}

			// If swap cooldown time has passed, request an aug swap when the trigger is pressed
			// Otherwise play swap failed sound
			AugTriggerRight = Input.GetAxisRaw (swapButtonAug + "Right");
			AugTriggerLeft = Input.GetAxisRaw (swapButtonAug + "Left");
			if (!requestAugSwap) {
				if ((AugTriggerRight > 0 || AugTriggerLeft > 0) && Time.time > nextAugSwap) {
					Debug.Log ("AugTrigger" + playerPrefix);
					requestSwapAugments();
					nextAugSwapFailedSound = false;
				} else if (AugTriggerRight <= 0 || AugTriggerLeft <= 0) {
					nextAugSwapFailedSound = true;
				} else if ((AugTriggerRight > 0 || AugTriggerLeft > 0) && nextAugSwapFailedSound) {
					audioPlacement.PlayClip ("SFX/EarlySwap", 1f);
					nextAugSwapFailedSound = false;
				}
			}

			// If swap request currently sent, check for a response from other player
			if (requestAugSwap) {
				checkRequestSwapAugments();
			} else if (!hudReset) {
				hudReset = true;
				otherPlayerHudImage.sprite = hudDefault;
				string otherElement = (otherPlayerStats.GetAugment() == null ? null : otherPlayerStats.GetAugment().Element);
				if (otherElement == "fire") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Red-Blank");
				} else if (otherElement == "ice") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Blue-Blank");
				} else if (otherElement == "earth") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Green-Blank");
				}
			}
		}
	}
		
	public iAugment GetAugment() {
		return playerAugment;
	}

	public void SetAugment (iAugment augment){
		playerAugment = augment;
		if (augment == null) {
			augmentName = "No Augment";
		} else {
			augmentName = augment.Element;
		}
	}

	void requestSwapAugments() {
		requestAugSwap = true;
		swapAugTimeout = 60;
		otherPlayerStats.InitSwapPrompt();
	}

	void checkRequestSwapAugments() {
		if (otherPlayerStats.RequestAugSwap) {
			SwapAugments();
		} else {
			hudReset = false;
			string myElement = (GetAugment() == null ? null : GetAugment().Element);
			string otherElement = (otherPlayerStats.GetAugment() == null ? null : otherPlayerStats.GetAugment().Element);
			otherPlayerHudImage.sprite = hudReq;
			if (otherElement == "fire") {
				if (myElement == "ice") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Red-Blue");
				} else if (myElement == "earth") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Red-Green");
				}
			} else if (otherElement == "ice") {
				if (myElement == "fire") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Blue-Red");
				} else if (myElement == "earth") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Blue-Green");
				}
			} else if (otherElement == "earth") {
				if (myElement == "ice") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Green-Blue");
				} else if (myElement == "fire") {
					otherPlayerAugSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ("Interface/Augment-Green-Red");
				}
			}

			// Vibrate?
		}
	}

	public void SwapAugments() {
		hudReset = false;
		if (!swapLock) {
			swapLock = true;

			otherPlayerStats.RequestAugSwap = false;
			otherPlayerStats.DestroySwapPrompt();
			requestAugSwap = false;
			DestroySwapPrompt();

			tempAug = GetAugment();
			tempSpr = playerAugSprite.GetComponent<Image> ().sprite;

			SetAugment(otherPlayerStats.GetAugment ());
			playerAugSprite.GetComponent<Image>().sprite = otherPlayerAugSprite.GetComponent<Image>().sprite;

			otherPlayerStats.SetAugment (tempAug);
			otherPlayerAugSprite.GetComponent<Image>().sprite = tempSpr;

			swapLock = false;
		}
		nextAugSwap = Time.time + 2;
		swapAugTimeout = 0;
	}

	public void InitSwapPrompt() {
		swapPrompt = Instantiate (playerPrompt, transform.position, playerPrompt.transform.rotation) as GameObject;
		swapPrompt.GetComponent<SwapPrompt>().FocusPlayer(playerPrefix);
	}

	public void DestroySwapPrompt() {
		Destroy(swapPrompt);
	}
}
