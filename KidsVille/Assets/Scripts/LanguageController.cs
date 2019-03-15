using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LanguageController : MainScript
{
    public TextAsset[] languageDictionaries = new TextAsset[2];

    public static LanguageController instance;
    public static string jsonDictionary;
    public static string[] jsonDictionaries =  new string[2];

    //public Text tx;

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        for(int i = 0; i < 2; i++)
        {
            jsonDictionaries[i] = languageDictionaries[i].text;           
        }
    }

    public void SetLanguage(string newLanguage)
    {
        lang = newLanguage;
    }

    public bool CheckForLanguageChanges(string newLanguage)
    {
        if (newLanguage == "pt" || newLanguage == "en")
        {
            if (newLanguage == langBefore)
            {
                print("new language: " + newLanguage + ", languageBefore: " + langBefore);
                return false;
            }
            else
            {
                print("new language: " + newLanguage + ", languageBefore: " + langBefore);                
                return true;
            }
        }
        else
        {
            return false;
        }
    }
   

    public string GetJsonDictionary()
    {
        return jsonDictionary;
    }
    public void LoadJsonDictionary(string newLanguage)
    {
        if (newLanguage == "pt")
        {
            jsonDictionary = jsonDictionaries[0];
        }
        else if (newLanguage == "en")
        {
            jsonDictionary = jsonDictionaries[1];
        }
    }
}

public class SaveLanguageData
{
    public string language;
    public string languageBefore;
}
