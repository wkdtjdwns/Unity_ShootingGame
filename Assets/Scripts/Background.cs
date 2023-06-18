using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float speed; // 배경 이동 속도

    // 배경 스크롤링에 대한 변수
    public int start_index;     // 배경의 시작
    public int end_index;       // 배경의 끝
    public Transform[] sprites; // 배경의 시작과 끝 스프라이트를 관리할 배열

    float view_height;         // 게임 카메라의 높이

    private void Awake() // 시작하자 마자
    {
        // 게임 카메라의 높이를 설정함

        // Perspective 카메라와 Orthographic 카메라의 차이점
        // Percpective - 사물에 대해 원근감과 공간감을 잘 표현하여서 보여줌

        // Orthographic - 사물에 대해서 원근감과 공간감 없이 표현하여서 보여줌
        //              - 보통 2D나 2.5D를 제작할 시에는 Orthographic을 사용하여 제작함

        // orthographicSize -> orthographic 카메라의 사이즈
        view_height = Camera.main.orthographicSize * 2;
    }

    void Update() // 매 프라임마다
    {
        // 배경을 아래로 계속 움직이게 함
        Vector3 cur_pos = transform.position;                     // 현재 배경 위치
        Vector3 next_pos = Vector3.down * speed * Time.deltaTime; // 다음에 이동할 배경 위치

        transform.position = cur_pos + next_pos;                  // 배경 이동

        // 배경 스크롤링 (배경을 화면에 무한대로 보여주기 위함)
        // 스크롤링 : 오브젝트 간의 시간차를 두고 움직이게 하는 기법
        if (sprites[end_index].position.y < view_height*(-1)) // 만약 배경의 맨 아랫지점이 화면에 보이지 않는 지점에 도달했다면
        {
            // 배경의 맨 아랫지점을 맨 윗지점으로 올려줌

            // localPosition과 position의 차이
            // position - 오브젝트의 위치를 항상 월드의 원점을 기준으로 설정함
            //          - 스크립트에서 position 으로 위치를 변경할 때 쓰인 숫자와 오브젝트 인스펙터의 트랜스폼에 보이는 숫자가 일치 하지 않을 수 있음 (오브젝트가 다른 오브젝트의 자식인 경우 오브젝트를 월드 원점 기준으로 위치시키기 위해 Local 좌표를 변경하기 때문)

            // localPosition - 오브젝트의 위치를 항상 부모 오브젝트의 위치 기준으로 설정함
            //               - 물론 부모 오브젝트가 존재하지 않으면 position 처럼 월드의 원점을 기준으로 위치를 설정함 -> localPosition = position
            //               - 부모 오브젝트가 기준이기 때문에 position 과 달리 오브젝트의 인스펙터의 트랜스폼에 나오는 숫자와 일치함

            // 차이점 - 오브젝트의 위치를 월드의 원점을 기준으로 잡을 것인가?   -> position
            //        - 오브젝트의 위치를 부모 오브젝트를 기준으로 잡을 것인가? -> localPosition

            // 배경을 옮기기 위한 변수
            Vector3 back_sprite_pos = sprites[start_index].localPosition;
            Vector3 front_sprite_pos = sprites[end_index].localPosition;

            // 배경 옮기기
            sprites[end_index].transform.localPosition = back_sprite_pos + Vector3.up * view_height; // 배경의 맨 아랫지점의 위치를 맨 윗지점 바로 위로 옮겨 줌

            // end_index 및 start_index 변수 업데이트 (배경을 옮겼기 때문에 맨 아랫지점과 맨 윗지점의 스프라이트가 바뀌기 때문)
            int start_index_save = start_index;
            start_index = end_index;
            end_index = (start_index_save - 1 == -1) ? sprites.Length - 1 : start_index_save - 1; // end_index에 start_index_save - 1 의 값이 -1 이면 sprites.Length - 1 의 값을, 그게 아니라면 start_index_save - 1  값을 대입함
        }
    }
}
