using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class OutlineController : MonoBehaviour
{
    public Material[] materials;
    public Color colour;
    [Range(0f, 2f)] public float outlineWidth;
    public bool outlineEnable;



    public void ChangeOutlineColor()
    {
        foreach(Material m in materials)
        {
            m.SetColor("_OutlineColor", colour); // Orange:  (0.96,0.54,0,1)
            Debug.Log("Outline color changed");
        }
    }

    public void ChangeOutlineWidht()
    {
        foreach (Material m in materials)
        {
            m.SetFloat("_Outline", outlineWidth); // 1.5f
            Debug.Log("Outline width changed");
        }
    }

    // Enable and Disable the outline.
    public void TurnOnOutline(bool b)
    {
        foreach (Material m in materials)
        {
            if (!outlineEnable)
            {
                float turnOn = 0;
                if (b)
                {
                    turnOn = 1;
                    print("Turn on outline.");
                }                

                m.SetFloat("TurnOnOutline", turnOn);
            }
            else
            {
                m.SetFloat("TurnOnOutline", 1);
            }
        }
    }
}

#if UNITY_EDITOR
// Inspector custom settings:
[CustomEditor(typeof(OutlineController))]
public class ManagerEditor : Editor
{
    public OutlineController script;

    public void OnEnable()
    {
        script = (OutlineController)target;
    }

    public override void OnInspectorGUI()
    {
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Change Outline Color"))
        {
            script.ChangeOutlineColor();
        }

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Change Outline Width"))
        {
            script.ChangeOutlineWidht();
        }

        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Turn ON/OFF Outline"))
        {
            script.TurnOnOutline(script.outlineEnable);
        }

        GUI.backgroundColor = Color.white;
        base.OnInspectorGUI();
    }
}
#endif
