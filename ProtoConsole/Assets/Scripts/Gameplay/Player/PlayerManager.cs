using Com.IsartDigital.Common.UI;
using nn.hid;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof (PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{
    public event Action<int> OnPlayerAdded;
    public event Action<int> OnPlayerRemoved;

    [SerializeField] private uint minNumberOfPlayers = 8;
    [SerializeField] private PlayerTagParameters playerTagSettings = default;
    [SerializeField] private HUD hud = default;

    private PlayerInputManager playerInputManager = default;
    private List<Player> players = new List<Player>();
    private List<PlayerInput> playersInputs = new List<PlayerInput>();

    VibrationManager vibrationManager = null;

    public bool EnoughPlayersToStart => playersInputs.Count >= minNumberOfPlayers;

    private int bestScore  = 0;
    private Player bestPlayer = null;


    void InitializeNPad()
    {
        // Switch
        Npad.Initialize();

        NpadStyle style = NpadStyle.JoyLeft | NpadStyle.JoyRight;
        Npad.SetSupportedStyleSet(style);
        NpadJoy.SetHoldType(NpadJoyHoldType.Horizontal);

        NpadJoy.SetHandheldActivationMode(NpadHandheldActivationMode.Dual);

        NpadId[] npadIds = { NpadId.No1, NpadId.No2, NpadId.No3, NpadId.No4, NpadId.No5, NpadId.No6, NpadId.No7, NpadId.No8 };
        Npad.SetSupportedIdType(npadIds);
    }

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();

        playerInputManager.onPlayerJoined += PlayerInputManager_OnPlayerJoined;
        playerInputManager.onPlayerLeft += PlayerInputManager_OnPlayerLeft;

        InitializeNPad();
    }

    public void EnablePlayerConnexion(bool enable = true)
    {
        playerInputManager.enabled = enable;
    }
    private void PlayerInputManager_OnPlayerJoined(PlayerInput player)
    {
        if (!playersInputs.Contains(player))
        {
            Debug.Log("Bienvenue !");

            playersInputs.Add(player);
            OnPlayerAdded?.Invoke(playersInputs.Count);
        }
    }
    private void PlayerInputManager_OnPlayerLeft(PlayerInput player)
    {
        if (playersInputs.Contains(player))
        {
            Debug.Log("Good bye !");

            playersInputs.Remove(player);
            OnPlayerRemoved?.Invoke(playersInputs.Count);
        }
    }
    public void StartLevel(LevelManager currentLevelManager)
    {
        Color playerColor;
        Player player = default;
        PlayerTag playerTag;

        hud.gameObject.SetActive(true);
        UIManager.Instance.CloseScreen<ConnexionScreen>();

        for (int i = 0; i < playersInputs.Count; i++)
        {
            playerColor = playerTagSettings.GetColorAtIndex(i);
            player = playersInputs[i].GetComponent<Player>();
            playerTag = player.GetComponentInChildren<PlayerTag>(true);

            players.Add(player);

            playerTag.DisplayPlayer(playerTagSettings.TagPrefix + (i + 1), playerColor, playerTagSettings.UpdateArrowColor);
            player.OnScoreUpdated += PlayerManagerScoreHandle;
            hud.CreatePlayerInfo(player, playerColor, i + 1);
        }

        bestScore = (player != null)? player.InitialScore : 0;
        currentLevelManager.InitPlayers(players);
    }

    #region Score
    private void PlayerManagerScoreHandle(Player player, int score = 0)
    {
        //If beat BestScore
        if (score >= bestScore)
        {
            //Update BestPlayer
            if (player != bestPlayer)
            {
                //Remove BestPlayer
                RemoveBestPlayer();

                //Add BestPlayer and there can be only one BestPlayer
                AddBestPlayer((score > bestScore) ? player : null);

                HUD.UpdateBestPlayer();
            }
            
            bestScore = score;
        }
        //if lost score, we need to check if someone become BestPlayer
        else if (player == bestPlayer)
        {
            //Remove BestPlayer
            RemoveBestPlayer();

            Player tempBestPlayer = null;
            int tempBestScore = 0;    
            
            foreach (Player currentPlayer in players)
            {
                if (currentPlayer.Score >= tempBestScore)
                {
                    //Set tempBestPlayer and there can be only one BestPlayer
                    tempBestPlayer = (currentPlayer.Score > tempBestScore) ? currentPlayer : null;
                    tempBestScore = currentPlayer.Score;
                }
            }

            AddBestPlayer(tempBestPlayer);
            bestScore = tempBestScore;
        }
    }
    private void RemoveBestPlayer()
    {
        if (bestPlayer)
        {
            bestPlayer.GetComponentInChildren<PlayerTag>(true).DesativateBestScore();
            bestPlayer.IsBestPlayer = false;
        }
    }
    private void AddBestPlayer(Player newBestPlayer)
    {
        bestPlayer = newBestPlayer;

        // Vibration test
        //StartCoroutine(vibrationManager.VibrateForAllDuringSeconds(new nn.hid.VibrationValue(0.40f, 160.0f, 0.55f, 320.0f), 5f));

        if(bestPlayer)
        {
            bestPlayer.IsBestPlayer = true;
            bestPlayer.GetComponentInChildren<PlayerTag>(true).ActivateBestScore();
        }
    }
    #endregion

    private void OnDestroy()
    {
        playerInputManager.onPlayerJoined -= PlayerInputManager_OnPlayerJoined;
        playerInputManager.onPlayerLeft -= PlayerInputManager_OnPlayerLeft;
    }
}
