using UnityEngine;
using UnityEngine.UI;

public class FontAdjustments : MonoBehaviour
{
    [SerializeField] private Text longerText;
    [SerializeField] private Text[] otherTexts;
    private int fontSize;

    private void Start()
    {
        string textVal = longerText.text.ToString();
        longerText.cachedTextGenerator.Invalidate();
        Vector2 size = (longerText.transform as RectTransform).rect.size;
        TextGenerationSettings tempSettings = longerText.GetGenerationSettings(size);
        tempSettings.scaleFactor = 1;//dont know why but if I dont set it to 1 it returns a font that is to small.
        if (!longerText.cachedTextGenerator.Populate(textVal, tempSettings))
            Debug.LogError("Failed to generate fit size");
        fontSize = longerText.resizeTextMaxSize = longerText.cachedTextGenerator.fontSizeUsedForBestFit;
        

        foreach (Text t in otherTexts)
        {
            t.resizeTextForBestFit = false;
            t.fontSize = fontSize;
            //print("Font Size set to " + fontSize);
        }
    }
}
