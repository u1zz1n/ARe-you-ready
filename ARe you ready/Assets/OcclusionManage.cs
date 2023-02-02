using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class OcclusionManage : MonoBehaviour
{
    private AROcclusionManager arOcclusionManager;

    // Start is called before the first frame update
    void Start()
    {
        arOcclusionManager = GetComponent<AROcclusionManager>();

    }
    public void ChangeQualityTo(EnvironmentDepthMode environmentDepthMode)
    {
        arOcclusionManager.requestedEnvironmentDepthMode = environmentDepthMode;
    }

    public EnvironmentDepthMode GetCurrentDepthMode()
    {
        return arOcclusionManager.requestedEnvironmentDepthMode;
    }

    public void ToggleQuality()
    {
        //EnvironmentDepthMode depthMode = GetCurrentDepthMode();

        //ChangeQualityTo(EnvironmentDepthMode.Fastest);
    }
}
