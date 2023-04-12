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

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private GameObject cube;

    private bool CanSpawn = false;
    static public bool CanChangeColor = false;

    //private GameObject spawnedCube;

    ARPlane Plane;

    /*private void Awake()
    {
        MasterManager.NetworkInstantiate(spawnedCube);
    }*/
    public void OnClick_Color()
    {
        CanChangeColor = true;
    }

    public void OnClick_Spawn()
    {
        CanSpawn = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        CanSpawn = false;
        CanChangeColor = false;
    }

    // Update is called once per frame
    void Update()
    {
        Plane = FindObjectOfType<ARPlane>();

        if (CanSpawn && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            //touchPosition = touch.position;
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if (arRaycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                MasterManager.Networkinstantiate(cube, hitPose.position, hitPose.rotation);
                ShowAndroidToastMessage("Spawned");
                //spawnedCube = Instantiate(cube, hitPose.position, hitPose.rotation).GetComponent<GameObject>();
                //float yDiff = Plane.transform.localPosition.y - (spawnedCube.GetComponent<BoxCollider>().bounds.min.y);
                //Vector3 spawnPosition = new Vector3(spawnedCube.transform.position.x, spawnedCube.transform.position.y + yDiff, spawnedCube.transform.position.z);
                //spawnedCube.transform.position = spawnPosition;

                CanSpawn = false;
            }

        }
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
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
