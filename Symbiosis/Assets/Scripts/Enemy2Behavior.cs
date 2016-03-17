using UnityEngine;
using System.Collections;

public class Enemy2Behavior : EnemyBehavior {

	public GameObject bullet;
	public GameObject bulletOrigin;
	public int bulletVelocity;
	private float nextFire;
	private bool enemyOriented = false;
	private bool setShootingOffset = false;
	private bool readyToWalk = false;
	private bool freezeRotation = true;

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

			if (roomController.EnemiesActive) {
				if (!setShootingOffset) {
					addShootingOffset(2.5f);
					setShootingOffset = true;
				}
				timer++;

				//rotate to look at the player
				CheckTimeUntilRotation();
				if (!freezeRotation) {
					Vector3 predictedPlayerPos = targetPlayer.Transform.position + targetPlayer.PlayerObject.GetComponent<Rigidbody>().velocity * Time.deltaTime * 2;
					Vector3 direction = predictedPlayerPos - myRigidBody.position;
					Quaternion angleTowardsPlayer = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(direction), 4 * Time.deltaTime);
					myRigidBody.MoveRotation(angleTowardsPlayer);
				}

				//move towards the player
				if ((targetPlayer.Distance > 4 || targetPlayer.Distance < 3.5) && targetPlayer.Distance > 1.5 && !readyToWalk) {
					CheckTimeUntilMovement();
				} else if (targetPlayer.Distance > 4) {
					readyToWalk = true;
					enemyAnimator.SetTrigger ("Walking");
					Vector3 moveDirection = CalculateMoveDirection(true);
					myRigidBody.AddForce (moveDirection * (moveSpeed * 10) * Time.deltaTime, ForceMode.VelocityChange);
					//myRigidBody.MovePosition(myTransform.position + moveDirection * moveSpeed * Time.deltaTime);
				} else if (targetPlayer.Distance < 3.5) {
					readyToWalk = true;
					enemyAnimator.SetTrigger ("Walking");
					Vector3 moveDirection = CalculateMoveDirection(false);
					myRigidBody.AddForce (moveDirection * (moveSpeed * 10) * Time.deltaTime, ForceMode.VelocityChange);
					// myRigidBody.MovePosition(myTransform.position + moveDirection * (moveSpeed - 1) * Time.deltaTime);

					if (Time.time > nextFire) {
						enemyAnimator.SetTrigger ("Shoot");
						Shoot (myTransform.forward);
					}
				} else if (Time.time > nextFire) {
					readyToWalk = false;
					enemyAnimator.SetTrigger ("Shoot");
					Shoot (myTransform.forward);
				} else {
					readyToWalk = false;
					enemyAnimator.SetTrigger ("Stopped");
				}

			} else {
				ActivateEnemiesOnProximity(2.5f);
			}
			collisionPosition = Vector3.zero;
		}

	}

	Vector3 CalculateMoveDirection(bool goForward) {
		Vector3 moveDirection;
		if (goForward) {
			moveDirection = myTransform.forward;
		} else {
			moveDirection = -myTransform.forward;
		}
		// Debug.Log(collisionPosition);
		// Vector3 collisionDirection = Vector3.zero;
		// if (collisionPosition != Vector3.zero) {
		// 	collisionDirection = myRigidBody.position - collisionPosition;
		// 	moveDirection = Quaternion.AngleAxis(-90, Vector3.up) * collisionDirection * 2;
		// 	//moveDirection = -1 * (collisionPosition - myRigidBody.position);
		// }
		

		// if (collisionNormal.z == -1 || collisionNormal.z == 1) {
		// 	moveDirection.z = -0.5f * collisionNormal.z;
		// } else if (collisionNormal.x == -1 || collisionNormal.x == 1) {
		// 	moveDirection.x = -0.5f * collisionNormal.x;
		// }

		moveDirection.y = 0;
		return moveDirection;
	}

	void Shoot(Vector3 shootDir) {
		if (transform.GetComponent<EnemyStats>().currentHP > 0) {
			GameObject clone = Instantiate (bullet, bulletOrigin.transform.position, bulletOrigin.transform.rotation) as GameObject;
			clone.transform.rotation = Quaternion.LookRotation (shootDir);
			Physics.IgnoreCollision (clone.GetComponent<Collider> (), GetComponent<Collider> ());
			clone.GetComponent<Rigidbody> ().velocity = (clone.transform.forward * bulletVelocity);

			nextFire = Time.time + Random.Range (0.5f, 1.5f);
		}
	}

	void addShootingOffset(float offset) {
		nextFire = Time.time + offset;
	}

	void CheckTimeUntilMovement() {
		readyToWalk = (timer % 40 == 0);
	}

	void CheckTimeUntilRotation() {
		freezeRotation = (Random.Range(1, 50) % 10 == 0);
	}
}