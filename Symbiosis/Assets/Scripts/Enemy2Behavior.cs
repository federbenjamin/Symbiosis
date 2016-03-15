using UnityEngine;
using System.Collections;

public class Enemy2Behavior : EnemyBehavior {

	public GameObject bullet;
	public GameObject bulletOrigin;
	public int bulletVelocity;
	private int nextFire;
	private bool enemyOriented = false;
	private bool setShootingOffset = false;

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate () {
		if (!enemyOriented) {
			IgnorePlayer();
			enemyOriented = true;
		}

		if (EnemyAlive()) {
			UpdateTargetPlayer(false);

			if (roomController.EnemiesActive) {
				if (!setShootingOffset) {
					addShootingOffset(100);
					setShootingOffset = true;
				}
				timer++;

				//rotate to look at the player
				Vector3 point = targetPlayer.Transform.position;
				point.y = myTransform.position.y;
				myTransform.LookAt(point);

				Vector3 moveDirection = myTransform.forward;
				moveDirection.y = 0;
				//move towards the player
				if (targetPlayer.Distance > 4) {
					enemyAnimator.SetTrigger ("Walking");
					myTransform.position += moveDirection * moveSpeed * Time.deltaTime;
				} else if (targetPlayer.Distance < 3) {
					if (collisionNormal.z == -1 || collisionNormal.z == 1) {
						moveDirection.z = 0;//(-0.5f * collisionNormal.z);
					} else if (collisionNormal.x == -1 || collisionNormal.x == 1) {
						moveDirection.x = 0;//(-0.5f * collisionNormal.x);
					}
					enemyAnimator.SetTrigger ("Walking");
					myTransform.position -= moveDirection * (moveSpeed - 1) * Time.deltaTime;

					if (timer > nextFire) {
						enemyAnimator.SetTrigger ("Shoot");
						Shoot (myTransform.forward);
					}
				} else if (timer > nextFire) {
					enemyAnimator.SetTrigger ("Shoot");
					Shoot (myTransform.forward);
				} else {
					enemyAnimator.SetTrigger ("Stopped");
				}

			} else {
				ActivateEnemiesOnProximity(2.5f);
			}
		}

	}

	void Shoot(Vector3 shootDir) {
		if (transform.GetComponent<EnemyStats>().currentHP > 0) {
			GameObject clone = Instantiate (bullet, bulletOrigin.transform.position, bulletOrigin.transform.rotation) as GameObject;
			clone.transform.rotation = Quaternion.LookRotation (shootDir);
			Physics.IgnoreCollision (clone.GetComponent<Collider> (), GetComponent<Collider> ());
			clone.GetComponent<Rigidbody> ().velocity = (clone.transform.forward * bulletVelocity);

			timer = 0;
			nextFire = Random.Range (50, 60);
		}
	}

	void addShootingOffset (int offset) {
		nextFire = offset;
	}
}