using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogEpisodesList : MonoBehaviour
{

    private int currentButtonIndex = 1;
    private int currentPage = 0;
    private const int MaximumExistingLevel = 16;
    public Button[] episodesButton;

    public Sprite unlockedEpisodeSprite;
    public Sprite lockedEpisodeSprite;
    public Sprite nextEpisodePageSprite;
    public Sprite previousEpisodePageSprite;

    public EpisodesMenu episodesMenu;

    void Start()
    {
        NextPage();
    }

    public void NextPage()
    {
        for (int i = 0; i < episodesButton.Length; i++)
        {
            TMP_Text buttonNumberText = i < 7 ? episodesButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>() : null;
            Image imageComponent = episodesButton[i].GetComponent<Image>();
            if (currentButtonIndex > MaximumExistingLevel)
            {
                if (buttonNumberText != null)
                {
                    buttonNumberText.gameObject.SetActive(false);
                }
                imageComponent.gameObject.SetActive(false);

                SetOnClickButton(i, null);

                currentButtonIndex++;
            }
            else if (currentButtonIndex > 1 && i == 0)
            {
                ShowPreviousPageButton(i, buttonNumberText, imageComponent);
            }
            else if (currentButtonIndex < MaximumExistingLevel && i == 7)
            {
                ShowNextPageButton(i, imageComponent);
            }
            else if (currentButtonIndex > SaveVariables.MaximumLevelUnlocked)
            {
                ShowLockedEpisodes(i, buttonNumberText, imageComponent);
            }
            else
            {
                ShowUnlockedEpisodes(i, buttonNumberText, imageComponent);
            }
        }
        print("Current Button index" + currentButtonIndex);
        currentPage++;
        print("Current PAGE" + currentPage);
    }

    public void PreviousPage()
    {
        currentPage--;
        currentButtonIndex -= 13;
        print("Current page after previous click" + currentPage);
        for (int i = currentPage == 1 ? 0 : 1; i < episodesButton.Length; i++)
        {
            TMP_Text buttonNumberText = i < 7 ? episodesButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>() : null;
            Image imageComponent = episodesButton[i].GetComponent<Image>();
            if (currentButtonIndex < MaximumExistingLevel && i == 7)
            {
                ShowNextPageButton(i, imageComponent);
            }
            else if (currentButtonIndex > SaveVariables.MaximumLevelUnlocked)
            {
                ShowLockedEpisodes(i, buttonNumberText, imageComponent);
            }
            else
            {
                ShowUnlockedEpisodes(i, buttonNumberText, imageComponent);
            }
        }
        print("Current Button index" + currentButtonIndex);
    }

    private void ShowUnlockedEpisodes(int i, TMP_Text buttonNumberText, Image imageComponent)
    {
        buttonNumberText.gameObject.SetActive(true);
        buttonNumberText.text = currentButtonIndex.ToString();

        imageComponent.gameObject.SetActive(true);
        imageComponent.sprite = unlockedEpisodeSprite;

        int auxIndex = currentButtonIndex;
        SetOnClickButton(i, () => episodesMenu.StartEpisode(auxIndex));

        currentButtonIndex++;
    }

    private void ShowLockedEpisodes(int i, TMP_Text buttonNumberText, Image imageComponent)
    {
        buttonNumberText.gameObject.SetActive(false);

        imageComponent.gameObject.SetActive(true);
        imageComponent.sprite = lockedEpisodeSprite;

        SetOnClickButton(i, null);

        currentButtonIndex++;
    }

    private void ShowPreviousPageButton(int i, TMP_Text buttonNumberText, Image imageComponent)
    {
        buttonNumberText.gameObject.SetActive(false);
        imageComponent.sprite = previousEpisodePageSprite;

        SetOnClickButton(i, () => PreviousPage());
    }

    private void ShowNextPageButton(int i, Image imageComponent)
    {
        imageComponent.gameObject.SetActive(true);
        imageComponent.sprite = nextEpisodePageSprite;

        SetOnClickButton(i, () => NextPage());
    }

    private void SetOnClickButton(int i, Action acao)
    {
        episodesButton[i].onClick.RemoveAllListeners();
        if (acao != null)
        {
            episodesButton[i].onClick.AddListener(() => acao());
        }
    }
}
