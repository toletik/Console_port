using Com.IsartDigital.Common.UI;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : UIScreen
{
	[SerializeField] private PlayerTagParameters playertag = default;
	[SerializeField] private List<ContainerRank> RankContainerList = default;

	public void UpdateRanking(List<Player> playerList)
	{
		for (int i = RankContainerList.Count-1; i > playerList.Count-1; i--) {
			RankContainerList[i].Disable();
		}
		for (int j = 0; j < playerList.Count; j++) {
			RankContainerList[j].ChangeRankInformation(playerList[j].Score.ToString(),(j+1).ToString(), playertag.GetColorAtIndex(j));
		}
	}
}
