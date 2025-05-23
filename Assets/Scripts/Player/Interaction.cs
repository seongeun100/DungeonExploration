using TMPro;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private LayerMask layerMask;

    private float lastCheckTime;
    private float maxCheckDistance; // 감지 최대 거리
    private float checkRate = 0.05f; // 감지 주기

    private GameObject curInteractGameObject; // 현재 감지된 오브젝트
    private IInteractable curInteractable; // 감지된 오브젝트의 인터페이스

    private CameraSwitch cameraSwitch;

    void Start()
    {
        cameraSwitch = GetComponent<CameraSwitch>();
    }

    void Update()
    {
        // 일정 주기마다 감지
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;
            Camera camera = cameraSwitch.GetActiveCamera(); // 현재 사용중인 카메라 가져오기
            maxCheckDistance = cameraSwitch.IsFirstPerson() ? 3f : 7f; // 1인칭이면 3f 3인칭이면 7f

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); // 화면 중앙으로 Ray생성

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                // 오브젝트에 닿았을 경우
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText(); // 상호작용 UI 업데이트
                }
            }
            else
            {
                // 감지되지 않을 경우 초기화
                curInteractGameObject = null;
                curInteractable = null;
                interactionUI.gameObject.SetActive(false);
            }
        }
    }

    // 감지된 오브젝트 있으면 UI 활성화
    void SetPromptText()
    {
        interactionUI.gameObject.SetActive(true);
        promptText.text = curInteractable.GetInteractPrompt();
    }

    // 상호작용 키 입력시 실행
    public void Interact()
    {
        if (curInteractable != null)
        {
            curInteractable.OnInteract();
            curInteractGameObject = null;
            curInteractable = null;
            interactionUI.gameObject.SetActive(false);
        }
    }

}
