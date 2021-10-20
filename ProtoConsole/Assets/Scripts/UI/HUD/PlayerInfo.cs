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

    
   
    public bool respawning = false;
    public Player player = default;
    public int playerId = 0;
    public Color baseColor = default;

    private float deathTimer = 0;
  
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
        CollectibleUpdate(0);
        
    }

    private void Player_OnCollectibleUpdate(Player player, int possessedCollectibles )
    {
        CollectibleUpdate(possessedCollectibles);
    }

    private void Player_OnDeath(Player player, int possessedCollectibles)
    {
        ChangeTextColor(Color.grey);
        timerRenderer.gameObject.SetActive(true);
        Debug.Log("die");
        StartCoroutine(DeathTimerCoroutine(4));
    }

    private IEnumerator DeathTimerCoroutine(float timer)
    {
        Debug.Log(deathTimer);
        while(deathTimer< timer)
        {
            deathTimer += Time.deltaTime;
            timerRenderer.fillAmount = 1 - deathTimer / timer;

            yield return null;
        }
        deathTimer = 0;
        Debug.Log("EndCoroutine");
        timerRenderer.gameObject.SetActive(false);
        ChangeTextColor(baseColor);
        yield break;
    }

    private void CollectibleUpdate(int nbrOfCollectible)
    {
        if (nbrOfCollectible> 0)
        {
            objectOn.gameObject.SetActive(true);
            objectOff.gameObject.SetActive(false);
        }
        else 
        {
            objectOn.gameObject.SetActive(false);
            objectOff.gameObject.SetActive(true);
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

    public void SetAllParam(Player playerRef, Color color, int PlayerId)
    {
        player = playerRef;
        playerId = PlayerId;
        baseColor = color;

    }
}
