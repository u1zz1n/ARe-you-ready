using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateLocationForDebug : MonoBehaviour
{
    public static bool checkUpdate = false;
    void Start()
    {
        checkUpdate = false;
    }

    public void Updating()
    {
        checkUpdate = true;
    }
}
