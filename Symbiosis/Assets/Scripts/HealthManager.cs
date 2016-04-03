using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour {

	public static HealthManager Instance;

	private PlayerMovement playerMovementP1;
	private PlayerMovement playerMovementP2;

	Animator animatorP1Body;
	Animator animatorP1Slime;
	Animator animatorP2Body;
	Animator animatorP2Slime;

	public static int totalHealth;
	public static int currentHealth;

	public Sprite BarHit;
	public Sprite BarNormal;
	public Sprite fullHeart;
	public Sprite halfHeart;
	public Sprite emptyHeart;

	private Transform healthBar;
	private Transform heart1;
	private Transform heart2;
	private Transform heart3;
	private Transform heart4;
	private Transform heart5;

	public static bool isGameOver = false;

	void Awake () {
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		totalHealth = 10;
		currentHealth = 10;

		/* Add generation of these children depending
		 * on the numberof totalHealth.
		 * Use division of total amount of hearts and the length
		 * of the layer before it to give offsets.
		 */


		healthBar = transform.FindChild("HealthBar");
		heart1 = healthBar.FindChild ("0,1,2");
		heart2 = healthBar.FindChild ("3,4");
		heart3 = healthBar.FindChild ("5,6");
		heart4 = healthBar.FindChild ("7,8");
		heart5 = healthBar.FindChild ("9,10");

		GameObject playerP1 = GameObject.Find ("P1");
		GameObject playerP2 = GameObject.Find ("P2");

		foreach (Transform child in playerP1.transform) {
			if (child.name == "Player_animated") {
				animatorP1Body = child.GetChild(0).GetComponent<Animator> ();
			}
		}
		animatorP1Slime = GameObject.Find("Player_blue_slime").GetComponent<Animator> ();

		foreach (Transform child in playerP2.transform) {
			if (child.name == "Player_animated") {
				animatorP2Body = child.GetChild(0).GetComponent<Animator> ();
			}
		}
		animatorP2Slime = GameObject.Find("Player_yellow_slime").GetComponent<Animator> ();
	}
	void FixedUpdate() {
		
	}

	// Update is called once per frame
	void Update () {
		
		if (currentHealth <= 0) {
			heart1.GetComponent<Image> ().sprite = emptyHeart;
			GameOver ();
		} if (currentHealth == 1) {
			heart1.GetComponent<Image> ().sprite = halfHeart;
		} if (currentHealth >= 2) {
			heart1.GetComponent<Image> ().sprite = fullHeart;
		} if (currentHealth < 3) {
			heart2.GetComponent<Image> ().sprite = emptyHeart;
		} if (currentHealth == 3) {
			heart2.GetComponent<Image> ().sprite = halfHeart;
		} if (currentHealth >= 4) {
			heart2.GetComponent<Image> ().sprite = fullHeart;
		} if (currentHealth < 5) {
			heart3.GetComponent<Image> ().sprite = emptyHeart;
		} if (currentHealth == 5) {
			heart3.GetComponent<Image> ().sprite = halfHeart;
		} if (currentHealth >= 6) {
			heart3.GetComponent<Image> ().sprite = fullHeart;
		} if (currentHealth < 7) {
			heart4.GetComponent<Image> ().sprite = emptyHeart;
		} if (currentHealth == 7) {
			heart4.GetComponent<Image> ().sprite = halfHeart;
		} if (currentHealth >= 8) {
			heart4.GetComponent<Image> ().sprite = fullHeart;
		} if (currentHealth < 9) {
			heart5.GetComponent<Image> ().sprite = emptyHeart;
		} if (currentHealth == 9) {
			heart5.GetComponent<Image> ().sprite = halfHeart;
		} if (currentHealth >= 10) {
			heart5.GetComponent<Image> ().sprite = fullHeart;
		}
	}

	//Increase health of players
	public static void HealHealth(int heal) {
		
		if ((currentHealth += heal) > totalHealth) {
			currentHealth = totalHealth;
		}
	}

	//Decrease health of players
	public static void DamageHealth(int damage) {
		HealthManager.Instance.healthBar.GetComponent<Image>().sprite = HealthManager.Instance.BarHit;
		HealthManager.Instance.healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 100);
		if (!GameStats.Instance.invincible) {
			currentHealth -= damage;
		}
		HealthManager.Instance.StartCoroutine ("ChangeHealthBar");
	}

	public void GameOver() {
		isGameOver = true;
		//TODO: destroy weapon model

		animatorP1Body.SetTrigger ("gameOver");
		animatorP1Slime.SetTrigger ("gameOver");
		animatorP2Body.SetTrigger ("gameOver");
		animatorP2Slime.SetTrigger ("gameOver");
		StartCoroutine ("Wait");
	}

	private	IEnumerator Wait() {
		yield return new WaitForSeconds (3f);
		SceneManager.LoadScene ("GameOver");
	}

	IEnumerator ChangeHealthBar() {
		yield return new WaitForSeconds (0.75f);
		healthBar.GetComponent<Image>().sprite = BarNormal;
		healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 100);
	}

	public void TranslateHealth() {
		StartCoroutine("EmphasizingWait");
	}

	IEnumerator EmphasizingWait() {
		yield return new WaitForSeconds (0.75f);
		healthBar.transform.rotation = Quaternion.Euler (0, 0, 0);
		heart1.transform.rotation = Quaternion.Euler(0,0,0);
		heart2.transform.rotation = Quaternion.Euler(0,0,0);
		heart3.transform.rotation = Quaternion.Euler(0,0,0);
		heart4.transform.rotation = Quaternion.Euler(0,0,0);
		heart5.transform.rotation = Quaternion.Euler(0,0,0);

		yield return new WaitForSeconds (0.75f);
		transform.GetComponent<RectTransform>().anchorMin = new Vector2 (0.5f, 1);
		transform.GetComponent<RectTransform>().anchorMax = new Vector2 (0.5f, 1);
		transform.GetComponent<RectTransform>().pivot = new Vector2 (0.5f, 1);
	}
}

