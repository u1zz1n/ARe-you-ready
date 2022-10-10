using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilteredPlaneCanvas : MonoBehaviour
{
    [SerializeField] private Toggle verticalPlaneToggle;
    [SerializeField] private Toggle horizontalPlaneToggle;
    [SerializeField] private Toggle bigPlaneToggle;

    private FilteredPlane filterPlane;
    private PlacementAndDragging placeDragging;
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

        filterPlane.OnVerticalPlaneFound += () => verticalPlaneToggle.isOn = true;
        filterPlane.OnHorizontalPlaneFound += () => horizontalPlaneToggle.isOn = true;
        filterPlane.OnBigPlaneFound += () => bigPlaneToggle.isOn = true;
    }

    private void OnDisable()
    {
        filterPlane.OnVerticalPlaneFound += () => verticalPlaneToggle.isOn = true;
        filterPlane.OnHorizontalPlaneFound += () => horizontalPlaneToggle.isOn = true;
        filterPlane.OnBigPlaneFound += () => bigPlaneToggle.isOn = true;
    }

    private void Update()
    {
        
    }

    private void CheckIfAllAreTrue()
    {
        if(VerticalPlaneToggle && HorizontalPlaneToggle && BigPlaneToggle)
        {
            Debug.Log("ready!");
        }
    }
}
