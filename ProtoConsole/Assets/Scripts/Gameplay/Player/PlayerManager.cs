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

        for (int i = 0; i < players.Count; i++)
        {
            playersAsPlayer.Add(players[i].GetComponent<Player>());
            playersAsPlayer[i].GetComponentInChildren<PlayerTag>(true).DisplayPlayer(playerTagSettings.TagPrefix + (i + 1), playerTagSettings.GetColorAtIndex(i), playerTagSettings.UpdateArrowColor);
        }

        currentLevelManager.InitPlayers(playersAsPlayer);
    }

    private void OnDestroy()
    {
        playerInputManager.onPlayerJoined -= PlayerInputManager_OnPlayerJoined;
        playerInputManager.onPlayerLeft -= PlayerInputManager_OnPlayerLeft;
    }
}
