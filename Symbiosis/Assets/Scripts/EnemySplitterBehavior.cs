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
			}
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
