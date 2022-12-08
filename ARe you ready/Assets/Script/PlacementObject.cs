using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine;

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

    public Text checkText;

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
        //scaleSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        //checkText.text = "select: " + scaleSliders.transform.position;
        scaleSliders.gameObject.SetActive(Selected);
        //scaleSliders.transform.position = camera.WorldToScreenPoint(new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z));
        scaleSliders.transform.position = new Vector3(originalSlider.transform.position.x, originalSlider.transform.position.y + 100f, originalSlider.transform.position.z);
        scaleSliders.value = preEachSliderValue;
        PlacementAndDragging.forAll = false;
    }

    private void ScaleChangedEach(float newValue)
    {
       if(PlacementAndDragging.forAll == false)
        {
            float newVal = this.preEachSliderValue - newValue;

            this.Size = newVal;
            this.preEachSliderValue = newValue;

            //aRSessionOrigin.MakeContentAppearAt(this.transform, Quaternion.identity);
            this.gameObject.transform.localScale = new Vector3(this.gameObject.transform.localScale.x - newVal, this.gameObject.transform.localScale.y - newVal, this.gameObject.transform.localScale.z - newVal);

            if (this.gameObject.transform.localScale.x <= 0 || this.gameObject.transform.localScale.y <= 0 || this.gameObject.transform.localScale.x <= 0)
            {
                this.gameObject.transform.localScale = new Vector3(0, 0, 0);
            }

            float yDiff = PlacementAndDragging.currPlaneY - (this.gameObject.transform.GetComponent<CapsuleCollider>().bounds.min.y);
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
