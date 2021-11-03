using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContainerRank : MonoBehaviour
{
    [Header("Rank Element")]
    [SerializeField] private TextMeshProUGUI scoreTxt= default;
    [SerializeField] private TextMeshProUGUI playerName= default;

    public void ChangeRankInformation(string score,string playerInt)
    {
        scoreTxt.text =score;
        playerName.text = playerInt;
    }

	public void Disable() 
    {
		gameObject.SetActive(false);
	}
}
