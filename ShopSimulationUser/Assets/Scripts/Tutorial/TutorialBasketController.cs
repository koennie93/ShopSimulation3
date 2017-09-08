﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBasketController : MonoBehaviour {

    public GameObject tutorialManagerGameObject;
    TutorialManager tutorialManager;

    bool isColliding;
    GroceryDataHandler gData;

    Vector3 firstPos;

    private void Start()
    {
        tutorialManager = tutorialManagerGameObject.GetComponent<TutorialManager>();
        firstPos = transform.parent.position;
    }

    private void Update()
    {
        isColliding = false;
        if (Mathf.Abs(firstPos.z - transform.parent.position.z) > 0.1)transform.parent.position = new Vector3(transform.parent.position.x, transform.parent.position.y, firstPos.z);       
        if (Mathf.Abs(firstPos.y - transform.parent.position.y) > 0.1)transform.parent.position = new Vector3(transform.parent.position.x, firstPos.y, transform.parent.position.z);  
    }

    private void OnTriggerStay(Collider col)
    {
        if(col.tag == "Grocery")
        {
            gData = col.GetComponent<GroceryDataHandler>();
            if (col.GetComponent<FixedJoint>() == null && !gData.inCart)
            {
                if (tutorialManager.tutorialState == 7)
                {
                    tutorialManager.InvokeMethod("ChangeState", 0, 8);
                    tutorialManager.InvokeMethod("ChangeText", 0, "Door op het handvat van de winkelwagen \nte drukken kun je je winkelwagen bewegen");
                }

                // Grocery has been added to the shopping cart.
                if (isColliding) return;
                isColliding = true;
                gData.inCart = true;
                col.transform.parent = transform.parent.parent.Find("Content");
            }
        }
    }

}
