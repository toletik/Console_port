using Com.IsartDigital.Common.UI;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : UIScreen
{
	[SerializeField] private PlayerTagParameters playertag = default;
	[SerializeField] private List<ContainerRank> RankContainerList = default;
	private int lastRank = default;
	private int index = default;
	public void UpdateRanking(List<Player> playerList)
	{
		for (int i = RankContainerList.Count-1; i > playerList.Count-1; i--) {
			RankContainerList[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < playerList.Count; j++) {

			RankContainerList[j].gameObject.SetActive(true);
			if(lastRank!= 0 && lastRank== playerList[j].ScoreDetails.rank) index = j;
			else index =playerList[j].ScoreDetails.rank-1;

			RankContainerList[index].ChangeRankInformation(playerList[j].playerID.ToString(), playertag.GetColorAtIndex(j),playerList[j].ScoreDetails);
			lastRank=playerList[j].ScoreDetails.rank;
		}
	}

	public void Replay()
	{
		lastRank=0;
		Debug.Log("REPLAY");
		UIManager.Instance.AddScreen<LevelSelectionScreen>(true);
	}

	public void ReturnToMenu()
	{
		lastRank=0;
		UIManager.Instance.AddScreen<TitleCard>(true);
	}
}
