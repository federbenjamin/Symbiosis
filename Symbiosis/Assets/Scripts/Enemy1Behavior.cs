using UnityEngine;
using System.Collections;

public class Enemy1Behavior : EnemyBehavior {

	private int nextHit = 60;

	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate () {
		timer++;
		float dist_1 = Vector3.Distance(myTransform.position, p1_Transform.position);
		float dist_2 = Vector3.Distance(myTransform.position, p2_Transform.position);

		Transform target;
		float targetDist;
		if (dist_1 < dist_2) {
			target = p1_Transform;
			targetDist = dist_1;
		} else {
			target = p2_Transform;
			targetDist = dist_2;
		}
		//rotate to look at the player
		myTransform.LookAt(target);
		if (targetDist > 0.9f) {
			Vector3 moveDirection = myTransform.forward;
			moveDirection.y = 0;
			//move towards the player
			myTransform.position += moveDirection * moveSpeed * Time.deltaTime;
		} else if (timer > nextHit) {
			DamagePlayer();
		}
	}

	void DamagePlayer() {
		playersHealth.DamageHealth (1);
		timer = 0;
		nextHit = 60;
	}
}
