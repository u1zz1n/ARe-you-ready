using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectWithArrow : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 newPos;

    public void Down()
    {
        newPos = TrackedImageInfoMultipleManager.curSelectedObject.transform.position;
        newPos.y -= 0.05f;

        TrackedImageInfoMultipleManager.curSelectedObject.transform.position = newPos;
    }

    // Update is called once per frame
    public void Up()
    {
        newPos = TrackedImageInfoMultipleManager.curSelectedObject.transform.position;
        newPos.y += 0.05f;

        TrackedImageInfoMultipleManager.curSelectedObject.transform.position = newPos;
    }

    public void Left()
    {
        newPos = TrackedImageInfoMultipleManager.curSelectedObject.transform.position;
        newPos.x -= 0.05f;

        TrackedImageInfoMultipleManager.curSelectedObject.transform.position = newPos;
    }

    public void Right()
    {
        newPos = TrackedImageInfoMultipleManager.curSelectedObject.transform.position;
        newPos.x += 0.05f;

        TrackedImageInfoMultipleManager.curSelectedObject.transform.position = newPos;
    }

    public void Back()
    {
        newPos = TrackedImageInfoMultipleManager.curSelectedObject.transform.position;
        newPos.z += 0.05f;

        TrackedImageInfoMultipleManager.curSelectedObject.transform.position = newPos;
    }

    public void Front()
    {
        newPos = TrackedImageInfoMultipleManager.curSelectedObject.transform.position;
        newPos.z -= 0.05f;

        TrackedImageInfoMultipleManager.curSelectedObject.transform.position = newPos;
    }
}
