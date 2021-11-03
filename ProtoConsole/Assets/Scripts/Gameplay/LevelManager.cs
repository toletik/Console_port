using Com.IsartDigital.Common.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static event Action<LevelManager> OnLevelSpawn;
    public static event Action<LevelManager> OnLevelDestroy;
    public event Action OnLevelStart;
    public event Action OnLevelEnd;


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

    public float TimeRemainingInSeconds => levelEndTime - Time.time;
    private float levelEndTime = 0;

    public  int LevelDuration { get; private set; } = 0;

    private void Awake()
    {
        LevelDuration = settings.LevelDuration;
        OnLevelSpawn?.Invoke(this);
        PauseScreen.OnLevelQuit += ClearLevel;
        foreach(Obstacle obstacle in obstacles)
            obstacle.gameObject.SetActive(true);
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
        levelEndTime = Time.time + LevelDuration;

        planetPartsRotation.InitLevelValues(settings.GravityCenter);

        Invoke("EndGame", LevelDuration);

        foreach(Player player in Players)
            player.SetModePlay();


        OnLevelStart?.Invoke();
        Debug.Log("START !!!");
    }

    private void EndGame()
    {
        Banner banner = Banner.Instance;

        Debug.Log("End of Game !!!");
        StopAllCoroutines();

        banner.PlayBanner(endLevelBannerMessage);
        banner.OnAnimationEnd += OpenLevelEndScreen;
    }

    private void OpenLevelEndScreen()
    {
        ClearLevel();

        UIManager.Instance.AddScreen<WinScreen>(true);
        Banner.Instance.OnAnimationEnd -= OpenLevelEndScreen;
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
        Vector3 newPos;
        newPos = UnityEngine.Random.onUnitSphere * settings.PlanetRadius;
        newPos.y = Mathf.Abs(newPos.y);
        newPos = Quaternion.FromToRotation(Vector3.up, -Camera.main.transform.forward) * (newPos - Vector3.zero);
        if (enablePlay) 
            player.SetModePlay();
        player.SpawnOnLevel(newPos, settings);


        return player;
    }
    #endregion

    public List<Player> GetRankedPlayers()
    {
        int currentRank = 1;

        Players.Sort((a, b) => { return a.Score < b.Score ? 1 : -1; });
        Players[0].SetRank(currentRank);

        for (int i = 1; i < Players.Count; i++)
        {
            Players[i].SetRank(Players[i].Score == Players[i - 1].Score ? currentRank : currentRank + 1);
            currentRank++;
        }

        return Players;
    }

    private void OnDestroy()
    {
        foreach(Player currentPlayer in Players)
            currentPlayer.OnDeath -= Player_OnDeath;

        Players.Clear();

        PauseScreen.OnLevelQuit -= ClearLevel;
    }
}
