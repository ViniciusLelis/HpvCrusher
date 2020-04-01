using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogEpisodesList : MonoBehaviour
{

    private int lastUnlockedLevel = 9;
    private int currentButtonIndex = 1;
    private int currentPage = 0;
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
            TMP_Text buttonNumberText = episodesButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Image imageComponent = episodesButton[i].GetComponent<Image>();
            if (currentButtonIndex > 1 && i == 0)
            {
                buttonNumberText.gameObject.SetActive(false);
                imageComponent.sprite = previousEpisodePageSprite;
                episodesButton[i].onClick.RemoveAllListeners();
                episodesButton[i].onClick.AddListener(() => PreviousPage());
            }
            else if (currentButtonIndex > lastUnlockedLevel)
            {
                buttonNumberText.gameObject.SetActive(false);
                imageComponent.sprite = lockedEpisodeSprite;
                episodesButton[i].onClick.RemoveAllListeners();
                currentButtonIndex++;
            }
            else
            {
                buttonNumberText.gameObject.SetActive(true);
                buttonNumberText.text = currentButtonIndex.ToString();
                imageComponent.sprite = unlockedEpisodeSprite;
                episodesButton[i].onClick.RemoveAllListeners();
                int auxIndex = currentButtonIndex;
                episodesButton[i].onClick.AddListener(() => episodesMenu.StartEpisode(auxIndex));
                episodesButton[i].interactable = true;
                currentButtonIndex++;
            }
        }
        print("Current Button index" + currentButtonIndex);
        currentPage++;
        print("Current PAGE" + currentPage);

    }

    public void PreviousPage()
    {
        currentPage--;
        currentButtonIndex -= currentPage == 1 ? 13 : 12;
        print("Current page after previous click" + currentPage);
        for(int i=currentPage == 1 ? 0 : 1; i<episodesButton.Length; i++)
        {
            TMP_Text buttonNumberText = episodesButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Image imageComponent = episodesButton[i].GetComponent<Image>();
            if (currentButtonIndex > lastUnlockedLevel)
            {
                buttonNumberText.gameObject.SetActive(false);
                imageComponent.sprite = lockedEpisodeSprite;
                episodesButton[i].onClick.RemoveAllListeners();
                currentButtonIndex++;
            }
            else
            {
                buttonNumberText.gameObject.SetActive(true);
                buttonNumberText.text = currentButtonIndex.ToString();
                imageComponent.sprite = unlockedEpisodeSprite;
                episodesButton[i].onClick.RemoveAllListeners();
                int auxIndex = currentButtonIndex;
                episodesButton[i].onClick.AddListener(() => episodesMenu.StartEpisode(auxIndex));
                episodesButton[i].interactable = true;
                currentButtonIndex++;
            }
        }
        print("Current Button index" + currentButtonIndex);

    }

}
