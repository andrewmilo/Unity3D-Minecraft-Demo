#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;

public class Menu : MonoBehaviour 
{
	[MenuItem ("Premier Inventory/Item Database")]
	static void InventoryDatabase () {
		InventoryDatabaseWindow.Init ();
	}
}
#endif