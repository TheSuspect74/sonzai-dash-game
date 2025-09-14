using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public DataSO gameData;
    public Player player;
    
    [SerializeField] GameObject prefabCollectible;
    [SerializeField] Transform rootCollectibleContainer;
    [SerializeField] Enemy prefabEnemy;
    [SerializeField] Transform rootEnemySpawn;

    List<GameObject> activeCollectibles;

    [Header("UI")] 
    public MenuPopup menuPopup;
    [SerializeField] TextMeshProUGUI textCollectible;
    [SerializeField] TextMeshProUGUI textTimer;
    [SerializeField] List<Image> imgLives;
    [SerializeField] Image imgDashIcon;
    [SerializeField] ResultPopup resultPopup;
    

    [Header("Runtime")] 
    public Enemy currEnemy;
    public int collectibleCollected;
    public float gameTimer;
    public int lives;

    public UserData userData;

    [HideInInspector] public bool firstTimePlay;
    
    //Debug
    int frameCount = 0;
    float elapsedTime = 0;
    float updateInterval = 0.5f;
    [SerializeField] TextMeshProUGUI textFPS;
    [SerializeField] TextMeshProUGUI textMem;
    [SerializeField] Button buttonAutoWin;
    [SerializeField] Button buttonGodMode;

    bool isGodMode;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Load();

        activeCollectibles = new List<GameObject>();

        buttonAutoWin.onClick.AddListener(OnClickAutoWin);
        buttonGodMode.onClick.AddListener(OnClickGodMode);
        
        player.Init((this));
        menuPopup.Init(this);
        resultPopup.Init(this);

        // Reset();
        
        firstTimePlay = userData.playerName == "";
        Time.timeScale = 0;
        
        menuPopup.Show();
    }

    public void Reset()
    {
        int count = activeCollectibles.Count;
        for (int i = 0; i < count; i++)
        {
            Destroy(activeCollectibles[i]);
        }
        
        activeCollectibles.Clear();
        
        count = gameData.collectibleAmount;
        for (int i = 0; i < count; i++)
        {
            float rndX = Random.Range(-19.5f, 19.5f);
            float rndY = Random.Range(-19.5f, 19.5f);
            
            Vector2 spawnPos =  new Vector2(rndX, rndY);
            GameObject collectible = Instantiate(prefabCollectible, spawnPos, Quaternion.identity);
            activeCollectibles.Add(collectible);
        }
        
        lives = gameData.startingLives;
        collectibleCollected = 0;
        gameTimer = 0;

        player.transform.position = Vector3.zero;

        if (currEnemy != null)
        {
            Destroy(currEnemy.gameObject);
        }
        
        SpawnEnemy();
        
        Time.timeScale = 1;
        
        RefreshUI();
    }
    
    // Update is called once per frame
    void Update()
    {
        float dt =  Time.deltaTime;
        player.DoUpdate(dt);

        gameTimer += dt;
        RefreshTimer();

        float tDashCooldown = player.tDashCooldown;
        float dashCD = gameData.dashCooldon;
        imgDashIcon.fillAmount = 1 - (tDashCooldown / dashCD);
        
        //Debug
        frameCount++;
        elapsedTime += Time.unscaledDeltaTime;
        if (elapsedTime >= updateInterval)
        {
            float fps = frameCount / elapsedTime;
            string fpsString = $"{fps:F1}fps";
            textFPS.SetText(fpsString);
            frameCount = 0;
            elapsedTime = 0f;
        }
        
        long totalMemory = Profiler.GetTotalAllocatedMemoryLong();
        float memoryMB = totalMemory / (1024f * 1024f);
        string memString = $"{memoryMB:F2}MB Alloc";
        textMem.SetText(memString);
    }
    
    void SpawnEnemy()
    {
        currEnemy = Instantiate(prefabEnemy, rootEnemySpawn.position, Quaternion.identity);
        currEnemy.transform.SetParent(rootEnemySpawn);
        currEnemy.Init(this);
    }
    
    public void AddCollectibleCount(GameObject collectible)
    {
        activeCollectibles.Remove(collectible);
        Destroy(collectible);

        collectibleCollected++;
        RefreshCollectibles();

        if (collectibleCollected >= gameData.collectibleAmount)
        {
            //Win
            EndGameplay(true);
        }
    }

    public void OnCollideWithEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
        if(!isGodMode) lives--;
        RefreshLives();
        
        if (lives <= 0)
        {
            EndGameplay(false);
        }
        else
        {
            SpawnEnemy();
        }
    }

    void EndGameplay(bool isWin)
    {
        if (isWin)
        {
            int score = (int)gameTimer;

            LeaderboardData playerLeaderboardData = new LeaderboardData();
            playerLeaderboardData.name = userData.playerName;
            playerLeaderboardData.score = score;

            userData.leaderboardDatas.Add(playerLeaderboardData);
            List<LeaderboardData> sorted = userData.leaderboardDatas.OrderBy(data => data.score).ToList();
            userData.leaderboardDatas = sorted;
            
            //only keep 5
            List<LeaderboardData> data = new List<LeaderboardData>();
            int count = 5;
            for (int i = 0; i < count; i++)
            {
                LeaderboardData leaderboard = userData.leaderboardDatas[i];
                data.Add(leaderboard);
            }

            userData.leaderboardDatas.Clear();
            userData.leaderboardDatas.AddRange(data);
        }
        
        resultPopup.Show(isWin);
        Time.timeScale = 0;
        
        Save();
    }

    void OnClickAutoWin()
    {
        EndGameplay(true);
    }

    void OnClickGodMode()
    {
        isGodMode = !isGodMode;
        buttonGodMode.image.color = isGodMode ? Color.green : Color.red;
    }
    
    #region UI Function
    public void RefreshUI()
    {
        RefreshLives();
        RefreshTimer();
        RefreshCollectibles();
    }
    
    public void RefreshLives()
    {
        int count = imgLives.Count;
        for (int i = 0; i < count; i++)
        {
            bool on = i < lives;
            imgLives[i].color = on ? Color.white : Color.black;
        }
    }

    public void RefreshTimer()
    {
        int sec = (int)gameTimer % 60;
        int min = (int)gameTimer / 60;

        string format = $"{min:D2}:{sec:D2}";
        textTimer.SetText(format);
    }

    public void RefreshCollectibles()
    {
        string format = $"{collectibleCollected}/{gameData.collectibleAmount}";
        textCollectible.SetText(format);
    }
    #endregion

    public void Save()
    {
        string KEY_USERDATA = "UserData";

        string userDataJSON = JsonUtility.ToJson(userData);
        PlayerPrefs.SetString(KEY_USERDATA, userDataJSON);
    }

    public void Load()
    {
        string KEY_USERDATA = "UserData";
        
        bool userDataExist = PlayerPrefs.HasKey(KEY_USERDATA);
        if (userDataExist)
        {
            string userDataJSON = PlayerPrefs.GetString(KEY_USERDATA);
            userData = JsonUtility.FromJson<UserData>(userDataJSON);
        }
        else
        {
            //First launch
            userData = new UserData();

            userData.playerName = string.Empty;
            
            userData.leaderboardDatas = new List<LeaderboardData>();
            LeaderboardData[] dummies = gameData.dummyLeaderboardDatas;
            userData.leaderboardDatas.AddRange(dummies);
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
