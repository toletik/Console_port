using Com.IsartDigital.Common.UI;
using System.Collections.Generic;
using UnityEngine;

public class ConnexionScreen : UIScreen
{
    [SerializeField] private PlayerManager playerManager = default;
    [SerializeField] private List<LevelManager> levelPrefabs = default;

    public void OnStartGame()
    {
        if (levelPrefabs.Count == 0 || !playerManager.EnoughPlayersToStart)
        {
            Debug.LogError("NO LEVEL or NOT ENOUGH PLAYERS");
            return;
        }

        playerManager.StartLevel(Instantiate(levelPrefabs[0]));

        Desactivate();
    }

    protected override void Activate()
    {
        base.Activate();
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

    protected override void Desactivate()
    {
        base.Desactivate();
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
