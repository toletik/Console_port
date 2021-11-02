using Com.IsartDigital.Common.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnexionScreen : UIScreen
{
    [SerializeField] private PlayerManager playerManager = default;
    [SerializeField] private PlayerTagParameters playerTag = default;
    [SerializeField] private PlayerControllerSprite playerIMG = default;
    [SerializeField] private List<GameObject> playerIndicatorList = default;
    [SerializeField] private List<GameObject> playerTypeOfController= default;
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
        if(playerIndicatorList[currentNumberOfPlayers-1].TryGetComponent(out Image image))image.color = playerTag.GetColorAtIndex(currentNumberOfPlayers-1);
        if(playerTypeOfController[currentNumberOfPlayers-1].TryGetComponent(out Image imageController))imageController.sprite = playerIMG.ImgController[0];
    }

    private void PlayerManager_OnPlayerRemoved(int currentNumberOfPlayers)
    {
        if(playerIndicatorList[currentNumberOfPlayers-1].TryGetComponent(out Image image))image.color = Color.white;
        if(playerTypeOfController[currentNumberOfPlayers-1].TryGetComponent(out Image imageController))imageController.sprite = default;
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
