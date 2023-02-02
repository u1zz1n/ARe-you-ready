using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]

public class TrackedImageInfoMultipleManager : MonoBehaviour
{
    [SerializeField]
    private Text DebugLog;

    [SerializeField]
    private Text DebugLog2;

    [SerializeField]
    private Text imageTrackedText;

    [SerializeField]
    private GameObject[] arObjectsToPlace;

    [SerializeField]
    private Vector3 scaleFactor = new Vector3(0.01f, 1f, 0.01f);

    [SerializeField]
    private  ARTrackedImageManager m_TrackedImageManager;

    private readonly Dictionary<string, GameObject> arObjects = new();

    private void Awake() {
       m_TrackedImageManager = GetComponent<ARTrackedImageManager>();

        /*foreach(GameObject arObject in arObjectsToPlace)
        {
            GameObject newARObject = Instantiate(arObject, Vector3.zero, Quaternion.identity);
            newARObject.name = arObject.name;
            arObjects.Add(arObject.name, newARObject);
            DebugLog2.text += arObjects[arObject.name].name;
        }*/
    }

    void OnEnable(){
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable(){
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach(var trackedImage in eventArgs.added)
        {
            imageTrackedText.text = trackedImage.referenceImage.name;

            var name = trackedImage.referenceImage.name;
            foreach(var curPrefab in arObjectsToPlace)
            {
                if(string.Compare(curPrefab.name, name, StringComparison.Ordinal) == 0 &&
                !arObjects.ContainsKey(name))
                {
                    var newARObject = Instantiate(curPrefab, trackedImage.transform);
                    arObjects[name] = newARObject;
                    arObjects[name].transform.position = trackedImage.transform.position;

                    ShowAndroidToastMessage("Instantiated!");
                }
            }
        }

        foreach(var trackedImage in eventArgs.updated)
        {
            arObjects[trackedImage.referenceImage.name].SetActive(trackedImage.trackingState == TrackingState.Tracking);
            if(trackedImage.trackingState == TrackingState.Tracking)
            {
                imageTrackedText.text = trackedImage.referenceImage.name;
                arObjects[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
            }
        }

        foreach(var trackedImage in eventArgs.removed)
        {
            Destroy(arObjects[trackedImage.referenceImage.name]);
            arObjects.Remove(trackedImage.referenceImage.name);
        }

        /*foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateARImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateARImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            
            arObjects[trackedImage.name].SetActive(false);
        }*/
    }

    /*private void UpdateARImage(ARTrackedImage trackedImage)
    {
        imageTrackedText.text = trackedImage.referenceImage.name;
        AssignGameObject(trackedImage.referenceImage.name, trackedImage.transform.position);
        //Debug.Log($"trackedImage.referenceImage.name: {trackedImage.referenceImage.name}");
    }

    void AssignGameObject(string name, Vector3 newPosition)
    {
        //if(arObjectsToPlace != null)
        {
            GameObject goARObject = arObjects[name];
            goARObject.transform.position = newPosition;
            goARObject.SetActive(true);

            //goARObject.transform.localScale = scaleFactor;
            foreach(GameObject go in arObjects.Values)
            {
                //Debug.Log($"Go in arObjects.Values: {go.name}");
                if(go.name != name) {
                    go.SetActive(false);      
                }
            }
        }
    }*/

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
