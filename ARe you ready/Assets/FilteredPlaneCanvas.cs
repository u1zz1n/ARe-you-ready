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
}
