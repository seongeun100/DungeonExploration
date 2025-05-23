using System.Collections.Generic;
using UnityEngine;

public class LiftPad : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float moveSpeed = 2f;

    private Vector3 target; // 현재 목표 위치
    private bool isMovingToEnd; // endPoint로 이동 여부
    private Vector3 lastPosition; // 이동량 계산용 위치

    private List<Transform> objectOnPad = new List<Transform>(); // 발판 위에 있는 오브젝트들


    void Start()
    {
        lastPosition = transform.position;
        SetTarget(); // 초기 타겟 설정
    }

    void Update()
    {
        // 현재 위치를 목표 지점으로 일정한 속도로 이동
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        // 이동 거리 계산
        Vector3 delta = transform.position - lastPosition;

        // 발판 위에 있는 모든 오브젝트 같이 이동
        foreach (Transform obj in objectOnPad)
        {
            if (obj != null)
            {
                obj.position += delta;
            }
        }

        lastPosition = transform.position;
    }

    // 현재 이동 상태에 따라 목표 위치 설정
    void SetTarget()
    {
        target = isMovingToEnd ? endPoint.position : startPoint.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody; // Collider에 연결된 Rigidbody 가져옴

        if (rb != null && !rb.isKinematic) // Rigidbody가 있고, 물리적으로 움직일 수 있을 때
        {
            if (!objectOnPad.Contains(rb.transform))
            {
                objectOnPad.Add(rb.transform);
            }
        }
        if (other.CompareTag("Player"))
        {
            isMovingToEnd = true;
            SetTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb != null)
        {
            objectOnPad.Remove(rb.transform);
        }

        if (other.CompareTag("Player"))
        {
            isMovingToEnd = false;
            SetTarget();
        }
    }
}
