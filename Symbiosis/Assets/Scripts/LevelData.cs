using UnityEngine;
using System.Collections;

public class LevelData : MonoBehaviour {

	public static LevelData Instance;

	public static int levelSeed = -2001603228;
	public static int levelSize = 3;
	public static bool randomLevel = true;

	void Awake () {
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void SetLevelSeed(int newSeed) {
		levelSeed = newSeed;
	}

	public static void GenerateRandomSeed() {
		levelSeed = (int)System.DateTime.Now.Ticks;
	}

	public static void SetLevelSize(int size) {
		size = Mathf.Min(size, 10);
		size = Mathf.Max(size, 3);
		levelSize = size;
	}
}
