using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float dmg;

    public bool is_rotate; // ������ ���ư��� �Ѿ����� ����

    void Update() // �� �����Ӹ���
    {
        if (is_rotate) // ������ ���ư��� �Ѿ��̸�
        {
            transform.Rotate(Vector3.forward * 10); // ���ư��� ����
        }
    }

    void OnTriggerEnter2D(Collider2D collision)         // �Ѿ��� �ٸ� ������Ʈ�� �������� ��
    {
        if (collision.gameObject.tag == "BorderBullet") // �Ѿ˰� ������ ������Ʈ�� �±װ� "BorderBullet"�̶��
        {
            gameObject.SetActive(false);                // �Ѿ��� ��Ȱ��ȭ ��Ŵ
        }

        else if (collision.gameObject.tag == "EnemySpawning") // �Ѿ˰� ������ ������Ʈ�� �±װ� "EnemySpawning"�̶��
        {
            gameObject.SetActive(false);                      // �Ѿ��� ��Ȱ��ȭ ��Ŵ
        }
    }
}