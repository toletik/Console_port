using Com.IsartDigital.Common.UI;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : UIScreen
{
	[SerializeField] private PlayerTagParameters playertag = default;
	[SerializeField] private List<ContainerRank> RankContainerList = default;

	public void UpdateRanking()
	{
		/*for (int i = RankContainerList.count; i > length-1; i--) {
			RankContainerList[i].Disable();
		}*/
	}
}
