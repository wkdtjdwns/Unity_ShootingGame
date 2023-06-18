using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // instance ������ Ȱ���ؼ� SoundManager�� ���� ���� �� �� �ְ� ��

    // ���带 �ٲٱ� ���� �迭 ����
    public AudioClip[] audio_clips;
    public AudioClip[] bgm_clips;

    // SoundManager�� �ִ� 2���� player ������Ʈ ��ü�� �ҷ���
    public AudioSource bgm_player;
    public AudioSource sfx_player;

    // �ɼ� â���� ������ ���� ������
    public Slider bgm_slider; // ����� �����̴� ����
    public Slider sfx_slider; // ȿ���� �����̴� ����

    void Awake() // ������ ������ �� �ѹ�
    {
        instance = this;

        bgm_player = GameObject.Find("Bgm Player").GetComponent<AudioSource>(); // Bgm Player�� Audio Souce ���۳�Ʈ�� ������
        sfx_player = GameObject.Find("Sfx Player").GetComponent<AudioSource>(); // Sfx Player�� Audio Souce ���۳�Ʈ�� ������

        bgm_slider = bgm_slider.GetComponent<Slider>();                         // bgm_slider�� ���۳�Ʈ�� ������
        sfx_slider = sfx_slider.GetComponent<Slider>();                         // sfx_slider�� ���۳�Ʈ�� ������

        // onValueChanged() -> ���� ���� �Ǿ��� �� ������ �̺�Ʈ�� ������ �� �ְ� ��
        bgm_slider.onValueChanged.AddListener(ChangeBgmSound);
        sfx_slider.onValueChanged.AddListener(ChangeSfxSound);
    }

    public void PlaySound(string type) // �Ҹ��� ���� �Լ�
    {
        int index = 0;

        // �� ��Ȳ�� �´� ���� ���
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

        // PlayOneShot() -> ������� ��ø�Ǿ ���ÿ� ����ϴ� �Լ�  /  Play() �Լ��� ������� ��ø�Ǹ� ���� ������ ������� �����
        sfx_player.PlayOneShot(sfx_player.clip);
    }

    public void ChangeBgmSound(float value) // float�� ���� �ϳ� �޾ƿ� �ڿ�
    {
        bgm_player.volume = value * 0.25f;  // bgm_player.volume�� �޾ƿ� float�� �Ű������� ������
    }

    void ChangeSfxSound(float value)        // float�� ���� �ϳ� �޾ƿ� �ڿ�
    {
        sfx_player.volume = value * 0.25f;  // sfx_player.volume�� �޾ƿ� float�� �Ű������� ������
    }
}
