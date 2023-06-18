using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float dmg;

    public bool is_rotate; // 스스로 돌아가는 총알인지 여부

    void Update() // 매 프레임마다
    {
        if (is_rotate) // 스스로 돌아가는 총알이면
        {
            transform.Rotate(Vector3.forward * 10); // 돌아가게 만듦
        }
    }

    void OnTriggerEnter2D(Collider2D collision)         // 총알이 다른 오브젝트와 겹쳐졌을 때
    {
        if (collision.gameObject.tag == "BorderBullet") // 총알과 겹쳐진 오브젝트의 태그가 "BorderBullet"이라면
        {
            gameObject.SetActive(false);                // 총알을 비활성화 시킴
        }

        else if (collision.gameObject.tag == "EnemySpawning") // 총알과 겹쳐진 오브젝트의 태그가 "EnemySpawning"이라면
        {
            gameObject.SetActive(false);                      // 총알을 비활성화 시킴
        }
    }
}