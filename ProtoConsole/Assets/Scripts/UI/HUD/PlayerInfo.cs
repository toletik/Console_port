using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{

    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI text = default;
    [SerializeField] private Image objectOn = default;
    [SerializeField] private Image objectOff = default;
    [SerializeField] private Image timerRenderer = default;

    
    public bool collectibleOn = false;
    public bool respawning = false;
    public Player player = default;
    public int playerId = 0;
    public Color baseColor = default;

    private float elapsedTime = 0;
    public static float timer = 4;
    private const string TAG_PLAYER = "Player";

    void Start()
    {
        if (player)
        {
            player.OnDeath += Player_OnDeath;
            player.OnCollectibleUpdate += Player_OnCollectibleUpdate;
        }
        
        ChangeTextColor(baseColor);
        text.text = TAG_PLAYER +" "+ playerId;
        CollectibleUpdate();
        
    }

    private void Player_OnCollectibleUpdate(Player player, int possessedCollectibles = 0)
    {
        throw new NotImplementedException();
    }

    private void Player_OnDeath(Player player, int possessedCollectibles = 0)
    {
       
    }

    private void CollectibleUpdate()
    {
        if (collectibleOn)
        {
            collectibleOn = false;
            objectOn.gameObject.SetActive(false);
            objectOff.gameObject.SetActive(true);
        }
        else if (!collectibleOn)
        {
            collectibleOn = true;
            objectOn.gameObject.SetActive(true);
            objectOff.gameObject.SetActive(false);
        }
    }

    public void ChangeTextColor(Color color)
    {
        text.color = color;
    }
    // Update is called once per frame
    
    private void OnDestroy()
    {
        if (player)
        {
            player.OnDeath -= Player_OnDeath;
            player.OnCollectibleUpdate -= Player_OnCollectibleUpdate;
        }
    }
    private void Update()
    {
        if (respawning)
        {
            ChangeTextColor(Color.grey);
            timerRenderer.gameObject.SetActive(true);
            elapsedTime += Time.deltaTime;
            timerRenderer.fillAmount = 1-elapsedTime / timer;
            if (elapsedTime >= timer)
            {
                elapsedTime = 0;
                respawning = false;
                timerRenderer.gameObject.SetActive(false);
                ChangeTextColor(baseColor);
            }
        }
    }

    public void SetAllParam(Player playerRef, Color color, int PlayerId)
    {
        player = playerRef;
        playerId = PlayerId;
        baseColor = color;

    }
}
