using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour {

	Color source;
	Color dest;
	public Image blackOverlay;
	public float offsetTime;
	public float fadeTime;

	IEnumerator fade()
	{		
		float t = 0.0f;
		yield return new WaitForSeconds (offsetTime);

		while (t < 1.0f) {

			t += Time.deltaTime / fadeTime; // could use Time.timeScale additionally

			blackOverlay.color = Color.Lerp (source, dest, t);
			yield return 0;
		}
	}

	void Awake()
	{
		blackOverlay.color = new Color(blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 0);
	}

	// Use this for initialization
	void Start () {

		source = new Color (blackOverlay.color.r, blackOverlay.color.g, blackOverlay.color.b, 0);
		dest = new Color (source.r, source.g, source.b, 1);
		StartCoroutine(fade());
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
