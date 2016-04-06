using UnityEngine;
using System.Collections;

public class FadeColor : MonoBehaviour {

	public Material colorMaterial;
	private Color materialColour;
	private Color currentColour;
	private Color lastMaterialColour;
	private float t;

	// Use this for initialization
	void Start () {
		materialColour = Color.black;
		currentColour = colorMaterial.color;
		lastMaterialColour = colorMaterial.color;
	}
	
	// Update is called once per frame
	void Update () {
		if (lastMaterialColour != materialColour) {
	       t = 0; 
	       lastMaterialColour = materialColour;
	    }

	    t += 0.03f;
	    //renderer.material.color = currentColour;
	    if (currentColour != materialColour) {
	    	colorMaterial.color = Color.Lerp(currentColour, materialColour, t);
	    	currentColour = materialColour;
	    }

	}

	public void SetColor(string augType) {
		if (augType == "fire") {
			ColorUtility.TryParseHtmlString ("#C70A09", out materialColour);
		} else if (augType == "ice") {
			ColorUtility.TryParseHtmlString ("#033EC7", out materialColour);
		} else if (augType == "earth") {
			ColorUtility.TryParseHtmlString ("#0E5910", out materialColour);
		}
	}
}
