using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof (PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{
    public event Action<int> OnPlayerAdded;
    public event Action<int> OnPlayerRemoved;

    [SerializeField] private uint minNumberOfPlayers = 2;
    [SerializeField] private PlayerTagParameters playerTagSettings = default;
    [SerializeField] private HUD hud = default;
    private PlayerInputManager playerInputManager = default;
    private List<PlayerInput> players = new List<PlayerInput>();

    public bool EnoughPlayersToStart => players.Count >= minNumberOfPlayers;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();

        playerInputManager.onPlayerJoined += PlayerInputManager_OnPlayerJoined;
        playerInputManager.onPlayerLeft += PlayerInputManager_OnPlayerLeft;
    }

    public void EnablePlayerConnexion(bool enable = true)
    {
        playerInputManager.enabled = enable;
    }

    private void PlayerInputManager_OnPlayerJoined(PlayerInput player)
    {
        if (!players.Contains(player)) 
        {
            Debug.Log("Bienvenue !");

            players.Add(player);
            OnPlayerAdded?.Invoke(players.Count);
        }
    }

    private void PlayerInputManager_OnPlayerLeft(PlayerInput player)
    {
        if (players.Contains(player))
        {
            Debug.Log("Good bye !");

            players.Remove(player);
            OnPlayerRemoved?.Invoke(players.Count);
        }
    }

    public void StartLevel(LevelManager currentLevelManager)
    {
        List<Player> playersAsPlayer = new List<Player>();
        Player player = null;
        PlayerTag playerTag;
        hud.gameObject.SetActive(true);

        for (int i = 0; i < players.Count; i++)
        {
            Color playerColor = playerTagSettings.GetColorAtIndex(i);
            player = players[i].GetComponent<Player>();
            playerTag = player.GetComponentInChildren<PlayerTag>(true);

            playersAsPlayer.Add(player);

            playerTag.DisplayPlayer(playerTagSettings.TagPrefix + (i + 1), playerColor, playerTagSettings.UpdateArrowColor);
            player.OnIsNewBestScore += playerTag.ActivateBestScore;
            player.OnBestScoreLost += playerTag.DesativateBestScore;
            hud.CreatePlayerInfo(player, playerColor, i + 1);


        }

        Player.ResetBestScore(player != null ? player.InitialScore : 0);

        currentLevelManager.InitPlayers(playersAsPlayer);
    }

    private void OnDestroy()
    {
        playerInputManager.onPlayerJoined -= PlayerInputManager_OnPlayerJoined;
        playerInputManager.onPlayerLeft -= PlayerInputManager_OnPlayerLeft;
    }
}
