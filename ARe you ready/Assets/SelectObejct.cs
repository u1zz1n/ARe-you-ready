using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObejct : MonoBehaviour
{
    public void changeObejct()
    {
        SoundManager.instance.PlaySfx("UI_Press");

        if (PlacementAndDragging.spawnObjectNum <= PlacementAndDragging.spawnObjectLength + 2)
        {
            PlacementAndDragging.spawnObjectNum++;
        }
        else
        {
            PlacementAndDragging.spawnObjectNum = 0;
        }
    }
}
