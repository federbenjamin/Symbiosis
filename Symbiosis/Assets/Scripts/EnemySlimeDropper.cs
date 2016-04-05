using UnityEngine;
using System.Collections;

public class EnemySlimeDropper : EnemyBehavior {

	private bool enemyOriented = false;
	private float turnSpeed = 4;
	private bool realigningRotation = false;
	private bool realignTimerSet = false;
	private float nextFire = 0;
	public GameObject slime;

	public float stoppingDistance;

	void Awake () {
	}

	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate () {
		if (!enemyOriented) {
			IgnorePlayer();
			enemyOriented = true;
		}

		if (IsEnemyAlive() && !HealthManager.isGameOver) {
			UpdateTargetPlayer();
			//UpdateTurnSpeed();

			if (timer >= 30) {
				realigningRotation = false;
			}

			if (roomController.EnemiesActive) {
				timer++;

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
				} else if (timer > nextHit) {
					enemyAnimator.SetTrigger ("Stopped");

					//DamagePlayer(1);
				} else {
					enemyAnimator.SetTrigger ("Stopped");
				}

				// Drop Slime
				if (Time.time > nextFire) {
					DropSlime();
				}
			} else {
				ActivateEnemiesOnProximity(2f);
			}
		} else {
			enemyAnimator.SetTrigger ("Stopped");
		}
	}

	void DropSlime() {
		if (transform.GetComponent<EnemyStats>().currentHP > 0) {
			Vector3 slimeRotation = transform.rotation.eulerAngles;
			Quaternion quatRotation = Quaternion.Euler(270f, slimeRotation.y, slimeRotation.z);
			GameObject clone = Instantiate (slime, transform.position, quatRotation) as GameObject;
			nextFire = Time.time + 2.5f;
		}
	}

	void OnCollisionStay(Collision collision) {
		if (collision.gameObject.tag == "Player") {
			if (timer > nextHit) {
				DamagePlayer(2);
			}
		}
	}
}
