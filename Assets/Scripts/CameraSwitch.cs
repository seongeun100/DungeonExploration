using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera firstPersonCamera; // 1인칭 시점 카메라
    public Camera thirdPersonCamera; // 3인칭 시점 카메라

    private bool isFirstPerson = true; // 현재 카메라 상태(true = 1인칭, false = 3인칭)

    // 시작 시 카메라 상태 설정
    void Start()
    {
        UpdateCameraView();
    }

    // 카메라 시점 전환
    public void SwitchView()
    {
        isFirstPerson = !isFirstPerson;
        UpdateCameraView();
    }

    // 카메라 상태를 실제 활성화/비활성화에 반영
    void UpdateCameraView()
    {
        firstPersonCamera.gameObject.SetActive(isFirstPerson);
        thirdPersonCamera.gameObject.SetActive(!isFirstPerson);
    }

    // 현재 카메라가 1인칭인지 여부 반환
    public bool IsFirstPerson()
    {
        return isFirstPerson;
    }

    // 현재 활성화된 카메라 반환
    public Camera GetActiveCamera()
    {
        return isFirstPerson ? firstPersonCamera : thirdPersonCamera;
    }
}
