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
    }

    void Start()
    {
        rb = GetComponent<Rigidbody> ();     
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(Input.touchCount > 0 && rollable)
        {
            Touch touch = Input.GetTouch(0);

            if(EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            //touchPosition = touch.position;
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if(touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;

                if(Physics.Raycast(ray, out hitObject))
                {
                    lastSelectedObject = hitObject.transform.GetComponent<PlacementObject>();

                    if(lastSelectedObject != null)
                    {
                        PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();

                        foreach (PlacementObject placementObject in allOtherObjects)
                        {
                           MeshRenderer meshRenderer = placementObject.GetComponent<MeshRenderer>();

                           if (placementObject != lastSelectedObject)
                           {
                               //placementObject.Selected = false;
                               //meshRenderer.material.color = inactiveColor;
                           }
                           else
                           {
                                if(placementObject == bowingBall)
                                {
                                    debugLog.text = "Roll start";
                                    startroll = true;
                                    touchTimeStart = Time.time;
                                    startPos = Input.GetTouch(0).position;
                                }
                               placementObject.Selected = true;
                               //meshRenderer.material.color = activeColor;
                           }

                           if (displayOverlay)
                               placementObject.ToggleOverlay();
                        }
                    }
                }
            }

            if(touch.phase == TouchPhase.Ended && rollable && startroll) 
            {
                debugLog.text = "Rolling";
                touchTimeFinish = Time.time;
                timeInterval = touchTimeFinish - touchTimeStart;
            
                endPos = Input.GetTouch(0).position;
                direction = startPos - endPos;

                rb.isKinematic = false;
                rb.AddForce(-direction.x * 1f, 0, throwForceInZ * 1f);
            
                Destroy(gameObject, 3f);
            }
        }
        */
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && rollable)
        {
            debugLog.text = "Roll start";
            touchTimeStart = Time.time;
            startPos = Input.GetTouch(0).position;
        }

        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && rollable) 
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
