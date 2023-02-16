using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class FilteredPlane : MonoBehaviour
{
    [SerializeField] private Vector2 dismenstionsForBigPlane;
    public static Vector2 dismenstionsForBigPlanes = new Vector2 ( 1.0f, 1.0f );

    public event Action OnVerticalPlaneFound;
    public event Action OnHorizontalPlaneFound;
    public event Action OnBigPlaneFound;
    public ARSession aRSession;

    public static bool isBig = false;

    private ARPlaneManager arPlaneManager;
    private List<ARPlane> arPlanes;

    void Start()
    {
        //foreach (ARPlane plane in arPlaneManager.trackables)
        //{
        //    plane.gameObject.SetActive(false);
        //}

        aRSession.Reset();

        isBig = false;
        dismenstionsForBigPlanes = dismenstionsForBigPlane;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added != null && args.added.Count > 0)
        {
            arPlanes.AddRange(args.added);
        }
        
        foreach (ARPlane plane in arPlanes.Where(plane => plane.extents.x * plane.extents.y >= 0.1f))
        {
            if(plane.alignment.IsVertical())
            {
                OnVerticalPlaneFound.Invoke();
            }
           
            if(plane.alignment.IsHorizontal())
            {
                OnHorizontalPlaneFound.Invoke();
            }
            //ShowAndroidToastMessage((plane.size.x * plane.size.y).ToString());

            if (plane.extents.x * plane.extents.y >= dismenstionsForBigPlane.x * dismenstionsForBigPlane.y)
            {
                //ShowAndroidToastMessage((plane.size.x * plane.size.y).ToString());

                isBig = true;
                arPlaneManager.enabled = false;
                OnBigPlaneFound.Invoke();
                break;
            }
            //else
            //{
                //isBig = false;
            //}
        }
    }

    private void OnEnable()
    {
        arPlanes = new List<ARPlane>();
        arPlaneManager = FindObjectOfType<ARPlaneManager>();
        arPlaneManager.planesChanged += OnPlanesChanged;
    }

    private void OnDisable()
    {
        arPlaneManager.planesChanged -= OnPlanesChanged;
    }

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
