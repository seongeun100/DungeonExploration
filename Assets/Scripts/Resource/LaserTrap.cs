using System.Collections;
using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    [SerializeField] private Transform laserStart;
    [SerializeField] private Transform laserEnd;
    [SerializeField] private LayerMask Player;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private LineRenderer lineRenderer;

    private Coroutine warning;
    private float checkRate = 0.05f;
    private float lastCheckTime;

    void Update()
    {
        // 일정 주기마다 감지
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;
            CheckPlayer();
        }

    }

    void CheckPlayer()
    {
        // 레이저 방향, 거리 계산
        Vector3 dir = laserEnd.position - laserStart.position;
        float distance = dir.magnitude;
        RaycastHit hit;
        // 레이저 시작 지점에서 방향으로 발사
        Ray ray = new Ray(laserStart.position, dir.normalized);

        // 레이저 시각화
        lineRenderer.SetPosition(0, laserStart.position);
        lineRenderer.SetPosition(1, laserEnd.position);

        if (Physics.Raycast(ray, out hit, distance, Player))
        {
            ShowWarning();
            
            // 플레이어 뒤로 밀어냄
            Transform player = hit.collider.transform;
            player.position -= transform.forward * 5f;
        }
    }

    void ShowWarning()
    {
        // 경고가 표시되고 있을때 중복 방지
        if (warning == null)
        {
            warningPanel.SetActive(true);
            warning = StartCoroutine(HideWarning(2f));
        }
    }

    IEnumerator HideWarning(float delay)
    {
        yield return new WaitForSeconds(delay);
        warningPanel.SetActive(false);
        warning = null;
    }
}
