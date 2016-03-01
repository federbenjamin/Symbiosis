using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

	protected Transform p1_Transform;
	protected Transform p2_Transform;

	protected int moveSpeed;

	protected Transform myTransform;

	void Awake () {
		myTransform = transform;
	}

	// Use this for initialization
	void Start () {
		moveSpeed = GetComponent<EnemyStats>().moveSpeed;
		p1_Transform = GameObject.Find ("P1").transform;
		p2_Transform = GameObject.Find ("P2").transform;
	}

	public int getMoveSpeed () {
		return moveSpeed;
	}

	public void setMoveSpeed (int newSpeed) {
		moveSpeed = newSpeed;
	}
	
}
