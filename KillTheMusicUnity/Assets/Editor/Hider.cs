using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Hider : EditorWindow
{

    [MenuItem("GameObject_Hider&Destroyer/Start")]
    public static void Create()
    {
        GetWindow<Hider>();
    }

    void OnGUI()
    {

        if (GUILayout.Button("Destory Selected Object"))
        {
            var allObjects = FindObjectsOfType<GameObject>();
            foreach (var go in allObjects)
            {
                if (go.tag == "Finish")
                {
                    Debug.Log("found one");
                    DestroyImmediate(go);
                }
            }

            
        }
    }

    private List<GameObject> HiddenObjects = new List<GameObject>();

    private void GatherHiddenObjects()
    {
        HiddenObjects.Clear();

        var allObjects = FindObjectsOfType<GameObject>();
        foreach (var go in allObjects)
        {
            if ((go.hideFlags & HideFlags.HideInHierarchy) != 0)
            {
                HiddenObjects.Add(go);
            }
        }

        Repaint();
    }
}