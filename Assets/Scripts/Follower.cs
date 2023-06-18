using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    // �Ѿ��� �߻��ϴ� ������ (������)
    public float cur_delay;    // ���� ���� �ð�
    public float reload_delay; // ������ �ð�

    // �÷��̾ ���󰡱� ���� ����
    public Vector3 follow_pos; // �ȷο��� ���� ��ġ
    public int follow_delay;   // �ȷο��� �̵��� �����ϴ� ����
    public Transform parent;   // �ȷο��� ���� �θ� ������Ʈ (�÷��̾�)
    public Queue<Vector3> parent_pos;
    // Queue - ����Ʈ�� �迭�� ����� �ڷᱸ��  /  ���� �Էµ� �����Ͱ� ���� ������ �ڷᱸ�� -> FIFO (First Input First Out)
    //       - ������ ����Ʈ�� �迭�� �޸� �����͸� ����ְų�(Enqueue), ������(Dequeue) �ΰ��� �۾����θ� �����͸� ������
    //       - �������� ������ ���ġ�ϰų� �߾��� �����͸� ����, �����ϴ� ���� ������ �Ҽ� ���ٴ� ����

    // ��� ���� - �����̳� ������ ũ�� ����� ���� �����Ͷ�� ����Ʈ�� �迭���� ������ ������ �Ǿ��ִ� ť�� �̿��ؼ� �����͸� ������ �ϴ� ���� �� ���� ���� �ֱ� ����

    public ObjectManager object_manager;

    void Awake() // �������� ����
    {
        parent_pos = new Queue<Vector3>();
    }

    void Update() // �� �����Ӹ���
    {
        Watch();  // �÷��̾��� ��ġ ������Ʈ �ϱ� (�÷��̾ ���󰡱� ����)

        Follow(); // �÷��̾� ���󰡱�

        Fire();   // �Ѿ� �߻� (�ܹ����� Ű �Է��� Update()���� ����)

        Reload(); // �Ѿ� ���� (�ܹ����� Ű �Է��� Update()���� ����)
    }

    void Watch() // �÷��̾��� ��ġ�� Ȯ���ϴ� �Լ�
    {
        // Contains(ã�� ��) -> Queue�� �� �߿� ã�� ���� �ִ��� ���θ� �Ǵ��� (bool)
        if (!parent_pos.Contains(parent.position)) // Queue�� �ִ� ������ �߿� ���� ��ġ�� ���� ������(�÷��̾ ������ ������) �����Ű�� ����
        {
            // Enqueue() -> �ش� Queue�� �����͸� �����ϴ� �Լ�
            parent_pos.Enqueue(parent.position);   // Queue�� �θ� ������Ʈ (�÷��̾�)�� ��ġ�� ������ (Input)
        }

        if (parent_pos.Count > follow_delay)   // Queue�� ������ �������� ������ ä�����ٸ�
        {
            // Dequeue() -> �ش� Queue�� ù �����͸� ���� ���� �� ���� ��ȯ�ϴ� �Լ� (Out)
            follow_pos = parent_pos.Dequeue(); // ������ ������ ���� Queue���� ���� ���� follow_pos ������ ������
        }

        else if (parent_pos.Count < follow_delay) // Queue�� ������ �������� ������ ä������ �ʾҴٸ�
        {
            follow_pos = parent.position;         // �θ� ������Ʈ (�÷��̾�)�� ��ġ�� �̵�
        }
    }

    void Follow() // �÷��̾ ���󰡴� �Լ�
    {
        transform.position = follow_pos; // Watch() �Լ����� �ٲ� ��ġ�� �̵��ϰ� ��
    }

    void Fire() // �Ѿ� �߻� �Լ�
    {
        // �߻� ��Ű�� �ʴ� ���� (�߻� ��ư�� ������ ����, ������ �Ǿ� ���� ����)
        if (!Input.GetButton("Fire1") || cur_delay < reload_delay) //  ������ ���� ���� �� (�߻� �ϰ� ���� ���� ��) || ���� ���� �ð��� ������ �ð����� �۴ٸ� (�������� �Ϸ�Ǿ� ���� �ʴٸ�) 
            return;                                                // ���� ��Ű�� ����

        // �ȷο��� ��ġ�� �Ѿ��� �÷��̾��� �������� ������
        GameObject bullet = object_manager.MakeObj("follower_bullet");
        bullet.transform.position = transform.position;

        // �Ѿ��� Rigidbody�� �����ͼ� AddForce()�� �Ѿ� �߻� ��� ����
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>(); // �Ѿ��� Rigidbody2D�� ������

        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);   // �Ѿ��� ���� �������� �̵� ��Ŵ (�÷��̾�� ������ �ٶ󺸰� �ֱ� ����) -> �߻�

        reload_delay = Random.Range(1.25f, 2f);                  // ���弱 �ð��� 1.25 ~ 2�ʻ����� �� �� �������� ����
        cur_delay = 0;                                          // ���� �ð� �ʱ�ȭ (�������� �ϰ� ����� ����)
    }

    void Reload() // ������ �Լ�
    {
        cur_delay += Time.deltaTime; // ���� ���� �ð��� �ð��� ������
    }
}
