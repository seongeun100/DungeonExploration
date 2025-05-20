using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;

    private bool isFirstPerson = true;

    void Start()
    {
        UpdateCameraView();
    }

    public void SwitchView()
    {
        isFirstPerson = !isFirstPerson;
        UpdateCameraView();
    }

    void UpdateCameraView()
    {
        firstPersonCamera.gameObject.SetActive(isFirstPerson);
        thirdPersonCamera.gameObject.SetActive(!isFirstPerson);
    }

    public bool IsFirstPerson()
    {
        return isFirstPerson;
    }

    public Camera GetActiveCamera()
    {
        return isFirstPerson ? firstPersonCamera : thirdPersonCamera;
    }
}
