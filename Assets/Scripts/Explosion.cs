using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    Animator anim;

    void Awake() // 시작하자 마자
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable() // 컴포넌트가 활성화 되었을 때
    {
        Invoke("Disable", 2f); // 2초 뒤에 해당 오브젝트를 비활성화 시킴
    }

    void Disable() // 해당 오브젝트를 비활성화 시키는 함수
    {
        gameObject.SetActive(false);
    }

    public void StartExplosion(string target) // 폭발 애니매이션을 실행하는 함수  /  파라미터 -> 어떤 오브젝트가 터지느냐에 따라서 폭발의 크기를 다르게 해주기 위함
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

        SoundManager.instance.PlaySound("Explosion"); // 폭발 하는 사운드 출력
    }
}
