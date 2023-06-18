using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string type; // 해당 아이템이 어떤 아이템인지 구분할 변수
    Rigidbody2D rigid;

    // 아이템이 플레이어를 향하게 하기 위한 변수 (프리펩은 이미 Scene에 올라인 오브젝트에 접근 할 수 없음 -> GameManager에서 컴포넌트를 가지게 해줌)
    public GameObject player;

    void Awake() // 시작하자 마자
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() // 일정한 간격마다
    {
        switch (type)
        {
            // 아이템의 종류에 따라 다른 속도로 플레이어의 방향으로 이동하게 함
            case "Boom":
                Vector3 dir_vec_b = player.transform.position - transform.position;
                rigid.AddForce(dir_vec_b.normalized * 0.08f, ForceMode2D.Impulse);
                break;

            case "Coin":
                Vector3 dir_vec_c = player.transform.position - transform.position;
                rigid.AddForce(dir_vec_c.normalized * 0.075f, ForceMode2D.Impulse);
                break;

            case "Power":
                Vector3 dir_vec_p = player.transform.position - transform.position;
                rigid.AddForce(dir_vec_p.normalized * 0.0625f, ForceMode2D.Impulse);
                break;

            case "Follower":
                Vector3 dir_vec_f = player.transform.position - transform.position;
                rigid.AddForce(dir_vec_f.normalized * 0.0575f, ForceMode2D.Impulse);
                break;

            case "Heal":
                Vector3 dir_vec_h = player.transform.position - transform.position;
                rigid.AddForce(dir_vec_h.normalized * 0.0525f, ForceMode2D.Impulse);
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)       // 아이템이 다른 오브젝트와 겹쳐졌을 때
    {
        if (collision.gameObject.tag == "ItemBullet") // 아이템과 겹쳐진 오브젝트의 태그가 "ItemBullet"이라면
        {
            gameObject.SetActive(false);              // 아이템을 비활성화 시킴
        }
    }
}
