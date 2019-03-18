using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Translation : MainScript
{
    LanguageController lc;

    private Dictionary<string, string> localizedText;
    private string pathDictionarie;

    [Header("GameObjects de textos a serem modificados conforme o idioma:")]
    public GameObject[] gameTexts;

    [Header("Acentos:")]
    [SerializeField] private GameObject[] accent;

    [Header("Imagens com textos:")]
    [SerializeField] private ImagesWithText[] uiImages;
    int completeFileSearch; // 1 para caso seja encontrado, -1 para caso não seja encontrado.


    public Text tx;

    private void Start()
    {
        languageDataPath = Application.persistentDataPath + "/languageKV.dat";
        tx.text = languageDataPath + "\n";

        if (SceneManager.GetActiveScene().name == "MenuScene")
        {
            //...
        }
        else
        {
            tx.text = "lang: " + lang + ", langBefore: " + langBefore;
            if (lang != "pt")
            {
                StartTranslation(lang);
            }
        }
    }

    public void StartMenuScene()
    {
        languageDataPath = Application.persistentDataPath + "/languageKV.dat";
        if (SceneManager.GetActiveScene().name == "MenuScene")
        {
            if (!File.Exists(languageDataPath))
            {
                SaveData("pt");
                print("Create file for storage language values. Default value = 'pt'.");
                lang = "pt";
                langBefore = "pt";

                tx.text += "File NOT exists. " + "lang: " + lang + ", langBefore: " + langBefore + "\n";
            }
            else if (File.Exists(languageDataPath))
            {
                StreamReader sr = new StreamReader(languageDataPath);
                SaveLanguageData data = new SaveLanguageData();
                data = JsonUtility.FromJson<SaveLanguageData>(sr.ReadLine());
                sr.Close();

                lang = data.language;
                langBefore = data.languageBefore;

                print("File loaded from " + languageDataPath + " with language " + lang + ".");
                tx.text += "File exist. " + "lang: " + lang + ", langBefore: " + langBefore + "\n";

                if (lang == "pt")
                {
                    timePassing = true;
                    //langBefore = "pt";                
                    return;
                }
                else
                {
                    StartTranslation(lang);
                }
            }
        }
    }

    public void StartTranslation(string newLanguage)
    {
        timePassing = false;
        lc = FindObjectOfType<LanguageController>();
        lang = newLanguage;
        langBefore = newLanguage;
        lc.LoadJsonDictionary(newLanguage);
        tx.text += "Start translation. " + "newLanguage: " + newLanguage;

        if (newLanguage == "pt")
        {
            StartCoroutine(Translations(lc.GetJsonDictionary()));
            Enable_ptAccents();
            Chang_UIImages();
        }
        else if (newLanguage == "en")
        {
            tx.text += " => entrou" + "\n";
           StartCoroutine(Translations(lc.GetJsonDictionary()));
            Disable_ptAccents();
            Chang_UIImages();
        }

        
            SaveData(newLanguage);
        

        tx.text += "New save. " + "lang: " + lang + ", langBefore: " + langBefore + "\n";

        timePassing = true;
    }

    public IEnumerator Translations(string langDictionary)
    {
        ReadJsonArchive(langDictionary);
        yield return new WaitUntil(() => completeFileSearch != 0);

        if (completeFileSearch == 1)
        {
            foreach (GameObject g in gameTexts)
            {
                if (g.GetComponent<TextMeshProUGUI>())
                {
                    KeyData k = g.GetComponent<KeyData>();
                    string s = GetLocalizedValue(k.GetKey());
                    g.GetComponent<TextMeshProUGUI>().text = s;
                }
                else if (g.GetComponent<Text>())
                {
                    KeyData k = g.GetComponent<KeyData>();
                    string s = GetLocalizedValue(k.GetKey());
                    g.GetComponent<Text>().text = s;
                }
            }
        }
        else if (completeFileSearch == -1)
        {
            Debug.Log("Não foi possível fazer a tradução pois o arquivo de idioma " + lang + " não foi encontrado.");
        }

        timePassing = true;
    }

    // Encontra o arquivo de tradução:
    void ReadJsonArchive(string langDictionary)
    {
        localizedText = new Dictionary<string, string>();
        //string filePath = Path.Combine(Application.streamingAssetsPath + "/", fileName);
        //string filePath = Path.Combine("jar:file://" + Application.dataPath + "!/assets/", fileName);

        string dataAsJson = langDictionary;
        if (dataAsJson != "")
        {
            LanguageData language = JsonUtility.FromJson<LanguageData>(dataAsJson);

            for (int i = 0; i < language.items.Length; i++)
            {
                localizedText.Add(language.items[i].key, language.items[i].translation);
                //   print(i + ": " + localizedText[language.items[i].key]);
            }
            Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
            completeFileSearch = 1;
        }
        else
        {
            Debug.LogError("Não foi possível achar/ler o arquivo de tradução.");
            completeFileSearch = -1;
        }

    }

    // Encontra a palavra a ser traduzida:
    string GetLocalizedValue(string key)
    {
        string result = "key was not found.";

        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }
        return result;
    }

    public void Disable_ptAccents()
    {
        foreach (GameObject accentuation in accent)
        {
            accentuation.SetActive(false);
        }
    }
    public void Enable_ptAccents()
    {
        foreach (GameObject accentuation in accent)
        {
            accentuation.SetActive(true);
        }
    }

    public void Chang_UIImages()
    {
        if (lang == "en")
        {
            foreach (ImagesWithText iwt in uiImages)
            {
                iwt.ptImg.SetActive(false);
                iwt.enImg.SetActive(true);
            }
        }
        else if (lang == "pt")
        {
            foreach (ImagesWithText iwt in uiImages)
            {
                iwt.ptImg.SetActive(true);
                iwt.enImg.SetActive(false);
            }
        }
    }

    public string GetLang()
    {
        return lang;
    }

    public void SaveData(string newLanguage)
    {
        SaveLanguageData data = new SaveLanguageData();
        data.language = newLanguage;
        data.languageBefore = newLanguage;

        //lang = newLanguage;
        //langBefore = newLanguage;

        if (!File.Exists(languageDataPath)) // Check if the file need to be created.
        {
            FileStream file = File.Create(languageDataPath);
            file.Close();
        }

        StreamWriter sw = new StreamWriter(languageDataPath);
        sw.WriteLine(JsonUtility.ToJson(data));
        sw.Close();

        print("File saved at " + languageDataPath);
        tx.text += "SaveData. " + "data.language: " + data.language + ", data.languageBefore: " + data.languageBefore + "\n";
    }
}

[Serializable]
public class LanguageData
{
    public WordDictionary[] items;
}

[Serializable]
public class WordDictionary
{
    public string key;
    public string translation;

}

[Serializable]
public class ImagesWithText
{
    public GameObject ptImg;
    public GameObject enImg;

}
