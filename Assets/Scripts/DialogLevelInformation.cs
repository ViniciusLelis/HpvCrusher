using UnityEngine;
using UnityEngine.UI;

public class DialogLevelInformation : MonoBehaviour {

    public Button[] buttonsToDisable;
    public int CurrentEpisode;

    void OnEnable()
    {
        foreach (Button button in buttonsToDisable)
            button.enabled = false;
    }

    void OnDisable()
    {
        foreach (Button button in buttonsToDisable)
            button.enabled = true;
    }

    public void OpenWebsite()
    {
        Application.OpenURL("https://www.inca.gov.br/perguntas-frequentes/hpv");
    }

    public void ShowLeaderboardUi()
    {
        string leaderboardId = LeaderboardHelper.GetLeaderboardIdForEpisode(CurrentEpisode);
        if (!string.IsNullOrEmpty(leaderboardId))
        {
            PlayGamesScript.ShowLeaderboardUi(leaderboardId);
        }
    }
}
