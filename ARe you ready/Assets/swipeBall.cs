using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class swipeBall : MonoBehaviour
{
    static public bool rollable = false;
    private bool startroll = false;

    float time = 0f;

    public ARRaycastManager arRaycastManager;
    public ARSessionOrigin aRSessionOrigin;
    public ARPlaneManager aRPlaneManager;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private GameObject bowingBall;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [SerializeField]
    private bool displayOverlay = false;
    private PlacementObject lastSelectedObject;
    private bool onTouchHold = false;

    Vector2 startPos, endPos, direction;
    float touchTimeStart, touchTimeFinish, timeInterval;

    [SerializeField]
    float throwForceInXandY = 0f;

    [SerializeField]
    float throwForceInZ = 5f;

    Rigidbody rb;

    [SerializeField]
    public Text debugLog;

    // Start is called before the first frame update
    private void Awake() {
        rollable = false;
        startroll = false;
        time = 0f;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody> ();     
        debugLog.text = "Cool time to roll";
    }

    // Update is called once per frame
    void Update()
    {
        if(rollable)
        {
            time += Time.deltaTime;

            if (time > 2f)
            {
                startroll = true;
                debugLog.text = "You can roll now";
            }
        }



        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && rollable && startroll)
        {
            debugLog.text = "Roll start";
            touchTimeStart = Time.time;
            startPos = Input.GetTouch(0).position;
        }

        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && rollable && startroll) 
        {
            debugLog.text = "Rolling";
            touchTimeFinish = Time.time;
            timeInterval = touchTimeFinish - touchTimeStart;
            
            endPos = Input.GetTouch(0).position;
            direction = startPos - endPos;

            rb.isKinematic = false;
            rb.AddForce(-direction.x * 0.3f, 0, -direction.y * 0.3f);
            
            Destroy(gameObject, 3f);
        }
        
    }
}
