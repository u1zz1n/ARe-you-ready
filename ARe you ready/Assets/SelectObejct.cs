using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObejct : MonoBehaviour
{
    public void changeObejct()
    {
        if (PlacementAndDragging.spawnObjectNum <= PlacementAndDragging.spawnObjectLength + 1)
        {
            PlacementAndDragging.spawnObjectNum++;
        }
        else
        {
            PlacementAndDragging.spawnObjectNum = 0;
        }
    }
}
