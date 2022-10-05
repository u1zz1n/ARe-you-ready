using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObejct : MonoBehaviour
{
    public void changeObejct()
    {
        if (ARPlaceOnPlane.selectedObject == 0)
        {
            ARPlaceOnPlane.selectedObject++;
        }
        else
        {
            ARPlaceOnPlane.selectedObject--;
        }
    }
}
