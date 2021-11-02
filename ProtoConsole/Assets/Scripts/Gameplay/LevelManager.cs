using Com.IsartDigital.Common.UI;
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
    [SerializeField] private string startLevelBannerMessage = "Let's Go!!!";
    [SerializeField] private string endLevelBannerMessage = "Time is up!";
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
            currentPlayer.ResetValues(true);
            RespawnPlayer(currentPlayer, false);
            currentPlayer.OnDeath += Player_OnDeath;
        }

        Invoke("StartLevel", startLevelDelayDuration);

        Banner.Instance.PlayBanner(startLevelBannerMessage);
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
        Banner banner = Banner.Instance;

        Debug.Log("End of Game !!!");
        StopAllCoroutines();

        banner.PlayBanner(endLevelBannerMessage);
        banner.OnAnimationEnd += () => 
        {
            UIManager.Instance.AddScreen<WinScreen>(true);
        };
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

        RespawnPlayer(player, true);
        
    }

    private Player RespawnPlayer(Player player, bool enablePlay)
    {
        if (enablePlay) 
            player.SetModePlay();
        player.SpawnOnLevel(new Vector3(0, settings.PlanetRadius, 0), settings);


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
