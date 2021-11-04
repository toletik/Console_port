using Com.IsartDigital.Common.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using nn.hid;
using UnityEngine.InputSystem.Switch.LowLevel;
using UnityEngine.InputSystem;

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

        OpenApplet();
    }

    private void PlayerManager_OnPlayerAdded(List<PlayerInput> playerInput)
    {

        int currentNumberOfPlayers = playerInput.Count;
        if(playerIndicatorList[currentNumberOfPlayers-1].TryGetComponent(out Image image))
            image.color = playerTag.GetColorAtIndex(currentNumberOfPlayers-1);
        if(playerTypeOfController[currentNumberOfPlayers-1].TryGetComponent(out Image imageController))
            imageController.sprite = Npad.GetStyleSet(playerManager.GetNPadID(currentNumberOfPlayers-1))==NpadStyle.JoyLeft ? playerIMG.ImgController[1] : playerIMG.ImgController[0];
    }

    private void PlayerManager_OnPlayerRemoved(int currentNumberOfPlayers)
    {
        if(playerIndicatorList[currentNumberOfPlayers-1].TryGetComponent(out Image image))
            image.color = Color.white;
        if(playerTypeOfController[currentNumberOfPlayers-1].TryGetComponent(out Image imageController))
            imageController.sprite = default;
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

    public void OpenApplet()
    {
        // 1. Preprocessor Directive
#if UNITY_SWITCH && !UNITY_EDITOR
        // 2. Set the Arguments For the Applet
        ControllerSupportArg controllerSupportArgs = new ControllerSupportArg();
        controllerSupportArgs.SetDefault();
        bool multiplayer = true;

        if (multiplayer)
        {
           controllerSupportArgs.playerCountMax = 8; // This will show X controller setup boxes in the in the applet 
           controllerSupportArgs.playerCountMin = 1; // Applet requires at least 1 player to connect
        }
        else // Single player
        {
           controllerSupportArgs.enableSingleMode = true; 
        }

        // 3. (Optional) Suspend Unity Processes to Call the Applet 
        UnityEngine.Switch.Applet.Begin(); //call before calling a system applet to stop all Unity threads (including audio and networking)

        // 4. Call the Applet
        nn.hid.ControllerSupport.Show(controllerSupportArgs);

        // 5. (Optional) Resume the Suspended Unity Processes
        UnityEngine.Switch.Applet.End(); //always call this if you called Switch.Applet.Begin()

        // End the Preprocessor Directive
#endif
    }

    private void OnDestroy()
    {
        ClearEvents();
    }

    public void Return()
    {
        UIManager.Instance.AddScreen<TitleCard>(true);
    }
}
