using UnityEngine;
using System.Collections;

public class Enemy1Behavior : EnemyBehavior {

	private bool enemyOriented = false;
	private float turnSpeed = 4;
	public float slowMoveSpeed = 2;
	public float fastMoveSpeed = 6;
	private bool realigningRotation = false;
	private bool realignTimerSet = false;

	public float stoppingDistance;
	private NavMeshAgent _navMeshAgent;

	void Awake () {
		_navMeshAgent = transform.GetComponent<NavMeshAgent>();
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

				_navMeshAgent.speed = moveSpeed;
				_navMeshAgent.stoppingDistance = stoppingDistance;
				_navMeshAgent.destination = targetPlayer.Transform.position;

				// rotate to look at the player
				Vector3 direction = targetPlayer.Transform.position - myRigidBody.position;
				direction.y = 0;
				Quaternion angleTowardsPlayer = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
				myRigidBody.MoveRotation(angleTowardsPlayer);

				if (targetPlayer.Distance > stoppingDistance) {
					enemyAnimator.SetTrigger ("Walking");
					// Vector3 moveDirection = myTransform.forward;
					// moveDirection.y = 0;
					// //move towards the player
					// myRigidBody.AddForce (moveDirection * (moveSpeed * 10) * Time.deltaTime, ForceMode.VelocityChange);
					//myTransform.position += moveDirection * moveSpeed * Time.deltaTime;
				} else if (timer > nextHit) {
					enemyAnimator.SetTrigger ("Stopped");

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

	void OnTriggerEnter (Collider col) {
		// if (col.gameObject.tag == "Wall" || col.gameObject.tag == "Door" || col.gameObject.tag == "Enemy" || col.gameObject.tag == "RoomObject") {
  //       	realigningRotation = true;
  //      	}

       	if (col.gameObject.tag != "Floor") {
       		//Debug.Log("Colided with: " + col.gameObject.tag);
        	//realigningRotation = true;
       	}
       	// if (col.gameObject.tag == "Player") {
       	// 	Debug.Log("in");
       	// 	if (timer > nextHit) {
       	// 		DamagePlayer(1);
       	// 	}
       	// }
	}
}
