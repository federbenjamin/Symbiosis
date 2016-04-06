using UnityEngine;
using System.Collections;

public class FadeColor : MonoBehaviour {

	public Material colorMaterial;
	private Color materialColour;
	private Color currentColour;
	private Color lastMaterialColour;
	private float t = 0;

	// Use this for initialization
	void Start () {
		colorMaterial.color = Color.black;
		currentColour = Color.black;
		materialColour = Color.black;
	}
	
	// Update is called once per frame
	void Update () {

	    t += 0.008f;
	    if (currentColour != materialColour) {
			Color newColor = Color.Lerp(currentColour, materialColour, t);
			colorMaterial.SetColor("_Color", newColor);
	    }

	    if (t >= 1) {
			currentColour = materialColour;
			t = 0;
	    }
	}

	public void SetColor(string augType) {
		if (augType == "black") {
			materialColour = Color.black;
		} else if (augType == "fire") {
			ColorUtility.TryParseHtmlString ("#C70A09", out materialColour);
		} else if (augType == "ice") {
			ColorUtility.TryParseHtmlString ("#033EC7", out materialColour);
		} else if (augType == "earth") {
			ColorUtility.TryParseHtmlString ("#0E5910", out materialColour);
		}
	}
}
