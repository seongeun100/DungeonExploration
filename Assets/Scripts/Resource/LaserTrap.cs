using System.Collections;
using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    public Transform laserStart;
    public Transform laserEnd;
    public LayerMask Player;
    public GameObject warningPanel;
    private Coroutine warning;

    public LineRenderer lineRenderer;

    private float checkRate = 0.05f;
    private float lastCheckTime;

    void Update()
    {
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

            Transform player = hit.collider.transform;

            player.position -= transform.forward * 5f;
        }
    }

    void ShowWarning()
    {
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
