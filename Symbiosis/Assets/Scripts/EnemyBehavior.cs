using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehavior : MonoBehaviour {

	protected Transform p1_Transform;
	protected Transform p2_Transform;
	protected TargetPlayer targetPlayer;
	protected bool ignorePlayer;

	protected float moveSpeed;
	protected int timer;
	protected int nextHit;
	protected Transform myTransform;
	protected Rigidbody myRigidBody;
	//protected Vector3 collisionNormal;
	protected Vector3 collisionPosition;

	public Animator enemyAnimator;

	protected HealthManager playersHealth;
	protected RoomController roomController;

	void Awake () {
	}

	// Use this for initialization
	void Start () {
		setStartVariables();
	}

	protected void setStartVariables() {
		myTransform = transform;
		myRigidBody = GetComponent<Rigidbody>();
		playersHealth = GameObject.Find("Health").GetComponent<HealthManager> ();
		roomController = transform.parent.GetComponent<RoomController> ();
		moveSpeed = GetComponent<EnemyStats>().moveSpeed;
		p1_Transform = GameObject.Find ("P1").transform;
		p2_Transform = GameObject.Find ("P2").transform;
		UpdateTargetPlayer(true);
		timer = 0;

		foreach (Transform child in transform) {
			if (child.name != "SpawnParticles") {
				enemyAnimator = child.GetComponent<Animator> ();
			}
		}
	}

	public float getMoveSpeed () {
		return moveSpeed;
	}

	public void setMoveSpeed (float newSpeed) {
		moveSpeed = newSpeed;
	}
	
	void OnCollisionEnter (Collision col) {
        if (col.gameObject.tag == "Wall" || col.gameObject.tag == "Door" || col.gameObject.tag == "Enemy" || col.gameObject.tag == "RoomObject") {
       		//collisionNormal = col.contacts[0].normal;
        	collisionPosition = col.transform.position;
       	} else if (col.gameObject.tag == "Player") {
       		//collisionNormal = col.contacts[0].normal;
       		//collisionPosition = col.transform.position;
       		ActivateEnemies();
       	}
    }

    void OnCollisionExit (Collision col) {
		if (col.gameObject.tag == "Player") {
			nextHit = 40;
		}
    }

    protected void ActivateEnemies() {
    	roomController.EnemiesActive = true;
    }

    protected void ActivateEnemiesOnProximity(float proximity) {
    	if (targetPlayer.Distance < proximity) {
    		ActivateEnemies();
    	}
    }

    protected bool IsEnemyAlive() {
    	return transform.GetComponent<EnemyStats>().currentHP > 0;
    }

    protected void IgnorePlayer() {
    	Vector3 point = targetPlayer.Transform.position;
		point.y = myTransform.position.y;
		myTransform.LookAt(point);

		float newYRotation = myTransform.rotation.eulerAngles.y + 180;
		float remainder = Mathf.Repeat(newYRotation, 90);
		if (remainder > 45) {
			newYRotation = newYRotation + (90 - remainder);
		} else {
			newYRotation = newYRotation - remainder;
		}
		myTransform.rotation = Quaternion.Euler (0, newYRotation, 0);
    }

    protected void UpdateTargetPlayer(bool isInit) {
    	float dist_1 = Vector3.Distance(myTransform.position, p1_Transform.position);
		float dist_2 = Vector3.Distance(myTransform.position, p2_Transform.position);

		if (dist_1 < dist_2) {
			if (isInit) {
				targetPlayer = new TargetPlayer(p1_Transform, dist_1);
			} else {
				targetPlayer.Transform = p1_Transform;
				targetPlayer.Distance = dist_1;
			}
		} else {
			if (isInit) {
				targetPlayer = new TargetPlayer(p2_Transform, dist_2);
			} else {
				targetPlayer.Transform = p2_Transform;
				targetPlayer.Distance = dist_2;
			}
		}
    }

    protected void DamagePlayer(int damage) {
		if (transform.GetComponent<EnemyStats>().currentHP > 0) {
			playersHealth.DamageHealth(damage);
			timer = 0;
			nextHit = 60;
		}
	}
}

public class TargetPlayer {
    private Transform transform;
    public Transform Transform {
     	get;
     	set;
    }
    private float distance;
    public float Distance {
		get;
     	set;
    }

    public TargetPlayer(Transform targetTransform, float targetDistance) {
        Transform = targetTransform;
        Distance = targetDistance;
    }
 }