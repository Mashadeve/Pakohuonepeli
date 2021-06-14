﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour
{
    private int speed = 8;
    private int multiplier = 100;
    
    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y + speed * multiplier * Time.deltaTime, transform.rotation.z));
    }
}
