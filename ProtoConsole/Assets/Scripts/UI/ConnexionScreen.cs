using System.Collections.Generic;
using UnityEngine;

public class ConnexionScreen : Screen
{
    [SerializeField] private PlayerManager playerManager = default;
    [SerializeField] private List<LevelManager> levelPrefabs = default;

    public void OnStartGame()
    {
        if (levelPrefabs.Count == 0)
        {
            Debug.LogError("NO LEVEL");
            return;
        }

        playerManager.StartLevel(Instantiate(levelPrefabs[0]));

        CloseScreen();
    }

    public override void OpenScreen()
    {
        base.OpenScreen();
        playerManager.EnablePlayerConnexion(true);
    }

    public override void CloseScreen()
    {
        base.CloseScreen();
        playerManager.EnablePlayerConnexion(false);
    }
}
