using UnityEngine;
using System.Collections;

public class EnemySplitterBehavior : EnemyBehavior {

	private bool enemyOriented = false;
	private float turnSpeed = 4;
	public float slowMoveSpeed = 2;
	public float mediumMoveSpeed = 3;
	public float fastMoveSpeed = 4;
	private bool realigningRotation = false;
	private bool realignTimerSet = false;

	private EnemyStats enemyStats;
	public int splitNum = 0;
	private float timerToRotate = 0;
	private Vector3 direction;
	private float nextRotate = 0;
	private bool checkCollider;

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

			if (roomController.EnemiesActive) {
				timer++;
				timerToRotate++;

				if (timerToRotate >= nextRotate) {
					direction = Random.insideUnitSphere;
					timerToRotate = 0;
					nextRotate = Random.Range(90f, 150f);
				}

				if (timerToRotate >= 2) {
					checkCollider = true;
				}

				// rotate in random direction
				direction.y = 0;
				myRigidBody.MoveRotation(Quaternion.LookRotation(direction));

				if (targetPlayer.Distance > stoppingDistance) {
					enemyAnimator.SetTrigger ("Walking");
					Vector3 moveDirection = myTransform.forward;
					moveDirection.y = 0;
					//move forwards
					myRigidBody.AddForce (moveDirection * (moveSpeed * 10) * Time.deltaTime, ForceMode.VelocityChange);
				} else {
					enemyAnimator.SetTrigger ("Stopped");
				}

			} else {
				ActivateEnemiesOnProximity(2f);
			}
		} else {
			enemyAnimator.SetTrigger ("Stopped");
		}
	}

	void UpdateTurnSpeed() {
		if (!realigningRotation) {
			turnSpeed = 5;
			moveSpeed = fastMoveSpeed;
		} else {
			// Debug.Log("realign");
			if (!realignTimerSet) {
				timer = 0;
				realignTimerSet = true;
				myRigidBody.velocity = Vector3.zero;
			}
			turnSpeed = 0;
			moveSpeed = 0;
		}
	}

	void OnCollisionStay(Collision collision) {
		if (collision.gameObject.tag == "Player") {
			if (timer > nextHit) {
				DamagePlayer(1);
				enemyAnimator.SetTrigger("Attack");
			}
		}
	}

	void OnCollisionEnter(Collision collision) {
		if ((collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy") && checkCollider) {
			timerToRotate = 200f;
			checkCollider = false;
		}
	}

	public void setChildSpeed(int splitNum) {
		if (splitNum == 0) {
			setMoveSpeed (slowMoveSpeed);
		} else if (splitNum == 1) {
			setMoveSpeed (mediumMoveSpeed);
		} else {
			setMoveSpeed (fastMoveSpeed);
		}
	}
}
