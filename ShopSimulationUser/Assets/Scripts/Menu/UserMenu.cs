﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserMenu : MonoBehaviour {

    private SoundsPlayer audio;

    [SerializeField]
    GameObject VRRoom;
    [SerializeField]
    GameObject Tutorial;
    [SerializeField]
    Sprite spriteLight;
    [SerializeField]
    Sprite spriteDark;

    SpriteRenderer sRendererVR;
    SpriteRenderer sRendererTut;

    RaycastHit hit;
    SteamVR_TrackedObject trackedObj;
    SteamVR_LaserPointer laser;

    private void Start()
    {
        audio = GameObject.Find("SoundsPlayer").GetComponent<SoundsPlayer>();
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        laser = GetComponent<SteamVR_LaserPointer>();
        sRendererVR = VRRoom.GetComponent<SpriteRenderer>();
        sRendererTut = Tutorial.GetComponent<SpriteRenderer>();
        laser.FakeStart();
    }

    private void Update()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider.gameObject == Tutorial)
            {
                sRendererTut.sprite = spriteDark;
                if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
                {
                    audio.DingSound();
                    SceneManager.LoadScene("TutorialScene");
                }
            }
            else if (hit.collider.gameObject == VRRoom)
            {
                sRendererVR.sprite = spriteDark;
                if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
                {
                    audio.DingSound();
                    SceneManager.LoadScene("VRScene");
                }
            }
            else
            {
                sRendererTut.sprite = spriteLight;
                sRendererVR.sprite = spriteLight;
            }
        }
    }

}
