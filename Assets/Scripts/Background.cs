using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float speed; // ��� �̵� �ӵ�

    // ��� ��ũ�Ѹ��� ���� ����
    public int start_index;     // ����� ����
    public int end_index;       // ����� ��
    public Transform[] sprites; // ����� ���۰� �� ��������Ʈ�� ������ �迭

    float view_height;         // ���� ī�޶��� ����

    private void Awake() // �������� ����
    {
        // ���� ī�޶��� ���̸� ������

        // Perspective ī�޶�� Orthographic ī�޶��� ������
        // Percpective - �繰�� ���� ���ٰ��� �������� �� ǥ���Ͽ��� ������

        // Orthographic - �繰�� ���ؼ� ���ٰ��� ������ ���� ǥ���Ͽ��� ������
        //              - ���� 2D�� 2.5D�� ������ �ÿ��� Orthographic�� ����Ͽ� ������

        // orthographicSize -> orthographic ī�޶��� ������
        view_height = Camera.main.orthographicSize * 2;
    }

    void Update() // �� �����Ӹ���
    {
        // ����� �Ʒ��� ��� �����̰� ��
        Vector3 cur_pos = transform.position;                     // ���� ��� ��ġ
        Vector3 next_pos = Vector3.down * speed * Time.deltaTime; // ������ �̵��� ��� ��ġ

        transform.position = cur_pos + next_pos;                  // ��� �̵�

        // ��� ��ũ�Ѹ� (����� ȭ�鿡 ���Ѵ�� �����ֱ� ����)
        // ��ũ�Ѹ� : ������Ʈ ���� �ð����� �ΰ� �����̰� �ϴ� ���
        if (sprites[end_index].position.y < view_height*(-1)) // ���� ����� �� �Ʒ������� ȭ�鿡 ������ �ʴ� ������ �����ߴٸ�
        {
            // ����� �� �Ʒ������� �� ���������� �÷���

            // localPosition�� position�� ����
            // position - ������Ʈ�� ��ġ�� �׻� ������ ������ �������� ������
            //          - ��ũ��Ʈ���� position ���� ��ġ�� ������ �� ���� ���ڿ� ������Ʈ �ν������� Ʈ�������� ���̴� ���ڰ� ��ġ ���� ���� �� ���� (������Ʈ�� �ٸ� ������Ʈ�� �ڽ��� ��� ������Ʈ�� ���� ���� �������� ��ġ��Ű�� ���� Local ��ǥ�� �����ϱ� ����)

            // localPosition - ������Ʈ�� ��ġ�� �׻� �θ� ������Ʈ�� ��ġ �������� ������
            //               - ���� �θ� ������Ʈ�� �������� ������ position ó�� ������ ������ �������� ��ġ�� ������ -> localPosition = position
            //               - �θ� ������Ʈ�� �����̱� ������ position �� �޸� ������Ʈ�� �ν������� Ʈ�������� ������ ���ڿ� ��ġ��

            // ������ - ������Ʈ�� ��ġ�� ������ ������ �������� ���� ���ΰ�?   -> position
            //        - ������Ʈ�� ��ġ�� �θ� ������Ʈ�� �������� ���� ���ΰ�? -> localPosition

            // ����� �ű�� ���� ����
            Vector3 back_sprite_pos = sprites[start_index].localPosition;
            Vector3 front_sprite_pos = sprites[end_index].localPosition;

            // ��� �ű��
            sprites[end_index].transform.localPosition = back_sprite_pos + Vector3.up * view_height; // ����� �� �Ʒ������� ��ġ�� �� ������ �ٷ� ���� �Ű� ��

            // end_index �� start_index ���� ������Ʈ (����� �Ű�� ������ �� �Ʒ������� �� �������� ��������Ʈ�� �ٲ�� ����)
            int start_index_save = start_index;
            start_index = end_index;
            end_index = (start_index_save - 1 == -1) ? sprites.Length - 1 : start_index_save - 1; // end_index�� start_index_save - 1 �� ���� -1 �̸� sprites.Length - 1 �� ����, �װ� �ƴ϶�� start_index_save - 1  ���� ������
        }
    }
}
