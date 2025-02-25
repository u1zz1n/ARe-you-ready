using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlacementObject : MonoBehaviour
{
    [SerializeField]
    private bool IsSelected;

    [SerializeField]
    private bool IsLocked;

    [SerializeField]
    private float objectSize = 1;

    [SerializeField]
    private float preSliderValue = 1;

    [SerializeField]
    private float preEachSliderValue = 1;

    [SerializeField]
    private Slider scaleSlider;
    
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private Camera camera;

    public ARSessionOrigin aRSessionOrigin;

    private Slider scaleSliders;

    private float yPosition;

    public Slider originalSlider;

    //public Text checkText;

    Vector3 sliderPosition;

    public bool Selected
    {
        get
        {
            return this.IsSelected;
        }
        set
        {
            IsSelected = value;
        }
    }

    public bool Locked
    {
        get
        {
            return this.IsLocked;
        }
        set
        {
            IsLocked = value;
        }
    }

    public float Size
    {
        get
        {
            return this.objectSize;
        }
        set
        {
            objectSize = value;
        }
    }

    public float PreSliderValue
    {
        get
        {
            return this.preSliderValue;
        }
        set
        {
            preSliderValue = value;
        }
    }

    public float PreEachSliderValue
    {
        get
        {
            return this.preEachSliderValue;
        }
        set
        {
            preEachSliderValue = value;
        }
    }

    public Slider ScaleSlider
    {
        get
        {
            return this.scaleSlider;
        }
        set
        {
            scaleSlider.value = value.value;
        }
    }

    public float YPosition
    {
        get
        {
            return this.yPosition;
        }
        set
        {
            yPosition = value;
        }
    }

    public void SetOverlayText(string text)
    {

    }

    void Awake()
    {
    }

    void Start()
    {
        //Canvas canvas = (Canvas)GameObject.FindObjectOfType(typeof(Canvas));
        scaleSliders = Instantiate(scaleSlider, canvas.transform).GetComponent<Slider>();
        scaleSliders.gameObject.transform.SetParent(canvas.gameObject.transform, false);
        scaleSliders.transform.position = this.transform.position;
        scaleSliders.onValueChanged.AddListener(ScaleChangedEach);

        if (SceneManager.GetActiveScene().name != "SampleScene")
        {
            sliderPosition = originalSlider.transform.position;
            originalSlider.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        //checkText.text = "select: " + scaleSliders.transform.position;
        scaleSliders.gameObject.SetActive(Selected);
        //scaleSliders.transform.position = camera.WorldToScreenPoint(new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z));
        
        if(SceneManager.GetActiveScene().name == "SampleScene")
        {
            scaleSliders.transform.position = new Vector3(originalSlider.transform.position.x, originalSlider.transform.position.y + 100f, originalSlider.transform.position.z);
        }
        else
        {
            scaleSliders.transform.position = new Vector3(sliderPosition.x, sliderPosition.y + 100f, sliderPosition.z);
        }

        scaleSliders.value = preEachSliderValue;
        PlacementAndDragging.forAll = false;
    }

    private void ScaleChangedEach(float newValue)
    {
       if(PlacementAndDragging.forAll == false)
        {
            float newVal = (this.preEachSliderValue - newValue) * 0.3f;

            this.Size = newVal;
            this.preEachSliderValue = newValue;

            //aRSessionOrigin.MakeContentAppearAt(this.transform, Quaternion.identity);
            this.gameObject.transform.localScale = new Vector3(this.gameObject.transform.localScale.x - newVal, this.gameObject.transform.localScale.y - newVal, this.gameObject.transform.localScale.z - newVal);

            if (this.gameObject.transform.localScale.x <= 0 || this.gameObject.transform.localScale.y <= 0 || this.gameObject.transform.localScale.x <= 0)
            {
                this.gameObject.transform.localScale = new Vector3(0, 0, 0);
            }

            float yDiff = 0;

            if (SceneManager.GetActiveScene().name == "SampleScene")
            {
                yDiff = PlacementAndDragging.currPlaneY - (this.gameObject.transform.GetComponent<CapsuleCollider>().bounds.min.y);
            }
            else
            {
                if (GPS.locationNumber == 2 || GPS.locationNumber == 3 || GPS.locationNumber == 4)
                {
                    yDiff = PlacingAndDragging.currPlaneY2 - (this.gameObject.transform.GetComponent<BoxCollider>().bounds.min.y);
                }
                else
                {
                    yDiff = PlacingAndDragging.currPlaneY2 - (this.gameObject.transform.GetComponent<CapsuleCollider>().bounds.min.y);
                }
            }
            Vector3 newPosition = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + yDiff, this.gameObject.transform.position.z);

            this.gameObject.transform.position = newPosition;
            this.YPosition = newPosition.y;
        }
    }
}

/*
         if (this.PreSliderValue > newValue)
        {
            this.Size -= (newValue * 0.1f);
        }
        else
        {
            this.Size += (newValue * 0.1f);
        }

        this.PreSliderValue = newValue;

        //if (this.name == "BowlingPins")
        //{
        //     for (int i = 0; i < this.transform.childCount; i++)
        //     {
        //        aRSessionOrigin.MakeContentAppearAt(this.transform.GetChild(i).gameObject.transform, Quaternion.identity);
        //        this.transform.GetChild(i).gameObject.transform.localScale = Vector3.one * this.Size;
        //
        //        if (this.gameObject.transform.localScale.x <= 0 || this.gameObject.transform.localScale.y <= 0 || this.gameObject.transform.localScale.z <= 0)
        //        {
        //            this.gameObject.transform.localScale = new Vector3(0, 0, 0);
        //        }
        //    }
        // }
        // else
        // {
            aRSessionOrigin.MakeContentAppearAt(this.transform, Quaternion.identity);
            this.transform.localScale = Vector3.one * this.Size;

            //if (this.gameObject.transform.localScale.x <= 0 || this.gameObject.transform.localScale.y <= 0 || this.gameObject.transform.localScale.z <= 0)
            //{
            //    this.gameObject.transform.localScale = new Vector3(0, 0, 0);
            //}
        //}
 */
