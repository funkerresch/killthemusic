using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ShowPanelAndText : MonoBehaviour
{
    Image img;
    Text txt;


    IEnumerator FadeInSub()
    {
        Color panCol = img.color;
        Color txtCol = txt.color;
        string textDisplayed = "THANKS FOR SILENCING US. YOUR TIME WAS " + Countdown.timeElapsed;
        txt.text = textDisplayed;

        for (float i = 0; i <= 5; i += 5*Time.deltaTime)
        {
            panCol.a = i/10f;
            txtCol.a = i/10f;
            img.color = panCol;
            txt.color = txtCol;
            yield return null;
        }
        
    }

    IEnumerator FadeOutSub()
    { 
            // loop over 1 second backwards
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            img.color = new Color(1, 1, 1, i);
            yield return null;
        }
        
       
       
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInSub());
    }

    public void InitAlpha()
    {
        Color panCol = img.color;
        Color txtCol = txt.color;
        
        panCol.a = 0;
        txtCol.a = 0;
        img.color = panCol;
        txt.color = txtCol;
    }

    private void Awake()
    {
        img = GameObject.Find("WinPanel").GetComponent<Image>();
        txt = GameObject.Find("WinText").GetComponent<Text>();
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
