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
    private Slider scaleSlider;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private Camera camera;

    public ARSessionOrigin aRSessionOrigin;

    private Slider scaleSliders;

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
        scaleSliders.onValueChanged.AddListener(ScaleChanged);
        //scaleSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        //checkText.text = "select: " + scaleSliders.transform.position;
        scaleSliders.gameObject.SetActive(Selected);
        scaleSliders.transform.position = camera.WorldToScreenPoint(new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z));
    }

    private void ScaleChanged(float newValue)
    {
         this.Size += (newValue * 0.1f);

         if (this.name == "BowlingPins")
         {
             for (int i = 0; i < this.transform.childCount; i++)
             {
                 aRSessionOrigin.MakeContentAppearAt(this.transform.GetChild(i).gameObject.transform, Quaternion.identity);
                this.transform.GetChild(i).gameObject.transform.localScale = Vector3.one * newValue;
             }
         }
         else
         {
            aRSessionOrigin.MakeContentAppearAt(this.transform, Quaternion.identity);
            this.transform.localScale = Vector3.one * this.Size;
         }
         
    }
}
