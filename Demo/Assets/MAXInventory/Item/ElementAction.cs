using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class ElementAction : ICooldown
{
	public bool foldout;
	public GameObject activationObject;
	public string activationObjectName;
	public string activationMethodName;
	public bool repeatingInvoke;
	public bool currentlyRepeating;
	public int selectedComponent;
	public int selectedMethod;
	public int selectedField;
	public string fieldValue;
	public bool sendThisItem;
	public Color tooltipColor = Color.white;
	public Font tooltipFont;
	public FontStyle tooltipFontStyle;
	public bool hasCooldown;
	public bool OnCooldown { get; set; }
	public List<CooldownSettings> cooldownSettings = new List<CooldownSettings>();
	public int selectedElement;
	public bool cooldownsFoldout;
	public bool elementsFoldout;
	public int selectedType;
	public bool hasDuration;
	public float durationTime;
	public bool parameterFoldout;
	public MethodInfo cachedMethod;
	public FieldInfo cachedField;
	public int selectedOption;
	public string[] options = new string[2]{"Method", "Field"};
	public MonoBehaviour selectedMonoBehaviour;
	public string selectedMonoBehaviourName;
	public string selectedFieldName;
	public string selectedComponentName;
	public GameObject itemActionGO;
	public UnityEngine.UI.Text itemActionText;
	public bool destroyAfterUse;
	public bool useOnActivation;
	public bool useOnDeactivation;
	public bool activateOnEquip;
	public bool activateOnUnEquip;
	public bool useOnClick;
	public bool clickedOnByMouse1;
	public bool respondToMouse0;
	public bool respondToMouse1;
	public bool tooltipFoldout;
	public bool displayInTooltip;
	public bool customTooltip;
	public string tooltipText;
	public bool onHotkey;
	public CooldownManager cooldownGO {get;set;}
	public List<string> stringParameters = new List<string>();

	public IEnumerator StartCooldown(float time)
	{
		OnCooldown = true;
		
		yield return new WaitForSeconds (time);

		OnCooldown = false;
	}

#if UNITY_EDITOR

	public void OnGUI()
	{
		if(foldout)
		{
			EditorGUI.BeginChangeCheck ();
			GUILayout.BeginHorizontal ();
			GUILayout.Space (45);
			activationObject = EditorGUILayout.ObjectField("Object", activationObject, typeof(GameObject), true) as GameObject;
			GUILayout.EndHorizontal ();
			if(EditorGUI.EndChangeCheck ())
			{
				if(activationObject != null)
					activationObjectName = activationObject.name;
				else
					activationObjectName = "";
			}
			
			if(activationObject != null)
			{
				//Get Components
				MonoBehaviour[] components = activationObject.GetComponents<MonoBehaviour>();
				
				if(components != null)
				{
					if(components.Length > 0)
					{
						//Fix mismatching
						string[] componentNames = new string[components.Length];
						for(int k = 0; k < components.Length; k++)
						{
							if(components[k] != null)
								componentNames[k] = components[k].GetType ().ToString ();
							
							if(components[k].GetType ().ToString () == selectedComponentName)
								selectedComponent = k;
						}
						
						if(selectedComponent >= components.Length)
							selectedComponent = components.Length - 1;
						
						//Get Methods
						MethodInfo[] methods = components[selectedComponent].GetType ().GetMethods (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static);
						
						//Get Fields
						FieldInfo[] fields = components[selectedComponent].GetType ().GetFields ();
						
						GUILayout.BeginHorizontal ();
						GUILayout.Space (45);
						selectedComponent = EditorGUILayout.Popup ("Components", selectedComponent, componentNames);
						GUILayout.EndHorizontal ();
						
						if(selectedComponent >= components.Length)
						{
							if(components.Length <= 0)
								selectedComponent = 0;
							else
								selectedComponent = components.Length - 1;
						}
						
						if(selectedComponent < componentNames.Length)
							selectedComponentName = componentNames[selectedComponent];
						
						GUILayout.BeginHorizontal ();
						GUILayout.Space (45);
						selectedOption = EditorGUILayout.Popup ("Modify", selectedOption, options);
						GUILayout.EndHorizontal ();
						
						if(selectedOption == 0)
						{
							string[] methodNames = new string[methods.Length];
							for(int q = 0; q < methods.Length; q++)
							{
								methodNames[q] = methods[q].Name;
								
								//Fix Mismatched methods
								if(methodNames[q] == activationMethodName)
									selectedMethod = q;
							}
							
							GUILayout.BeginHorizontal ();
							GUILayout.Space (45);
							selectedMethod = EditorGUILayout.Popup ("Methods", selectedMethod, methodNames);
							GUILayout.EndHorizontal ();

							if(selectedMethod >= methodNames.Length)
							{
								if(methodNames.Length <= 0)
									selectedMethod = 0;
								else
									selectedMethod = methods.Length - 1;
							}
							
							if(selectedMethod < methodNames.Length)
								activationMethodName = methodNames[selectedMethod];
						}
						else if(selectedOption == 1)
						{
							string[] fieldNames = new string[fields.Length];
							for(int o = 0; o < fieldNames.Length; o++)
							{
								fieldNames[o] = fields[o].Name;
								
								if(fieldNames[o] == selectedFieldName)
									selectedField = o;
							}
							
							if(fieldNames.Length > 0)
							{
								GUILayout.BeginHorizontal ();
								GUILayout.Space (45);
								selectedField = EditorGUILayout.Popup ("Fields", selectedField, fieldNames);
								GUILayout.EndHorizontal ();
								
								if(selectedField >= fields.Length)
								{
									if(fieldNames.Length <= 0)
										selectedField = 0;
									else
										selectedField = fields.Length - 1;
								}
								
								if(selectedField < fieldNames.Length)
									selectedFieldName = fieldNames[selectedField];
								
								GUILayout.BeginHorizontal ();
								GUILayout.Space (45);
								fieldValue = EditorGUILayout.TextField ("Field +=", fieldValue);
								GUILayout.EndHorizontal ();
								
								GUILayout.BeginHorizontal ();
								GUILayout.Space (45);
								hasDuration = EditorGUILayout.Toggle ("Has Duration", hasDuration);
								GUILayout.EndHorizontal ();
								
								if(hasDuration)
								{
									GUILayout.BeginHorizontal ();
									GUILayout.Space (45);
									durationTime = EditorGUILayout.FloatField ("Duration Time", durationTime);
									GUILayout.EndHorizontal ();
								}
							}
							else
							{
								selectedFieldName = null;
								selectedField = 0;
							}
						}
					}
				}
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (45);
				repeatingInvoke = EditorGUILayout.Toggle("Invoke Repeatedly", repeatingInvoke);
				GUILayout.EndHorizontal ();
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (45);
				parameterFoldout = EditorGUILayout.Foldout (parameterFoldout, "Parameters");
				GUILayout.EndHorizontal ();
				
				if(parameterFoldout)
				{
					GUILayout.BeginHorizontal ();
					GUILayout.Space (60);
					sendThisItem = EditorGUILayout.Toggle ("Send Item", sendThisItem);
					GUILayout.EndHorizontal ();

					for(int i = 0; i < stringParameters.Count; i++)
					{
						GUILayout.BeginHorizontal ();
						GUILayout.Space (60);
						stringParameters[i] = EditorGUILayout.TextField ("String Parameter", stringParameters[i]);
						if(GUILayout.Button ("x"))
							stringParameters.RemoveAt(i);
						GUILayout.EndHorizontal ();
					}

					GUILayout.BeginHorizontal ();
					GUILayout.Space (60);
					if(GUILayout.Button("Add", EditorStyles.miniButton))
						stringParameters.Add (null);
					GUILayout.EndHorizontal ();

					string sig = "";
					
					if(sendThisItem)
						sig += "InventoryElement";
					if(stringParameters.Count > 0)
						stringParameters.ForEach(x => sig += " string");
					
					GUILayout.BeginHorizontal ();
					GUILayout.Space (60);
					EditorGUILayout.LabelField ("Signature", sig, EditorStyles.miniBoldLabel);
					GUILayout.EndHorizontal ();
				}
	
				GUILayout.BeginHorizontal ();
				GUILayout.Space (45);
				tooltipFoldout = EditorGUILayout.Foldout(tooltipFoldout, "Tooltip");
				GUILayout.EndHorizontal ();
				
				if(tooltipFoldout)
				{
					GUILayout.BeginHorizontal ();
					GUILayout.Space (60);
					displayInTooltip = EditorGUILayout.Toggle("Custom", displayInTooltip);
					GUILayout.EndHorizontal ();
					
					if(displayInTooltip)
					{
						GUILayout.BeginHorizontal ();
						GUILayout.Space (60);
						tooltipText = EditorGUILayout.TextField("Text", tooltipText);
						GUILayout.EndHorizontal ();
						
						GUILayout.BeginHorizontal ();
						GUILayout.Space (60);
						tooltipColor = EditorGUILayout.ColorField("Color", tooltipColor);
						GUILayout.EndHorizontal ();
					}
				}
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (45);
				EditorGUILayout.LabelField("Activate On", EditorStyles.boldLabel);
				GUILayout.EndHorizontal ();
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (60);
				onHotkey = EditorGUILayout.Toggle("On Slot Hotkey", onHotkey);
				GUILayout.EndHorizontal ();
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (60);
				useOnActivation = EditorGUILayout.Toggle("Slot Activation", useOnActivation);
				GUILayout.EndHorizontal ();
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (60);
				useOnDeactivation = EditorGUILayout.Toggle("Slot De-Activation", useOnDeactivation);
				GUILayout.EndHorizontal ();
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (60);
				respondToMouse0 = EditorGUILayout.Toggle("Slot Activated + Mouse 0 Click", respondToMouse0);
				GUILayout.EndHorizontal ();
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (60);
				respondToMouse1 = EditorGUILayout.Toggle("Slot Activated + Mouse 1 Click", respondToMouse1);
				GUILayout.EndHorizontal ();
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (60);
				activateOnEquip = EditorGUILayout.Toggle("On Equip", activateOnEquip);
				GUILayout.EndHorizontal ();
				
				if(selectedOption == 0)
				{
					GUILayout.BeginHorizontal ();
					GUILayout.Space (60);
					activateOnUnEquip = EditorGUILayout.Toggle("On UnEquip", activateOnUnEquip);
					GUILayout.EndHorizontal ();
				}
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (60);
				clickedOnByMouse1 = EditorGUILayout.Toggle("Clicked on by Mouse 1", clickedOnByMouse1);
				GUILayout.EndHorizontal ();
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (45);
				EditorGUILayout.LabelField("Post-Activation", EditorStyles.boldLabel);
				GUILayout.EndHorizontal ();
				
				GUILayout.BeginHorizontal ();
				GUILayout.Space (60);
				destroyAfterUse = EditorGUILayout.Toggle("Destroy Item", destroyAfterUse);
				GUILayout.EndHorizontal ();
			}
			else
				activationObjectName = "";
		}
	}
#endif
}
