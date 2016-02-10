using UnityEngine;
using System.Collections;

public class ItemsAndAugmentControls : MonoBehaviour {

	public string swapKey;
	public GameObject twin;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(swapKey)){
			print("swap key was pressed");
			StatsManager aManager = GetComponent<StatsManager> ();
			StatsManager bManager = twin.GetComponent<StatsManager> ();

			iAugment a = aManager.GetAugment();
			iAugment b = bManager.GetAugment ();

			aManager.SetAugment (b);
			bManager.SetAugment (a);

		}
	}
}
