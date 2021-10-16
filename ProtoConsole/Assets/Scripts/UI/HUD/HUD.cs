using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private PlayerInfo playerInfoPrefab = default;
    [SerializeField] private Transform verticalBox = default;

    private List<PlayerInfo> playerInfos = new List<PlayerInfo>();
    void Start()
    {
        CreatePlayerInfo(new Player(), Color.red,1);
        CreatePlayerInfo(new Player(), Color.blue,2);
        CreatePlayerInfo(new Player(), Color.green, 3);
        CreatePlayerInfo(new Player(), Color.black, 4);

    }

    // Update is called once per frame
    public void CreatePlayerInfo(Player player, Color color,int PlayerId)
    {
        PlayerInfo playerInfo = playerInfoPrefab;
        playerInfo.player = player;
        playerInfo.playerId = PlayerId;
        playerInfos.Add(playerInfo);
        playerInfo.baseColor = color;
        Instantiate(playerInfo, verticalBox);
        
    }
}
