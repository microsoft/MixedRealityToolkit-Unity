// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomListView.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

using UnityEngine;

using Photon.Realtime;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// Region list UI View.
    /// </summary>
	public class RegionListView : MonoBehaviour
    {

        public RegionListCell CellPrototype;

		Dictionary<string, RegionListCell> regionCellList = new Dictionary<string, RegionListCell>();


        public void OnEnable()
        {
            ResetList();

            CellPrototype.gameObject.SetActive(false);
        }

        public void OnRegionListUpdate(List<Region> regionList)
        {
			int i = 0;
			foreach (Region entry in regionList)
            {
                // we create the cell
				regionCellList[entry.Code] = Instantiate(CellPrototype);
				regionCellList[entry.Code].gameObject.SetActive(true);
				regionCellList[entry.Code].transform.SetParent(CellPrototype.transform.parent, false);
				regionCellList[entry.Code].AddToList(entry,i);

				i++;
            }

        }

        public void ResetList()
        {
			foreach (KeyValuePair<string, RegionListCell> entry in regionCellList)
            {

                if (entry.Value != null)
                {
                    Destroy(entry.Value.gameObject);
                }

            }
			regionCellList = new Dictionary<string, RegionListCell>();
        }
    }
}