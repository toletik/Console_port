using Com.IsartDigital.Common.UI;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionScreen : UIScreen
{
    [SerializeField] private List<LevelManager> levelPrefabs = default;
    [SerializeField] private PlayerManager playerManager = default;
    [SerializeField] private HUD hud = default;

    public void SelectLevel(int levelSelected)
    {
        hud.gameObject.SetActive(true);
        LevelManager level = Instantiate(levelPrefabs[levelSelected]);


        //hud.levelManager = level;
        playerManager.StartLevel(level);

        UIManager.Instance.CloseScreen<LevelSelectionScreen>();
    }
    
    public void Return()
    {
        UIManager.Instance.AddScreen<ConnexionScreen>();
        UIManager.Instance.CloseScreen<LevelSelectionScreen>();
    }
}