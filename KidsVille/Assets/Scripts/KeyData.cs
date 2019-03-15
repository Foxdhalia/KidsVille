using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyData : MonoBehaviour
{
    [SerializeField] private string keyData;

    public string GetKey()
    {
        return keyData;
    }
}
