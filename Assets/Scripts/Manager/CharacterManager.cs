using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    private static CharacterManager _instance;

    // 외부에서 접근 가능한 싱글톤 인스턴스
    public static CharacterManager Instance
    {
        get
        {
            // 인스턴스가 없으면 새로운 GameObject를 생성해서 컴포넌트 추가
            if (_instance == null)
            {
                _instance = new GameObject("CharacterManager").AddComponent<CharacterManager>();
            }
            return _instance;
        }
    }

    // 현재 플레이어 오브젝트 참조
    public Player _player;

    public Player Player
    {
        get { return _player; }
        set { _player = value; }
    }

    // 씬 변경 시에도 유지되도록 설정
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (_instance == this)
            {
                Destroy(gameObject);
            }
        }
    }
}
