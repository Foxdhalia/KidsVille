using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public static bool finishGame;
    public static bool timePassing; // Para gerenciar se a contagem dos dias está correndo ou não.
    public static string lang;
    public static string langBefore;
    protected static string languageDataPath;
    protected static int month = 1, day = 1, year = 0; // autoexplicativo... 


    private void Awake()
    {
        timePassing = false;        
    }

    public string GetLanguageDataPath()
    {
        languageDataPath = Application.persistentDataPath + "/languageKV.dat";
        return languageDataPath;
    }

    public void SetTimePassing(bool isTimePassing)
    {
        timePassing = isTimePassing;
    }
}