using TMPro;
using UnityEngine;

public class LeaderboardItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textNo;
    [SerializeField] TextMeshProUGUI textName;
    [SerializeField] TextMeshProUGUI textScore;

    public void SetItem(int no, LeaderboardData data)
    {
        string noString = $"{no}.";

        int min = data.score / 60;
        int sec = data.score % 60;
        string scoreString = $"{min:D2}:{sec:D2}";
        
        textNo.SetText(noString);
        textName.SetText(data.name);
        textScore.SetText(scoreString);
    }
}
