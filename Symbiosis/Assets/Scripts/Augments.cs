using UnityEngine;
using System.Collections;

// interface for Bullet Augments
public interface iAugment {
	// Name of bullet's element
	string Element{ 
		get; 
		set;
	}
	// chance out of 100 of effect occuring
	int OnHitChance{ 
		get; 
		set;
	}
	// effect to trigger on hit
	void onHitEffect(GameObject other);
}

public class Augment : MonoBehaviour, iAugment{

	public Augment(string a){
		ac = (AudioClip) Resources.Load(a);
	}

	public AudioClip ac;

	private string element = "TEST";
	public string Element{ 
		get{return element;}
		set{ element = value;}
	}

	private int onHitChance;
	public int OnHitChance{ 
		get{return onHitChance;}
		set{ onHitChance = value;}
	}

	public void onHitEffect(GameObject other){
		if (other != null) {
			//Destroy (other);
		}
		Debug.Log ("Effect occuring");
		Destroy (other);
	}
}
public class GrowAugment : MonoBehaviour, iAugment{

	public GrowAugment(){
	}


	private string element = "TEST";
	public string Element{ 
		get{return element;}
		set{ element = value;}
	}

	private int onHitChance;
	public int OnHitChance{ 
		get{return onHitChance;}
		set{ onHitChance = value;}
	}

	public void onHitEffect(GameObject other){
		Debug.Log ("Effect occuring");
		other.transform.localScale += new Vector3 (0.5f, 0.5f, 0.5f);
	}
}
public class FireAugment : MonoBehaviour, iAugment{

	public FireAugment(){
	}


	private string element = "fire";
	public string Element{ 
		get{return element;}
		set{ element = value;}
	}

	private int onHitChance;
	public int OnHitChance{ 
		get{return onHitChance;}
		set{ onHitChance = value;}
	}

	public void onHitEffect(GameObject other){
		Debug.Log ("Effect occuring");
	}
}
public class IceAugment : MonoBehaviour, iAugment{

	public IceAugment(){
	}


	private string element = "ice";
	public string Element{ 
		get{return element;}
		set{ element = value;}
	}

	private int onHitChance;
	public int OnHitChance{ 
		get{return onHitChance;}
		set{ onHitChance = value;}
	}

	public void onHitEffect(GameObject other){
		Debug.Log ("Effect occuring");
	}
}

public class Augments : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
