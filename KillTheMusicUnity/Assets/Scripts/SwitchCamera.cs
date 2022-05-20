using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public Camera overviewCamera;
    public Camera playerCamera;
    private RenderLayer instance;
    private int camIndex = -1;

    void  Awake()
    {
        overviewCamera.enabled = true;
        playerCamera.enabled = false;
    }

    private void Start()
    {
        instance = RenderLayer.GetInstance();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            camIndex++;
            if (camIndex >= 3)
                camIndex = -1;

            if (camIndex == -1)
            {
                overviewCamera.enabled = true;
                playerCamera.enabled = false;
            }
            else
            {
                overviewCamera.enabled = false;
                playerCamera.enabled = true;
                instance.currentCamSettings = instance.camSettings[camIndex];
                playerCamera.fieldOfView = instance.currentCamSettings.fov;
                playerCamera.transform.eulerAngles = new Vector3(instance.currentCamSettings.xRotation, 0, 0);
            }
        }
    }
}
