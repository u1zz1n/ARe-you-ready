using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;

public class SpawnInMulti : MonoBehaviour
{
    public ARRaycastManager arRaycastManager;
    public ARSessionOrigin aRSessionOrigin;
    public ARPlaneManager aRPlaneManager;

    public Vector3 tempColor;
    public Color snycColor;

    private SpawnObject lastSelectedObject;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private GameObject cube;

    private bool CanSpawn = false;
    //private GameObject spawnedCube;

    ARPlane Plane;

    /*private void Awake()
    {
        MasterManager.NetworkInstantiate(spawnedCube);
    }*/
    public void OnClick_Spawn()
    {
        CanSpawn = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        CanSpawn = false;
        lastSelectedObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        Plane = FindObjectOfType<ARPlane>();

        if (CanSpawn && Input.touchCount > 0)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            //if (touch.phase == TouchPhase.Began)
            //{
            //    Ray ray = arCamera.ScreenPointToRay(touch.position);
            //    RaycastHit hitObject;

                if (arRaycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    //if (lastSelectedObject == null)
                    //{
                        MasterManager.Networkinstantiate(cube, hitPose.position, hitPose.rotation);

                        /*uijin comment code
                        //spawnedCube = Instantiate(cube, hitPose.position, hitPose.rotation).GetComponent<GameObject>();
                        //float yDiff = Plane.transform.localPosition.y - (spawnedCube.GetComponent<BoxCollider>().bounds.min.y);
                        //Vector3 spawnPosition = new Vector3(spawnedCube.transform.position.x, spawnedCube.transform.position.y + yDiff, spawnedCube.transform.position.z);
                        //spawnedCube.transform.position = spawnPosition;
                        */

                        CanSpawn = false;
                    //}
                }

                /*if (Physics.Raycast(ray, out hitObject))
                {
                    lastSelectedObject = hitObject.transform.GetComponent<SpawnObject>();

                    if (lastSelectedObject != null)
                    {
                        ShowAndroidToastMessage("selected!");

                        SpawnObject[] allOtherObjects = FindObjectsOfType<SpawnObject>();

                        foreach (SpawnObject placementObject in allOtherObjects)
                        {
                            MeshRenderer meshRenderer = placementObject.GetComponent<MeshRenderer>();

                            if (placementObject != lastSelectedObject)
                            {
                                meshRenderer.material.color = Color.gray;
                            }
                            else
                            {
                                meshRenderer.material.color = Color.red;
                            }
                        }
                    }
                }*/
            //}
        }
    }

/*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
{
    if (stream.isWriting)
    {
        //send color
        MeshRenderer meshRenderer = lastSelectedObject.GetComponent<MeshRenderer>();

        tempColor = new Vector3(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b);

        stream.Serialize(ref tempColor);
    }
    else
    {
        //get color
        stream.Serialize(ref tempColor);
        snycColor = new Color(tempColor.x, tempColor.y, tempColor.z, 1.0f);
    }
}*/

    private static void ShowAndroidToastMessage(string message)
    {
        #if UNITY_ANDROID
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (unityActivity == null) return;
            var toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                // Last parameter = length. Toast.LENGTH_LONG = 1
                using var toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText",
                    unityActivity, message, 1);
                toastObject.Call("show");
            }));
        #endif
    }
}
