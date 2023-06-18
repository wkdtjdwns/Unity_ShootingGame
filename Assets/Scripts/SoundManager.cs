using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // instance 변수를 활용해서 SoundManager에 쉽게 접근 할 수 있게 함

    // 사운드를 바꾸기 위한 배열 변수
    public AudioClip[] audio_clips;
    public AudioClip[] bgm_clips;

    // SoundManager에 있는 2개의 player 오브젝트 객체를 불러옴
    public AudioSource bgm_player;
    public AudioSource sfx_player;

    // 옵션 창에서 설정할 사운드 변수들
    public Slider bgm_slider; // 배경음 슬라이더 변수
    public Slider sfx_slider; // 효과음 슬라이더 변수

    void Awake() // 게임을 시작할 때 한번
    {
        instance = this;

        bgm_player = GameObject.Find("Bgm Player").GetComponent<AudioSource>(); // Bgm Player의 Audio Souce 컴퍼넌트를 가져옴
        sfx_player = GameObject.Find("Sfx Player").GetComponent<AudioSource>(); // Sfx Player의 Audio Souce 컴퍼넌트를 가져옴

        bgm_slider = bgm_slider.GetComponent<Slider>();                         // bgm_slider의 컴퍼넌트를 가져옴
        sfx_slider = sfx_slider.GetComponent<Slider>();                         // sfx_slider의 컴퍼넌트를 가져옴

        // onValueChanged() -> 값이 변경 되었을 때 실행할 이벤트를 지정할 수 있게 함
        bgm_slider.onValueChanged.AddListener(ChangeBgmSound);
        sfx_slider.onValueChanged.AddListener(ChangeSfxSound);
    }

    public void PlaySound(string type) // 소리를 내는 함수
    {
        int index = 0;

        // 각 상황에 맞는 사운드 출력
        switch (type)
        {
            case "Shot": index = 0; break;
            case "Enemy Shot": index = 1; break;
            case "Explosion": index = 2; break;
            case "Heal": index = 3; break;
            case "Button": index = 4; break;
            case "Spawn": index = 5; break;
            case "Option": index = 6; break;
            case "Get Score": index = 7; break;
            case "Get Power": index = 8; break;
            case "Get Boom": index = 9; break;
            case "Start": index = 10; break;
            case "Clear": index = 11; break;
            case "Get Follower": index = 12; break;
            case "Spawning": index = 13; break;
            case "End Spawning": index = 14; break;
            case "Enemy Hit": index = 15; break;
            case "Exit": index = 16; break;
        }

        sfx_player.clip = audio_clips[index];

        // PlayOneShot() -> 오디오가 중첩되어도 동시에 출력하는 함수  /  Play() 함수는 오디오가 중첩되면 가장 마지막 오디오만 출력함
        sfx_player.PlayOneShot(sfx_player.clip);
    }

    public void ChangeBgmSound(float value) // float형 값을 하나 받아온 뒤에
    {
        bgm_player.volume = value * 0.25f;  // bgm_player.volume을 받아온 float형 매개변수로 저장함
    }

    void ChangeSfxSound(float value)        // float형 값을 하나 받아온 뒤에
    {
        sfx_player.volume = value * 0.25f;  // sfx_player.volume을 받아온 float형 매개변수로 저장함
    }
}
