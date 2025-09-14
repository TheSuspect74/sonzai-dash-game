using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textResult;
    [SerializeField] Button buttonRestart;

    GameManager gameManager;
    
    public void Init(GameManager inGameManager)
    {
        gameManager =  inGameManager;
        buttonRestart.onClick.AddListener(OnClickRestart);
    }

    public void Show(bool isWin)
    {
        string result = isWin ? "WIN" : "LOSE";
        textResult.SetText(result);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    void OnClickRestart()
    {
        gameManager.menuPopup.Show();
        Hide();
    }
}
