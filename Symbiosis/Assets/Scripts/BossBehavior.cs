using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossBehavior : EnemyBehavior {

	private System.Random pseudoRandom;

	private bool enemyStarted = false;
	private bool enemyActive = true;
	private bool enemyOriented = false;
	private float turnSpeed = 4;
	private int phase = 0;

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
		}

		if (IsEnemyAlive() && !HealthManager.isGameOver) {
			UpdateTargetPlayer();
			enemyStarted = true;
		} else {
			enemyAnimator.SetTrigger ("Stopped");
			enemyStarted = false;
		}

	}

	void FixedUpdate () {
		if (enemyStarted && enemyActive && roomController.EnemiesActive) {
			timer++;
			if (timer == 30) {
				// SwitchAugment();
				StartCoroutine("RandomAugmentSwap");
				timer = 0;
			}

			if (phase == 0) {

				// rotate to look at the player
				Vector3 direction = targetPlayer.Transform.position - myRigidBody.position;
				direction.y = 0;
				Quaternion angleTowardsPlayer = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
				myRigidBody.MoveRotation(angleTowardsPlayer);

				if (targetPlayer.Distance > stoppingDistance) {
					enemyAnimator.SetTrigger ("Walking");
					Vector3 moveDirection = myTransform.forward;
					moveDirection.y = 0;
					//move towards the player
					myRigidBody.AddForce (moveDirection * (moveSpeed * 10) * Time.deltaTime, ForceMode.VelocityChange);
					myTransform.position += moveDirection * moveSpeed * Time.deltaTime;
				} else if (Time.time > nextHit) {
					enemyAnimator.SetTrigger ("Stopped");

					DamagePlayer(1);
				} else {
					enemyAnimator.SetTrigger ("Stopped");
				}
			}

			else if (phase == 1) {

			} 

			else if (phase == 2) {
				
			}

		
			

		} else {
			ActivateEnemiesOnProximity(2f);
		}
	}

	protected void SwitchAugment() {
		StartCoroutine("RandomAugmentSwap");
	}

	IEnumerator RandomAugmentSwap() {
		enemyActive = false;
		enemyAnimator.SetTrigger ("Stopped");

		EnemyStats enemyStats = gameObject.GetComponent<EnemyStats>();
		if (remainingAugments.Count == 0) {
			FillEmptyPhaseColorsList(enemyStats.elementType);
		}

		int nextAugIndex = pseudoRandom.Next(remainingAugments.Count);
		string nextAug = remainingAugments[nextAugIndex];
		remainingAugments.RemoveAt(nextAugIndex);

		gameObject.GetComponent<FadeColor>().SetColor(nextAug);

		yield return new WaitForSeconds (1f);
		enemyStats.elementType = nextAug;
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
				nextHit = Time.time + 60f;
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
		if (collision.gameObject.tag == "Player") {
			if (timer > nextHit) {
				DamagePlayer(1);
			}
		}
	}
}
