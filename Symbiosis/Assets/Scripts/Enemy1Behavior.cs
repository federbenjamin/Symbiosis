using UnityEngine;
using System.Collections;

public class Enemy1Behavior : EnemyBehavior {
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		float dist_1 = Vector3.Distance(myTransform.position, p1_Transform.position);
		float dist_2 = Vector3.Distance(myTransform.position, p2_Transform.position);

		Transform target;

		if (dist_1 < dist_2) {
			target = p1_Transform;
		} else {
			target = p2_Transform;
		}
		//rotate to look at the player
		myTransform.LookAt(target);

		//move towards the player
		myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
	}
}
