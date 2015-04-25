using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LootingScript : MonoBehaviour 
{
	public bool firstPerson;
	public bool thirdPerson;
	public Camera cameraComponent;
	public float distance = 10f;
	public KeyCode lootKey = KeyCode.F;
	public LayerMask layerMask;
	public List<InventoryObject> priority = new List<InventoryObject>();
	private RaycastHit hit;
	private Ray ray;

	InventoryElement temp;
	
	// Use this for initialization
	void Start () {
		
		if(cameraComponent == null)
			cameraComponent = InventoryManager.Instance.cameraComponent;
		
		layerMask = LayerMask.NameToLayer("Everything");
		
		if(priority.Count == 0)
		{
			priority.Add (FindObjectOfType<Inventory>());
			priority.Add (FindObjectOfType<ActionBar>());
		}
	}

	void Raycast()
	{
		if(firstPerson)
		{
			if(Physics.Raycast (transform.position, cameraComponent.transform.forward, out hit, distance, 1))
				Loot ();
		}
		else if(thirdPerson)
		{
			if(Physics.Raycast (ray, out hit, distance, 1))
				Loot ();
		}
	}

	void Loot()
	{
		if(hit.transform.gameObject != null)
		{
			GameObject possibleItem = hit.transform.gameObject;
			
			LootableObject cache = possibleItem.GetComponent<LootableObject>();
			
			if(cache != null)
			{
				InventoryElement invElem = InventoryDatabase.GetElement (cache.elementID);
					
				temp = new InventoryElement(invElem);
					
				if(temp != null)
				{
					temp.stack = cache.stack;
					
					if(priority.Count > 0)
					{
						foreach(InventoryObject invOb in priority)
						{
							if(invOb != null)
							{
								if(temp.stack > 0)
									invOb.AddItem (ref temp, false);
								
								if(temp.stack == 0)
								{
									Destroy (cache.gameObject);
									break;
								}
							}
						}
					}
					else
						Debug.Log ("Set up the priority system in FirstPersonLooting!");
				}
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Input.GetKeyDown (lootKey))
			Raycast ();
	}
}
