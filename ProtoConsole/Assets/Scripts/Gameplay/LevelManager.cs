using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static event Action<LevelManager> OnLevelSpawn;
    public static event Action<LevelManager> OnLevelDestroy;

    [SerializeField] private int startLevelDelayDuration = 3;
    [SerializeField] private int destroyLevelDelayDuration = 5;
    [Space(8)]
    [SerializeField] private LevelSettings settings = default;
    [SerializeField] private RotationForPlanetPart planetPartsRotation = default;
    [Space(8)]
    [SerializeField] private List<Obstacle> obstacles = default;
    [Space(8)]
    [SerializeField] private CollectibleManager collectibleManager = default;

    public LevelSettings Settings => settings;
    public List<Player> Players { get; private set; } = default;

    private int levelDuration = 0;

    private void Awake()
    {
        OnLevelSpawn?.Invoke(this);
    }

    public void InitPlayers(List<Player> players)
    {
        Players = players;

        foreach (Player currentPlayer in players)
        {
            RespawnPlayer(currentPlayer, false, true);
            currentPlayer.OnDeath += Player_OnDeath;
        }

        Invoke("StartLevel", startLevelDelayDuration);
    }

    private void StartLevel()
    {
        levelDuration = settings.LevelDuration;

        planetPartsRotation.InitLevelValues(settings.GravityCenter);

        Invoke("EndGame", levelDuration);
        Invoke("ClearLevel", levelDuration + destroyLevelDelayDuration);

        foreach(Player player in Players)
            player.SetModePlay();

        foreach(Obstacle obstacle in obstacles)
            obstacle.gameObject.SetActive(true);

        Debug.Log("START !!!");
    }

    private void EndGame()
    {
        Debug.Log("End of Game !!!");
        StopAllCoroutines();
    }

    private void ClearLevel()
    {
        Debug.Log("Clear Level");
        OnLevelDestroy?.Invoke(this);

        Destroy(gameObject);
    }

    #region Player cycle : Die / Respawn
    private void Player_OnDeath(Player player, int numberOfPowerups)
    {
        collectibleManager.LoseCollectibleWhenDead(player.transform, numberOfPowerups);
        StartCoroutine(PlayerRespawnCooldown(player));
    }

    private IEnumerator PlayerRespawnCooldown(Player player)
    {
        yield return new WaitForSeconds(settings.RespawnPlayerCooldownDuration);

        RespawnPlayer(player, true, false);
        
    }

    private Player RespawnPlayer(Player player, bool enablePlay, bool resetScore)
    {
        player.ResetValues(resetScore);
        player.SpawnOnLevel(new Vector3(0, settings.PlanetRadius, 0), settings);

        if (enablePlay) 
            player.SetModePlay();

        return player;
    }
    #endregion

    private void OnDestroy()
    {
        foreach(Player currentPlayer in Players)
            currentPlayer.OnDeath -= Player_OnDeath;

        Players.Clear();
    }
}
