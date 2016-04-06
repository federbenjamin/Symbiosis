using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossBehavior : EnemyBehavior {

	private System.Random pseudoRandom;

	private bool enemyStarted = false;
	private bool enemyActive = true;
	private bool enemyOriented = false;
	private float turnSpeed = 4;
	private string phase;
	private float nextFire = 0;
	public GameObject slime;

	private List<string> remainingAugments = new List<string>();
	private string[] augList = new string[] {"fire", "ice", "earth"};

	public float stoppingDistance;

	void Awake () {
		pseudoRandom = new System.Random((int)System.DateTime.Now.Ticks);
		foreach (string augment in augList) {
			remainingAugments.Add(augment);
		}
	}

	// Update is called once per frame
	void Update () {
		if (!enemyOriented) {
			IgnorePlayer();
			enemyOriented = true;
			timer = 1000;
		}

		if (IsEnemyAlive() && !HealthManager.isGameOver) {
			UpdateTargetPlayer();
			enemyStarted = true;
		// } else if (!IsEnemyAlive()) {
		// 	enemyStarted = false;
			// StartCoroutine("GameWin");
		} else if (IsEnemyAlive() && HealthManager.isGameOver) {
			enemyAnimator.SetTrigger ("Win");
			enemyStarted = false;
		} else if (!IsEnemyAlive()) {
			enemyStarted = false;
			// StartCoroutine("GameWin");
		}

	}

	void FixedUpdate () {
		if (enemyStarted && roomController.EnemiesActive) {
			timer++;

			if (timer >= 1000) {
				StartCoroutine("RandomAugmentSwap");
				timer = 0;
			}

			if (phase == "ice" && enemyActive) {

				// rotate to look at the player
				Vector3 direction = targetPlayer.Transform.position - myRigidBody.position;
				direction.y = 0;
				Quaternion angleTowardsPlayer = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
				myRigidBody.MoveRotation(angleTowardsPlayer);

				if (targetPlayer.Distance > stoppingDistance + 2) {
					enemyAnimator.SetTrigger ("Walking");
					Vector3 moveDirection = myTransform.forward;
					moveDirection.y = 0;
					//move towards the player
					myRigidBody.AddForce (moveDirection * (moveSpeed * 10) * Time.deltaTime, ForceMode.VelocityChange);
					myTransform.position += moveDirection * moveSpeed * Time.deltaTime;
				} else if (targetPlayer.Distance > stoppingDistance + 1) {
					enemyAnimator.SetTrigger ("Walking_Claws");
					Vector3 moveDirection = myTransform.forward;
					moveDirection.y = 0;
					//move towards the player
					myRigidBody.AddForce (moveDirection * (moveSpeed * 10) * Time.deltaTime, ForceMode.VelocityChange);
					myTransform.position += moveDirection * moveSpeed * Time.deltaTime;
				} else if (Time.time > nextHit) {

					enemyAnimator.SetTrigger ("Walking_Claws");

					DamagePlayer(1);
				} else {
					enemyAnimator.SetTrigger ("Walking");
				}
			}

			else if (phase == "earth" && enemyActive) {

				// Drop Slime
				if (Time.time > nextFire) {
					DropSlime();
				}
			} 

			else if (phase == "fire" && enemyActive) {
				
			}

			else {
				enemyAnimator.SetTrigger ("Stopped");
			}

		
			

		} else {
			enemyAnimator.SetTrigger ("Stopped");
			ActivateEnemiesOnProximity(2f);
		}
	}

	void DropSlime() {
		if (transform.GetComponent<EnemyStats>().currentHP > 0) {
			Vector3 slimeRotation = transform.rotation.eulerAngles;
			Quaternion quatRotation = Quaternion.Euler(270f, slimeRotation.y, slimeRotation.z);
			GameObject clone = Instantiate (slime, transform.position, quatRotation) as GameObject;
			nextFire = Time.time + 1.5f;
		}
	}

	IEnumerator RandomAugmentSwap() {
		enemyActive = false;
		enemyAnimator.SetTrigger ("Stopped");

		EnemyStats enemyStats = gameObject.GetComponent<EnemyStats>();
		enemyStats.elementType = "black";

		gameObject.GetComponent<FadeColor>().SetColor("black");
		yield return new WaitForSeconds (2f);

		if (remainingAugments.Count == 0) {
			FillEmptyPhaseColorsList(enemyStats.elementType);
		}

		int nextAugIndex = pseudoRandom.Next(remainingAugments.Count);
		string nextAug = remainingAugments[nextAugIndex];
		remainingAugments.RemoveAt(nextAugIndex);

		gameObject.GetComponent<FadeColor>().SetColor(nextAug);

		yield return new WaitForSeconds (2f);
		enemyStats.elementType = nextAug;
		phase = nextAug;
		enemyActive = true;
	}

	protected void FillEmptyPhaseColorsList(string exceptElement) {
		foreach (string augment in augList) {
			remainingAugments.Add(augment);
		}
		remainingAugments.Remove(exceptElement);
	}

	protected void DamagePlayer(int damage) {
    	if (IsEnemyAlive()) {
    		bool playersTogether = GameObject.FindWithTag("Canvas").GetComponent<GameStats>().PlayersTogether;
			if (!playersTogether) {
				GameObject.Find ("Camera"+ targetPlayer.PlayerObject.name).GetComponent<CameraShaker> ().shake = 0.25f;
			} else {
				GameObject.Find ("CameraP1").GetComponent<CameraShaker> ().shake = 0.25f;
			}
			if (transform.GetComponent<EnemyStats>().currentHP > 0) {
				HealthManager.DamageHealth(damage);
				nextHit = Time.time + 1f;
			}
    	}
	}

	IEnumerator AttackAnimation() {
		enemyAnimator.SetTrigger ("Stopped");
		yield return new WaitForSeconds (1f);
	}

	void OnTriggerEnter (Collider col) {
	}

	void OnCollisionStay(Collision collision) {
		// if (collision.gameObject.tag == "Player") {
		// 	if (timer > nextHit) {
		// 		DamagePlayer(1);
		// 	}
		// }
	}
}
