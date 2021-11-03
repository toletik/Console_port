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
    [SerializeField] private TextMeshProUGUI textNbrCollectible = default;

    [SerializeField] private TextMeshProUGUI scoreTxt = default;
    [SerializeField] private MulticolorText multicolorText = default;
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
            player.OnScoreUpdated += Player_OnScoreUpdated;
        }
        
        ChangeTextColor(baseColor);
        text.text = TAG_PLAYER + playerId;
        scoreTxt.text = player.InitialScore.ToString();
       CollectibleUpdate(0);
    }

    private void Player_OnScoreUpdated(Player player, int score = 0)
    {
        scoreTxt.text = score.ToString();
    }

    private void Player_OnCollectibleUpdate(Player player, int possessedCollectibles )
    {
        CollectibleUpdate(possessedCollectibles);
    }

    private void Player_OnDeath(Player player, int possessedCollectibles)
    {
        ChangeTextColor(Color.grey);
        timerRenderer.gameObject.SetActive(true);
        StartCoroutine(DeathTimerCoroutine(4));
    }

    private IEnumerator DeathTimerCoroutine(float timer)
    {
        while(deathTimer< timer)
        {
            deathTimer += Time.deltaTime;
            timerRenderer.fillAmount = 1 - deathTimer / timer;

            yield return null;
        }

        deathTimer = 0;
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
            textNbrCollectible.gameObject.SetActive(true);
            textNbrCollectible.text = nbrOfCollectible.ToString();
        }
        else 
        {
            objectOn.gameObject.SetActive(false);
            objectOff.gameObject.SetActive(true);
            textNbrCollectible.gameObject.SetActive(false);
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
            player.OnScoreUpdated -= Player_OnScoreUpdated;
        }
    }

    public void setAsBestPlayer()
    {
        multicolorText.enabled = true;
        Debug.Log("oui");
    }
    public void removeBestPlayer()
    {
        multicolorText.enabled = false;
    }

    public void SetAllParam(Player playerRef, Color color, int PlayerId)
    {
        player = playerRef;
        playerId = PlayerId;
        baseColor = color;
    }
}
