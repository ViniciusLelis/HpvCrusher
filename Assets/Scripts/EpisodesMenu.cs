using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EpisodesMenu : MonoBehaviour
{
    public Slider loadingProgressSlider;
    public GameObject episodesListMenu;
    public GameObject loadingScreen;

    public void Cancel()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void StartEpisode(int episodeIndex)
    {
        episodesListMenu.transform.localScale = new Vector3(0, 0, 0);
        loadingScreen.SetActive(true);
        ScenesHelper.SetParameter("episodeNumber", episodeIndex);
        StartCoroutine(LoadNewScene("EpisodeScene" + episodeIndex));
    }

    IEnumerator LoadNewScene(string scene)
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            loadingProgressSlider.value = asyncOperation.progress;
            if (asyncOperation.progress >= 0.9f)
            {
                loadingProgressSlider.value = loadingProgressSlider.maxValue;
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

}
