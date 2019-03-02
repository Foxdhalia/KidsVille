using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidersSettings : MonoBehaviour
{
    [SerializeField] private GameObject topBar;
    

    public void DisableTopBar(float f)
    {

        if (f < 100f)
        {
            topBar.SetActive(true);
        }
        else
        {
            topBar.SetActive(false);
        }
    }

}
