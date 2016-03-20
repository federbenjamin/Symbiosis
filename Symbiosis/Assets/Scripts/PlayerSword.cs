using UnityEngine;
using System.Collections;

public class PlayerSword : MonoBehaviour {

	Animator swordAnimator;
	public iAugment augment;
	public int swordDamage;

	public bool isSwinging;
	private GameObject swordTrail;
	private Material trailColor;

	public void setAugment(iAugment aug){
		Debug.Log ("On-hit effects set");
		augment = aug;
	}

	// Use this for initialization
	void Start () {
		swordAnimator = transform.GetComponent<Animator> ();
		foreach (Transform child in transform) {
			if (child.name == "Trail") {
				swordTrail = child.gameObject;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		Color newColor = new Color ();

		if (augment != null) {
			if (augment.Element == "fire") {
				ColorUtility.TryParseHtmlString ("#C70A09", out newColor);
			} else if (augment.Element == "ice") {
				ColorUtility.TryParseHtmlString ("#033EC7", out newColor);
			} else if (augment.Element == "earth") {
				ColorUtility.TryParseHtmlString ("#0E5910", out newColor);
			}
		}else {
			ColorUtility.TryParseHtmlString ("#CCCCCC", out newColor);
		}

		Renderer renderer = transform.GetComponent<Renderer> ();
		foreach (Material matt in renderer.materials) {
			if (matt.name == "RedGlow (Instance)") {
				matt.SetColor("_Color", newColor);
			}
		}
	}

	public void StopSwinging() {
		isSwinging = false;
		swordTrail.SetActive(false);
		swordAnimator.SetTrigger ("Stop");
	}

	public void Swing() {
		isSwinging = true;
		
		if (augment != null) {
			if (augment.Element == "fire") {
				trailColor = Resources.Load<Material>("RedTrail");
			} else if (augment.Element == "ice") {
				trailColor = Resources.Load<Material>("BlueTrail");
			} else if (augment.Element == "earth") {
				trailColor = Resources.Load<Material>("GreenTrail");
			}
		} else {
			trailColor = Resources.Load<Material>("WhiteTrail");
		}

		swordTrail.GetComponent<TrailRenderer>().material = trailColor;
		swordTrail.SetActive(true);
		swordAnimator.SetTrigger ("Attack");
	}

	void OnTriggerEnter(Collider c) {
		GameObject other = c.gameObject;

		if (other.tag == "Enemy") {
			string damageType = "none";
			float force = 500;
			if (augment != null) {
				augment.onHitEffect (other);
				damageType = augment.Element;
				if (augment.Element == "earth") {
					force = 1000;
				}
			}

			//TODO: Add enemy Knockback

			EnemyStats enemyHP = other.GetComponent<EnemyStats> ();
			if (isSwinging) {
				enemyHP.TakeDamage (swordDamage, damageType);
			}
		}
	}
}

