using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PlayerInfo : MonoBehaviour
{

    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI text = default;
    [SerializeField] private SpriteRenderer objectOn = default;
    [SerializeField] private SpriteRenderer objectOff = default;

    public bool collectibleOn = false;
    public Player player = default;
    public int playerId = 0;

    private const string TAG_PLAYER = "Player";

    void Start()
    {
        player.OnDeath += Player_OnDeath;

        text.text = TAG_PLAYER +" "+ playerId;
        CollectibleUpdate();
       
    }

    private void Player_OnDeath(Player player)
    {
        ChangeTextColor(Color.gray);
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
    void Update()
    {
        
    }
}
