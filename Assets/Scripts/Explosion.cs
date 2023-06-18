using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    Animator anim;

    void Awake() // �������� ����
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable() // ������Ʈ�� Ȱ��ȭ �Ǿ��� ��
    {
        Invoke("Disable", 2f); // 2�� �ڿ� �ش� ������Ʈ�� ��Ȱ��ȭ ��Ŵ
    }

    void Disable() // �ش� ������Ʈ�� ��Ȱ��ȭ ��Ű�� �Լ�
    {
        gameObject.SetActive(false);
    }

    public void StartExplosion(string target) // ���� �ִϸ��̼��� �����ϴ� �Լ�  /  �Ķ���� -> � ������Ʈ�� �������Ŀ� ���� ������ ũ�⸦ �ٸ��� ���ֱ� ����
    {
        anim.SetTrigger("on_explosion");

        switch (target) 
        {
            case "A":
                transform.localScale = Vector3.one * 1.25f;
                break;

            case "B":
            case "P":
                transform.localScale = Vector3.one * 1.75f;
                break;

            case "C":
                transform.localScale = Vector3.one * 2f;
                break;

            case "Boss":
                transform.localScale = Vector3.one * 3f;
                break;
        }

        SoundManager.instance.PlaySound("Explosion"); // ���� �ϴ� ���� ���
    }
}
