using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class NextScene : MonoBehaviour
{	
	IEnumerator LoadSceneAsync(string sceneName, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}

	public void LoadSlow(string sceneName)
	{
		StartCoroutine(LoadSceneAsync(sceneName, 4f));
	}

	public void Load(string sceneName)
	{	
		StartCoroutine(LoadSceneAsync(sceneName, 0f));
	}
}