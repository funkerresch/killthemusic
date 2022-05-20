using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Countdown : MonoBehaviour {

	public static float timeElapsed;

	void Start() 
	{
		timeElapsed = 0;
	}

	void Update()
	{
		timeElapsed += Time.deltaTime;
	}
}
