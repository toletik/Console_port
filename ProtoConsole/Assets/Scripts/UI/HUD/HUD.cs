using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private PlayerInfo playerInfoPrefab = default;
    [SerializeField] private Transform verticalBox = default;

    static private List<PlayerInfo> playerInfos = new List<PlayerInfo>();
   
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

    // Update is called once per frame
    public void CreatePlayerInfo(Player player, Color color,int playerId)
    {
        PlayerInfo playerInfo = playerInfoPrefab;
        playerInfo.SetAllParam(player, color, playerId);
        playerInfos.Add(playerInfo);
        Instantiate(playerInfo, verticalBox);
        
    }
}
