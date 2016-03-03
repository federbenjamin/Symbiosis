using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour {

	public bool invincible;
	public int totalHealth;
	public int currentHealth;

	public Sprite fullHeart;
	public Sprite halfHeart;

	private Transform heart1;
	private Transform heart2;
	private Transform heart3;
	private Transform heart4;
	private Transform heart5;

	private bool isGameOver = false;

	// Use this for initialization
	void Start () {
		totalHealth = 10;
		currentHealth = 10;

		/* Add generation of these children depending
		 * on the numberof totalHealth.
		 * Use division of total amount of hearts and the length
		 * of the layer before it to give offsets.
		 */


		heart1 = transform.FindChild ("0,1,2");
		heart2 = transform.FindChild ("3,4");
		heart3 = transform.FindChild ("5,6");
		heart4 = transform.FindChild ("7,8");
		heart5 = transform.FindChild ("9,10");

	}
	void FixedUpdate() {
		
	}

	// Update is called once per frame
	void Update () {
		
		if (currentHealth == 0) {
			heart1.GetComponent<Image> ().enabled = false;
			GameOver ();
		} if (currentHealth == 1) {
			heart1.GetComponent<Image> ().sprite = halfHeart;
		} if (currentHealth >= 2) {
			heart1.GetComponent<Image> ().sprite = fullHeart;
		} if (currentHealth < 3) {
			heart2.GetComponent<Image> ().enabled = false;
		} if (currentHealth == 3) {
			heart2.GetComponent<Image> ().sprite = halfHeart;
		} if (currentHealth >= 4) {
			heart2.GetComponent<Image> ().sprite = fullHeart;
		} if (currentHealth < 5) {
			heart3.GetComponent<Image> ().enabled = false;
		} if (currentHealth == 5) {
			heart3.GetComponent<Image> ().sprite = halfHeart;
		} if (currentHealth >= 6) {
			heart3.GetComponent<Image> ().sprite = fullHeart;
		} if (currentHealth < 7) {
			heart4.GetComponent<Image> ().enabled = false;
		} if (currentHealth == 7) {
			heart4.GetComponent<Image> ().sprite = halfHeart;
		} if (currentHealth >= 8) {
			heart4.GetComponent<Image> ().sprite = fullHeart;
		} if (currentHealth < 9) {
			heart5.GetComponent<Image> ().enabled = false;
		} if (currentHealth == 9) {
			heart5.GetComponent<Image> ().sprite = halfHeart;
		} if (currentHealth >= 10) {
			heart5.GetComponent<Image> ().sprite = fullHeart;
		}
	}

	//Increase health of players
	public void HealHealth(int heal) {
		currentHealth += heal;
	}

	//Decrease health of players
	public void DamageHealth(int damage) {
		if (!invincible) {
			currentHealth -= damage;
		}
	}

	void GameOver() {
		SceneManager.LoadScene ("GameOver");
	}
}

