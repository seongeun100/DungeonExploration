# DungeonExploration

## ✅ 개요

캐릭터 이동과 물리 처리, 상호 작용을 구현해보기 위해 만든 Unity 기반의 3D 프로젝트 입니다.

---

## 🎮 조작법 (Controls)

| 키 | 동작 |
|----|------|
| `W` `A` `S` `D` | 캐릭터 이동 |
| `Space` | 점프 |
| `F` | 상호작용 (아이템 줍기) |
| `Tab` | 인벤토리 열기/닫기 |
| `V` | 카메라 시점 전환 (1인칭/3인칭) |
| `E` | 벽 매달리기 |

## ✨ 주요 기능

- **1인칭/3인칭 카메라 전환**  
  플레이어 시점을 자유롭게 전환할 수 있습니다.

- **아이템 상호작용 시스템**  
  필드의 아이템과 상호작용하여 수집하고 인벤토리에 저장할 수 있습니다.

- **버프 시스템**  
  버프 소모품 사용 시 이동 속도 증가, 점프력 증가 등의 능력치를 부여받습니다.

- **스태미나 시스템**  
  행동에 따라 스태미나가 감소하며, 스태미나가 0이 되며 특정 행동이 불가합니다.

- **점프 패드**  
  점프 패드에 닿으면 위쪽으로 캐릭터를 튕겨올립니다.

- **움직이는 플랫폼**  
  시간에 따라 정해진 구역을 이동하는 발판과 플레이어가 올라갈 때 이동하는 발판이 있습니다.

- **벽 타기 및 매달리기**  
  플레이어가 벽에 붙어 오르거나 매달릴 수 있습니다.
  
- **레이저 트랩**  
  플레이어가 레이저를 통과하면 경고 메시지와 함께 트랩이 발동됩니다.

---

## 📁 프로젝트 구조

- `Scripts/Interface`: Interactable 인터페이스
- `Scripts/Item`: ItemObject, ItemData 등 아이템 관련 스크립트
- `Scripts/Manager`: CharacterManager (전역 플레이어 참조 싱글톤)
- `Scripts/Player`: CameraSwitch, Interaction, PlayerCondition, PlayerController 등 플레이어 제어 클래스
- `Scripts/Resource`: JumpPad, LiftPad, LoopPad, LaserTrap 등 환경 요소
- `Scenes/ScriptableObject`: Item_Apple, Item_Carrot 등 스크립터블 오브젝트 기반 아이템 데이터
- `Scripts/UI`: BuffSlot, BuffSlotPool, Condition, ItemSlot 등 UI 및 상태 표시 관련 클래스
- `Prefabs/`: UI 프리팹

## ▶️ 실행 방법
1. Unity 2022.3 LTS 이상 버전으로 프로젝트 열기
2. MainScene 실행하면 게임이 시작됩니다

## 🛠 사용한 기술 및 패키지
- Unity 3D BRP
- TextMeshPro
- Unity Input System
- PlayerPrefs
