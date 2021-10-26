using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int startLevelDelayDuration = 3;
    [Space(8)]
    [SerializeField] private LevelSettings settings = default;
    [SerializeField] private RotationForPlanetPart planetPartsRotation = default;
    [Space(8)]
    [SerializeField] private List<Obstacle> obstacles = default;
    [Space(8)]
    [SerializeField] private CollectibleManager collectibleManager = default;

    public LevelSettings Settings => settings;

    private List<Player> players = default;

    private Action doAction = default;
    private float elapsedTime = 0;
    private int levelDuration = 0;


    public void InitPlayers(List<Player> players)
    {
        this.players = players;

        for (int i = 0; i < players.Count; i++)
        {
            RespawnPlayer(players[i], false, true).OnDeath += Player_OnDeath;
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

        planetPartsRotation.InitLevelValues(settings.GravityCenter);

        doAction = DoActionRunGame;

        for (i = 0; i < players.Count; i++)
        {
            players[i].SetModePlay();
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

            StopAllCoroutines();
        }
    }

    #region Player cycle : Die / Respawn
    private void Player_OnDeath(Player player, int numberOfPowerups)
    {
        collectibleManager.LoseCollectibleWhenDead(player.transform, numberOfPowerups);
        StartCoroutine(PlayerRespawnCooldown(player));
    }

    private IEnumerator PlayerRespawnCooldown(Player player)
    {
        float elapsedTime = 0;
        float duration = settings.RespawnPlayerCooldownDuration;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            //Update hud

            yield return null;
        }

        RespawnPlayer(player, true, false);

        yield break;
    }

    private Player RespawnPlayer(Player player, bool enablePlay, bool resetScore)
    {
        player.ResetValues(resetScore);
        player.SpawnOnLevel(new Vector3(0, settings.PlanetRadius, 0), settings);

        if (enablePlay) player.SetModePlay();

        return player;
    }
    #endregion

    private void OnDestroy()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].OnDeath -= Player_OnDeath;
        }

        players = null;
    }
}
