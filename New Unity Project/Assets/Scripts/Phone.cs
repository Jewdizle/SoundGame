using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Phone : MonoBehaviour
{
    private bool gyroEnabled;
    private Gyroscope gyro;

    private GameObject cameraContainer;
    private Quaternion rot;

    private AudioSource audio;
    public string RaycastReturn;

    public float audioMin;
    public float audioMax;
    public float volumeStep;

    public GameObject startScreen;
    public GameObject gameScreen;

    public GameObject sources;
    private GameObject source;  
    private GameObject sourceContainer;

    private Touch touch;
    bool touching;

    public GameObject pauseButton;
    private Text pauseText;
    public GameObject resetButton;
    bool playing;

    void Start()
    {
        pauseText = pauseButton.GetComponent<Text>();

        cameraContainer = new GameObject("Camera Container");
        cameraContainer.transform.position = transform.position;
        transform.SetParent(cameraContainer.transform);

        gyroEnabled = EnabledGyro();

        gameScreen.SetActive(false);
        startScreen.SetActive(true);

        sources.SetActive(false);
        resetButton.SetActive(false);
    }

    private bool EnabledGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            cameraContainer.transform.rotation = Quaternion.Euler(0, 0, 0);
            rot = new Quaternion(0, 0, 1, 0);

            return true;
        }
        return false;
    }

    void Update()
    {
        if(gyroEnabled)
        {
            transform.localRotation = gyro.attitude;
        }

        touch = Input.GetTouch(0);

        if (gameScreen.activeSelf == true)
        {
            if (touch.phase == TouchPhase.Began)
            {
                ray();
                touching = true;             
                Debug.Log("started");
            }

            if (touch.phase == TouchPhase.Ended)
            {
                touching = false;
                Debug.Log("ended");
            }
        }

        if (touching == true)
        {
            source.transform.parent.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 
                                                                            source.transform.parent.rotation.y,
                                                                            source.transform.parent.rotation.z));
        }
    }

    void ray()
    {
        int layerMask = 1 << 8;

        layerMask = ~layerMask;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            RaycastReturn = hit.collider.gameObject.name;
            source = GameObject.Find(RaycastReturn);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log(source);
        }
    }

    public void Up()
    {
        ray();
        if (source != null)
        {
            audio = source.GetComponent<AudioSource>();
            if (audio.volume < audioMax)
            {
                audio.volume = audio.volume + volumeStep;
            }
            source = null;
        }
    }

    public void Down()
    {
        ray();
        if (source != null)
        {
            audio = source.GetComponent<AudioSource>();
            if (audio.volume > audioMin)
            {
                audio.volume = audio.volume - volumeStep;
                
            }
        }
        source = null;
    }

    public void Begin()
    {
        startScreen.SetActive(false);
        gameScreen.SetActive(true);
        sources.SetActive(true);
        playing = true;
    }

    public void PlayPause()
    {  
        if(playing == true)
        {
            Time.timeScale = 0.0f;
            pauseText.text = "Play";
            resetButton.SetActive(true);

        }

        if(playing == false)
        {
            Time.timeScale = 1.0f;
            pauseText.text = "Pause";
            resetButton.SetActive(false);
        }

        playing = !playing;
    }

    public void ResetPlease()
    {
        gameScreen.SetActive(false);
        startScreen.SetActive(true);
        sources.SetActive(false);
        resetButton.SetActive(false);
        playing = false;
        pauseText.text = "Play";

    }
}
