using UnityEngine;
using System.Collections;

public class EnemyTurretBehavior : EnemyBehavior {

	public GameObject bullet;
	private GameObject bulletOrigin;
	public GameObject bulletOrigin1;
	public GameObject bulletOrigin2;
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
						Shoot (myTransform.forward);
				}

			} else {
				ActivateEnemiesOnProximity(2.5f);
			}
			collisionPosition = Vector3.zero;
		} else {
			//enemyAnimator.SetTrigger ("Stopped");
		}

	}

	void Shoot(Vector3 shootDir) {
		if (transform.GetComponent<EnemyStats>().currentHP > 0) {
			//Right Cannon
			GameObject clone = Instantiate (bullet, bulletOrigin1.transform.position, bulletOrigin1.transform.rotation) as GameObject;
			clone.transform.rotation = Quaternion.LookRotation (shootDir);
			Physics.IgnoreCollision (clone.GetComponent<Collider> (), GetComponent<Collider> ());
			clone.GetComponent<Rigidbody> ().velocity = (clone.transform.forward * bulletVelocity);
		
			//Left Cannon
			GameObject clone2 = Instantiate (bullet, bulletOrigin2.transform.position, bulletOrigin2.transform.rotation) as GameObject;
			clone2.transform.rotation = Quaternion.LookRotation (shootDir);
			Physics.IgnoreCollision (clone2.GetComponent<Collider> (), GetComponent<Collider> ());
			clone2.GetComponent<Rigidbody> ().velocity = (clone2.transform.forward * bulletVelocity);
			nextFire = Time.time + Random.Range (0.4f - (fireRate * 0.25f), 1.5f - (fireRate * 0.47f)); //- (fireRate * 0.1f);
			

		}
	}

	void addShootingOffset(float offset) {
		nextFire = Time.time + offset;
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
}