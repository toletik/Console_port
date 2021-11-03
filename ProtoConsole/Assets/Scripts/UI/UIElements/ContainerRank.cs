using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContainerRank : MonoBehaviour
{
    [Header("Rank Element")]
    [SerializeField] private TextMeshProUGUI scoreTxt= default;
    [SerializeField] private Image img= default;
    [SerializeField] private TextMeshProUGUI playerName= default;

    public void ChangeRankInformation(string score,string playerInt,Color color)
    {
        img.color = color;
        scoreTxt.text =score;
        playerName.text += playerInt;
    }

	public void Disable() 
    {
		gameObject.SetActive(false);
	}
}
