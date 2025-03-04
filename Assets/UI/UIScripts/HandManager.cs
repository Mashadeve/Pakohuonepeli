﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandManager : MonoBehaviour
{
    public GameObject currentInterObj = null;

    public GameObject currentPickObj = null;

    public Pickup currentPickupScript = null;

    public Inventory inventory; 


    //public GameObject itemButton;
    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Hand").GetComponent<Inventory>();
        
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentInterObj) { 
            if (currentPickupScript.inventory)
            {
                currentPickupScript.Additem(currentInterObj);
            }
        }
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            currentInterObj = other.gameObject;
            currentPickupScript = currentInterObj.GetComponent<Pickup>();
            
        }
        else if (other.CompareTag("Minigame"))
        {
            currentPickObj = other.gameObject;

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            if (other.gameObject == currentInterObj)
            {
                currentInterObj = null;
                currentPickupScript = null;
            }
        }
        else if (other.CompareTag("Minigame"))
        {
            currentPickObj = null;
        }
    }
   
}
