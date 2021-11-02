using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConnexionScreen : Screen
{
    [SerializeField] private PlayerManager playerManager = default;
    [SerializeField] private GameObject button = default;

	public void OnStartGame()
    {
        if (!playerManager.EnoughPlayersToStart)
        {
            Debug.LogError("NOT ENOUGH PLAYERS");
            return;
        }

        //CloseScreen();
    }

    public override void OpenScreen()
    {
        base.OpenScreen();
        playerManager.EnablePlayerConnexion(true);

        playerManager.OnPlayerAdded += PlayerManager_OnPlayerAdded;
        playerManager.OnPlayerRemoved += PlayerManager_OnPlayerRemoved;
    }

    private void PlayerManager_OnPlayerAdded(int currentNumberOfPlayers)
    {
        
    }

    private void PlayerManager_OnPlayerRemoved(int currentNumberOfPlayers)
    {
        
    }

    public override void CloseScreen()
    {
        base.CloseScreen();
        playerManager.EnablePlayerConnexion(false);

        ClearEvents();
    }

    private void ClearEvents()
    {
        if (playerManager)
        {
            playerManager.OnPlayerAdded -= PlayerManager_OnPlayerAdded;
            playerManager.OnPlayerRemoved -= PlayerManager_OnPlayerRemoved;
        }
    }

    private void OnDestroy()
    {
        ClearEvents();
    }
}
