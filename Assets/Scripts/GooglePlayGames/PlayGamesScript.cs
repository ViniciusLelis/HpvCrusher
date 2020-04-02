using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class PlayGamesScript : MonoBehaviour
{

    void Start()
    {
        PlayGamesClientConfiguration gamesClientConfiguration = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();
        PlayGamesPlatform.InitializeInstance(gamesClientConfiguration);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        SignIn();
    }

    void SignIn()
    {
        Social.localUser.Authenticate((bool success) => {});
    }

    string GetPlayerGameId()
    {
        return ((PlayGamesLocalUser)Social.localUser).gameId;
    }

    #region Leaderboard

    public static void RegisterLeaderboardScore(string idLeaderboard, long score)
    {
        Social.ReportScore(score, idLeaderboard, sucess => { });
    }

    public static void ShowLeaderboardUi(string idLeaderboard)
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI(idLeaderboard);
    }

    #endregion

}
