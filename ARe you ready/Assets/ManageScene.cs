using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManageScene : MonoBehaviour
{
    float time = 0f;
    static bool digipenLogo = false;
    static bool teamlogof = false;
    [SerializeField]
    public Image digipen;

    public GameObject teamLogo;

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start() {
        if(SceneManager.GetActiveScene().name == "Menu")
        {
            teamLogo.gameObject.SetActive(false);
        }
    }
    private void Update() {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            if (!digipenLogo)
            {
                time += Time.deltaTime;
                if (time > 2f)
                {
                    digipenLogo = true;
                    time = 0f;
                }
            }
            else
            {
                digipen.gameObject.SetActive(false);
                //
            }

            if (digipenLogo && !teamlogof)
            {
                teamLogo.gameObject.SetActive(true);
                time += Time.deltaTime;
                if (time > 2f)
                {
                    teamlogof = true;
                    teamLogo.gameObject.SetActive(false);
                }
            }
        }
    }

    public void TechDemo()
    {
        SceneManager.LoadScene("SampleScene");
        SoundManager.instance.PlaySfx("UI_Press");
    }

    public void BowlingDemo()
    {
        SceneManager.LoadScene("RollBall");
        SoundManager.instance.PlaySfx("UI_Press");
    }

    public void mainMenu()
    {
        SoundManager.instance.PlaySfx("UI_Press");
        SceneManager.LoadScene("Menu");
    }

    public void Credit()
    {
        SceneManager.LoadScene("Credit");
        SoundManager.instance.PlaySfx("UI_Press");

    }
    public void ImageTracking()
    {
        SoundManager.instance.PlaySfx("UI_Press");
        SceneManager.LoadScene("ImageTracking");
    }

    public void ObjectTracking()
    {
        SoundManager.instance.PlaySfx("UI_Press");
        SceneManager.LoadScene("ObjectTracking");
    }

}
