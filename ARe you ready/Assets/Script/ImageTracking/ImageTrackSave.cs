using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageTrackSave : MonoBehaviour
{
    private static ImageTrackSave instance = null;

    public Vector3 youtube;

    void Start()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static ImageTrackSave Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    void Update()
    {
        
    }

    public void InitGame()
    {

    }
}
