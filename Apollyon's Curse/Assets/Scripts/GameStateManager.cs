using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour {

    public Image fadeImage;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += FadeInScene;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= FadeInScene;
    }

    void FadeInScene(Scene scene, LoadSceneMode mode)
    {
        fadeImage.enabled = true;
        fadeImage.CrossFadeAlpha(0f, 40.0f, false);
        fadeImage.enabled = false;
    }
}
