using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScript : MonoBehaviour
{
    Toggle toggle;
    [SerializeField] private string langValue;
    [SerializeField] Translation translation;
    
    private void Start()
    {
        toggle = GetComponent<Toggle>();
        if (langValue == translation.GetLang() || (langValue == "pt" && translation.GetLang() == ""))
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }
    }

    public void GetInspectorLangValue()
    {
        if (toggle.isOn)
        {           
            ReceiveSettings receiver = FindObjectOfType<ReceiveSettings>();
            receiver.SetLanguage(langValue);
            print("Language pre-loaded: " + langValue);
        }
    }

    /*public void CheckToggleValue()
    {
        if (toggle.isOn)
        {
            ReceiveSettings receiver = FindObjectOfType<ReceiveSettings>();
            receiver.SetLanguage(langValue);
        }
    }*/
}
