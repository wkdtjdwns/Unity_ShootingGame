using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string type; // �ش� �������� � ���������� ������ ����
    Rigidbody2D rigid;

    // �������� �÷��̾ ���ϰ� �ϱ� ���� ���� (�������� �̹� Scene�� �ö��� ������Ʈ�� ���� �� �� ���� -> GameManager���� ������Ʈ�� ������ ����)
    public GameObject player;

    void Awake() // �������� ����
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() // ������ ���ݸ���
    {
        switch (type)
        {
            // �������� ������ ���� �ٸ� �ӵ��� �÷��̾��� �������� �̵��ϰ� ��
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

    void OnTriggerEnter2D(Collider2D collision)       // �������� �ٸ� ������Ʈ�� �������� ��
    {
        if (collision.gameObject.tag == "ItemBullet") // �����۰� ������ ������Ʈ�� �±װ� "ItemBullet"�̶��
        {
            gameObject.SetActive(false);              // �������� ��Ȱ��ȭ ��Ŵ
        }
    }
}
