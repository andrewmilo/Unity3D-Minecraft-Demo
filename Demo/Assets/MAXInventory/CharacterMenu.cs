using UnityEngine;
using System.Collections;

public class CharacterMenu : InventoryObject {

	void OnEnable()
	{
		InventoryManager.characterMenu = this;
	}

	protected override void onShiftClick (InventoryElement element)
	{

	}
}
