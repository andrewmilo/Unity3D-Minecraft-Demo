using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public class InventoryDatabaseWindow : EditorWindow{

	private bool actionInfoFoldout;
	private Vector2 listScrollPos;
	private Vector2 mainScrollPos;
	private string searchString;
	private bool searchChanged;
	private int selectedAddition;

	[NonSerialized]
	private InventoryElement editItem;
	[NonSerialized]
	private InventoryElement selectedItem;
	[NonSerialized]
	private ElementType editType;
	[NonSerialized]
	private ElementType selectedType;

	public enum ListState
	{
		DEFAULT,
		SEARCHITEMS
	}
	
	public enum EditState
	{
		EMPTY,
		EDITITEM,
		EDITTYPE,
		PLAYING
	}
	
	private EditState editState;
	private ListState listState;
	private GameObject prefab;

	[MenuItem("Database/Inventory Database %#w")]
	public static void Init () 
	{
		InventoryDatabaseWindow window = EditorWindow.GetWindow<InventoryDatabaseWindow>();
		window.Show ();
		window.autoRepaintOnSceneChange = true;
	}

	public void OnFocus()
	{
		EditorApplication.MarkSceneDirty ();
	}

	void OnGUI() 
	{
		EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
		ListArea();
		MainArea();
		EditorGUILayout.EndHorizontal();
		
		if(Application.isPlaying)
			editState = EditState.PLAYING;
		
		if (GUI.changed) 
		{
			EditorUtility.SetDirty (InventoryDatabase.Instance);

			if(prefab == null)
				prefab = Resources.Load ("InventoryDatabase") as GameObject;

			if(prefab != null)
				PrefabUtility.ReplacePrefab(InventoryDatabase.Instance.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
		}

		this.Repaint();
	}
	
	void ListArea() 
	{
		EditorGUILayout.BeginVertical(GUILayout.Width(250));
		EditorGUILayout.Space();
		listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos, "box", GUILayout.ExpandHeight(true));
		EditorGUI.BeginChangeCheck ();
		EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
		GUILayout.Label ("Search");
		searchString = EditorGUILayout.TextField (searchString);
		EditorGUILayout.EndHorizontal();
		searchChanged = EditorGUI.EndChangeCheck();
		
		if(searchString != "")
		{
			if(searchChanged)
				listState =	ListState.SEARCHITEMS;
		}
		else
		{
			if(listState == ListState.SEARCHITEMS)
			{
				listState = ListState.DEFAULT;
				editState = EditState.EMPTY;
			}
		}
		GUILayout.Space (15);
		GUIStyle gs = new GUIStyle();
		gs.fontStyle = FontStyle.BoldAndItalic;
		gs.alignment = TextAnchor.MiddleCenter;

		GUIStyle gs2 = new GUIStyle();
		gs2.fontStyle = FontStyle.BoldAndItalic;
		gs2.fontSize = 10;
		gs2.alignment = TextAnchor.MiddleCenter;

		switch(listState)
		{
		case ListState.DEFAULT:
			if(selectedType != null)
			{
				if(GUILayout.Button("Back", GUILayout.ExpandWidth(true))) 
				{
					GUI.FocusControl (null);
					selectedType = InventoryDatabase.GetElementType(selectedType.parentID);
					editState = EditState.EMPTY;
					listState = ListState.DEFAULT;
				}
			}
			if(selectedType != null)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Label (selectedType.name + "/...", gs,GUILayout.ExpandWidth(true));
				GUILayout.EndHorizontal ();

				DisplayItems();
			}
			if(selectedType != null)
			{
				if(selectedType.GetSubTypes().Count > 0)
					GUILayout.Label ("Types");
			}

			DisplayItemTypes ();
			break;
		case ListState.SEARCHITEMS:
			Search ();
			break;
		}

		EditorGUILayout.EndScrollView();
		UnderList ();
		EditorGUILayout.Space();
		EditorGUILayout.EndVertical();
	}

	void MainArea() 
	{
		EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
		mainScrollPos = EditorGUILayout.BeginScrollView (mainScrollPos);
		EditorGUILayout.Space();
		
		switch(editState)
		{
		case EditState.EDITITEM:
			EditElement (editItem);
			break;
		case EditState.EDITTYPE:
			EditItemType ();
			break;
		case EditState.EMPTY:
			break;
		case EditState.PLAYING:
			EditorGUILayout.HelpBox ("Cannot edit items during Play Mode", MessageType.Info);
			break;
		}
		
		EditorGUILayout.EndScrollView ();
		EditorGUILayout.EndVertical();
	}

	void Search()
	{
		if(InventoryDatabase.ElementCount > 0)
			GUILayout.Label ("Elements", EditorStyles.boldLabel);
		
		for(int i = 0; i < InventoryDatabase.ElementCount; i++)
		{
			InventoryElement item = InventoryDatabase.GetElement(i);

			if(item != null)
			{
				if(item.name.IndexOf (searchString, StringComparison.CurrentCultureIgnoreCase) != -1)
				{
					if(GUILayout.Button (item.name))
					{
						editItem = item;
						editState = EditState.EDITITEM;
					}
				}
			}
		}
		
		if(InventoryDatabase.ElementTypeCount > 0)
			GUILayout.Label ("Types", EditorStyles.boldLabel);

		for(int i = 0; i < InventoryDatabase.ElementTypeCount; i++)
		{
			ElementType elemType = InventoryDatabase.GetElementType(i);

			if(elemType != null)
			{
				if(elemType.name != null)
				{
					if(elemType.name.IndexOf (searchString, StringComparison.CurrentCultureIgnoreCase) != -1)
					{
						if(GUILayout.Button (elemType.name))
						{
							editType = elemType;
							editState = EditState.EDITTYPE;
						}
					}
				}
			}
		}
	}
	
	void DisplayItems()
	{
		if(selectedType != null)
		{
			//for(int i = 0; i < InventoryDatabase.ElementCount; i++)
			for(int i = 0; i < selectedType.elementIDs.Count; i++)
			{
				InventoryElement item = InventoryDatabase.GetElement(selectedType.elementIDs[i]);

				if(item != null)
				{
					if(item.type != null)
					{
						//if(selectedType.ID == item.type.ID)
						{
							EditorGUILayout.BeginHorizontal();
							if(GUILayout.Button(item.name, GUILayout.ExpandWidth(true))) 
							{
								GUI.FocusControl (null);
								editItem = item;
								editState = EditState.EDITITEM;
							}
							EditorGUILayout.EndHorizontal ();
						}
					}
				}
			}
		}
	}
	
	void DisplayItemTypes()
	{
		if(selectedType != null)
		{
			foreach(int i in selectedType.childrenIDs)
			{
				ElementType e = InventoryDatabase.GetElementType(i);

				if(e != null)
				{
					EditorGUILayout.BeginHorizontal();
					if(GUILayout.Button(e.name, GUILayout.ExpandWidth(true)))
					{
						GUI.FocusControl (null);
						editType = e;
						editState = EditState.EDITTYPE;
					}

					if(GUILayout.Button (">",  GUILayout.MaxWidth (25)))
					{
						GUI.FocusControl (null);
						selectedType = e;
						listState = ListState.DEFAULT;
						editState = EditState.EMPTY;
					}
					EditorGUILayout.EndHorizontal ();
				}
			}
		}
		else
		{
			for(int n = 0; n < InventoryDatabase.ElementTypeCount; n++)
			{
				ElementType et = InventoryDatabase.GetElementType(n);

				if(et != null)
				{
					EditorGUILayout.BeginHorizontal();
					if(et.ID > -1 && et.parentID == -1)
					{
						if(GUILayout.Button(et.name, GUILayout.ExpandWidth(true)))
						{
							GUI.FocusControl (null);
							editType = et;
							editState = EditState.EDITTYPE;
						}
						if(GUILayout.Button (">",  GUILayout.MaxWidth (25)))
						{
							GUI.FocusControl (null);
							selectedType = et;
							listState = ListState.DEFAULT;
							editState = EditState.EMPTY;
						}
					}
					EditorGUILayout.EndHorizontal ();
				}
			}
		}
	}
	
	void UnderList()
	{
		EditorGUILayout.BeginHorizontal();
		switch(listState)
		{
		case ListState.DEFAULT:
			if(GUILayout.Button ("Add Type"))
			{
				GUI.FocusControl (null);
				ElementType e = new ElementType();

				if(selectedType != null)
					e.parentID = selectedType.ID;

				InventoryDatabase.Add (e, selectedType);

				editType = e;
				editState = EditState.EDITTYPE;
			}

			if(selectedType != null)
			{
				if(GUILayout.Button ("Add Element"))
				{
					GUI.FocusControl (null);
					InventoryElement invEl = new InventoryElement();
					InventoryDatabase.Add (invEl, selectedType);

					selectedItem = invEl;
					editItem = invEl;
					editState = EditState.EDITITEM;
				}
			}
			break;
		}
		EditorGUILayout.EndHorizontal();
	}
	
	void EditItemType()
	{
		if(editType != null)
		{
			editType.name = EditorGUILayout.TextField ("Name", editType.name);
			editType.tooltipColor = EditorGUILayout.ColorField("Tooltip Color", editType.tooltipColor);

			GUILayout.BeginHorizontal ();
			editType.windowActionFoldout = EditorGUILayout.Foldout (editType.windowActionFoldout, "Action Management");
			GUILayout.EndHorizontal ();
			
			if(editType.windowActionFoldout)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (15);
				actionInfoFoldout = EditorGUILayout.Foldout (actionInfoFoldout, "Info");
				GUILayout.EndHorizontal ();
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (15);
				if(actionInfoFoldout)
				{
					EditorGUILayout.HelpBox ("When you choose an Object for an action, make sure you choose an Object in the current scene" +
					                         " if you need to work with an instance of a script. Example: If you need to affect your character's health, then " +
					                         "select the object that has the Health script in the Scene as opposed to the Assets.", MessageType.Info);
				}
				GUILayout.EndHorizontal ();
				
				for(int m = 0; m < editType.actions.Count; m++)
				{
					ElementAction itemAction = editType.actions[m];
					
					if(itemAction != null)
					{
						GUILayout.BeginHorizontal ();
						GUILayout.Space (30);
						itemAction.foldout = EditorGUILayout.Foldout (itemAction.foldout, "Action " + m.ToString ());
						if(GUILayout.Button ("x", EditorStyles.miniButton))
							editType.actions.RemoveAt (m);
						GUILayout.EndHorizontal ();

						itemAction.OnGUI ();
					}
				}
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (30);
				if(GUILayout.Button ("Add Action"))
					editType.actions.Add (new ElementAction());
				GUILayout.EndHorizontal ();
			}

			GUILayout.Space (10);

			editType.deleteFoldout = EditorGUILayout.Foldout (editType.deleteFoldout, "Delete");
			
			if(editType.deleteFoldout)
			{
				editType.areYouSure = EditorGUILayout.Toggle ("Are you sure?", editType.areYouSure);
				if(editType.areYouSure)
				{
					editType.areYouSure2 = EditorGUILayout.Toggle ("Are you REALLY sure?", editType.areYouSure2);
					if(editType.areYouSure2)
					{
						if(GUILayout.Button ("Delete"))
						{
							GUI.FocusControl (null);
							editState = EditState.EMPTY;

							InventoryDatabase.Remove (editType);
						}
					}
				}
			}
		}
	}

	void EditElement(InventoryElement inventoryElement) 
	{
		if(inventoryElement != null)
		{
			GUILayout.BeginHorizontal ();
			GUILayout.Space (15);
			inventoryElement.name = EditorGUILayout.TextField ("Name", inventoryElement.name);
			GUILayout.EndHorizontal ();
			
			GUILayout.BeginHorizontal ();
			GUILayout.Space (15);
			inventoryElement.description = EditorGUILayout.TextField ("Description", inventoryElement.description);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Space (15);
			inventoryElement.gameObject = EditorGUILayout.ObjectField ("GameObject", inventoryElement.gameObject, typeof(GameObject), true) as GameObject;
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Space (15);
			inventoryElement.isStackable = EditorGUILayout.Toggle ("Is Stackable", inventoryElement.isStackable);
			GUILayout.EndHorizontal ();

			if(inventoryElement.isStackable)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (15);
				inventoryElement.maxStack = EditorGUILayout.IntField ("Max Stack", inventoryElement.maxStack);
				GUILayout.EndHorizontal ();
			}

			GUILayout.BeginHorizontal ();
			GUILayout.Space (15);
			inventoryElement.nameColor = EditorGUILayout.ColorField ("Name Tooltip Color", inventoryElement.nameColor);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Space (15);
			inventoryElement.descriptionColor = EditorGUILayout.ColorField ("Description Tooltip Color", inventoryElement.descriptionColor);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Space (15);
			inventoryElement.icon = EditorGUILayout.ObjectField ("Icon", inventoryElement.icon, typeof(Texture), true) as Texture;
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Space (15);
			inventoryElement.windowActionFoldout = EditorGUILayout.Foldout (inventoryElement.windowActionFoldout, "Action Management");
			GUILayout.EndHorizontal ();

			if(inventoryElement.windowActionFoldout)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Space (30);
				actionInfoFoldout = EditorGUILayout.Foldout (actionInfoFoldout, "Info");
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Space (30);
				if(actionInfoFoldout)
				{
					EditorGUILayout.HelpBox ("When you choose an Object for an action, make sure you choose an Object in the current scene" +
						" if you need to work with an instance of a script. Example: If you need to affect your character's health, then " +
						"select the object that has the Health script in the Scene as opposed to the Assets.", MessageType.Info);
				}
				GUILayout.EndHorizontal ();

				for(int m = 0; m < inventoryElement.actions.Count; m++)
				{
					ElementAction itemAction = inventoryElement.actions[m];
					
					if(itemAction != null)
					{
						GUILayout.BeginHorizontal ();
						GUILayout.Space (30);
						itemAction.foldout = EditorGUILayout.Foldout (itemAction.foldout, "Action " + m.ToString ());
						if(GUILayout.Button ("x"))
							inventoryElement.actions.RemoveAt (m);
						GUILayout.EndHorizontal ();

						itemAction.OnGUI ();
					}
				}
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (30);
				if(GUILayout.Button ("Add Action"))
					inventoryElement.actions.Add (new ElementAction());
				GUILayout.EndHorizontal ();
			}
	
			GUIStyle gs = new GUIStyle();
			gs.alignment = TextAnchor.MiddleCenter;

			GUILayout.Space (10);
			editItem.deleteFoldout = EditorGUILayout.Foldout (editItem.deleteFoldout, "Delete");

			if(editItem.deleteFoldout)
			{
				editItem.areYouSure = EditorGUILayout.Toggle ("Are you sure?", editItem.areYouSure);
				if(editItem.areYouSure)
				{
					editItem.areYouSure2 = EditorGUILayout.Toggle ("Are you REALLY sure?", editItem.areYouSure2);
					if(editItem.areYouSure2)
					{
						if(GUILayout.Button ("Delete"))
						{
							GUI.FocusControl (null);
							editState = EditState.EMPTY;

							if(InventoryManager.Instance != null)
							{
								for(int i = 0; i < InventoryManager.Instance.allInventoryObjects.Count; i++)
								{
									for(int m = 0; m < InventoryManager.Instance.allInventoryObjects[i].Slots.Count; m++)
									{
										if(InventoryManager.Instance.allInventoryObjects[i].Slots[m].inventoryElement.id == editItem.id)
											InventoryManager.Instance.allInventoryObjects[i].Slots[m].inventoryElement = new InventoryElement();
									}
								}
							}

							if(InventoryDatabase.Instance != null)
							{
								if(editItem.id != -1)
									InventoryDatabase.Remove (editItem);
							}
						}
					}
				}
			}
		}
	}
}