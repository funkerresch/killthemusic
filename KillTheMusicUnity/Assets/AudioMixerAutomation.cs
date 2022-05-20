using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerAutomation : MonoBehaviour
{
    public AudioMixer mixer;

    public IEnumerator StartFade(string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        mixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            mixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }
        yield break;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
