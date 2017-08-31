using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    [HideInInspector]
    public int tutorialState;
    [SerializeField]
    private TextMesh textMesh;
    [SerializeField]
    private Camera cam;

    TutorialGrocerySpawnHandler spawnhandler = new TutorialGrocerySpawnHandler();

    float spaceTimer = 0;

    Color colorStart;
    Color colorEnd;
    [SerializeField]
    private GameObject shelf;
    [SerializeField]
    private GameObject cart;
    private float duration = 5;
    [SerializeField]
    private GameObject continueButton, replayButton;
    [SerializeField]
    private GameObject controller;
    [SerializeField]
    private GameObject arrow;
    [SerializeField]
    private GameObject shelfPlane, cartPlane, circle, playerPosition;

    public int stateNumber;
    public string newText;

    private bool isLookingAtShelf, isLookingAtCart;
    public bool hasPressedTrigger = false;
    bool laser;

    //private Animation anim;

    private void Awake()
    {
        // State 1 (Look at your hands)
        tutorialState = 0;
        textMesh.text = "Verplaats je naar de cirkel op de grond!";
        laser = false;
        continueButton.SetActive(false);
        replayButton.SetActive(false);
    }

    void start()
    {
        isLookingAtCart = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tutorialState++;
        }

        if (tutorialState == 3)
        {
            //if (arrow != null)
            //{
            //    arrow.gameObject.transform.LookAt(shelfPlane.transform);
            //    arrow.transform.Rotate(new Vector3(-90, 0, 0));
            //}
            shelfPlane.SetActive(true);
            Vector3 screenPoint = cam.WorldToViewportPoint(shelfPlane.transform.position);
            if (screenPoint.x >= 0.4 && screenPoint.x <= .6 && screenPoint.y >= 0.4 && screenPoint.y <= .6 && screenPoint.z >= 0)
            {
                isLookingAtShelf = true;
            }
        }
        if (tutorialState == 6)
        {
            //if (arrow != null)
            //{
            //    arrow.gameObject.transform.LookAt(cart.transform);
            //    arrow.transform.Rotate(new Vector3(-90, 0, 0));
            //}
            textMesh.text = "Kijk naar het zwarte vierkantje rechts van de schap!";
            cartPlane.SetActive(true);
            Vector3 screenPoint = cam.WorldToViewportPoint(cartPlane.transform.position);
            if (screenPoint.x >= 0.4 && screenPoint.x <= .6 && screenPoint.y >= 0.4 && screenPoint.y <= .6 && screenPoint.z >= 0)
            {
                isLookingAtCart = true;
            }
        }
        if (tutorialState == 9)
        {
            //if (arrow != null)
            //{
            //    arrow.gameObject.transform.LookAt(continueButton.transform);
            //    arrow.transform.Rotate(new Vector3(-90, 0, 0));
            //}

        }
        if (tutorialState == 0)
        {
            if (circle != null)
            {
                Vector2 playerPos = new Vector2(playerPosition.transform.position.x, playerPosition.transform.position.z);
                Vector2 circlePos = new Vector2(circle.transform.position.x, circle.transform.position.z);

                if (Vector2.Distance(playerPos, circlePos) < 0.2)
                {
                    circle.SetActive(false);
                    textMesh.text = "";
                    InvokeMethod("ChangeState", 2, 1);
                    InvokeMethod("ChangeText", 3, "Kijk naar je handen!");
                }
            }
        }

        switch (tutorialState)
        {
            case 0:
                break;
            case 1:
                Debug.Log("TutMngr State 1");
                // Look at your hands
                break;
            case 2:
                Debug.Log("TutMngr State 2");
                // Pull the triggers to grab items
                textMesh.text = "Druk de triggers op de achterkant van je \ncontrollers en kijk wat er \ngebeurd met je handen!";
                break;
            case 3:
                Debug.Log("TutMngr State 3");
                if (isLookingAtShelf)
                {
                    shelfPlane.SetActive(false);
                    tutorialState = 4;
                }
                break;
            case 4:
                Debug.Log("TutMngr State 4");
                // Try to grab a pack of milk
                textMesh.text = "";
                bool hasPressedSpace = true;

                if (hasPressedSpace)
                {
                    spaceTimer -= Time.deltaTime;
                }
                if (spaceTimer <= 0f && isLookingAtShelf == true)
                {
                    shelf.GetComponent<Animation>().Play("FadeIn");
                    hasPressedSpace = false;
                    tutorialState = 5;
                    InvokeMethod("ChangeText", 4, "Probeer een melkpak op te pakken");
                }
                break;
            case 5:
                Debug.Log("TutMngr State 5");
                break;
            case 6:
                Debug.Log("TutMngr State 6");
                if (isLookingAtCart)
                {
                    cartPlane.SetActive(false);
                    tutorialState = 7;
                }
                break;
            case 7:
                // Put it in your cart
                Debug.Log("TutMngr State 7");
                cart.GetComponent<Animation>().Play("CartFadeIn");
                InvokeMethod("ChangeText", 1, "Leg een melkpak in je winkelwagen");
                break;
            case 8:
                Debug.Log("TutMngr State 8");
                // You can also move your cart
                break;
            case 9:
                Debug.Log("TutMngr State 9");
                // Are you ready to begin
                //textMesh.text = "Richt met je controller naar \neen knop en druk op de trigger \nom een keuze te maken";
                if (!laser) { 
                    controller.GetComponent<SteamVR_LaserPointer>().FakeStart();
                    //arrow.SetActive(true);
                    //cart.SetActive(false);
                    //shelf.SetActive(false);
                    replayButton.SetActive(true);
                    continueButton.SetActive(true);
                    laser = true;
                    InvokeMethod("ChangeText", 0, "Kijk naar de knoppen aan de andere kant van de kamer \nEn maak een keuze door te richten met je \ncontroller en op de trigger te drukken.");
                }
                //Buttons
                //textMesh.fontSize = 80;
                //textMesh.text = "Are you ready to begin?\n Left trigger to stay\n Right trigger to begin";
                break;
        }
    }

    public void ChangeState()
    {
        tutorialState = stateNumber;
    }

    public void ChangeText()
    {
        textMesh.text = newText;
    }

    public void InvokeMethod(string methodName, float time, int stateNumber)
    {
        this.stateNumber = stateNumber;
        Invoke(methodName.ToString(), time);
    }
    public void InvokeMethod(string methodName, float time, string newText)
    {
        this.newText = newText;
        Invoke(methodName.ToString(), time);
    }  

}
