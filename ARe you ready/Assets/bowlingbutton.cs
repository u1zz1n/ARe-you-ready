using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bowlingbutton : MonoBehaviour
{
    // Start is called before the first frame update
    public void clickBowlingButton()
    {
        PlacementAndDragging.placeBowling = true;
        //PlacementAndDragging.placeBowling = true;
    }

    public void clickRollButton()
    {
        PlacementAndDragging.RollBowling = true;
    }
}
