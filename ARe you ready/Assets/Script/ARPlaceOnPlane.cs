using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaceOnPlane : MonoBehaviour
{
    public ARRaycastManager arRaycaster;
    
    public GameObject placeObject;
    public GameObject placeObject2;

    GameObject spawnObject;
    GameObject spawnObject2;

    public static int selectedObject = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateCenterObject();
        PlaceObjectByTouch();
    }

    private void PlaceObjectByTouch()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if(arRaycaster.Raycast(touch.position, hits, TrackableType.Planes))
            {
                Pose placementPose = hits[0].pose; //first hit place

                if(selectedObject == 0)
                {
                    if (!spawnObject)
                    {
                        spawnObject = Instantiate(placeObject, placementPose.position, placementPose.rotation);
                    }
                    else
                    {
                        //spawnObject.transform.position = placementPose.position;
                        //spawnObject.transform.rotation = placementPose.rotation;
                    }
                }
                else if(selectedObject == 1)
                {
                    if (!spawnObject2)
                    {
                        spawnObject2 = Instantiate(placeObject2, placementPose.position, placementPose.rotation);
                    }
                }
            }

        }
    }


    private void UpdateCenterObject()
    {
        Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        arRaycaster.Raycast(screenCenter, hits, TrackableType.Planes);

        if(hits.Count > 0) //After the raycast, if you find a location to desplay, put object that location
        {
            Pose placementPose = hits[0].pose; //first hit place
            placeObject.SetActive(true);
            placeObject.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            //placeObject.SetActive(false); //no plane to display
        }
    }
}
