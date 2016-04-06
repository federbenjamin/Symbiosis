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
	private bool checkCollider;
	private float timerToRotate = 0;
	private Vector3 direction;
	private float nextRotate = 0;
	private int layersToIgnore = 1 << 8;
	private bool notInCenter = true;
	public GameObject bullet;
	public GameObject bulletOrigin1;
	public GameObject bulletOrigin2;
	public GameObject bulletAwayTarget1;
	public GameObject bulletAwayTarget2;
	public GameObject cannon1;
	public GameObject cannon2;
	private int bulletVelocity = 20;
	private bool shooting = false;

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
			timer = 1300;
		}

		if (IsEnemyAlive() && !HealthManager.isGameOver) {
			UpdateTargetPlayer();
			enemyStarted = true;
		} else if (IsEnemyAlive() && HealthManager.isGameOver) {
			enemyAnimator.SetTrigger ("Win");
			enemyStarted = false;
		} else if (!IsEnemyAlive()) {
			enemyStarted = false;
		}

		cannon1.transform.position = bulletOrigin1.transform.position;//bulletOrigin1.GetComponent<Renderer>().bounds.center;
		cannon2.transform.position = bulletOrigin2.transform.position;//bulletOrigin2.GetComponent<Renderer>().bounds.center;

	}

	void FixedUpdate () {
		if (enemyStarted && roomController.EnemiesActive) {
			timer++;

			if (timer >= 1300) {
				StartCoroutine("RandomAugmentSwap");
				timer = 0;
			}

			if (phase == "ice" && enemyActive) {
				moveSpeed = 1.5f;

				// rotate to look at the player
				direction = targetPlayer.Transform.position - myRigidBody.position;
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
				moveSpeed = 14f;
				timerToRotate++;

				RaycastHit hit;
				Vector3 fwd = transform.TransformDirection(Vector3.forward);
				Vector3 pos = transform.position;
				pos.y = 1;
        		if (Physics.Raycast(pos, fwd, out hit, 1f, layersToIgnore)) {
        			Debug.Log(hit.transform.name);
        			if (hit.transform.tag == "Wall") {
        				Debug.Log(Time.time + " : WALL");
        				if (Random.Range(0, 2) == 0){
        					direction = Random.insideUnitSphere;
    					} else {
    						direction = -direction;
    					}
						timerToRotate = 0;
						nextRotate = Random.Range(60f, 80f);
        			}
        		}

				if (timerToRotate >= nextRotate) {
					direction = Random.insideUnitSphere;
					timerToRotate = 0;
					nextRotate = Random.Range(90f, 140f);
				}

				// if (timerToRotate >= 2) {
				// 	checkCollider = true;
				// }

				// rotate in random direction
				direction.y = 0;
				myRigidBody.MoveRotation(Quaternion.LookRotation(direction));

				enemyAnimator.SetTrigger ("Walking");
				Vector3 moveDirection = myTransform.forward;
				moveDirection.y = 0;
				//move forwards
				myRigidBody.AddForce (moveDirection * (moveSpeed * 10) * Time.deltaTime, ForceMode.VelocityChange);

				// Drop Slime
				if (Time.time > nextFire) {
					DropSlime();
				}
			} 

			else if (phase == "fire" && enemyActive) {
				moveSpeed = 1f;
				if (notInCenter) {
					enemyAnimator.SetTrigger ("Walking");

					direction = transform.parent.position - myRigidBody.position;
					direction.y = 0;
					Quaternion angleTowardsCenter = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
					myRigidBody.MoveRotation(angleTowardsCenter);

					float step = moveSpeed * Time.deltaTime;
        			transform.position = Vector3.MoveTowards(transform.position, transform.parent.position, step);
        			if (Vector3.Distance(transform.position, transform.parent.position) < 0.2f) {
        				notInCenter = false;
        			}
				} else {
					StartCoroutine("GetInShootingPosition");
					if (shooting && Time.time > nextFire) {
						Shoot (myTransform.forward);
					}
				}
			}

			else {
				enemyAnimator.SetTrigger ("Stopped");
			}


		} else {
			enemyAnimator.SetTrigger ("Stopped");
			ActivateEnemiesOnProximity(2f);
		}
	}

	IEnumerator GetInShootingPosition() {
		enemyAnimator.SetTrigger ("Shoot");
		yield return new WaitForSeconds (0.4f);
		shooting = true;
	}

	void Shoot(Vector3 shootDir) {
		if (transform.GetComponent<EnemyStats>().currentHP > 0) {
			Vector3 center = bulletOrigin1.GetComponent<Renderer>().bounds.center;
			Vector3 centerOffset = bulletAwayTarget1.GetComponent<Renderer>().bounds.center;
			Vector3 bulletDirection = center - centerOffset;
			GameObject clone = Instantiate (bullet, centerOffset, Quaternion.Euler(bulletDirection)) as GameObject;
			clone.transform.SetParent(bulletAwayTarget1.transform);
			Physics.IgnoreCollision (clone.GetComponent<Collider> (), GetComponent<Collider> ());
			clone.GetComponent<Rigidbody> ().velocity = (bulletDirection * bulletVelocity);

			center = bulletOrigin2.GetComponent<Renderer>().bounds.center;
			centerOffset = bulletAwayTarget2.GetComponent<Renderer>().bounds.center;
			bulletDirection = center - centerOffset;
			clone = Instantiate (bullet, centerOffset, Quaternion.Euler(bulletDirection)) as GameObject;
			clone.transform.SetParent(bulletAwayTarget2.transform);
			Physics.IgnoreCollision (clone.GetComponent<Collider> (), GetComponent<Collider> ());
			clone.GetComponent<Rigidbody> ().velocity = (bulletDirection * bulletVelocity);

			nextFire = Time.time + 0.15f;
		}
	}

	void DropSlime() {
		if (transform.GetComponent<EnemyStats>().currentHP > 0) {
			Vector3 slimeRotation = transform.rotation.eulerAngles;
			Quaternion quatRotation = Quaternion.Euler(270f, slimeRotation.y, slimeRotation.z);
			GameObject clone = Instantiate (slime, transform.position, quatRotation) as GameObject;
			nextFire = Time.time + Random.Range(0.8f, 1.2f);
		}
	}

	IEnumerator RandomAugmentSwap() {
		enemyActive = false;
		enemyAnimator.SetTrigger ("Stopped");

		EnemyStats enemyStats = gameObject.GetComponent<EnemyStats>();
		enemyStats.elementType = "black";

		float backupMass = myRigidBody.mass;
		myRigidBody.mass = 20f;
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
		// phase = "fire";
		myRigidBody.mass = backupMass;
		notInCenter = true;
		shooting = false;
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

	void OnCollisionEnter (Collision collision) {
		if (phase == "earth" && enemyActive) {
			if (collision.gameObject.tag == "Wall") {
				timerToRotate = 200f;
				checkCollider = false;
			}

			if (collision.gameObject.tag == "Player") {
				if (Time.time > nextHit) {
					DamagePlayer(1);
				}
			}
		}
	}

	void OnCollisionStay(Collision collision) {
		// if (collision.gameObject.tag == "Player") {
		// 	if (timer > nextHit) {
		// 		DamagePlayer(1);
		// 	}
		// }
	}
}
