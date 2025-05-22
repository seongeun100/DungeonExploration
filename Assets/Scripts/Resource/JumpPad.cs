using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jumpPower = 500f;

    // 콜라이더 충돌 발생 시 호출
    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 플레이어인지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어의 Rigidbody 가져오기
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(transform.up * jumpPower, ForceMode.Impulse); // 점프 패드 위쪽 방향으로 힘을 가함
            }
        }
    }
}
