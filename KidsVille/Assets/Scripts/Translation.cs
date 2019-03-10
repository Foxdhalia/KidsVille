using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Translation : MainScript
{
    public static Translation instance; 

    string path;
    private Dictionary<string, string> localizedText;

    [Header("GameObjects de textos a serem modificados conforme o idioma:")]
    public GameObject[] gameTexts;

    [Header("Acentos:")]
    [SerializeField] private GameObject[] accent;

    [Header("Imagens com textos:")]
    [SerializeField] private ImagesWithText[] uiImages;
    int completeFileSearch; // 1 para caso seja encontrado, -1 para caso não seja encontrado.

    private void Awake()
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

       

#if UNITY_ANDROID
        path = "jar:file://" + Application.dataPath + "!/assets/";
#else
                         path = Application.dataPath + "/StreamingAssets";
#endif


       
    }

    public IEnumerator Translations(string fileName)
    {
        StartCoroutine(ReadJsonArchive(fileName));
        yield return new WaitUntil(() => completeFileSearch != 0);

        if (completeFileSearch == 1)
        {
            foreach (GameObject g in gameTexts)
            {
                if (g.GetComponent<TextMeshProUGUI>())
                {
                    string s = GetLocalizedValue(g.GetComponent<TextMeshProUGUI>().text);
                    g.GetComponent<TextMeshProUGUI>().text = s;
                }
                else if (g.GetComponent<Text>())
                {
                    string s = GetLocalizedValue(g.GetComponent<Text>().text);
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
    IEnumerator ReadJsonArchive(string fileName)
    {
        localizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath + "/", fileName);

        string dataAsJson;

        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();

            dataAsJson = www.downloadHandler.text;
        }
        else
        {
            dataAsJson = File.ReadAllText(filePath);
        }

        if (dataAsJson != "")
        {
            LanguageData language = JsonUtility.FromJson<LanguageData>(dataAsJson);

            for (int i = 0; i < language.items.Length; i++)
            {
                localizedText.Add(language.items[i].ptKey, language.items[i].translation);
                print(i + ": " + localizedText[language.items[i].ptKey]);
            }
            Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
            completeFileSearch = 1;
        }
        else
        {
            Debug.LogError("Cannot find file at " + filePath + ".");
            completeFileSearch = -1;
        }

    }

    // Encontra a palavra a ser traduzida:
    string GetLocalizedValue(string key)
    {
        string result = "ptKey was not found.";

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

    public void Chang_UIImages()
    {
        foreach(ImagesWithText iwt in uiImages)
        {
            iwt.ptImg.SetActive(false);
            iwt.enImg.SetActive(true);
        }
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
    public string ptKey;
    public string translation;

}

[Serializable]
public class ImagesWithText
{
    public GameObject ptImg;
    public GameObject enImg;

}
