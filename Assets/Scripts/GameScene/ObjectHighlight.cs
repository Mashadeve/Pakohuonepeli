﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectHighlight : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private Color stroke;
    private bool highlightObject;
    public bool mouseOnObject;
    


    void Start()
    {

    }
    private void Update()
    {
        if (!highlightObject)
        {
            highlight.SetActive(false);
        }

        if (highlightObject)
        {
            highlight.SetActive(true);
            
        }
    }
    private void OnMouseEnter()
    {
        highlightObject = true;
        mouseOnObject = true;
        highlight.GetComponent<SpriteRenderer>().color = stroke;

    }
    private void OnMouseExit()
    {
        highlightObject = false;
        mouseOnObject = false;
    }

    private void OnMouseDown()
    {
        if (mouseOnObject && GameObject.FindGameObjectWithTag("Locker"))
        {
            SceneManager.LoadScene(1);
        }
    }
}