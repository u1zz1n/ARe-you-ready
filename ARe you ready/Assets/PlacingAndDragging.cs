using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class PlacingAndDragging : MonoBehaviour
{
    static public bool spawnable = false;

    public ARRaycastManager arRaycastManager;
    public ARSessionOrigin aRSessionOrigin;
    public ARPlaneManager aRPlaneManager;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private GameObject bowingBall;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Vector2 touchPosition = default;

    [SerializeField]
    private GameObject placedPrefab;

    ///////select object
    [SerializeField]
    private Color activeColor = Color.red;

    [SerializeField]
    private Color inactiveColor = Color.gray;

    [SerializeField]
    private bool displayOverlay = false;
    private PlacementObject lastSelectedObject;
    private bool onTouchHold = false;

    private GameObject PlacedPrefab
    {
        get
        {
            return placedPrefab;
        }
        set
        {
            placedPrefab = value;
        }
    }

    private void Awake() {
        spawnable = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            touchPosition = touch.position;
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if(touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;

                if(Physics.Raycast(ray, out hitObject) && spawnable)
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
                               placementObject.Selected = false;
                               meshRenderer.material.color = inactiveColor;
                           }
                           else
                           {
                               placementObject.Selected = true;
                               meshRenderer.material.color = activeColor;
                           }

                           if (displayOverlay)
                               placementObject.ToggleOverlay();
                        }
                    }
                }
            }

            if (touch.phase == TouchPhase.Ended)
           {
               lastSelectedObject.Selected = false;
           }

            if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                
                if(spawnable)
                {                 
                    if (lastSelectedObject == null)
                    {
                        lastSelectedObject = Instantiate(bowingBall, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
                    }
                    else
                    {
                        if (lastSelectedObject.Selected)
                        {
                            lastSelectedObject.transform.position = hitPose.position;
                            lastSelectedObject.transform.rotation = hitPose.rotation;
                        }
                    }

                    //spawnable = false;
                }
                else if(!swipeBall.rollable)
                {
                    if (lastSelectedObject == null)
                    {
                        lastSelectedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
                    }
                    else
                    {
                        if (lastSelectedObject.Selected)
                        {
                            lastSelectedObject.transform.position = hitPose.position;
                            lastSelectedObject.transform.rotation = hitPose.rotation;
                        }
                    }
                }
  
            }
        }
    }
}
