using UnityEngine;
using System.Collections;

public class Enemy1Behavior : EnemyBehavior {

	private bool enemyOriented = false;
	private float turnSpeed = 4;
	public float slowMoveSpeed = 2;
	public float fastMoveSpeed = 8;
	private bool realigningRotation = false;

	void Awake () {
		nextHit = 0;
	}

	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate () {
		if (!enemyOriented) {
			IgnorePlayer();
			enemyOriented = true;
		}

		if (IsEnemyAlive()) {
			UpdateTargetPlayer(false);
			UpdateTurnSpeed();
			ResetAlignTimer();

			if (roomController.EnemiesActive) {
				timer++;

				//rotate to look at the player
				Vector3 direction = targetPlayer.Transform.position - myRigidBody.position;
				direction.y = 0;
				Quaternion angleTowardsPlayer = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
				myRigidBody.MoveRotation(angleTowardsPlayer);

				if (targetPlayer.Distance > 0.9f) {
					enemyAnimator.SetTrigger ("Walking");
					Vector3 moveDirection = myTransform.forward;
					moveDirection.y = 0;
					//move towards the player
					myRigidBody.AddForce (moveDirection * (moveSpeed * 10) * Time.deltaTime, ForceMode.VelocityChange);
					//myTransform.position += moveDirection * moveSpeed * Time.deltaTime;
				} else if (timer > nextHit) {
					DamagePlayer(1);
				} else {
					enemyAnimator.SetTrigger ("Stopped");
				}

			} else {
				ActivateEnemiesOnProximity(2f);
			}
			collisionPosition = Vector3.zero;
		}
	}

	void UpdateTurnSpeed() {
		if (collisionPosition == Vector3.zero && !realigningRotation) {
			turnSpeed = 4;
			moveSpeed = fastMoveSpeed;
		} else {
			if (!realigningRotation) {
				timer = 0;
				realigningRotation = true;
				myRigidBody.velocity = Vector3.zero;
			}
			turnSpeed = 9;
			moveSpeed = slowMoveSpeed;
		}
	}

	void ResetAlignTimer() {
		if (timer >= 20) {
			realigningRotation = false;
		}
	}
}
