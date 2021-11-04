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
    [SerializeField] private GameObject killGrid= default;
    [SerializeField] private GameObject deathGrid= default;

    [Header("Prefab Icon")]
    [SerializeField] private GameObject killIcon= default;
    [SerializeField] private GameObject deathIcon= default;
    [SerializeField] private GameObject suicideIcon= default;


    private string baseTxt=default;
    private string basePlayerNameTxt=default;
	public void Start() {
		baseTxt = scoreTxt.text;
        basePlayerNameTxt=playerName.text;
	}

	public void ChangeRankInformation(string playerInt,Color color,ScoreDetails scoreDetails)
    {
        ClearInfo();
        img.color = color;
        scoreTxt.text =scoreDetails.score.ToString();
        playerName.text = playerInt;
		for (int i = 0; i < scoreDetails.numberOfKills; i++) 
        {
            Instantiate(killIcon,killGrid.transform);
		}
        for (int j = 0; j < scoreDetails.allDeaths.Count; j++) 
        {
            if(scoreDetails.allDeaths[j]==DeathType.ACCIDENT) Instantiate(suicideIcon,deathGrid.transform);
            else Instantiate(deathIcon,deathGrid.transform);
		}
    }

    private void ClearInfo()
    {
		for (int i = 0; i < killGrid.transform.childCount; i++) 
            {
            Destroy(killGrid.transform.GetChild(i).gameObject);
		}
        for (int i = 0; i < deathGrid.transform.childCount; i++) 
            {
            Destroy(deathGrid.transform.GetChild(i).gameObject);
		}
    }

	public void Disable() 
    {
		gameObject.SetActive(false);
	}
}
