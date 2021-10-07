using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelSettings settings = default;

    private List<Player> livingPlayers = default;

    public void InitPlayers(List<Player> players)
    {
        Player player;

        livingPlayers = players;

        for (int i = 0; i < players.Count; i++)
        {
            player = players[i];

            player.Reset();
            player.SpawnOnLevel(new Vector3(0, settings.PlanetRadius, 0), settings);

            player.OnDeath += Player_OnDeath;
        }
    }

    private void Player_OnDeath(Player player)
    {
        if (livingPlayers.Contains(player)) 
        { 
            livingPlayers.Remove(player);
            player.OnDeath -= Player_OnDeath;
        }

        if (livingPlayers.Count < 2)
        {
            Debug.Log(" ====== " + livingPlayers[0].name + " WINS ! ====== ");
            Time.timeScale = 0;
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < livingPlayers.Count; i++)
        {
            livingPlayers[i].OnDeath -= Player_OnDeath;
        }

        livingPlayers = null;
    }
}
