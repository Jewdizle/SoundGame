using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : MonoBehaviour
{
    private bool gyroEnabled;
    private Gyroscope gyro;

    private GameObject cameraContainer;
    private Quaternion rot;

    GameObject source;
    AudioSource audio;
    public string RaycastReturn;

    public float audioMin;
    public float audioMax;
    public float volumeStep;

    public GameObject startScreen;
    public GameObject gameScreen;

    void Start()
    {
        cameraContainer = new GameObject("Camera Container");
        cameraContainer.transform.position = transform.position;
        transform.SetParent(cameraContainer.transform);

        gyroEnabled = EnabledGyro();

        gameScreen.SetActive(false);
        startScreen.SetActive(true);
    }

    private bool EnabledGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            cameraContainer.transform.rotation = Quaternion.Euler(90f, 90f, 0);
            rot = new Quaternion(0, 0, 1, 0);

            return true;
        }
        return false;
    }

    void Update()
    {
        if(gyroEnabled)
        {
            transform.localRotation = gyro.attitude * rot;
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
            Debug.Log("Did Hit");
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
    }
}
