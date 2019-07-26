﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    private GameObject[] inventorySlots = new GameObject[11];
    private GameObject world;
    private GameObject player;
    private int selected = 0;
    private Color selectedColor = new Color32(255, 237, 0, 100);
    private Color slotColor = new Color32(255, 255, 255, 100);

    private void Awake()
    {
        world = GameObject.Find("World");
        player = GameObject.Find("Player");
        inventorySlots[0] = (GameObject)Resources.Load("Prefab/Pickaxe");
        inventorySlots[1] = (GameObject)Resources.Load("Prefab/Laser");
        inventorySlots[2] = (GameObject)Resources.Load("Prefab/Melee");
        inventorySlots[3] = (GameObject)Resources.Load("Prefab/AOE");
        inventorySlots[4] = (GameObject)Resources.Load("Prefab/Single");
        inventorySlots[5] = (GameObject)Resources.Load("Prefab/Projectile");
        inventorySlots[6] = (GameObject)Resources.Load("Prefab/Dash");
    }


    private void Start()
    {

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null) continue;
            inventorySlots[i] = Instantiate(inventorySlots[i], player.transform);
            inventorySlots[i].SetActive(false);
            Sprite mySprite = inventorySlots[i].GetComponent<SpriteRenderer>().sprite;
            transform.GetChild(i).Find("Item").GetComponent<UnityEngine.UI.Image>().enabled = true;
            transform.GetChild(i).Find("Item").GetComponent<UnityEngine.UI.Image>().sprite = mySprite;
        }
    }

    public void SelectSlot(int position)
    {
        transform.GetChild(selected).GetComponent<UnityEngine.UI.Image>().color = slotColor;
        selected = position;
        transform.GetChild(selected).GetComponent<UnityEngine.UI.Image>().color = selectedColor;
    }

    public bool AddItem(GameObject pickUp)
    {
        for(int i=0; i<inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null) continue;
            Sprite mySprite = inventorySlots[i].GetComponent<SpriteRenderer>().sprite;
            Sprite newSprite = pickUp.GetComponent<SpriteRenderer>().sprite;
            if (mySprite == newSprite)
            {
                Destroy(pickUp);
                int amount = Convert.ToInt32(transform.GetChild(i).Find("Amount").GetComponent<UnityEngine.UI.Text>().text);
                amount += 1;
                transform.GetChild(i).Find("Amount").GetComponent<UnityEngine.UI.Text>().text = amount.ToString();
                return true;
            }
        }
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null)
            {
                inventorySlots[i] = pickUp;
                transform.GetChild(i).Find("Amount").GetComponent<UnityEngine.UI.Text>().text = "1";
                transform.GetChild(i).Find("Item").GetComponent<UnityEngine.UI.Image>().enabled = true;
                transform.GetChild(i).Find("Item").GetComponent<UnityEngine.UI.Image>().sprite = pickUp.GetComponent<SpriteRenderer>().sprite;
                return true;
            }
        }
        return false;
    }

    public bool ItemSelected()
    {
        if (inventorySlots[selected] != null)
        {
            return true;
        }
        return false;
    }

    public GameObject RemoveItem()
    {
        GameObject item;

        if (inventorySlots[selected] != null)
        {
            int amount = Convert.ToInt32(transform.GetChild(selected).Find("Amount").GetComponent<UnityEngine.UI.Text>().text);
            amount -= 1;
            transform.GetChild(selected).Find("Amount").GetComponent<UnityEngine.UI.Text>().text = amount.ToString();
            if (amount == 0)
            {
                item = inventorySlots[selected];
                inventorySlots[selected] = null;
                transform.GetChild(selected).Find("Item").GetComponent<UnityEngine.UI.Image>().enabled = false;
                transform.GetChild(selected).Find("Amount").GetComponent<UnityEngine.UI.Text>().text = "";
                return item;
            }
            return Instantiate(inventorySlots[selected], world.transform);
        }
        return null;
    }

    public GameObject GetSelected()
    {
        return inventorySlots[selected];
    }

    

    public void UseSelected()
    {
        if (inventorySlots[selected] == null) return;
        Text cooldownUI = transform.GetChild(selected).Find("Cooldown").GetComponent<Text>();
        if (cooldownUI.text != "") return;
        inventorySlots[selected].GetComponent<Ability>().Use();
        cooldownUI.text = "" + inventorySlots[selected].GetComponent<Ability>().CoolDown();
    }


    public void DecreaseCooldowns()
    {
        for(int i=0; i<inventorySlots.Length; i++)
        {
            Text cooldownUI = transform.GetChild(i).Find("Cooldown").GetComponent<Text>();
            if (cooldownUI.text != "")
            {
                int cooldown = Convert.ToInt32(cooldownUI.text);
                cooldownUI.text = "" + (cooldown - 1);
                if (cooldownUI.text == "0") cooldownUI.text = "";
            }
        }
    }

    public int GetSelectedRange()
    {
        if (inventorySlots[selected] != null)
        {
            return inventorySlots[selected].GetComponent<Ability>().GetRange();
        }

        return 0;
    }
}
