using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EpisodesFileHandler : MonoBehaviour {

    /* Episodes configuration file */
    public TextAsset episodesFile;

    /* Information required to initialize the GameGrid */
    public int MDimension { get; private set; }
    public int NDimension { get; private set; }
    public int RequiredScore { get; private set; }
    public float MaximumTime { get; private set; }

    /* Instances to set */
    public GameGrid gameGrid;
    public TMP_Text scoreText;

    // Use this for initialization
    void Start () {
        print("Iniciando a leitura do arquivo de configuração de episódios.");
        string episodesFileText = episodesFile.text; // Reading the whole episodes file
        var episodeNumber = (int)ScenesHelper.GetParameter("episodeNumber"); // Getting the episode number
        string[] episodesInformation = episodesFileText.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); // Splitting by each episode information
        string[] specificEpisodeInformation = episodesInformation[episodeNumber - 1].Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); // Getting each information on the specific episode
        MDimension = ParseMDimension(specificEpisodeInformation);
        NDimension = ParseNDimension(specificEpisodeInformation);
        RequiredScore = ParseRequiredScore(specificEpisodeInformation);
        MaximumTime = ParseMaximumTime(specificEpisodeInformation);

        gameGrid.MDimension = MDimension;
        gameGrid.NDimension = NDimension;
        gameGrid.requiredScore = RequiredScore;
        gameGrid.maximumTime = MaximumTime;

        scoreText.text = RequiredScore.ToString();

        string teste = String.Format("{0} {1} {2} {3}", MDimension, NDimension, RequiredScore, MaximumTime);
        print(teste);
	}

    public int ParseMDimension(string[] episodeInformation)
    {
        return int.Parse(episodeInformation[0].Split('x')[0]);
    }

    public int ParseNDimension(string[] episodeInformation)
    {
        return int.Parse(episodeInformation[0].Split('x')[1]);
    }

    public int ParseRequiredScore(string[] episodeInformation)
    {
        return int.Parse(episodeInformation[1]);
    }

    public float ParseMaximumTime(string[] episodeInformation)
    {
        return float.Parse(episodeInformation[2]);
    }

}
