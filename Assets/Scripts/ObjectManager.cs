using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    // 오브젝트 풀링 : 미리 생성해둔 풀에서 활성화/비활성화로 사용
    // 게임에서 사용한 수많은 Instantiate()와 Destroy() 함수는 생성, 및 삭제를 하면서 필요없는 쓰레기 메모리를 생성함 (제거하지 않으면 렉이 심하게 걸림)
    // -> 그래서 가비지컬렉트(GC)를 사용함  /  GC : 쌓인 조각난 메모리를 비우는 기술

    // 여타 다른 게임들의 첫 로딩 시간들도 장면 배치 및 오브젝트 풀 생성 때문에 필요함!

    // 굉장히 많은 오브젝트가 들어갈 것이기 때문에 public으로 선언하지 않음

    // 프리펩을 생성하여 저장할 배열들
    // 적들을 저장할 배열
    GameObject[] enemy_a;
    GameObject[] enemy_b;
    GameObject[] enemy_c;
    GameObject[] enemy_boss;

    // 아이템들을 저장할 배열
    GameObject[] item_boom;
    GameObject[] item_coin;
    GameObject[] item_power;
    GameObject[] item_follower;
    GameObject[] item_heal;

    // 플레이어의 총알을 저장할 배열
    GameObject[] player_bullet_a;
    GameObject[] player_bullet_b;

    // 팔로워의 총알을 저장할 배열
    GameObject[] follower_bullet;

    // 적의 총알을 저장할 배열
    GameObject[] enemy_bullet_a;
    GameObject[] enemy_bullet_b;

    // 보스의 총알을 저장할 배열
    GameObject[] boss_bullet_a;
    GameObject[] boss_bullet_b;

    // 오브젝트 폭발 이펙트를 저장할 배열
    GameObject[] explosion;

    // 프리펩 변수들
    // 적 프리펩 변수
    public GameObject enemy_a_prefab;
    public GameObject enemy_b_prefab;
    public GameObject enemy_c_prefab;
    public GameObject enemy_boss_prefab;

    // 아이템 프리펩 변수
    public GameObject item_boom_prefab;
    public GameObject item_coin_prefab;
    public GameObject item_power_prefab;
    public GameObject item_follower_prefab;
    public GameObject item_heal_prefab;

    // 플레이어의 총알 프리펩 변수
    public GameObject player_bullet_a_prefab;
    public GameObject player_bullet_b_prefab;

    // 팔로워의 총알 프리펩 변수
    public GameObject follower_bullet_prefab;

    // 적의 총알 프리펩 변수
    public GameObject enemy_bullet_a_prefab;
    public GameObject enemy_bullet_b_prefab;

    // 보스의 총알 프리펩 변수
    public GameObject boss_bullet_a_prefab;
    public GameObject boss_bullet_b_prefab;

    // 오브젝트 폭발 이펙트 프리펩 변수
    public GameObject explosion_prefab;

    // 코드 작업을 조금 더 쉽게 할 수 있게 하는 배열 변수
    GameObject[] target_pool;

