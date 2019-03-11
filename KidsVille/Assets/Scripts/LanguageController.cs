using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LanguageController : MainScript
{
    LanguageController instance;
    protected string path;

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        path = Application.persistentDataPath + "/languageKV.dat";
    }

    /*private void Start()
    {
        path = Application.persistentDataPath + "/languageKV.dat";
        if (lang == "" && !File.Exists(path))
        {
            SaveData("pt");           
        }
        else if (File.Exists(path))
        {
            StreamReader sr = new StreamReader(path);
            SaveLanguageData data = new SaveLanguageData();
            data = JsonUtility.FromJson<SaveLanguageData>(sr.ReadLine());
            sr.Close();

            lang = data.language;
            

            print("File loaded from " + path);

            if (lang == "pt")
            {
                timePassing = true;
                langBefore = data.languageBefore;
                print(lang + " " + langBefore);
                return;
            }
            else
            {
                StartTranslation(lang);
            }
        }
    }*/

    public void StartTranslation(string newLanguage)
    {
        timePassing = false;
        lang = newLanguage;

        if (newLanguage == "pt")
        {
            Translation translation = FindObjectOfType<Translation>();
            StartCoroutine(translation.Translations("pt_Translation.json"));
            translation.Enable_ptAccents();
            translation.Chang_UIImages();
        }
        else if (newLanguage == "en")
        {
            Translation translation = FindObjectOfType<Translation>();
            StartCoroutine(translation.Translations("en_Translation.json"));
            translation.Disable_ptAccents();
            translation.Chang_UIImages();
        }

        if (lang != langBefore)
        {
            SaveData(newLanguage);
        }

        timePassing = true;
    }

    public void SetLanguage(string newLanguage)
    {
        lang = newLanguage;
    }

    public bool CheckForLanguageChanges(string newLanguage)
    {
        if (newLanguage == langBefore)
            return false;
        else
            return true;
    }

    public void SaveData(string newLanguage)
    {
        SaveLanguageData data = new SaveLanguageData();
        data.language = newLanguage;
        data.languageBefore = newLanguage;

        lang = newLanguage;
        langBefore = newLanguage;

        // FileStream file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamWriter sw = new StreamWriter(path);

        sw.WriteLine(JsonUtility.ToJson(data));

        sw.Close();

        print("File saved at " + path);
    }
}

public class SaveLanguageData
{
    public string language;
    public string languageBefore;
}
