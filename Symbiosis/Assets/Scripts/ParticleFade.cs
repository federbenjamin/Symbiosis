using UnityEngine;
using System.Collections;

public class ParticleFade : MonoBehaviour {

	// private bool startFadeOut = false;
	private ParticleSystem particleSystem;

	// Use this for initialization
	void Start () {
		particleSystem = gameObject.GetComponent<ParticleSystem>();
		StartCoroutine("WaitForFadeOut");
	}
	
	// Update is called once per frame
	void Update () {
		// if (startFadeOut) {
		// 	ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
		// 	particleSystem.GetParticles(particles);

		// 	for (int p = 0; p < particles.Length; p++) {
		// 		Color oldColor = particles[p].color;
		// 		particles[p].color = new Color(oldColor.r, oldColor.g, oldColor.b, oldColor.a - 0.04f);
		// 	}

		// 	particleSystem.SetParticles(particles, particles.Length);
		// }
	}

	IEnumerator WaitForFadeOut() {
		yield return new WaitForSeconds (4f);
		// startFadeOut = true;
		particleSystem.Stop();
	}
}
