using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Dictionary<string, string> gameConfigs = new Dictionary<string, string>();

    public Slider volumeSlider;
    public GameObject Menu;
    public GameObject Options;

    public AudioManager rootAudioManager;

    void Awake()
    {
        print("Iniciando a leitura do arquivo de configuração do jogo.");
        if (!File.Exists(Application.persistentDataPath + "/configs.txt"))
            SetDefaultValues();
        else
        {
            string configurationFileText = File.ReadAllText(Application.persistentDataPath + "/configs.txt"); // Reading the whole configuration file
            string[] configs = configurationFileText.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string config in configs)
            {
                string[] configKeyValuePair = config.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                gameConfigs.Add(configKeyValuePair[0], configKeyValuePair[1]);
            }
        }
        SetGameVolume();
        volumeSlider.onValueChanged.AddListener(delegate { VolumeChangeCheck();});
        if (AudioManager._instance == null)
            rootAudioManager.gameObject.SetActive(true);
    }

    public void VolumeChangeCheck()
    {
        ScenesHelper.VolumeScalar = volumeSlider.value;
        AudioManager._instance.UpdateVolume();
    }

    void SetGameVolume()
    {
        print("Volume " + gameConfigs["volume"]);
        ScenesHelper.VolumeScalar = float.Parse(gameConfigs["volume"]);
        volumeSlider.value = float.Parse(gameConfigs["volume"]);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("EpisodesScene");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (Options.activeInHierarchy)
                CancelOptions();
            else
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call<bool>("moveTaskToBack", true);
            }
        }
    }

    public void CancelOptions()
    {
        gameConfigs["volume"] = volumeSlider.value.ToString();
        WriteConfigurationFile();
        ScenesHelper.VolumeScalar = float.Parse(gameConfigs["volume"]);
        Options.SetActive(false);
        Menu.SetActive(true);
    }

    void WriteConfigurationFile()
    {
        var sr = File.CreateText(Application.persistentDataPath + "/configs.txt");
        foreach (var pair in gameConfigs)
        {
            sr.WriteLine("{0}={1}", pair.Key, pair.Value);
        }
        sr.Close();
    }

    void SetDefaultValues()
    {
        print("Setei default");
        gameConfigs.Add("volume", "1.0");
        WriteConfigurationFile();
    }

    public void OpenFacebookPage()
    {
        Application.OpenURL("http://www.facebook.com");
    }

    public void OpenTwitterPage()
    {
        Application.OpenURL("http://www.twitter.com");
    }
}
