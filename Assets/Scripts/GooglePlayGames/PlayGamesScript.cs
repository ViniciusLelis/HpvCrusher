using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Linq;
using UnityEngine;

public class PlayGamesScript : MonoBehaviour
{

    internal static class SaveConstants
    {
        public const string KEY_SAVE_MAXIMUM_LVL_UNLOCKED = "HpvCrusher";
        public const string KEY_FIRST_TIME_PLAYING = "IsFirstTime";
    }

    private static PlayGamesScript _instance;

    public static PlayGamesScript Instance
    {
        get { return _instance; }
    }

    private bool isSaving = false;
    private bool isSaveLoaded = false;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            if (!PlayerPrefs.HasKey(SaveConstants.KEY_SAVE_MAXIMUM_LVL_UNLOCKED))
            {
                PlayerPrefs.SetInt(SaveConstants.KEY_SAVE_MAXIMUM_LVL_UNLOCKED, 1);
            }
            if (!PlayerPrefs.HasKey(SaveConstants.KEY_FIRST_TIME_PLAYING))
            {
                PlayerPrefs.SetInt(SaveConstants.KEY_FIRST_TIME_PLAYING, 1);
            }
            LoadLocalData();

            InitializePlayGamesPlatform();

            SignIn();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    #region

    private void InitializePlayGamesPlatform()
    {
        PlayGamesClientConfiguration gamesClientConfiguration = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();
        PlayGamesPlatform.InitializeInstance(gamesClientConfiguration);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    #endregion

    #region Saved games

    void UpdateMaximumLevelUnlocked(int cloudData, int localData)
    {
        if (PlayerPrefs.GetInt(SaveConstants.KEY_FIRST_TIME_PLAYING) == 1)
        {
            PlayerPrefs.SetInt(SaveConstants.KEY_FIRST_TIME_PLAYING, 0);
            if (cloudData > localData)
            {
                PlayerPrefs.SetInt(SaveConstants.KEY_SAVE_MAXIMUM_LVL_UNLOCKED, cloudData);
            }
        }
        else
        {
            if (localData > cloudData)
            {
                SaveVariables.MaximumLevelUnlocked = localData;
                isSaveLoaded = true;
                SaveData();
                return;
            }
        }
        SaveVariables.MaximumLevelUnlocked = cloudData;
        isSaveLoaded = true;
    }

    public void SaveData()
    {
        if (!isSaveLoaded)
        {
            SaveLocalData();
        }

        if (Social.localUser.authenticated)
        {
            isSaving = true;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithManualConflictResolution(
                SaveConstants.KEY_SAVE_MAXIMUM_LVL_UNLOCKED,
                DataSource.ReadCacheOrNetwork,
                true,
                ResolveSaveConflict,
                OnSavedGameOpened);
        }
        else
        {
            SaveLocalData();
        }
    }

    public void LoadSaveData()
    {
        if (Social.localUser.authenticated)
        {
            isSaving = false;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithManualConflictResolution(
                SaveConstants.KEY_SAVE_MAXIMUM_LVL_UNLOCKED,
                DataSource.ReadCacheOrNetwork,
                true,
                ResolveSaveConflict,
                OnSavedGameOpened);
        }
        else
        {
            LoadLocalData();
        }
    }

    private void LoadLocalData()
    {
        SaveVariables.MaximumLevelUnlocked = PlayerPrefs.GetInt(SaveConstants.KEY_SAVE_MAXIMUM_LVL_UNLOCKED);
    }

    private void SaveLocalData()
    {
        PlayerPrefs.SetInt(SaveConstants.KEY_SAVE_MAXIMUM_LVL_UNLOCKED, SaveVariables.MaximumLevelUnlocked);
    }

    private void ResolveSaveConflict(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData)
    {
        if (originalData == null || !originalData.Any())
        {
            resolver.ChooseMetadata(unmerged);
        }
        else if (unmergedData == null || !unmergedData.Any())
        {
            resolver.ChooseMetadata(original);
        }
        else
        {
            var originalMaxLvl = BitConverter.ToInt32(originalData, 0);
            var unmergedMaxLvl = BitConverter.ToInt32(unmergedData, 0);
            if (originalMaxLvl >= unmergedMaxLvl)
            {
                resolver.ChooseMetadata(original);
            }
            else
            {
                resolver.ChooseMetadata(unmerged);
            }
        }
    }

    private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            if (!isSaving)
            {
                LoadGame(game);
            }
            else
            {
                SaveGame(game);
            }
        }
        else
        {
            if (!isSaving)
            {
                LoadLocalData();
            }
            else
            {
                SaveLocalData();
            }
        }
    }

    private void LoadGame(ISavedGameMetadata game)
    {
        ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, OnSaveGameDataRead);
    }

    private void SaveGame(ISavedGameMetadata game)
    {
        SaveLocalData();
        byte[] dataToSave = BitConverter.GetBytes(SaveVariables.MaximumLevelUnlocked);

        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();

        ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, update, dataToSave, (writeStatus, writeGame) =>
        {
            if (writeStatus == SavedGameRequestStatus.Success)
            {

            }
        });
    }

    private void OnSaveGameDataRead(SavedGameRequestStatus status, byte[] savedData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            int cloudData = 1;
            if (savedData != null && savedData.Any())
            {
                cloudData = BitConverter.ToInt32(savedData, 0);
            }

            int localData = PlayerPrefs.GetInt(SaveConstants.KEY_SAVE_MAXIMUM_LVL_UNLOCKED);
            UpdateMaximumLevelUnlocked(cloudData, localData);
        }
    }

    #endregion

    #region Sign In

    void SignIn()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            LoadSaveData();
        });
    }

    string GetPlayerGameId()
    {
        return ((PlayGamesLocalUser)Social.localUser).gameId;
    }

    #endregion

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
