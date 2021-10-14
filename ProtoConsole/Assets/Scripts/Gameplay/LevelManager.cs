using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int startLevelDelayDuration = 3;
    [Space(8)]
    [SerializeField] private LevelSettings settings = default;
    [Space(8)]
    [SerializeField] private List<Obstacle> obstacles = default;

    public LevelSettings Settings => settings;

    private List<Player> livingPlayers = default;

    private Action doAction = default;
    private float elapsedTime = 0;
    private int levelDuration = 0;

    public void InitPlayers(List<Player> players)
    {
        Player player;

        livingPlayers = players;

        for (int i = 0; i < players.Count; i++)
        {
            player = players[i];

            player.ResetValues();
            player.SpawnOnLevel(new Vector3(0, settings.PlanetRadius, 0), settings);

            player.OnDeath += Player_OnDeath;
        }

        elapsedTime = 0;
        doAction = DoActionDelayLevelStart;
    }

    private void Update()
    {
        doAction();
    }

    private void StartLevel()
    {
        int i;

        elapsedTime = 0;
        levelDuration = settings.LevelDuration;

        doAction = DoActionRunGame;

        for (i = 0; i < livingPlayers.Count; i++)
        {
            livingPlayers[i].StartGame();
        }

        for (i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].gameObject.SetActive(true);
        }

        Debug.Log("START !!!");
    }

    private void DoActionDelayLevelStart()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= startLevelDelayDuration) 
            StartLevel();
    }

    private void DoActionRunGame()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= levelDuration)
        {
            Debug.Log("End of Game !!!");
            doAction = () => { };
        }
    }

    private void Player_OnDeath(Player player, int numberOfPowerups)
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
