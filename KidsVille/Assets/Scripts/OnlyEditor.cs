using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class OnlyEditor : MonoBehaviour
{
    [SerializeField] private Translation translation;
    private void Start()
    {
        Destroy(gameObject);
    }

    public void SearchLanguageItens()
    {
        translation.gameTexts = GameObject.FindGameObjectsWithTag("Language");
    }
}

#if UNITY_EDITOR
// Inspector custom settings:
[CustomEditor(typeof(OnlyEditor))]
public class OnlyEditor_Editor : Editor
{
    public OnlyEditor script;

    public void OnEnable()
    {
        script = (OnlyEditor)target;
    }

    public override void OnInspectorGUI()
    {
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Search for Language Itens (tagged 'Language')"))
        {
            script.SearchLanguageItens();
        }

        GUI.backgroundColor = Color.white;
        base.OnInspectorGUI();
    }
}
#endif