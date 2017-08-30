﻿//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class VRController : MonoBehaviour
{
    SpawnController spawnController;

    List<GameObject> inRange = new List<GameObject>();
    GameObject closestSnapZone;
    
    public Rigidbody attachPoint;
    private Transform goParent;
    private GameObject toBeParent;

    public Shader outline;
    public Shader empty;
    Renderer rend;
    GameObject target;

    public bool showShaders;

    RaycastHit raycastHit;
    SteamVR_TrackedObject trackedObj;
    FixedJoint joint;
    HingeJoint hJoint;
    GameObject cart;

    public List<GameObject> zones = new List<GameObject>();
    GameObject snapTarget;

    Vector3 direction;
    GroceryDataHandler gData;

    private Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        empty = Shader.Find("Standard");
        outline = Shader.Find("Custom/OutlineEffect");
        if(SceneManager.GetActiveScene().name != "Menu" && SceneManager.GetActiveScene().name != "HeatMapScene")spawnController = GameObject.FindGameObjectWithTag("Floor").GetComponent<SpawnController>();
    }

    void Update()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);
        if (joint != null || hJoint != null || cart != null) anim.Play("Grip");

        HandleOutline();
        StateManager();

        if (cart != null)
        {
            // Cart follow the controller on its x-axis only:
            cart.transform.position = new Vector3(attachPoint.position.x, cart.transform.position.y, cart.transform.position.z);
        }

        if (SceneManager.GetActiveScene().name != "Menu")
        {
            vrGui();
        }
    }

    private void vrGui()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (hit.collider.name == "Exit")
                {
                    LoadScene("Menu");   
                }
            }
        }
    }

    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void StateManager()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);
        if ((joint == null || hJoint == null || cart == null) && device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            AttachJoint(); // += delagte
            anim.Play("Gripping");
        }
        else if ((joint != null) && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {           
            DetachJoint(); // -= delegate
            anim.Play("Idle");
        }
        if (hJoint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            DetachJoint();
            anim.Play("Idle");
        }
        if (cart != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            DetachJoint();
            anim.Play("Idle");
        }

    }

    void AttachJoint()
    {
        RaycastHit raycastHit;
        GameObject gameObject = null;
        direction = Quaternion.AngleAxis(60, transform.right) * transform.forward;

        if (Physics.Raycast(transform.position,direction, out raycastHit) &&
            raycastHit.transform.gameObject.tag == "Grocery" && raycastHit.distance < 0.1f &&
            !raycastHit.collider.GetComponent<GroceryDataHandler>().inCart) //raycast hit + een ray kleiner dan #  

        {
            gameObject = raycastHit.collider.gameObject; //het object dat de ray raakt wordt gezet in gameObject
            
            goParent = gameObject.transform.parent;
            gameObject.transform.parent = null;

            // Hardcode
            gameObject.GetComponent<Rigidbody>().isKinematic = false;

            gameObject.transform.position = attachPoint.transform.position; //Verplaatst het object naar de controller

            joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = attachPoint;

        }

        if (Physics.Raycast(transform.position, direction, out raycastHit) &&
            raycastHit.transform.gameObject.tag == "Basket" && raycastHit.distance < 0.1f &&
            raycastHit.collider.GetComponent<HingeJoint>() == null) 
        {
            // Basket attach:
            gameObject = raycastHit.collider.gameObject;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            gameObject.transform.position = attachPoint.transform.position;
            hJoint = gameObject.AddComponent<HingeJoint>();
            hJoint.connectedBody = attachPoint;
        }

        if(Physics.Raycast(transform.position, direction, out raycastHit) &&
            raycastHit.transform.gameObject.tag == "Cart" && raycastHit.distance < 0.1f)
        {   
            // Cart attach on z-axis (The setting of the position happens in update):
            cart = raycastHit.collider.gameObject;
            //cart.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
    }

    void DetachJoint()
    {
        if (joint != null)
        {
            GameObject go = joint.gameObject;
            var device = SteamVR_Controller.Input((int)trackedObj.index);
            var rigidbody = joint.gameObject.GetComponent<Rigidbody>();
            DestroyImmediate(joint);
            joint = null;

            if (go.tag == "Grocery")
            {
                HandleChild(go);
            }
            
            var origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
            if (origin != null)
            {
                rigidbody.velocity = origin.TransformVector(device.velocity);
                rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity);
            }
            else
            {
                rigidbody.velocity = device.velocity;
                rigidbody.angularVelocity = device.angularVelocity;
            }
        }

        if(cart != null)
        {
            // Detach the cart.
            //cart.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            cart = null;
        }
    }

    void HandleChild(GameObject grocery)
    {
        gData = grocery.GetComponent<GroceryDataHandler>();
        closestSnapZone = null;
        direction = Quaternion.AngleAxis(60, transform.right) * transform.forward;
        float prevDistance = 100;
        gData.inCart = false;

        for (int i = 0; i < inRange.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, inRange[i].transform.position);
            if (distance < prevDistance)
                {
                    closestSnapZone = inRange[i];
                    prevDistance = distance;
                }
            
        }
        
        if (inRange.Count != 0)
        {
            if (Vector3.Distance(gameObject.transform.position, closestSnapZone.transform.position) < 0.3f)
            {
                grocery.transform.rotation = new Quaternion(0, 180, 0, 0);
                grocery.transform.position = new Vector3(closestSnapZone.transform.position.x, closestSnapZone.transform.position.y, closestSnapZone.transform.position.z);
                grocery.GetComponent<Rigidbody>().isKinematic = true;
                grocery.gameObject.transform.SetParent(closestSnapZone.transform);
            }
        }
        

        inRange.Clear();
    }

    void SetShader(Renderer render,Color color)
    {
        if (showShaders)
        {
            render.material.shader = outline;
            render.material.SetColor("_OutlineColor", color);
        }
    }

    void EmptyShader(Renderer render)
    {
        if (showShaders)
        {
            if (render != null)
                render.material.shader = empty;
        }
    }

    void HandleOutline()
    {
        if (showShaders)
        {
            direction = Quaternion.AngleAxis(60, transform.right) * transform.forward;
            if (joint == null)
            {
                if (Physics.Raycast(transform.position, direction, out raycastHit) && raycastHit.distance < 0.1f && raycastHit.transform.gameObject.tag == "Grocery" )
                {
                    if (target == null && raycastHit.transform.gameObject.GetComponent<Rigidbody>().isKinematic)
                    {
                        if (raycastHit.transform.gameObject.tag == "Grocery")
                        {   //if there hasn't been a target to outline before, outline this
                            rend = raycastHit.transform.gameObject.GetComponent<MeshRenderer>();
                            target = raycastHit.transform.gameObject;
                            SetShader(rend, Color.white);
                        }
                    }
                    else if (raycastHit.transform.gameObject.tag == "Grocery" && raycastHit.transform.gameObject.GetComponent<Rigidbody>().isKinematic)
                    {
                        if (raycastHit.transform.gameObject != target)
                        {   //if there is a different grocery target, outline this and revert previous outline
                            EmptyShader(rend);
                            target = raycastHit.transform.gameObject;
                            rend = raycastHit.transform.gameObject.GetComponent<MeshRenderer>();
                            SetShader(rend, Color.white);
                        }
                    }
                    else if (raycastHit.transform.gameObject != target || raycastHit.transform.gameObject.tag != "Grocery")
                    {   //if the raycast hits no target or no grocery revert previous outline
                        EmptyShader(rend);
                        target = null;
                        rend = null;
                    }
                }
                else
                {   //if hit nothing revert previous outline
                    EmptyShader(rend);
                    target = null;
                    rend = null;
                }
            }
            if(joint != null)
            {
                EmptyShader(rend);
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.transform.gameObject.tag == "SnapZone" && collision.transform.gameObject.transform.childCount <= 0 && joint != null)
        {
            if (Vector3.Distance(gameObject.transform.position, collision.gameObject.transform.position) <= 0.3f &&
             collision.transform.parent.parent.parent.GetComponent<PlankSpawnController>().type == joint.GetComponent<GroceryDataHandler>().groceryName &&
             !inRange.Contains(collision.gameObject))
            {
                inRange.Add(collision.gameObject);
            }
        }            
    }

    private void OnTriggerExit(Collider collision)
    {
        if (inRange.Contains(collision.gameObject))
        {
            inRange.Remove(collision.gameObject);
        }       
        
    }
}
