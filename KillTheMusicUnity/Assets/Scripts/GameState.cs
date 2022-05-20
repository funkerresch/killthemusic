
using UnityEngine;
using System;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class GameState : MonoBehaviour
{
	public static GameState state = null;

	RenderLayer instance;
	Layer layer = null;

	private int nextColorIndex = 0;

	private void Awake()
	{
		if (state != null && state != this)
		{
			Destroy(this.gameObject);
		}
		else
		{	
			state = this;
			DontDestroyOnLoad(this.gameObject);
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		instance = RenderLayer.GetInstance();
		Layer layer = instance.layer;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
