using UnityEngine;
using System.Collections;

public class Enemy1Behavior : EnemyBehavior {

	private bool enemyOriented = false;

	void Awake () {
		nextHit = 60;
	}

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
				timer++;

				//rotate to look at the player
				Vector3 point = targetPlayer.Transform.position;
				point.y = myTransform.position.y;
				myTransform.LookAt(point);

				if (targetPlayer.Distance > 0.9f) {
					enemyAnimator.SetTrigger ("Walking");
					Vector3 moveDirection = myTransform.forward;
					moveDirection.y = 0;
					//move towards the player
					myTransform.position += moveDirection * moveSpeed * Time.deltaTime;
				} else if (timer > nextHit) {
					DamagePlayer(1);
				} else {
					enemyAnimator.SetTrigger ("Stopped");
				}

			} else {
				ActivateEnemiesOnProximity(2f);
			}
		}
	}
}
