using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn // 구조체를 사용할 것이기 때문에 변수만 사용해도 됨 - > MonoBehaviour는 필요 X  /  MonoBehaviour : 유니티에서 생성하는 모든 스크립트가 상속받는 기본 클래스
                                                                                                                 // : Start() 메소드나 Update() 메소드와 같은 것들도 MonoBehaviour에 구현되어 있는 기능들임
{
    // 적들을 구조체로 정리

    // 적을 소환하기 위해서 필요한 요소
    public float delay; // 소환까지 걸리는 시간
    public string type; // 적의 타입
    public int point;   // 적을 소환하는 위치
}