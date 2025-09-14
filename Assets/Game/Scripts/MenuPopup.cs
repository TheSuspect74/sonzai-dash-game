using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuPopup : MonoBehaviour
{
    [SerializeField] GameObject objLeaderboard;
    [SerializeField] GameObject objInputName;

    [SerializeField] TMP_InputField inputName;
    [SerializeField] Button buttonSubmitName;
    [SerializeField] Button buttonPlay;
    [SerializeField] Button buttonLeaderboard;
    [SerializeField] Button buttonSettings;
    [SerializeField] Button buttonCloseLeaderboard;

    [SerializeField] RectTransform rootLeaderboardContainer;
    [SerializeField] LeaderboardItemUI prefabLeaderboardItem;
    
    GameManager gameManager;

    List<LeaderboardItemUI> leaderboardItems;
    
    public void Init(GameManager inGameManager)
    {
        gameManager = inGameManager;

        leaderboardItems = new List<LeaderboardItemUI>();
        
        buttonSubmitName.onClick.AddListener(OnClickSubmitName);
        buttonPlay.onClick.AddListener(OnClickPlay);
        buttonLeaderboard.onClick.AddListener(OnClickLeaderboard);
        buttonCloseLeaderboard.onClick.AddListener(OnClickCloseleaderboard);
        buttonSettings.onClick.AddListener(OnClickSettings);

        objLeaderboard.gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        
        if (gameManager.firstTimePlay)
        {
            objInputName.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void OnClickSubmitName()
    {
        string name = inputName.text;
        gameManager.userData.playerName = name;
        objInputName.gameObject.SetActive(false);

        gameManager.firstTimePlay = false;
    }
    
    void OnClickPlay()
    {
        gameManager.Reset();
        Hide();
    }

    void OnClickLeaderboard()
    {
        int count = leaderboardItems.Count;
        for (int i = 0; i < count; i++)
        {
            Destroy(leaderboardItems[i].gameObject);
        }
        leaderboardItems.Clear();
        count = gameManager.userData.leaderboardDatas.Count;
        for (int i = 0; i < count; i++)
        {
            LeaderboardData data = gameManager.userData.leaderboardDatas[i];
            LeaderboardItemUI item = Instantiate(prefabLeaderboardItem, rootLeaderboardContainer);
            int no = i + 1;
            item.SetItem(no, data);
            leaderboardItems.Add(item);
        }
        
        objLeaderboard.SetActive(true);
    }

    void OnClickCloseleaderboard()
    {
        objLeaderboard.SetActive(false);
    }
    
    void OnClickSettings()
    {
        
    }
}
