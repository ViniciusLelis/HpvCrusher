using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogSecondChance : MonoBehaviour
{

    public Button[] buttonsToDisable;
    public GameGrid gameGrid;

    public Slider loadingProgressSlider;
    public GameObject loadingScreen;

    public int Score;

    public int CurrentEpisode;

    void OnEnable()
    {
        var gameGridLocation = gameGrid.transform.position;
        gameGrid.transform.position = new Vector3(gameGridLocation.x, gameGridLocation.y, -1000.0f);

        foreach (Button button in buttonsToDisable)
            button.enabled = false;
    }

    void OnDisable()
    {
        var gameGridLocation = gameGrid.transform.position;
        gameGrid.transform.position = new Vector3(gameGridLocation.x, gameGridLocation.y, 0.0f);

        foreach (Button button in buttonsToDisable)
            button.enabled = true;
    }

    public void CorrectChoiceSecondChance()
    {
        print("Chamou correct");
        gameGrid.AllowSecondChance();
        gameObject.SetActive(false);
    }

    public void WrongChoiceSecondChance()
    {
        gameGrid.UnsubscribeEvent();
        int t = (int)(ScenesHelper.GetParameter("episodeNumber"));
        ScenesHelper.ClearParameters();
        loadingScreen.SetActive(true);
        ScenesHelper.SetParameter("episodeNumber", t);
        StartCoroutine(LoadNewScene("EpisodeScene" + t));
    }

    public void OpenHome()
    {
        RegisterScoreForEpisode();
        gameGrid.UnsubscribeEvent();
        ScenesHelper.ClearParameters();
        loadingScreen.SetActive(true);
        StartCoroutine(LoadNewScene("MainMenuScene"));
    }

    public void OpenEpisodes()
    {
        RegisterScoreForEpisode();
        gameGrid.UnsubscribeEvent();
        ScenesHelper.ClearParameters();
        loadingScreen.SetActive(true);
        StartCoroutine(LoadNewScene("EpisodesScene"));
    }

    IEnumerator LoadNewScene(string scene)
    {
        yield return new WaitForSeconds(0.8f);
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

    private void RegisterScoreForEpisode()
    {
        string leaderboardId = LeaderboardHelper.GetLeaderboardIdForEpisode(CurrentEpisode);
        if (!string.IsNullOrEmpty(leaderboardId))
        {
            PlayGamesScript.RegisterLeaderboardScore(leaderboardId, Score);
        }
    }
}
