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
        levelManager = null;
    }

    private void LevelManager_OnLevelStart()
    {
        StartCoroutine(TimerCoroutine());
    }

    private void RemovePlayerInfos()
    {
        for (int i = 0; i < playerInfos.Count; i++)
        {
            Destroy(playerInfos[i].gameObject);
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
        
    }

   

    static public void UpdateBestPlayer()
    {
        PlayerInfo playerInfo;
        for (int i = 0; i < playerInfos.Count; i++)
        {
            playerInfo = playerInfos[i];
            if (playerInfo.player.IsBestPlayer) playerInfo.setAsBestPlayer();
            else playerInfo.removeBestPlayer();
        }
    }

    
    public void CreatePlayerInfo(Player player, Color color,int playerId)
    {
        PlayerInfo playerInfo = playerInfoPrefab;
        playerInfo.SetAllParam(player, color, playerId);
        playerInfos.Add(playerInfo);
        Instantiate(playerInfo, verticalBox);
        
    }
    

    public string CalculateTimeLeft(float timeleft)
    {
        int min = 0;
        int sec = 0;
        min = (int)timeleft / 60;
        sec = (int)timeleft - (60 * min);
        return min + ":" + sec;
    }
    private void UpdateTimeText(float timer)
    {
        timeText.text = CalculateTimeLeft(timer);
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
