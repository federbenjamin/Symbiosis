using UnityEngine;
using System.Collections;

public class EnemyTurretBehavior : EnemyBehavior {

	public GameObject bullet;
	public GameObject bulletOrigin;
	public GameObject bulletOrigin1;
	public GameObject bulletOrigin2;
	private int cannonNum = 1;
	public float fireRate = 1;
	public int bulletVelocity;
	private float nextFire;
	private bool enemyOriented = false;
	private bool setShootingOffset = false;
	private bool readyToWalk = false;
	private bool freezeRotation = true;
	private bool nearDeath = false;

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
			CheckNearDeath();

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

				if (Time.time > nextFire) {
						//enemyAnimator.SetTrigger ("Shoot");
						Shoot (myTransform.forward, bulletOrigin);
						AlternateCannon();
				}

			} else {
				ActivateEnemiesOnProximity(2.5f);
			}
			collisionPosition = Vector3.zero;
		} else {
			//enemyAnimator.SetTrigger ("Stopped");
		}

	}

	void Shoot(Vector3 shootDir, GameObject cannon) {
		if (transform.GetComponent<EnemyStats>().currentHP > 0) {
			GameObject clone = Instantiate (bullet, cannon.transform.position, cannon.transform.rotation) as GameObject;
			clone.transform.rotation = Quaternion.LookRotation (shootDir);
			Physics.IgnoreCollision (clone.GetComponent<Collider> (), GetComponent<Collider> ());
			clone.GetComponent<Rigidbody> ().velocity = (clone.transform.forward * bulletVelocity);
			nextFire = Time.time + Random.Range (0.4f - (fireRate * 0.25f), 1.5f - (fireRate * 0.47f)); //- (fireRate * 0.1f);
			

		}
	}

	void addShootingOffset(float offset) {
		nextFire = Time.time + offset;
		//Set first cannon
		bulletOrigin = bulletOrigin1;
	}

	void CheckTimeUntilRotation() {
		freezeRotation = (Random.Range(1, 50) % 10 == 0);
	}

	void CheckNearDeath() {
		if (!nearDeath && (GetComponent<EnemyStats>().currentHP <= (GetComponent<EnemyStats>().maxHP / 3))) {
			nearDeath = true;
			fireRate = fireRate * 3;
		}
	}

	void AlternateCannon() {
		if (cannonNum % 2 == 1) {
			bulletOrigin = bulletOrigin2;
			cannonNum = 2;
		} else {
			bulletOrigin = bulletOrigin1;
			cannonNum = 1;
		}
	}
}