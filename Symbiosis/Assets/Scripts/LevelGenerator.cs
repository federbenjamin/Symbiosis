using UnityEngine;
using System.Collections;
using System;

public class LevelGenerator : MonoBehaviour {

	public static LevelGenerator Instance;

	public GameObject root;
	private string seed;
	private bool useRandomSeed = true;
	private int size = 5;

	void Awake () {
		Instance = this;
		root = GameObject.Find("Root");
	}

	// Use this for initialization
	void Start () {
		if (useRandomSeed) {
			seed = Time.time.ToString();
		}

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		LoadRemainingAssets();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LoadRemainingAssets() {
		Transform rootTransform = root.transform;
		string[] objectNameList = new string[] {"Divider", "DividerHealth", "Canvas", "P1", "P2", "CameraParentP1", "CameraParentP2"};
		foreach (string objectName in objectNameList) {
			GameObject newObj = Instantiate (Resources.Load ("Procedural_Gen_Prefabs/" + objectName) as GameObject);
			newObj.name = objectName;
			newObj.transform.parent = rootTransform;
		}
	}
}
