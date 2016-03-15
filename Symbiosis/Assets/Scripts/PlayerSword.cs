using UnityEngine;
using System.Collections;

public class PlayerSword : MonoBehaviour {

	Animator swordAnimator;
	public iAugment augment;
	public int swordDamage;

	public bool isSwinging;

	public void setAugment(iAugment aug){
		Debug.Log ("On-hit effects set");
		augment = aug;
	}

	// Use this for initialization
	void Start () {
		swordAnimator = transform.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		Color newColor = new Color ();

		if (augment != null) {
			if (augment.Element == "fire") {
				//newColor.r = 199;
				//newColor.g = 10;
				//newColor.b = 9;
				ColorUtility.TryParseHtmlString ("#C70A09", out newColor);
			} else if (augment.Element == "ice") {
				//newColor.r = 3;
				//newColor.g = 62;
				//newColor.b = 199;
				Debug.Log ("Ice AUgment");
				ColorUtility.TryParseHtmlString ("#033EC7", out newColor);
			} else if (augment.Element == "earth") {
				//newColor.r = 14;
				//newColor.g = 89;
				//newColor.b = 16;
				ColorUtility.TryParseHtmlString ("#0E5910", out newColor);
			}
		}else {
			//newColor.r = 204;
			//newColor.g = 204;
			//newColor.b = 204;
			Debug.Log ("Regular");
			ColorUtility.TryParseHtmlString ("#CCCCCC", out newColor);
		}

		Renderer renderer = transform.GetComponent<Renderer> ();
		foreach (Material matt in renderer.materials) {
			if (matt.name == "RedGlow (Instance)") {
				Debug.Log ("Color " + matt.GetColor ("_Color"));
				matt.SetColor("_Color", newColor);
			}
		}
	}

	public void StopSwinging() {
		isSwinging = false;
		swordAnimator.SetTrigger ("Stop");
	}

	public void Swing() {
		isSwinging = true;
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
			enemyHP.TakeDamage (swordDamage, damageType);
		}
	}
}

