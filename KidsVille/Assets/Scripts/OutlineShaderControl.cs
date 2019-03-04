using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineShaderControl : MonoBehaviour
{
    public Material material; 
    private bool receiveShadows;
    private bool outlineEnableAlways;
    private bool moveOutline;

    public BlendModes_Class blendModes = new BlendModes_Class(); // BlendMode controls the materials transparency.
    [SerializeField] private int customSrc_, customDst_; // BlendMode source and destination, respectively (recommend 1 and 0).


    private void Start()
    {
        ChooseBlendMode();
    }

    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChooseBlendMode();
        }
    }

    public void SetReceiveShadows(bool b)
    {
        receiveShadows = b;
        if (receiveShadows)
        {
            material.SetFloat("ReceiveShadows", 1);
            print("aaaaaaaa");
        }
        else
        {
            material.SetFloat("ReceiveShadows", 0);
            print("bbbbbbb");
        }
    }

    /////////OUTLINE SETTINGS:////////////////
    /////////////////////////////////////////

    // Manage the outline width.
    public void OutlineWidth(float width)
    {
        material.SetFloat("_Outline", width);
    }

    // Method to be called on Canvas button/toogle.
    public void SetOutlineEnableAlways(bool b)
    {
        outlineEnableAlways = b;
        TurnOnOutline(outlineEnableAlways);
    }

    // Enable and Disable the outline.
    public void TurnOnOutline(bool b)
    {
        if (!outlineEnableAlways)
        {
            float turnOn = 0;
            if (b)
            {
                turnOn = 1;
                print("Turn on outline.");
            }
            /* else
             {
                 turnOn = 0;
                 print("Turn off outline.");
             }*/

            material.SetFloat("TurnOnOutline", turnOn);
        }
        else
        {
            material.SetFloat("TurnOnOutline", 1);
        }
    }

    public void SetOutlineMove(bool b)
    {
        if (b)
        {
            material.SetFloat("MoveOutlineColor", 1);
        }
        else
        {
            material.SetFloat("MoveOutlineColor", 0);
        }
    }

    public void SetOutlineWidth(float f)
    {
        material.SetFloat("_Outline", f);
    }
    /////////END OUTLINE SETTINGS:////////////

    private void OnMouseOver()
    {
        TurnOnOutline(true);
    }

    private void OnMouseExit()
    {
        TurnOnOutline(false);
    }

    /////////BLEND MODE SETTINGS://///////////
    /////////////////////////////////////////
    void SetBlendMode(int src, int dest)
    {
        material.SetInt("_SrcBlend", src);
        material.SetInt("_DstBlend", dest);
    }

    public void ChooseBlendMode()
    {
        if (blendModes.SrcAlpha_OneMinusSrcAlpha) // Traditional  Transparency
        {
            SetBlendMode(5, 10);
        }
        else if (blendModes.One_OneMinusSrcAlpha) // Premultiplied transparency
        {
            SetBlendMode(1, 10);
        }
        else if (blendModes.One_One) // Additive
        {
            SetBlendMode(1, 1);
        }
        else if (blendModes.One_OneMinusDstColor) // Soft Additive
        {
            SetBlendMode(1, 4);
        }
        else if (blendModes.DstColor_Zero) // Multiplicative
        {
            SetBlendMode(2, 0);
        }
        else if (blendModes.DstColor_SrcColor)  // 2x Multiplicative
        {
            SetBlendMode(2, 3);
        }
        else if (blendModes.CustomBlend)
        {
            print("Custom Blend.");
            SetBlendMode(customSrc_, customDst_);
        }
        else
        {
            print("Blend One Zero.");
            SetBlendMode(1, 0);
        }
    }
    /////////END BLEND MODE SETTINGS:////////////

}

[Serializable]
public class BlendModes_Class
{
    public bool SrcAlpha_OneMinusSrcAlpha;
    public bool One_OneMinusSrcAlpha;
    public bool One_One;
    public bool One_OneMinusDstColor;
    public bool DstColor_Zero;
    public bool DstColor_SrcColor;
    public bool CustomBlend;
}
