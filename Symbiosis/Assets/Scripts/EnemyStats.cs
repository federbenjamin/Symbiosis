using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyStats : MonoBehaviour {

	public float currentHP;
	public float maxHP;
	public float moveSpeed;
	private float baseMoveSpeed;
	public string elementType;
	private bool onFire;
	private bool frozen;
	private float ongoingDamage;
	private int ongoingTimer;
	public Animator enemyAnimator;
	public GameObject spawnParticles;
	private ParticleSystem particleSystem;
	float halfDmg, doubleDmg;
	public Object hitSpark;

	public bool isSplitter = false;
	public int splitNum = 0;
	public GameObject spawnChild;
	private bool enemyFirstDeath = true;
	public float divideBy = 0;
	private float numChildren;
	private float fireTimer = 0;
	private float freezeTimer = 0;
	public bool isBoss;
	private GameObject bossWalls;

	// Use this for initialization
	void Start () {
		onFire = false;
		frozen = false;
		currentHP = maxHP;
		ongoingTimer = 30;
		baseMoveSpeed = moveSpeed;

		foreach (Transform child in transform) {
			if (child.name == "SpawnParticles") {
				spawnParticles = child.gameObject;
			} else {
				enemyAnimator = child.GetComponent<Animator> ();
			}
		}
		particleSystem = spawnParticles.GetComponent<ParticleSystem>();
		ParticleSystem.EmissionModule em = particleSystem.emission;
		em.enabled = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (onFire) {
			if (ongoingTimer <= 0) {
				currentHP = currentHP - ongoingDamage;
				ongoingTimer = 30;
			} else {
				ongoingTimer = ongoingTimer - 1;
			}
			if (fireTimer < Time.time) {
				ParticleSystem.EmissionModule em = particleSystem.emission;
				em.enabled = false;
				onFire = false;
			}
		}
		if (frozen) {
			if (freezeTimer < Time.time) {
				ParticleSystem.EmissionModule em = particleSystem.emission;
				em.enabled = false;
				EnemyBehavior mover = GetComponent<EnemyBehavior> ();
				mover.setMoveSpeed(baseMoveSpeed);
				frozen = false;
			}
		}

		if ((elementType == "black") || (onFire && frozen)) {
			fireTimer = Time.time - 1f;
			freezeTimer = Time.time - 1f;
		}

		if (currentHP <= 0) {
			Die ();
		}
	}

	void Die(){
		//Play dying animation, and destroy
		// enemyAnimator.SetTrigger("Dead");
		if (isBoss) {
			StartCoroutine("GameWin");
		} else if (splitNum != 2 && enemyFirstDeath && isSplitter) {
			enemyFirstDeath = false;
			SpawnChildren (splitNum);
			enemyAnimator.Play("Player_Death");
		} else {
			enemyAnimator.Play ("Player_Death");
			StartCoroutine ("WaitBeforeDeath");
		}
	}

	IEnumerator GameWin() {
		// enemyAnimator.SetTrigger ("Dead");
		enemyAnimator.Play ("Player_Death");
		Time.timeScale = 0.4f;
		gameObject.GetComponent<Collider>().enabled = false;
		bossWalls = GameObject.Find ("boss-walls");
		bossWalls.GetComponent<Animator>().SetTrigger("Explode");
		Destroy (GameObject.Find("Wall-Explodable_001"));

		yield return new WaitForSeconds (2.5f);
		Time.timeScale = 1.0f;
		SceneManager.LoadScene ("WinScreen");
	}

	public void TakeDamage(float incomingDamage, string damageType){
		if (elementType != "black") {
			if (currentHP > 0) {
				GameObject spark = Instantiate (hitSpark, transform.position, transform.rotation) as GameObject;
				spark.transform.SetParent(transform);
			}

			DamageMultiplier(incomingDamage, damageType);
			if (!(damageType == "fire" && elementType == "ice")) {
				StatusEffect(damageType);
			} else {
				fireTimer = Time.time - 1f;
				freezeTimer = Time.time - 1f;
			}
		}
	}

	void DamageMultiplier(float incomingDamage, string damageType) {
		doubleDmg = (incomingDamage * 2f);
		halfDmg = incomingDamage / 2f;
		halfDmg = (halfDmg == 0f) ? 0.5f : halfDmg;

		if (damageType == elementType) {
			currentHP = currentHP - incomingDamage;
		} else if (damageType == "fire") {
			if (elementType == "ice") {
				currentHP = currentHP - halfDmg;
			} else if (elementType == "earth") {
				currentHP = currentHP - doubleDmg;
			}
		} else if (damageType == "ice") {
			if (elementType == "earth") {
				currentHP = currentHP - halfDmg;
			} else if (elementType == "fire") {
				currentHP = currentHP - doubleDmg;
			}
		} else if (damageType == "earth") {
			if (elementType == "fire") {
				currentHP = currentHP - halfDmg;
			} else if (elementType == "ice") {
				currentHP = currentHP - doubleDmg;
			}
		} else {
			currentHP = currentHP - halfDmg;
		}
	}

	void StatusEffect(string damageType) {
		if (damageType == "fire" && elementType != "fire") {
			Ignite ();
		}
		if (damageType == "ice" && elementType != "ice") {
			Freeze ();
		}
	}

	void Freeze(){
		frozen = true;
		spawnParticles.SetActive(true);
		spawnParticles.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture>("particle-blue");
		ParticleSystem.EmissionModule em = particleSystem.emission;
		em.enabled = true;
		EnemyBehavior mover = GetComponent<EnemyBehavior> ();
		mover.setMoveSpeed(moveSpeed / 2f);
		freezeTimer = Time.time + 3.5f;
	}

	void Ignite(){
		onFire = true;
		spawnParticles.SetActive(true);
		spawnParticles.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture>("particle-red");
		ParticleSystem.EmissionModule em = particleSystem.emission;
		em.enabled = true;
		ongoingDamage = 0.25f;
		fireTimer = Time.time + 2f;
	}


	IEnumerator WaitBeforeDeath() {
		yield return new WaitForSeconds (1f);
		Destroy (gameObject);
	}

	void SpawnChildren(int parentSplitNum) {
		int childSplitNum = parentSplitNum + 1;
		float childChange = Mathf.Pow (divideBy, childSplitNum);
		float newHP;
		if (elementType == "ice") {
			newHP = maxHP / 2f;
			numChildren = 4;
		} else {
			newHP = maxHP;
			numChildren = 2;
		}

		for (int i = 0; i < numChildren; i++) {
			GameObject newEnemy = Instantiate (spawnChild, transform.position, transform.rotation) as GameObject;
			newEnemy.transform.parent = transform.parent;
			newEnemy.GetComponent<EnemyStats> ().splitNum = childSplitNum;
			newEnemy.GetComponent<EnemyStats> ().currentHP = newHP * childChange;
			newEnemy.GetComponent<EnemyStats> ().maxHP = newHP * childChange;
			newEnemy.GetComponent<EnemySplitterBehavior> ().setChildSpeed (childSplitNum);
			newEnemy.transform.localScale = new Vector3 (childChange, childChange, childChange);
		}

		Destroy (gameObject);
	}
}
