using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 적의 스탯
    public float speed;       // 적의 이동 속도
    public float health;      // 적의 체력
    public string enemy_name; // 적의 이름
    public int enemy_score;   // 각각의 적이 가지는 점수

    public int stage;

    // 보스의 패턴 구현에 필요한 변수
    public int pattern_index;       // 실행할 패턴 인덱스 변수
    public int cur_pattern_count;   // 현재 실행 횟수 변수
    public int[] max_pattern_count; // 해당 패턴의 최대 실행 횟수 배열 변수 (여러개의 패턴이 존재하기 때문에 배열 변수로 선언)

    public float ran_num;           // 패턴에 필요한 랜덤한 수

    // 총알 변수
    public GameObject bullet_obj_a; // A 타입 총알
    public GameObject bullet_obj_b; // B 타입 총알

    // 총알을 발사하는 딜레이 (재장전)
    public float cur_delay;    // 현재 장전 시간
    public float reload_delay; // 재장전 시간

    // 아이템 변수
    public GameObject item_boom;
    public GameObject item_coin;
    public GameObject item_power;
    public GameObject item_follower;

    public Sprite[] sprites; // 적의 스프라이트를 관리할 배열 변수

    // 프리펩을 오브젝트에 접근 시키는 변수 (프리펩은 이미 Scene에 올라인 오브젝트에 접근 할 수 없음 -> GameManager에서 컴포넌트를 가지게 해줌)
    public GameObject player;
    public GameManager game_manager;
    public ObjectManager object_manager;

    // 초기화 할 변수
    SpriteRenderer sprite_renderer;
    Animator anim;
    Animator spawn_anim;

    void Awake() // 시작하자 마자
    {
        // 초기화 함
        sprite_renderer = GetComponent<SpriteRenderer>();

        stage = 1;

        if (enemy_name == "Boss") // 적 이름이 보스 일 때만
        {
            // 애니매이션 사용
            anim = GetComponent<Animator>();
        }
    }

    void OnEnable() // 컴포넌트가 활성화 되었을 때
    {
        // 소모되는 변수는 활성화 될때 다시 초기화 해야함
        switch (enemy_name)
        {
            case "A":
                health = 5 * stage;
                break;

            case "B":
                health = 15 * stage;
                break;

            case "C":
                health = 50 * stage;
                break;

            case "Boss":
                health = 3000 * stage;
                Invoke("Stop", 2);
                break;
        }
    }

    void Stop() // 적을 멈추게 하는 함수
    {
        // Stop() 함수가 2번 실행되지 않게 함
        if (!gameObject.activeSelf) // 해당 오브젝트가 해당 오브젝트가 활성화 되어있지 않으면
            return;                 // 실행시키지 않음

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 0.75f);
    }

    void Think() // 적이 생각하게 하는 함수
    {
        // 적의 태그 및 애니매이터 초기화
        gameObject.tag = "Boss";
        game_manager.ChangeAc(anim, 0);

        // 현재 체력에 따라 실행할 패턴을 다르게 정함
        if (health > 3000 * stage * 0.75f)
        {
            pattern_index = Random.Range(0, 2);
        }

        else if (health > 3000 * stage * 0.5f)
        {
            pattern_index = Random.Range(0, 4);
        }

        else if (health < 3000 * stage * 0.5f)
        {
            pattern_index = Random.Range(0, 5);
        }

        Debug.Log("스킬 사용 " +  pattern_index);

        cur_pattern_count = 0; // 현재 실행 횟수 초기화

        // 위에서 정해진 패턴 인덱스의 값마다 패턴을 다르게 함
        switch (pattern_index)
        {
            case 0:
                max_pattern_count[pattern_index] = Random.Range(2, 4); // 해당 패턴의 최대 실행 횟수를 2 ~ 3 범위의 수 중 랜덤으로 정하고

                FireForward();                                         // 패턴 실행
                break; 
            
            case 1:
                max_pattern_count[pattern_index] = Random.Range(3, 7); // 해당 패턴의 최대 실행 횟수를 3 ~ 6 범위의 수 중 랜덤으로 정하고

                FireShot();                                            // 패턴 실행
                break;

            case 2:
                max_pattern_count[pattern_index] = Random.Range(80, 101); // 해당 패턴의 최대 실행 횟수를 80 ~ 100 범위의 수 중 랜덤으로 정하고

                if (max_pattern_count[pattern_index] % 2 == 0)            // 정해진 값이 짝수면
                {
                    max_pattern_count[pattern_index]++;                   // 1을 더해서 홀수로 만들어 줌 (플레이어가 가만히 있어도 피해지는 현상 방지)
                }

                ran_num = Random.Range(4.5f, 10f);                      // 패턴에 필요한 랜덤한 수를 정한 뒤에

                FireArc();                                                // 패턴 실행
                break;

            case 3:
                max_pattern_count[pattern_index] = Random.Range(8, 13); // 해당 패턴의 최대 실행 횟수를 8 ~ 12 범위의 수 중 랜덤으로 정하고

                FireAround();                                           // 패턴 실행
                break;

            case 4:
                int boss_spawn = Random.Range(0, 3);

                Spawn(boss_spawn);
                break;

            case 5:
                break;
        }
    }

    // 패턴 인덱스 값에 따른 패턴들
    void FireForward()
    {
        // 앞으로 총알 8발 발사

        if (health <= 0) // 보스가 죽었다면
            return;      // 실행시키지 않음

        // 적의 오른쪽에 보스의 A 타입 총알을 적의 방향으로 생성함
        GameObject bullet_r = object_manager.MakeObj("boss_bullet_a");
        bullet_r.transform.position = transform.position + Vector3.right * 0.25f;

        // 적의 오오른쪽에 보스의 A 타입 총알을 적의 방향으로 생성함
        GameObject bullet_rr = object_manager.MakeObj("boss_bullet_a");
        bullet_rr.transform.position = transform.position + Vector3.right * 0.6f;

        // 적의 오오오른쪽에 보스의 A 타입 총알을 적의 방향으로 생성함
        GameObject bullet_rrr = object_manager.MakeObj("boss_bullet_a");
        bullet_rrr.transform.position = transform.position + Vector3.right * 0.85f;
        
        // 적의 오오오오른쪽에 보스의 A 타입 총알을 적의 방향으로 생성함
        GameObject bullet_rrrr = object_manager.MakeObj("boss_bullet_a");
        bullet_rrrr.transform.position = transform.position + Vector3.right * 1.15f;

        // 적의 왼쪽에 보스의 A 타입 총알을 적의 방향으로 생성함
        GameObject bullet_l = object_manager.MakeObj("boss_bullet_a");
        bullet_l.transform.position = transform.position + Vector3.left * 0.25f;

        // 적의 왼왼쪽에 보스의 A 타입 총알을 적의 방향으로 생성함
        GameObject bullet_ll = object_manager.MakeObj("boss_bullet_a");
        bullet_ll.transform.position = transform.position + Vector3.left * 0.6f;

        // 적의 왼왼왼쪽에 보스의 A 타입 총알을 적의 방향으로 생성함
        GameObject bullet_lll = object_manager.MakeObj("boss_bullet_a");
        bullet_lll.transform.position = transform.position + Vector3.left * 0.85f;

        // 적의 왼왼왼왼쪽에 보스의 A 타입 총알을 적의 방향으로 생성함
        GameObject bullet_llll = object_manager.MakeObj("boss_bullet_a");
        bullet_llll.transform.position = transform.position + Vector3.left * 1.15f;

        // 총알의 Rigidbody를 가져와서 AddForce()로 총알 발사 기능 구현
        Rigidbody2D rigid_r = bullet_r.GetComponent<Rigidbody2D>();       // 오른쪽 총알의 Rigidbody를 가져옴
        Rigidbody2D rigid_rr = bullet_rr.GetComponent<Rigidbody2D>();     // 오오른쪽 총알의 Rigidbody를 가져옴
        Rigidbody2D rigid_rrr = bullet_rrr.GetComponent<Rigidbody2D>();   // 오른쪽 총알의 Rigidbody를 가져옴
        Rigidbody2D rigid_rrrr = bullet_rrrr.GetComponent<Rigidbody2D>(); // 오오른쪽 총알의 Rigidbody를 가져옴

        Rigidbody2D rigid_l = bullet_l.GetComponent<Rigidbody2D>();       // 왼쪽 총알의 Rigidbody를 가져옴
        Rigidbody2D rigid_ll = bullet_ll.GetComponent<Rigidbody2D>();     // 왼왼쪽 총알의 Rigidbody를 가져옴
        Rigidbody2D rigid_lll = bullet_lll.GetComponent<Rigidbody2D>();   // 왼왼왼쪽 총알의 Rigidbody를 가져옴
        Rigidbody2D rigid_llll = bullet_llll.GetComponent<Rigidbody2D>(); // 왼왼왼왼쪽 총알의 Rigidbody를 가져옴

        // 아래 쪽으로 총알들 발사
        rigid_r.AddForce(Vector2.down * 8, ForceMode2D.Impulse);    // 오른쪽 총알을 플레이어 방향으로 이동 시킴-> 발사
        rigid_rr.AddForce(Vector2.down * 8, ForceMode2D.Impulse);   // 오오른쪽 총알을 플레이어 방향으로 이동 시킴-> 발사
        rigid_rrr.AddForce(Vector2.down * 8, ForceMode2D.Impulse);  // 오오오른쪽 총알을 플레이어 방향으로 이동 시킴-> 발사
        rigid_rrrr.AddForce(Vector2.down * 8, ForceMode2D.Impulse); // 오오오오른쪽 총알을 플레이어 방향으로 이동 시킴-> 발사

        rigid_l.AddForce(Vector2.down * 8, ForceMode2D.Impulse);    // 왼쪽 총알을 플레이어 방향으로 이동 시킴-> 발사
        rigid_ll.AddForce(Vector2.down * 8, ForceMode2D.Impulse);   // 왼왼쪽 총알을 플레이어 방향으로 이동 시킴-> 발사
        rigid_lll.AddForce(Vector2.down * 8, ForceMode2D.Impulse);  // 왼왼왼쪽 총알을 플레이어 방향으로 이동 시킴-> 발사
        rigid_llll.AddForce(Vector2.down * 8, ForceMode2D.Impulse); // 왼왼왼왼쪽 총알을 플레이어 방향으로 이동 시킴-> 발사

        // 패턴 카운트
        cur_pattern_count++;                                      // 현재 실행 횟수를 1 늘리고

        SoundManager.instance.PlaySound("Enemy Shot");            // 총을 쏘는 사운드 출력

        if (cur_pattern_count < max_pattern_count[pattern_index]) // 현재 실행 횟수가 위에서 정해진 최대 실행 횟수를 넘기지 않았다면
        {
            Invoke("FireForward", 2);                             // 해당 패턴 다시 실행
        }

        else                                                      // 그게 아니라면 (현재 실행 횟수가 최대 실행 횟수를 넘겼다면)
        {
            Invoke("Think", 2.5f);                                // 다음 패턴을 생각함
        }
    }

    void FireShot()
    {
        // 플레이어 방향으로 샷건 발사

        if (health <= 0) // 보스가 죽었다면
            return;      // 실행시키지 않음

        // 적의 위치에 B 타입 총알을 적의 방향으로 20개 생성함
        for (int index = 0; index < 20; index++)
        {
            GameObject bullet = object_manager.MakeObj("enemy_bullet_b");
            bullet.transform.position = transform.position;

            // 총알의 Rigidbody를 가져와서 AddForce()로 총알 발사 기능 구현
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();           // 총알의 Rigidbody2D를 가져옴

            Vector3 dir_vec = player.transform.position - transform.position; // 목표물 (플레이어)로의 방향 (플레이어 위치 - 자신의 위치)을 변수에 저장해준 뒤
            
            // 위치가 겹치지 않게 랜덤한 벡터 값을 더한 뒤에 이동 시킴 (x축은 -1 ~ 1 범위의 수 중 하나  /  y축은 0 ~ 3의 범위의 수 중 하나)
            Vector3 ran_vec = new Vector2(Random.Range(-0.75f, 0.75f), Random.Range(0f, 3f));
            dir_vec += ran_vec;

            // 단위 벡터로 만들기 (dir_vec_r, dir_vec_l의 값은 좌표 값이기 때문에 왠만하면 1을 넘기기 때문) -> normalized (벡터가 단위 값(1)로 변환된 변수) 사용
            rigid.AddForce(dir_vec.normalized * 6.5f, ForceMode2D.Impulse);   // 총알을 플레이어 방향으로 이동 시킴 -> 발사
        }

        // 패턴 카운트
        cur_pattern_count++;                                      // 현재 실행 횟수를 1 늘리고

        SoundManager.instance.PlaySound("Enemy Shot");            // 총을 쏘는 사운드 출력

        if (cur_pattern_count < max_pattern_count[pattern_index]) // 현재 실행 횟수가 위에서 정해진 최대 실행 횟수를 넘기지 않았다면
        {
            Invoke("FireShot", 1.5f);                             // 해당 패턴 다시 실행
        }

        else                                                      // 그게 아니라면 (현재 실행 횟수가 최대 실행 횟수를 넘겼다면)
        {
            Invoke("Think", 2.5f);                                // 다음 패턴을 생각함
        }
    }

    void FireArc()
    {
        // 부채 모양으로 총알 발사

        if (health <= 0) // 보스가 죽었다면
            return;      // 실행시키지 않음

        // 적의 위치에 A 타입 총알을 적의 방향으로 생성함
        GameObject bullet = object_manager.MakeObj("enemy_bullet_a");

        // 총알의 위치 및 회전 초기화
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        // 총알의 Rigidbody를 가져와서 AddForce()로 총알 발사 기능 구현
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();           // 총알의 Rigidbody2D를 가져옴

        // Mathf.Sin() -> 삼각 함수의 SIn (Mathf.Cos()를 사용해도 시작 각도만 다를 뿐이지, 발사 모양은 같음)  /  Mathf.PI -> 원주율 PI(π)  /  PI * ? -> 얼마나 빠르게 파형을 그릴지 정함  /  cur_pattern_count / max_pattern_count[pattern_index] -> 0 ~ 1까지의 값을 만들기 위함
        Vector3 dir_vec = new Vector2(Mathf.Sin( Mathf.PI * ran_num * cur_pattern_count / max_pattern_count[pattern_index] ), -1);

        // 단위 벡터로 만들기 (dir_vec_r, dir_vec_l의 값은 좌표 값이기 때문에 왠만하면 1을 넘기기 때문) -> normalized (벡터가 단위 값(1)로 변환된 변수) 사용
        rigid.AddForce(dir_vec.normalized * 6.5f, ForceMode2D.Impulse);   // 총알을 플레이어 방향으로 이동 시킴 -> 발사

        // 패턴 카운트
        cur_pattern_count++;                                      // 현재 실행 횟수를 1 늘리고

        SoundManager.instance.PlaySound("Enemy Shot");            // 총을 쏘는 사운드 출력

        if (cur_pattern_count < max_pattern_count[pattern_index]) // 현재 실행 횟수가 위에서 정해진 최대 실행 횟수를 넘기지 않았다면
        {
            Invoke("FireArc", 0.15f);                             // 해당 패턴 다시 실행
        }

        else                                                      // 그게 아니라면 (현재 실행 횟수가 최대 실행 횟수를 넘겼다면)
        {
            Invoke("Think", 2.5f);                                // 다음 패턴을 생각함
        }
    }

    void FireAround()
    {
        // 원 형태로 총알 발사

        if (health <= 0) // 보스가 죽었다면
            return;      // 실행시키지 않음

        // 적의 위치에 A 타입 총알을 적의 방향으로 각각 변수의 값만큼 생성함
        int round_num_a = 50;
        int round_num_b = 37;

        int round_num = cur_pattern_count % 2 == 0 ? round_num_a : round_num_b;

        for (int index = 0; index < round_num; index++)
        {
            GameObject bullet = object_manager.MakeObj("boss_bullet_b");

            // 총알의 위치 및 회전 초기화
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            // 총알의 Rigidbody를 가져와서 AddForce()로 총알 발사 기능 구현
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();           // 총알의 Rigidbody2D를 가져옴

            // Mathf.Cos() -> 삼각 함수의 Cos (Mathf.Sin()를 사용해도 시작 각도만 다를 뿐이지, 발사 모양은 같음)  /  Mathf.PI -> 원주율 PI(π)  /  PI * ? -> 얼마나 빠르게 파형을 그릴지 정함  /  index / round_num -> 0 ~ 1까지의 값을 만들기 위함
            Vector3 dir_vec = new Vector2(Mathf.Sin( Mathf.PI * 2 * index / round_num)
                                         ,Mathf.Cos( Mathf.PI * 2 * index / round_num) * (-1)); // x축과 y축에 곱하는 수를 각각 Sin, Cos로 서로 다르게 설정해야 함 (그러지 않으면 x축, y축이 모두 똑같은 값이 되어서 일직선으로 총알이 날아가기 떄문)

            // 단위 벡터로 만들기 (dir_vec_r, dir_vec_l의 값은 좌표 값이기 때문에 왠만하면 1을 넘기기 때문) -> normalized (벡터가 단위 값(1)로 변환된 변수) 사용
            rigid.AddForce(dir_vec.normalized * 3, ForceMode2D.Impulse);   // 총알을 플레이어 방향으로 이동 시킴 -> 발사

            // 총알들이 자신의 방향을 바라보게 함
            Vector3 rot_vec = Vector3.forward * 360 * index / round_num;
            bullet.transform.Rotate(rot_vec);
        }

        // 패턴 카운트
        cur_pattern_count++;                                      // 현재 실행 횟수를 1 늘리고

        SoundManager.instance.PlaySound("Enemy Shot");            // 총을 쏘는 사운드 출력

        if (cur_pattern_count < max_pattern_count[pattern_index]) // 현재 실행 횟수가 위에서 정해진 최대 실행 횟수를 넘기지 않았다면
        {
            Invoke("FireAround", 0.7f);                           // 해당 패턴 다시 실행
        }

        else                                                      // 그게 아니라면 (현재 실행 횟수가 최대 실행 횟수를 넘겼다면)
        {
            Invoke("Think", 2.5f);                                // 다음 패턴을 생각함
        }
    }

    void Spawn(int boss_spawn)
    {
        // 보스가 적을 생성함

        if (health <= 0) // 보스가 죽었다면
            return;      // 실행시키지 않음

        // 사운드 출력
        SoundManager.instance.PlaySound("Spawning");

        // 보스의 태그 바꾸기 (피격 받지 않기 위함)
        gameObject.tag = "EnemySpawning";

        // 보스 색깔 바꾸기 (애니매이션으로 해도 괜찮을 듯 / 겜 시작할때 검정이 없어지는거 마냥)
        game_manager.ChangeAc(anim, 1); // 애니매이터를 변경함

        // 적을 소환함
        game_manager.ReadBossFile(boss_spawn);

        // 모든 적을 소환했는지 판단
        while (game_manager.cur_spawn_delay > game_manager.spawn_delay && !game_manager.spawn_end)
        {
            game_manager.SpawnEnemy();

            game_manager.cur_spawn_delay = 0;
        }

        Invoke("EndBossSpawn", 20f); // 20
    }

    void EndBossSpawn()
    {
        // 사운드 출력
        SoundManager.instance.PlaySound("End Spawning");

        // 위에서 바꾼 색깔 원래대로 돌려놓기 (애니매이션 가능)
        game_manager.ChangeAc(anim, 2); // 애니매이터를 변경함

        Invoke("Think", 1.5f);
    }

    void Update() // 매 프레임마다
    {
        // 치트키 부분
        if (Input.GetKeyDown(KeyCode.G)) // G 키를 누르면
        {
            OnHit(1000);                 // 적이 1000데미지를 입음 -> 죽음
        }

        if (enemy_name == "Boss")            // 적 이름이 보스가 이여야만 실행함
        {
            if (Input.GetKeyDown(KeyCode.T)) // T 키를 누르면
            {
                cur_pattern_count = max_pattern_count[pattern_index];

                CancelInvoke();              // 모든 Invoke() 함수를 멈추고

                Think();                     // 다시 생각함
            }
        }

        // 코드 부분
        stage = game_manager.stage;

        // 현재 체력에 따라서 보스의 색상을 다르게 함
        if (enemy_name == "Boss")
        {
            if (health > 3000 * stage * 0.75f)
            {
                sprite_renderer.color = new Color(1, 1, 1, 1);
            }

            else if (health > 3000 * stage * 0.5f)
            {
                sprite_renderer.color = new Color(0, 0.5f, 1, 1);
            }

            else if (health < 3000 * stage * 0.5f)
            {
                sprite_renderer.color = new Color(0, 0, 0, 1);
            }
        }

        if (enemy_name == "Boss" || enemy_name == "B")
            return;

        // 일반적인 적들의 공격
        Fire();   // 총알 발사

        Reload(); // 총알 장전

    }

    void Fire() // 총알 발사 함수
    {
        // 발사 시키지 않는 조건 -> 장전이 되어 있지 않음
        if (cur_delay < reload_delay) // 현재 장전 시간이 재장전 시간보다 작다면 (재장전이 완료되어 있지 않다면) 
            return;                   // 실행 시키지 않음

        if(enemy_name == "A")       // 적의 이름이 A라면 (적의 타입이 A 타입이라면)
        {
            // 적의 위치에 A 타입 총알을 적의 방향으로 생성함
            GameObject bullet = object_manager.MakeObj("enemy_bullet_a");
            bullet.transform.position = transform.position;

            // 총알의 Rigidbody를 가져와서 AddForce()로 총알 발사 기능 구현
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                // 총알의 Rigidbody2D를 가져옴

            Vector3 dir_vec = player.transform.position - transform.position;                      // 목표물 (플레이어)로의 방향 (플레이어 위치 - 자신의 위치)을 변수에 저장해준 뒤

            // 단위 벡터로 만들기 (dir_vec_r, dir_vec_l의 값은 좌표 값이기 때문에 왠만하면 1을 넘기기 때문) -> normalized (벡터가 단위 값(1)로 변환된 변수) 사용
            rigid.AddForce(dir_vec.normalized * 3, ForceMode2D.Impulse);                           // 총알을 플레이어 방향으로 이동 시킴 -> 발사
        }

        else if (enemy_name == "C") // 적의 이름이 C라면 (적의 타입이 C 타입이라면)
        {
            // 적의 오른쪽에 B 타입 총알을 적의 방향으로 생성함
            GameObject bullet_r = object_manager.MakeObj("enemy_bullet_b");
            bullet_r.transform.position = transform.position + Vector3.right * 0.3f;

            // 적의 왼쪽에 B 타입 총알을 적의 방향으로 생성함
            GameObject bullet_l = object_manager.MakeObj("enemy_bullet_b");
            bullet_l.transform.position = transform.position + Vector3.left * 0.3f;

            // 총알의 Rigidbody를 가져와서 AddForce()로 총알 발사 기능 구현
            Rigidbody2D rigid_r = bullet_r.GetComponent<Rigidbody2D>();                                                     // 오른쪽 총알의 Rigidbody를 가져옴
            Rigidbody2D rigid_l = bullet_l.GetComponent<Rigidbody2D>();                                                     // 왼쪽 총알의 Rigidbody를 가져옴

            Vector3 dir_vec_r = player.transform.position - (transform.position + Vector3.right * 0.3f);                    // 목표물 (플레이어)로의 방향 (플레이어 위치 - 자신의 위치)을 변수에 저장해준 뒤
            Vector3 dir_vec_l = player.transform.position - (transform.position + Vector3.right * 0.3f);                    // 목표물 (플레이어)로의 방향 (플레이어 위치 - 자신의 위치)을 변수에 저장해준 뒤

            // 단위 벡터로 만들기 (dir_vec_r, dir_vec_l의 값은 좌표 값이기 때문에 왠만하면 1을 넘기기 때문) -> normalized (벡터가 단위 값(1)로 변환된 변수) 사용
            rigid_r.AddForce(dir_vec_r.normalized * 4, ForceMode2D.Impulse);                                                // 오른쪽 총알을 플레이어 방향으로 이동 시킴-> 발사
            rigid_l.AddForce(dir_vec_l.normalized * 4, ForceMode2D.Impulse);                                                // 왼쪽 총알을 플레이어 방향으로 이동 시킴-> 발사
        }
    
        cur_delay = 0;                                                                                                      // 장전 시간 초기화 (재장전을 하게 만들기 위함)

        SoundManager.instance.PlaySound("Enemy Shot");                                                                      // 총을 쏘는 사운드 출력
    }

    void Reload() // 재장전 함수
    {
        cur_delay += Time.deltaTime; // 현재 장전 시간에 시간을 더해줌
    }

    public void OnHit(float dmg) // 적이 맞았을 때 실행할 함수  /  매개변수로 데미지를 가짐
    {
        if (health <= 0) // 적이 이미 죽은 상태이면
            return;      // 실행 시키지 않음 (아이템이 여러개 만들어지지 않게 하기 위함)

        Player player_logic = player.GetComponent<Player>(); // 플레이어의 컴포넌트를 가져온 후에

        health -= dmg + player_logic.player_dmg;             // 적의 체력을 총 데미지만큼 깎음
        Debug.Log("health = " + health.ToString());

        SoundManager.instance.PlaySound("Enemy Hit");       // 적이 피격 당하는 사운드 출력

        if (enemy_name == "Boss")                // 적 이름이 보스라면
        {
            anim.SetTrigger("on_hit");           // 맞았을 때 실행하는 애니매이션 실행 
        }

        else                                     // 그게 아니라면 (일반적인 적들이라면)
        {
            sprite_renderer.sprite = sprites[1]; // 피격 당한 스프라이트로 바꿈

            Invoke("ReturnSprite", 0.1f);            // 0.1초 뒤에 스프라이트를 원래대로 바꿈
        }
          
        if (health <= 0)                         // 체력이 0보다 작거나 같을 때 (죽었을 때)
        {
            // 점수 얻기
            // 플레이어의 스크립트를 가져와서 점수를 부여함
            player_logic.score += enemy_score;   // 플레이어 스크립트에 있는 score 변수에 enemy_score의 값을 더함

            // 아이템 드랍하기
            int ran = enemy_name == "Boss" ? -1 : Random.Range(0, 13); // 적 이름이 Boss이면 -1, 아니면 0 ~ 11 중 랜덤한 값 (이 값으로 드랍할 아이템의 종류를 결정함)

            // 랜덤한 아이템 드랍
            if (ran < 0)       // ran  = -1  /  보스가 죽었을 때만 실행
            {
                Debug.Log("Not Item");
            }

            else if (ran < 5)  // ran = 0, 1, 2, 3, 4  /  확률 = 약 41.67%
            {
                // Coin을 적의 위치에 드랍함
                GameObject item_coin = object_manager.MakeObj("item_coin");
                item_coin.transform.position = transform.position;

                Item item_coin_logic = item_coin.GetComponent<Item>();
                item_coin_logic.player = player;
            }

            else if (ran < 8)  // ran =  5, 6, 7  /  확률 = 30%
            {
                // Power를 적의 위치에 드랍함
                GameObject item_power = object_manager.MakeObj("item_power");
                item_power.transform.position = transform.position;

                Item item_power_logic = item_power.GetComponent<Item>();
                item_power_logic.player = player;
            }

            else if (ran < 10) // ran = 8, 9  /  확률 = 20%
            {
                // Boom을 적의 위치에 드랍함
                GameObject item_boom = object_manager.MakeObj("item_boom");
                item_boom.transform.position = transform.position;

                Item item_boom_logic = item_boom.GetComponent<Item>();
                item_boom_logic.player = player;
            }

            else if (ran < 12) // ran = 10, 11  /  확률 = 20%
            {
                GameObject item_follower = object_manager.MakeObj("item_follower");
                item_follower.transform.position = transform.position;

                Item item_follower_logic = item_follower.GetComponent<Item>();
                item_follower_logic.player = player;
            }

            else                // ran = 12  /  확률 = 
            {
                GameObject item_heal = object_manager.MakeObj("item_heal");
                item_heal.transform.position = transform.position;

                Item item_heal_logic = item_heal.GetComponent<Item>();
                item_heal_logic.player = player;
            }

            gameObject.SetActive(false); // 적을 비활성화 시킴

            // Quaternion.identity -> 기본 회전값 -> 0
            transform.rotation = Quaternion.identity;

            game_manager.CallExplosion(transform.position, enemy_name); // 적의 이름에 따른 크기로 적을 그 자리에서 폭발 시킴

            if (enemy_name == "Boss")    // 보스를 죽였을 때 
            {
                game_manager.StageEnd(); // 다음 스테이지로 넘어감
            }
        }
    }

    void ReturnSprite() // 적의 스프라이트를 원래대로 돌려놓는 함수
    {
        sprite_renderer.sprite = sprites[0]; // 피격 당하기 전의 스프라이트로 바꿈
    }

    void OnTriggerEnter2D(Collider2D collision) // 적이 다른 오브젝트와 겹쳐졌을 때
    {
        if (collision.gameObject.tag == "BorderBullet" && enemy_name != "Boss") // 적과 겹쳐진 오브젝트의 태그가 "BorderBullet"이라면 && 적의 이름이 "Boss"가 아니라면
        {
            gameObject.SetActive(false);                                        // 적을 비활성화 시킴
            transform.rotation = Quaternion.identity;
        }

        else if (collision.gameObject.tag == "PlayerBullet" && gameObject.tag != "EnemySpawning") // 적과 겹쳐진 오브젝트의 태그가 "PlayerBullet"이라면 && 적의 태그가 "EnemySpawning"이 아니라면
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();                          // 적과 겹쳐진 오브젝트에게 Bullet 콤포넌트를 부여하고
            OnHit(bullet.dmg);                                                                    // 적이 피격당했을 때 실행하는 함수를 실행함

            collision.gameObject.SetActive(false);                                                // 총알이 비활성화 됨
        }
    }
}