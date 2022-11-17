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

    public event Action OnVerticalPlaneFound;
    public event Action OnHorizontalPlaneFound;
    public event Action OnBigPlaneFound;

    public static bool isBig = false;

    private ARPlaneManager arPlaneManager;
    private List<ARPlane> arPlanes;

    void Start()
    {
        isBig = false;
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

            if(plane.extents.x * plane.extents.y >= dismenstionsForBigPlane.x * dismenstionsForBigPlane.y)
            {
                isBig = true;
                OnBigPlaneFound.Invoke();
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


}
