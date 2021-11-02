using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private PlayerInfo playerInfoPrefab = default;
    [SerializeField] private Transform verticalBox = default;

    private List<PlayerInfo> playerInfos = new List<PlayerInfo>();
   

    // Update is called once per frame
    public void CreatePlayerInfo(Player player, Color color,int playerId)
    {
        PlayerInfo playerInfo = playerInfoPrefab;
        playerInfo.SetAllParam(player, color, playerId);
        playerInfos.Add(playerInfo);
        Instantiate(playerInfo, verticalBox);
        
    }
}
