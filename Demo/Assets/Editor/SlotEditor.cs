using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(Slot))]
public class SlotEditor : Editor 
{
	private List<string> items = new List<string>();
	private List<string> itemTypes = new List<string>();

	public override void OnInspectorGUI()
	{
		Slot slot = (Slot)target;
		serializedObject.Update ();

		items.Clear ();
		items.Add ("None");

		for(int a = 0; a < InventoryDatabase.ElementCount; a++)
		{
			InventoryElement element = InventoryDatabase.GetElement(a);

			if(element != null)
			{
				if(element.id != -1)
					items.Add(element.name);
			}
		}

		itemTypes.Clear ();
		itemTypes.Add ("None");

		for(int i = 0; i < InventoryDatabase.ElementTypeCount; i++)
		{
			ElementType it = InventoryDatabase.GetElementType(i);

			if(it.name != "")
				itemTypes.Add (it.name);
		}

		GUILayout.BeginHorizontal();
		slot.itemTypesFoldout = EditorGUILayout.Foldout (slot.itemTypesFoldout, "Accepted Element Types");
		GUILayout.EndHorizontal();

		if(slot.itemTypesFoldout)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			slot.selectedItemType = EditorGUILayout.Popup ("Element Types", slot.selectedItemType, itemTypes.ToArray ());
			if(GUILayout.Button ("Add", EditorStyles.miniButton) && slot.selectedItemType > 0)
			{
				ElementType elementType = InventoryDatabase.FindElementType(itemTypes[slot.selectedItemType]);
				slot.acceptedTypes.Add (elementType);
			}

			GUILayout.EndHorizontal();
		}
		for(int b = 0; b < slot.acceptedTypes.Count; b++)
		{
			string it = slot.acceptedTypes[b].name;

			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			GUILayout.Space (15);
			EditorGUILayout.LabelField (b.ToString (), it);
			if(GUILayout.Button ("-", EditorStyles.miniButton))
			{
				slot.acceptedTypes.RemoveAt (b);
			}
			GUILayout.EndHorizontal();
		}

		GUILayout.BeginHorizontal();
		slot.itemFoldout = EditorGUILayout.Foldout(slot.itemFoldout, "Element");
		GUILayout.EndHorizontal();

		if(slot.itemFoldout)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			if(slot.inventoryElement != null)
				EditorGUILayout.LabelField ("Current", slot.inventoryElement.name);
			else
				EditorGUILayout.LabelField ("Current", "None", EditorStyles.boldLabel);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			slot.lockItem = EditorGUILayout.Toggle("Lock In Slot", slot.lockItem);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			slot.itemSelection = EditorGUILayout.Popup ("Add", slot.itemSelection, items.ToArray ());

			if(GUILayout.Button ("Add", EditorStyles.miniButton))
			{
				if(slot.itemSelection == 0)
					slot.inventoryElement = new InventoryElement();
				else if(slot.inventoryElement.name == "" || slot.inventoryElement.id != InventoryDatabase.FindElement(items[slot.itemSelection]).id)
					slot.inventoryElement = new InventoryElement(InventoryDatabase.FindElement(items[slot.itemSelection]));
				else
				{
					if(slot.inventoryElement.stack < slot.inventoryElement.maxStack)
						slot.inventoryElement.stack++;
				}
				if(slot.inventoryObject != null)
					slot.inventoryObject.Save (slot.inventoryObject.GetType ().ToString ());
			}
			GUILayout.EndHorizontal();
		}

		GUILayout.BeginHorizontal();
		slot.slotActivationSettingsFoldout = EditorGUILayout.Foldout(slot.slotActivationSettingsFoldout, "Hotkey Settings");
		GUILayout.EndHorizontal();

		if(slot.slotActivationSettingsFoldout)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("activationCharacterText"), new GUIContent("Text"));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			slot.activationCharacterFoldout = EditorGUILayout.Foldout(slot.activationCharacterFoldout, "Hotkey Character");
			GUILayout.EndHorizontal();

			if(slot.activationCharacterFoldout)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (30);
				slot.activationInt = EditorGUILayout.Popup ("Keys", slot.activationInt, slot.activationCharacters);
				GUILayout.EndHorizontal ();
			}

				GUILayout.BeginHorizontal();
				GUILayout.Space (15);
				slot.activationResponseFoldout = EditorGUILayout.Foldout(slot.activationResponseFoldout, "Response");
				GUILayout.EndHorizontal();
			
				if(slot.activationResponseFoldout)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Space (30);
					slot.ifActivateOnHotkey = EditorGUILayout.Toggle("Activate Slot", slot.ifActivateOnHotkey);
					GUILayout.EndHorizontal();

				if(slot.ifActivateOnHotkey)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Space (30);
					slot.changeSize = EditorGUILayout.Toggle("Change Size", slot.changeSize);
					GUILayout.EndHorizontal();
					
					if(slot.changeSize)
					{
						GUILayout.BeginHorizontal();
						GUILayout.Space (30);
						slot.changeSizeVector2 = EditorGUILayout.Vector2Field("Size", slot.changeSizeVector2);
						GUILayout.EndHorizontal();
					}
					
					GUILayout.BeginHorizontal();
					GUILayout.Space (30);
					slot.changeTexture = EditorGUILayout.Toggle("Change Texture", slot.changeTexture);
					GUILayout.EndHorizontal();
					
					if(slot.changeTexture)
					{
						GUILayout.BeginHorizontal ();
						GUILayout.Space (30);
						slot.changeTextureImage = (Texture) EditorGUILayout.ObjectField ("Texture", slot.changeTextureImage, typeof(Texture), true);
						GUILayout.EndHorizontal ();
					}
				}
			}
		}

		GUILayout.BeginHorizontal();
		slot.backgroundFoldout = EditorGUILayout.Foldout(slot.backgroundFoldout, "Background");
		GUILayout.EndHorizontal();
		
		if(slot.backgroundFoldout)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("backgroundRawImage"), new GUIContent("Activation Character"));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			slot.backgroundRawImage.texture = (Texture) EditorGUILayout.ObjectField("Background", slot.backgroundRawImage.texture, typeof(Texture), true);
			GUILayout.EndHorizontal();
		}

		GUILayout.BeginHorizontal();
		slot.slotTextFoldout = EditorGUILayout.Foldout(slot.slotTextFoldout, "Text");
		GUILayout.EndHorizontal();

		if(slot.slotTextFoldout)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			slot.disableTextIfItem = EditorGUILayout.Toggle("Disable Text if Element Exists", slot.disableTextIfItem);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Space (15);
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("slotText"), new GUIContent("Activation Character"));
			GUILayout.EndHorizontal();
		}
		serializedObject.ApplyModifiedProperties ();

		if(GUI.changed)
			EditorUtility.SetDirty(target);
	}
}
