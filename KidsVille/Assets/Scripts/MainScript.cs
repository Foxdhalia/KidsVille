using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public static bool finishGame;
    public static bool timePassing; // Para gerenciar se a contagem dos dias está correndo ou não.
    public static string lang;
    protected static int month = 1, day = 1, year = 0; // autoexplicativo... 

    [SerializeField] private GameObject[] gameTexts;

    private void Awake()
    {
        timePassing = false;

        /* if(lang == "pt")
         {
             return;
         }
         else if(lang == "en")
         {

         }*/


        //Debug.Log("Language objects found: " + go.Length);
    }

    void DivideAndTranslate()
    {
        foreach (GameObject g in gameTexts)
        {
            if (g.GetComponent<Text>())
            {
                Translate(g.GetComponent<Text>().text);
            }
            else if (g.GetComponent<TextMeshProUGUI>())
            {
                Translate(g.GetComponent<TextMeshProUGUI>().text);
            }
        }
    }

    void Translate(string word)
    {

    }

    public void SetTimePassing(bool isTimePassing)
    {
        timePassing = isTimePassing;
    }





}
