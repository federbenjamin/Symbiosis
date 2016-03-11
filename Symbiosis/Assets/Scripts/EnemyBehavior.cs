using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

	protected Transform p1_Transform;
	protected Transform p2_Transform;

	protected int moveSpeed;
	protected int timer;
	protected Transform myTransform;
	protected Vector3 collisionNormal;

	public Animator enemyAnimator;

	protected HealthManager playersHealth;

	void Awake () {
		myTransform = transform;
	}

	// Use this for initialization
	void Start () {
		setStartVariables();
	}

	protected void setStartVariables() {
		playersHealth = GameObject.Find("Health").GetComponent<HealthManager> ();
		moveSpeed = GetComponent<EnemyStats>().moveSpeed;
		p1_Transform = GameObject.Find ("P1").transform;
		p2_Transform = GameObject.Find ("P2").transform;
		timer = 0;

		foreach (Transform child in transform) {
			enemyAnimator = child.GetComponent<Animator> ();
		}
	}

	public int getMoveSpeed () {
		return moveSpeed;
	}

	public void setMoveSpeed (int newSpeed) {
		moveSpeed = newSpeed;
	}
	
	void OnCollisionEnter (Collision col) {
        if (col.gameObject.tag == "Wall" || col.gameObject.tag == "Door" || col.gameObject.tag == "Enemy" || col.gameObject.tag == "Player") {
       		collisionNormal = col.contacts[0].normal;
       	} 
    }
}
