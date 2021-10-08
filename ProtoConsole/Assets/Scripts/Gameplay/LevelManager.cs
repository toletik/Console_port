using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelSettings settings = default;

    public LevelSettings Settings => settings;

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

            player.StartGame();
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
            Destroy(gameObject);
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
