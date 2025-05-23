using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopPad : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float waitTime = 2f;

    private Vector3 target; // 현재 목표 위치
    private bool isMovingToB = true; // pointB로 이동 여부
    private bool isWaiting = false; // 대기 여부
    private Vector3 lastPosition; // 이동량 계산용 위치

    private List<Transform> objectOnPad = new List<Transform>(); // 발판 위에 있는 오브젝트들

    void Start()
    {
        target = pointB.position;
        lastPosition = transform.position;

    }

    void Update()
    {
        if (isWaiting) return;

        // 현재 위치를 목표 지점으로 일정한 속도로 이동
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        // 목표 위치에 도착 시
        if (!isWaiting && Vector3.Distance(transform.position, target) < 0.01f)
        {
            StartCoroutine(WaitBeforeSwitch());
        }

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

    IEnumerator WaitBeforeSwitch()
    {
        isWaiting = true; // 대기 상태

        yield return new WaitForSeconds(waitTime);

        // 이동 방향 바꿈
        isMovingToB = !isMovingToB;
        target = isMovingToB ? pointB.position : pointA.position;

        isWaiting = false; // 대기 상태 해제
    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody; // Collider에 연결된 Rigidbody 가져옴

        if (rb != null && !rb.isKinematic) // Rigidbody가 있고, 물리적으로 움직일 수 있을 때
        {
            if (!objectOnPad.Contains(rb.transform))
            {
                objectOnPad.Add(rb.transform);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb != null)
        {
            objectOnPad.Remove(rb.transform);
        }
    }
}
