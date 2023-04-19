using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FilteredPlaneCanvas : MonoBehaviour
{
    [SerializeField] private Toggle verticalPlaneToggle;
    [SerializeField] private Toggle horizontalPlaneToggle;
    [SerializeField] private Toggle bigPlaneToggle;

    private FilteredPlane filterPlane;
    private PlacingAndDragging checkDone;
    private SpawnInMulti checkDoneMulti;
    //private PlacementAndDragging placeDragging;
    public bool VerticalPlaneToggle 
    { 
        get => verticalPlaneToggle.isOn; 
        set
        {
            verticalPlaneToggle.isOn = value;
            CheckIfAllAreTrue();
        }
    }

    public bool HorizontalPlaneToggle
    {
        get => horizontalPlaneToggle.isOn;
        set
        {
            horizontalPlaneToggle.isOn = value;
            CheckIfAllAreTrue();
        }
    }

    public bool BigPlaneToggle
    {
        get => bigPlaneToggle.isOn;
        set
        {
            bigPlaneToggle.isOn = value;
            CheckIfAllAreTrue();
        }
    }

    private void OnEnable()
    {
        filterPlane = FindObjectOfType<FilteredPlane>();

        if (SceneManager.GetActiveScene().name == "ObjectTracking")
        {
            checkDone = FindObjectOfType<PlacingAndDragging>();
            checkDone.CanPlayBall += () => verticalPlaneToggle.isOn = true;
        }
        else if(SceneManager.GetActiveScene().name == "Multiplayer")
        {
            checkDoneMulti = FindObjectOfType<SpawnInMulti>();
            checkDoneMulti.CanPlayScene += () => verticalPlaneToggle.isOn = true;
        }
        else
        {
            filterPlane.OnVerticalPlaneFound += () => verticalPlaneToggle.isOn = true;
        }

        filterPlane.OnHorizontalPlaneFound += () => horizontalPlaneToggle.isOn = true;
        filterPlane.OnBigPlaneFound += () => bigPlaneToggle.isOn = true;
    }

    private void OnDisable()
    {
        if(SceneManager.GetActiveScene().name == "ObjectTracking")
        {
            checkDone.CanPlayBall += () => verticalPlaneToggle.isOn = true;
        }
        else if (SceneManager.GetActiveScene().name == "Multiplayer")
        {
            checkDoneMulti.CanPlayScene += () => verticalPlaneToggle.isOn = true;
        }
        else
        {
            filterPlane.OnVerticalPlaneFound += () => verticalPlaneToggle.isOn = true;
        }

        filterPlane.OnHorizontalPlaneFound += () => horizontalPlaneToggle.isOn = true;
        filterPlane.OnBigPlaneFound += () => bigPlaneToggle.isOn = true;
    }

    private void Update()
    {
        
    }

    private void CheckIfAllAreTrue()
    {
        //if(VerticalPlaneToggle && HorizontalPlaneToggle && BigPlaneToggle)
        //{
        //    Debug.Log("ready!");
        //}
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
