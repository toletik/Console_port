using nn.hid;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Switch.LowLevel;

public class ConnexionScreen : Screen
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

        CloseScreen();
    }

    public override void OpenScreen()
    {
        base.OpenScreen();
        playerManager.EnablePlayerConnexion(true);

        playerManager.OnPlayerAdded += PlayerManager_OnPlayerAdded;
        playerManager.OnPlayerRemoved += PlayerManager_OnPlayerRemoved;

        OpenApplet();
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
