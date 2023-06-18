using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    // 총알을 발사하는 딜레이 (재장전)
    public float cur_delay;    // 현재 장전 시간
    public float reload_delay; // 재장전 시간

    // 플레이어를 따라가기 위한 변수
    public Vector3 follow_pos; // 팔로워가 따라갈 위치
    public int follow_delay;   // 팔로워의 이동을 지연하는 변수
    public Transform parent;   // 팔로워가 따라갈 부모 오브젝트 (플레이어)
    public Queue<Vector3> parent_pos;
    // Queue - 리스트나 배열과 비슷한 자료구조  /  먼저 입력된 데이터가 먼저 나가는 자료구조 -> FIFO (First Input First Out)
    //       - 하지만 리스트나 배열과 달리 데이터를 집어넣거나(Enqueue), 빼내는(Dequeue) 두개의 작업으로만 데이터를 관리함
    //       - 데이터의 순서를 재배치하거나 중앙의 데이터를 삭제, 삽입하는 등의 편집을 할수 없다는 뜻임

    // 사용 이유 - 편집이나 순서가 크게 상관이 없는 데이터라면 리스트나 배열보다 간단한 구조로 되어있는 큐를 이용해서 데이터를 관리를 하는 것이 더 좋을 때가 있기 때문

    public ObjectManager object_manager;

    void Awake() // 시작하자 마자
    {
        parent_pos = new Queue<Vector3>();
    }

    void Update() // 매 프레임마다
    {
        Watch();  // 플레이어의 위치 업데이트 하기 (플레이어를 따라가기 위함)

        Follow(); // 플레이어 따라가기

        Fire();   // 총알 발사 (단발적인 키 입력은 Update()에서 구현)

        Reload(); // 총알 장전 (단발적인 키 입력은 Update()에서 구현)
    }

    void Watch() // 플레이어의 위치를 확인하는 함수
    {
        // Contains(찾을 값) -> Queue의 값 중에 찾을 값이 있는지 여부를 판단함 (bool)
        if (!parent_pos.Contains(parent.position)) // Queue에 있는 데이터 중에 현재 위치의 값이 있으면(플레이어가 가만히 있으면) 실행시키지 않음
        {
            // Enqueue() -> 해당 Queue에 데이터를 저장하는 함수
            parent_pos.Enqueue(parent.position);   // Queue에 부모 오브젝트 (플레이어)의 위치를 저장함 (Input)
        }

        if (parent_pos.Count > follow_delay)   // Queue에 일정한 데이터의 갯수가 채워졌다면
        {
            // Dequeue() -> 해당 Queue의 첫 데이터를 빼낸 다음 그 값을 반환하는 함수 (Out)
            follow_pos = parent_pos.Dequeue(); // 위에서 저장한 값을 Queue에서 빼낸 다음 follow_pos 변수에 대입함
        }

        else if (parent_pos.Count < follow_delay) // Queue에 일정한 데이터의 갯수가 채워지지 않았다면
        {
            follow_pos = parent.position;         // 부모 오브젝트 (플레이어)의 위치로 이동
        }
    }

    void Follow() // 플레이어를 따라가는 함수
    {
        transform.position = follow_pos; // Watch() 함수에서 바꾼 위치로 이동하게 함
    }

    void Fire() // 총알 발사 함수
    {
        // 발사 시키지 않는 조건 (발사 버튼을 누르지 않음, 장전이 되어 있지 않음)
        if (!Input.GetButton("Fire1") || cur_delay < reload_delay) //  누르고 있지 않을 때 (발사 하고 있지 않을 때) || 현재 장전 시간이 재장전 시간보다 작다면 (재장전이 완료되어 있지 않다면) 
            return;                                                // 실행 시키지 않음

        // 팔로워의 위치에 총알을 플레이어의 방향으로 생성함
        GameObject bullet = object_manager.MakeObj("follower_bullet");
        bullet.transform.position = transform.position;

        // 총알의 Rigidbody를 가져와서 AddForce()로 총알 발사 기능 구현
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>(); // 총알의 Rigidbody2D를 가져옴

        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);   // 총알을 위쪽 방향으로 이동 시킴 (플레이어는 위쪽을 바라보고 있기 때문) -> 발사

        reload_delay = Random.Range(1.25f, 2f);                  // 재장선 시간을 1.25 ~ 2초사이의 수 중 랜덤으로 정함
        cur_delay = 0;                                          // 장전 시간 초기화 (재장전을 하게 만들기 위함)
    }

    void Reload() // 재장전 함수
    {
        cur_delay += Time.deltaTime; // 현재 장전 시간에 시간을 더해줌
    }
}
