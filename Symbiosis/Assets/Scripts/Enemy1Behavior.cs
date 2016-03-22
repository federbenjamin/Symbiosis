using UnityEngine;
using System.Collections;

public class Enemy1Behavior : EnemyBehavior {

	private bool enemyOriented = false;
	private float turnSpeed = 4;
	public float slowMoveSpeed = 2;
	public float fastMoveSpeed = 8;
	private bool realigningRotation = false;
	private bool realignTimerSet = false;

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

		if (IsEnemyAlive() && !playersHealth.IsGameOver) {
			UpdateTargetPlayer();
			UpdateTurnSpeed();
			if (timer >= 30) {
				realigningRotation = false;
			}

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

					bool playersTogether = GameObject.FindWithTag("Canvas").GetComponent<GameStats>().PlayersTogether;
					if (!playersTogether) {
						GameObject.Find ("Camera"+ targetPlayer.PlayerObject.name).GetComponent<CameraShaker> ().shake = 0.25f;
					} else {
						GameObject.Find ("CameraP1").GetComponent<CameraShaker> ().shake = 0.25f;
					}

					DamagePlayer(1);
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
			turnSpeed = 4;
			moveSpeed = fastMoveSpeed;
		} else {
			// Debug.Log("realign");
			if (!realignTimerSet) {
				timer = 0;
				realignTimerSet = true;
				myRigidBody.velocity = Vector3.zero;
			}
			turnSpeed = 0;
			moveSpeed = -4;
		}
	}

	protected void OnCollisionEnter (Collision col) {
		// if (col.gameObject.tag == "Wall" || col.gameObject.tag == "Door" || col.gameObject.tag == "Enemy" || col.gameObject.tag == "RoomObject") {
  //       	realigningRotation = true;
  //      	}
       	if (col.gameObject.tag != "Floor") {
       		Debug.Log("Colided with: " + col.gameObject.tag);
        	realigningRotation = true;
       	}
	}
}
