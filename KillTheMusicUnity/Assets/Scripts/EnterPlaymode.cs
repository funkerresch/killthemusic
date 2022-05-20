using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterPlaymode : MonoBehaviour
{
    public GameObject editorGUI;
    public GameObject gameGUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaymodeOnOff(bool onOff)
    {
        if (onOff)
        {
            Debug.Log("Play On");
            gameGUI.SetActive(true);
            editorGUI.SetActive(false);
        }
        else
        {
            Debug.Log("Editor On");
            gameGUI.SetActive(false);
            editorGUI.SetActive(true);
        }

    }
}
