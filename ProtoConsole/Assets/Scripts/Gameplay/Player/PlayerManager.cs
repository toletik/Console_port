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
    private List<PlayerInput> players = new List<PlayerInput>();
    VibrationManager vibrationManager = null;

    public bool EnoughPlayersToStart => players.Count >= minNumberOfPlayers;

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
        if (!players.Contains(player)) 
        {
            Debug.Log("Bienvenue !");

            players.Add(player);
            OnPlayerAdded?.Invoke(players.Count);

            // Vibration test
            //StartCoroutine(vibrationManager.VibrateForAllDuringSeconds(new nn.hid.VibrationValue(0.40f, 160.0f, 0.55f, 320.0f), 1f));
        }
    }

    private void PlayerInputManager_OnPlayerLeft(PlayerInput player)
    {
        if (players.Contains(player))
        {
            Debug.Log("Good bye !");

            players.Remove(player);
            OnPlayerRemoved?.Invoke(players.Count);

            Destroy(player.gameObject);
        }
    }

    public void StartLevel(LevelManager currentLevelManager)
    {
        List<Player> playersAsPlayer = new List<Player>();
        Player player = null;
        PlayerTag playerTag;
        hud.gameObject.SetActive(true);

        for (int i = 0; i < players.Count; i++)
        {
            Color playerColor = playerTagSettings.GetColorAtIndex(i);
            player = players[i].GetComponent<Player>();
            playerTag = player.GetComponentInChildren<PlayerTag>(true);

            playersAsPlayer.Add(player);

            playerTag.DisplayPlayer(playerTagSettings.TagPrefix + (i + 1), playerColor, playerTagSettings.UpdateArrowColor);
            player.OnIsNewBestScore += playerTag.ActivateBestScore;
            player.OnBestScoreLost += playerTag.DesativateBestScore;
            hud.CreatePlayerInfo(player, playerColor, i + 1);


        }

        Player.ResetBestScore(player != null ? player.InitialScore : 0);

        currentLevelManager.InitPlayers(playersAsPlayer);


        vibrationManager = new VibrationManager();

        // Vibration test
        StartCoroutine(vibrationManager.VibrateForAllDuringSeconds(new nn.hid.VibrationValue(0.40f, 160.0f, 0.55f, 320.0f), 5f));
    }

    private void OnDestroy()
    {
        playerInputManager.onPlayerJoined -= PlayerInputManager_OnPlayerJoined;
        playerInputManager.onPlayerLeft -= PlayerInputManager_OnPlayerLeft;
    }
}
