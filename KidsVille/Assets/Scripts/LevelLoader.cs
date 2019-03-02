using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    public GameObject loadingScreen;
    public Slider slider;
    public GameObject handler;
    bool handlerGone;

    float progress;
    //bool startProgress; // Serve só para testar o andamento da barra.

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadAsynchronously(sceneName));
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = 1 - progress;
            if (!handlerGone && slider.value <= 0.04f)
            {
                handlerGone = true;
                handler.SetActive(false);
            }
            yield return null;
        }
    }

    // PARA TESTAR O ANDAMENTO DA BARRA. Comentar o "IEnumerator LoadAsynchronously(string sceneName)" acima.
    /*private void Update()
    {
        if (startProgress)
        {
            progress += Time.deltaTime / 2f;
            print(progress);
        }
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        startProgress = true;
        loadingScreen.SetActive(true);

        while (progress < 1f)
        {

            slider.value = 1 - progress;
            if (!handlerGone && slider.value <= 0.04f)
            {
                handlerGone = true;
                handler.SetActive(false);
            }
            yield return null;
        }
    }*/

}
