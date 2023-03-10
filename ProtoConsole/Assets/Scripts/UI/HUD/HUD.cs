using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private PlayerInfo playerInfoPrefab = default;
    [SerializeField] private Transform verticalBox = default;
    [SerializeField] private TextMeshProUGUI timeText = default;
    [SerializeField] private Color baseTimerColor = default;
    [SerializeField] private Color EndTimerColor = default;


    [HideInInspector] public LevelManager levelManager = default;
        
    
    static private List<PlayerInfo> playerInfos = new List<PlayerInfo>();

    private void Awake()
    {
        
          LevelManager.OnLevelSpawn += LevelManager_OnLevelSpawn;
        LevelManager.OnLevelDestroy += LevelManager_OnLevelDestroy;

    }

    private void LevelManager_OnLevelDestroy(LevelManager obj)
    {
        levelManager.OnLevelEnd -= LevelManager_OnLevelEnd;
        levelManager.OnLevelStart -= LevelManager_OnLevelStart;
        RemovePlayerInfos();
        StopAllCoroutines();
        levelManager = null;
    }

    private void LevelManager_OnLevelStart()
    {
        StartCoroutine(TimerCoroutine());
    }

    private void RemovePlayerInfos()
    {
        for (int i = playerInfos.Count - 1; i >= 0; i--)
        {
            Destroy(playerInfos[i].gameObject);
            playerInfos.RemoveAt(i);
        }
    }

    private void LevelManager_OnLevelSpawn(LevelManager _levelManager)
    {
        levelManager = _levelManager;
        levelManager.OnLevelStart += LevelManager_OnLevelStart;
        levelManager.OnLevelEnd += LevelManager_OnLevelEnd;
        Debug.Log("HEHO UPDATE TOI LE COMPTEUR");
        UpdateTimeText(levelManager.LevelDuration);
    }

    private void LevelManager_OnLevelEnd()
    {
        levelManager.OnLevelEnd -= LevelManager_OnLevelEnd;
        levelManager.OnLevelStart -= LevelManager_OnLevelStart;
        RemovePlayerInfos();
        levelManager = null;
        StopAllCoroutines();
    }

   

    static public void UpdateBestPlayer()
    {
        PlayerInfo playerInfo;

        for (int i = 0; i < playerInfos.Count; i++)
        {
            playerInfo = playerInfos[i];

            if (playerInfo.player.IsBestPlayer) playerInfo.SetAsBestPlayer();
            else playerInfo.RemoveBestPlayer();
        }
    }

    
    public void CreatePlayerInfo(Player player, Color color,int playerId)
    {
        PlayerInfo playerInfo =  Instantiate(playerInfoPrefab, verticalBox);
        playerInfo.SetAllParam(player, color, playerId);
      
        playerInfos.Add(playerInfo);
        
    }
    

    public string CalculateTimeLeft(float timeleft)
    {
        int min = 0;
        int sec = 0;
        min = (int)timeleft / 60;
        sec = (int)timeleft - (60 * min);
        if (min == 0 && sec <= 10) timeText.color = EndTimerColor;
        else timeText.color = baseTimerColor;
        return min + ":" + sec;
    }
    private void UpdateTimeText(float timer)
    {
        if(timeText.text != CalculateTimeLeft(timer))
        {
            timeText.text = CalculateTimeLeft(timer);

        }
    }
    private IEnumerator TimerCoroutine()
    {
        while (levelManager.TimeRemainingInSeconds > 0)
        {
            UpdateTimeText(levelManager.TimeRemainingInSeconds);
            yield return null;
        }
    }
    private void OnDestroy()
    {

        if (levelManager)
        {
            levelManager.OnLevelEnd -= LevelManager_OnLevelEnd;
            levelManager.OnLevelStart -= LevelManager_OnLevelStart;
            levelManager = null;
        }
            LevelManager.OnLevelSpawn -= LevelManager_OnLevelSpawn;
  
    }
}
