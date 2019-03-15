using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveSettings : MonoBehaviour
{
    private string language;
      
    public string GetLanguage()
    {
        return language;
    }
    public void SetLanguage(string newLanguage)
    {
        language = newLanguage;
        //print("Selected language: " + language);
    }

    public void ChangeLanguage()
    {
        Translation tr = FindObjectOfType<Translation>();
        LanguageController lc = FindObjectOfType<LanguageController>();
        if (lc.CheckForLanguageChanges(language))
        {            
            //lc.LoadJsonDictionary(language);
            tr.StartTranslation(language);            
        }
        else
        {
            print("Language not changed.");
        }
    }

    /* public void ChangeLanguage()
     {
         LanguageController lc = FindObjectOfType<LanguageController>();
         if (lc.CheckForLanguageChanges(language))
         {
             lc.StartTranslation(language);
         }
         else
         {
             print("Language not changed.");
         }
     }*/

}
