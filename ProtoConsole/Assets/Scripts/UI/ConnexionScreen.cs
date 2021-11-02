using Com.IsartDigital.Common.UI;
using System.Collections.Generic;
using UnityEngine;

public class ConnexionScreen : UIScreen
{
    [SerializeField] private PlayerManager playerManager = default;

    public void OnStartGame()
    {
        UIManager.Instance.AddScreen<LevelSelectionScreen>(true);
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
