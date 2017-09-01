using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickHandler : MonoBehaviour {
    public GameObject colGO;

    int caseSwitch = 0;
    
    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "SnapZone")
            colGO = col.gameObject;               
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "SnapZone")
            colGO = null;        
    }
}
