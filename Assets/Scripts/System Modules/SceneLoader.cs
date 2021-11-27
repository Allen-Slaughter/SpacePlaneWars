using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : PersistentSingleton<SceneLoader>
{
    [SerializeField] Image transitionImage;
    [SerializeField] float fadeTime = 3.5f;

    Color color;

    const string GAMEPLAY = "Gameplay";

    IEnumerator LoadCoroutine(string sceneName)
    {
        //Load new scene in background
        var loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        //Set this scene inactive
        loadingOperation.allowSceneActivation = false;

        transitionImage.gameObject.SetActive(true);

        //Fade out
        while (color.a < 1f)
        {
            color.a = Mathf.Clamp01(color.a + Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }

        //Activate this new scene
        loadingOperation.allowSceneActivation = true;

        //Fade in
        while (color.a > 0f)
        {
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }

        transitionImage.gameObject.SetActive(false);
    }

    public void LoadGameplayScene()
    {
        StartCoroutine(LoadCoroutine(GAMEPLAY));
    }
}
