using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof (PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{
    private PlayerInputManager playerInputManager = default;
    private List<PlayerInput> players = new List<PlayerInput>();

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();

        playerInputManager.onPlayerJoined += PlayerInputManager_OnPlayerJoined;
        playerInputManager.onPlayerLeft += PlayerInputManager_OnPlayerLeft;
    }

    public void EnablePlayerConnexion(bool enable = true)
    {
        playerInputManager.gameObject.SetActive(enable);
    }

    private void PlayerInputManager_OnPlayerJoined(PlayerInput player)
    {
        if (!players.Contains(player)) players.Add(player);
    }

    private void PlayerInputManager_OnPlayerLeft(PlayerInput player)
    {
        if (players.Contains(player)) players.Remove(player);
    }

    public void StartLevel(LevelManager currentLevelManager)
    {
        List<Player> playersAsPlayer = new List<Player>();

        for (int i = 0; i < players.Count; i++)
        {
            playersAsPlayer.Add(players[i].GetComponent<Player>());
        }

        currentLevelManager.InitPlayers(playersAsPlayer);
    }

    private void OnDestroy()
    {
        playerInputManager.onPlayerJoined -= PlayerInputManager_OnPlayerJoined;
        playerInputManager.onPlayerLeft -= PlayerInputManager_OnPlayerLeft;
    }
}
