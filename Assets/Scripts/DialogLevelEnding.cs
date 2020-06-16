using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogLevelEnding : MonoBehaviour
{

    public Button[] buttonsToDisable;
    public int Stars { get; set; }
    public int Score { get; set; }
    public Image firstStar;
    public Image secondStar;
    public Image thirdStar;
    public Sprite filledStar;
    public TMP_Text scoreText;
    public GameGrid gameGrid;

    public Slider loadingProgressSlider;
    public GameObject loadingScreen;

    public int CurrentEpisode;
    public bool LevelCompleted { get; set; }

    void OnEnable()
    {
        var gameGridLocation = gameGrid.transform.position;
        gameGrid.transform.position = new Vector3(gameGridLocation.x, gameGridLocation.y, -1000.0f);

        if (LevelCompleted)
        {
            RegisterScoreForEpisode();
            if (SaveVariables.MaximumLevelUnlocked < CurrentEpisode + 1 && CurrentEpisode + 1 < 17)
            {
                SaveVariables.MaximumLevelUnlocked = CurrentEpisode + 1;
                PlayGamesScript.Instance.SaveData();
            }
        }
        foreach (Button button in buttonsToDisable)
        {
            button.enabled = false;
        }
    }

    void OnDisable()
    {
        var gameGridLocation = gameGrid.transform.position;
        gameGrid.transform.position = new Vector3(gameGridLocation.x, gameGridLocation.y, 0.0f);

        foreach (Button button in buttonsToDisable)
        {
            button.enabled = true;
        }
    }

    public void UpdateInformation()
    {
        switch (Stars)
        {
            case 1: firstStar.sprite = filledStar; break;
            case 2:
                firstStar.sprite = filledStar;
                secondStar.sprite = filledStar; break;
            case 3:
                firstStar.sprite = filledStar;
                secondStar.sprite = filledStar;
                thirdStar.sprite = filledStar; break;
        }
        scoreText.text = Score.ToString();
    }

    public void OpenHome()
    {
        gameGrid.UnsubscribeEvent();
        ScenesHelper.ClearParameters();
        loadingScreen.SetActive(true);
        StartCoroutine(LoadNewScene("MainMenuScene"));
    }

    public void OpenEpisodes()
    {
        gameGrid.UnsubscribeEvent();
        ScenesHelper.ClearParameters();
        loadingScreen.SetActive(true);
        StartCoroutine(LoadNewScene("EpisodesScene"));
    }

    public void OpenNextEpisode()
    {
        gameGrid.UnsubscribeEvent();
        int t = (int)(ScenesHelper.GetParameter("episodeNumber"));
        ScenesHelper.ClearParameters();
        loadingScreen.SetActive(true);
        ScenesHelper.SetParameter("episodeNumber", t + 1);
        StartCoroutine(LoadNewScene("EpisodeScene" + (t + 1)));
    }

    public void RestartEpisode()
    {
        gameGrid.UnsubscribeEvent();
        int t = (int)(ScenesHelper.GetParameter("episodeNumber"));
        ScenesHelper.ClearParameters();
        loadingScreen.SetActive(true);
        ScenesHelper.SetParameter("episodeNumber", t);
        StartCoroutine(LoadNewScene("EpisodeScene" + t));
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
