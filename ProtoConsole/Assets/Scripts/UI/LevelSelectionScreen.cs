using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionScreen : Screen
{
    [SerializeField] private List<LevelManager> levelPrefabs = default;
    [SerializeField] private PlayerManager playerManager = default;
    public void SelectLevel(int levelSelected)
    {
        playerManager.StartLevel(Instantiate(levelPrefabs[levelSelected]));
    }

    public void Return()
    {

    }
}