void Awake() // 시작하자 마자
    {
        // 화면에 한번에 등장할 개수를 고려해서 배열의 길이를 할당해줌
        // 적들을 저장할 배열 길이 할당
        enemy_a = new GameObject[35];
        enemy_b = new GameObject[35];
        enemy_c = new GameObject[35];
        enemy_boss = new GameObject[1];

        // 아이템들을 저장할 배열 길이 할당
        item_boom = new GameObject[10];
        item_coin = new GameObject[20];
        item_power = new GameObject[10];
        item_follower = new GameObject[10];
        item_heal = new GameObject[10];

        // 플레이어의 총알을 저장할 배열 길이 할당
        player_bullet_a = new GameObject[100];
        player_bullet_b = new GameObject[100];
        
        // 팔로워의 총알을 저장할 배열 길이 할당
        follower_bullet = new GameObject[100];

        // 적의 총알을 저장할 배열 길이 할당
        enemy_bullet_a = new GameObject[100];
        enemy_bullet_b  = new GameObject[100];

        // 보스의 총알을 저장할 배열 길이 할당
        boss_bullet_a = new GameObject[50];
        boss_bullet_b = new GameObject[1000];

        // 오브젝트 폭발 이펙트를 저장할 배열 길이 할당
        explosion = new GameObject[50];

        Generate();
    }

    void Generate()
    {
        // 적들을 생성하고 배열에 저장함
        for (int index = 0; index < enemy_a.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            enemy_a[index] = Instantiate(enemy_a_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            enemy_a[index].SetActive(false); 
        }

        for (int index = 0; index < enemy_b.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            enemy_b[index] = Instantiate(enemy_b_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            enemy_b[index].SetActive(false);
        }

        for (int index = 0; index < enemy_c.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            enemy_c[index] = Instantiate(enemy_c_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            enemy_c[index].SetActive(false);
        }

        for (int index = 0; index < enemy_boss.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            enemy_boss[index] = Instantiate(enemy_boss_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            enemy_boss[index].SetActive(false);
        }

        // 아이템들을 생성하고 배열에 저장함
        for (int index = 0; index < item_boom.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            item_boom[index] = Instantiate(item_boom_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            item_boom[index].SetActive(false);
        }

        for (int index = 0; index < item_coin.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            item_coin[index] = Instantiate(item_coin_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            item_coin[index].SetActive(false);
        }

        for (int index = 0; index < item_power.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            item_power[index] = Instantiate(item_power_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            item_power[index].SetActive(false);
        }

        for (int index = 0; index < item_follower.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            item_follower[index] = Instantiate(item_follower_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            item_follower[index].SetActive(false);
        }

        for (int index = 0; index < item_heal.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            item_heal[index] = Instantiate(item_heal_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            item_heal[index].SetActive(false);
        }

        // 플레이어의 총알을 생성하고 배열에 저장함
        for (int index = 0; index < player_bullet_a.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            player_bullet_a[index] = Instantiate(player_bullet_a_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            player_bullet_a[index].SetActive(false);
        }

        for (int index = 0; index < player_bullet_b.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            player_bullet_b[index] = Instantiate(player_bullet_b_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            player_bullet_b[index].SetActive(false);
        }
        
        // 팔로워의 총알을 생성하고 배열의 저장함
        for (int index = 0; index < follower_bullet.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            follower_bullet[index] = Instantiate(follower_bullet_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            follower_bullet[index].SetActive(false);
        }

        // 적의 총알을 생성하고 배열에 저장함
        for (int index = 0; index < enemy_bullet_a.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            enemy_bullet_a[index] = Instantiate(enemy_bullet_a_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            enemy_bullet_a[index].SetActive(false);
        }

        for (int index = 0; index < enemy_bullet_b.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            enemy_bullet_b[index] = Instantiate(enemy_bullet_b_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            enemy_bullet_b[index].SetActive(false);
        }

        for (int index = 0; index < boss_bullet_a.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            boss_bullet_a[index] = Instantiate(boss_bullet_a_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            boss_bullet_a[index].SetActive(false);
        }

        for (int index = 0; index < boss_bullet_b.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            boss_bullet_b[index] = Instantiate(boss_bullet_b_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            boss_bullet_b[index].SetActive(false);
        }

        // 오브젝트 폭발 이펙트를 생성하고 배열에 저장함
        for (int index = 0; index < explosion.Length; index++)
        {
            // Instantiate() 로 생성한 프리펩을 배열에 저장함
            explosion[index] = Instantiate(explosion_prefab);

            // Instantiate() 로 생성한 뒤에는 바로 비활성화 시켜줌 (화면에 모든 오브젝트가 전부 생기면 안되기 때문)
            explosion[index].SetActive(false);
        }
    }
    public GameObject MakeObj(string type) // 오브젝트들을 만드는 함수  /  반환값 -> GameObject
    {
        // 무슨 오브젝트를 만들 것인지 정함 
        switch (type)
        {
            case "enemy_a":
                target_pool = enemy_a;
                break;

            case "enemy_b":
                target_pool = enemy_b;
                break;

            case "enemy_c":
                target_pool = enemy_c;
                break;

            case "enemy_boss":
                target_pool = enemy_boss;
                break;

            case "item_boom":
                target_pool = item_boom;
                break;

            case "item_coin":
                target_pool = item_coin;
                break;

            case "item_power":
                target_pool = item_power;
                break;

            case "item_follower":
                target_pool = item_follower;
                break;

            case "item_heal":
                target_pool = item_heal;
                break;

            case "player_bullet_a":
                target_pool = player_bullet_a;
                break;

            case "player_bullet_b":
                target_pool = player_bullet_b;
                break;

            case "follower_bullet":
                target_pool = follower_bullet;
                break;

            case "enemy_bullet_a":
                target_pool = enemy_bullet_a;
                break;

            case "enemy_bullet_b":
                target_pool = enemy_bullet_b;
                break;

            case "boss_bullet_a":
                target_pool = boss_bullet_a;
                break;

            case "boss_bullet_b":
                target_pool = boss_bullet_b;
                break;

            case "explosion":
                target_pool = explosion;
                break;
        }

        // 위에서 정해진 오브젝트를 생성함
        for (int index = 0; index < target_pool.Length; index++) // 위에서 정해진
        {
            // activeSelf -> 현재 오브젝트의 상태
            if (!target_pool[index].activeSelf)     // 현재 생성할 오브젝트가 활성화 되어있지 않다면
            {
                target_pool[index].SetActive(true); // 생성할 오브젝트를 활성화 시켜주고
                return target_pool[index];          // 생성할 오브젝트를 반환함
            }
        }

        // 아무것도 정해지지 않았다면
        return null; // null 값을 반환함
    }

    public GameObject[] GetPool(string type) // 지정한 오브젝트의 풀을 가져오는 함수  / 반환값 -> GameObject 배열
    {
        switch (type)
        {
            case "enemy_a":
                target_pool = enemy_a;
                break;

            case "enemy_b":
                target_pool = enemy_b;
                break;

            case "enemy_c":
                target_pool = enemy_c;
                break;

            case "enemy_boss":
                target_pool = enemy_boss;
                break;

            case "item_boom":
                target_pool = item_boom;
                break;

            case "item_coin":
                target_pool = item_coin;
                break;

            case "item_power":
                target_pool = item_power;
                break;

            case "item_follower":
                target_pool = item_follower;
                break;

            case "item_heal":
                target_pool = item_heal;
                break;

            case "player_bullet_a":
                target_pool = player_bullet_a;
                break;

            case "player_bullet_b":
                target_pool = player_bullet_b;
                break;
                
            case "follower_bullet":
                target_pool = follower_bullet;
                break;

            case "enemy_bullet_a":
                target_pool = enemy_bullet_a;
                break;

            case "enemy_bullet_b":
                target_pool = enemy_bullet_b;
                break;

            case "boss_bullet_a":
                target_pool = boss_bullet_a;
                break;

            case "boss_bullet_b":
                target_pool = boss_bullet_b;
                break;

            case "explosion":
                target_pool = explosion;
                break;
        }

        return target_pool;
    }
}
