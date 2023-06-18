using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 플레이어의 스탯
    public float speed;          // 플레이어의 이동 속도
    public float power;          // 플레이어의 파워
    public int life;             // 플레이어의 체력
    public int player_dmg;       // 플레이어의 데미지

    public bool is_respawn_time; // 현재 플레이어가 무적 시간인지 여부

    // 플레이어 팔로우에 대한 변수
    public int follower_num;       // 현재 팔로워의 개수
    public GameObject[] followers; // 팔로우들을 관리할 배열 변수

    // 필살기 관련 스탯
    public int boom;       // 현재 장전되어 있는 필살기 개수
    public int max_boom;   // 최대로 장전하고 있을 수 있는 필살기 개수

    // 게임에 대한 변수
    public int score;   // 점수
    public bool is_hit; // 플레이어가 맞았는 지 여부

    // 총알을 발사하는 딜레이 (재장전)
    public float cur_delay;    // 현재 장전 시간
    public float reload_delay; // 재장전 시간

    // 플레이어 맵 경계
    public bool is_touch_top;
    public bool is_touch_bottom;
    public bool is_touch_right;
    public bool is_touch_left;

    // 총알 변수
    public GameObject bullet_obj_a; // A 타입 총알
    public GameObject bullet_obj_b; // B 타입 총알

    // 필살기 변수
    public GameObject boom_effect;  // 필살기 총알
    public bool is_boom;            // 필살기 사용 여부

    public GameManager game_manager;
    public ObjectManager object_manager;
    
    Animator anim;
    SpriteRenderer sprite_renderer;

    void Awake() // 시작하자 마자
    {
        // 초기화 함
        anim = GetComponent<Animator>();
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        Unbeatable();

        Invoke("Unbeatable", 3);
    }
    
    void Unbeatable() // 현재 무적 시간인지 판단하는 함수 
    {
        is_respawn_time = !is_respawn_time; // 실행되면 무적 시간의 여부를 지금과 반대로 만들어주고 판단함

        if (is_respawn_time) // 현재 무적 시간이라면
        {
            sprite_renderer.color = new Color(1, 1, 1, 0.5f); // 플레이어의 색깔을 살짝 투명하게 바꿔주고

            // 팔로워도 똑같이 만들어줌
            for (int index = 0; index < followers.Length; index++)
            {
                followers[index].GetComponent<SpriteRenderer>().color = sprite_renderer.color = new Color(1, 1, 1, 0.5f);
            }
        }

        else                 // 현재 무적 시간이 아니라면
        {
            sprite_renderer.color = new Color(1, 1, 1, 1);   // 플레이어의 색깔을 원래대로 돌려놓고

            // 팔로워도 똑같이 만들어줌
            for (int index = 0; index < followers.Length; index++)
            {
                followers[index].GetComponent<SpriteRenderer>().color = sprite_renderer.color = new Color(1, 1, 1, 1);
            }
        }
    }

    void Update() // 매 프레임마다
    {
        Move();   // 플레이어 이동 (단발적인 키 입력은 Update()에서 구현)

        Fire();   // 총알 발사 (단발적인 키 입력은 Update()에서 구현)
        
        // 필살기의 효과 발동
        Boom();           // 필살기 오브젝트 활성화
        if (is_boom)      // 필살기를 시전하고 있다면
            BoomAffect(); // 필살기의 기능 함수를 호출함

        Reload(); // 총알 장전 (단발적인 키 입력은 Update()에서 구현)
    }

    void Move() // 플레이어 이동 함수
    {
        // 입력받기
        float h = Input.GetAxisRaw("Horizontal"); // 키 입력에 따라 1, 0, -1의 값을 가져옴 (가로)
        if ((is_touch_right && h == 1) || (is_touch_left && h == -1)) // 오른쪽 경계 오브젝트에 닿았을 때 && 오른쪽으로 이동 중 일때 || 왼쪽 경계 오브젝트에 닿았을 때 && 왼쪽으로 이동 중 일때
            h = 0;

        float v = Input.GetAxisRaw("Vertical");   // 키 입력에 따라 1, 0, -1의 값을 가져옴 (세로)
        if ((is_touch_top && v == 1) || (is_touch_bottom && v == -1)) // 위쪽 경계 오브젝트에 닿았을 때 && 위쪽으로 이동 중 일때 || 아래쪽 경계 오브젝트에 닿았을 때 && 아래쪽으로 이동 중 일때
            v = 0;

        // 입력 받은 대로 이동시키기
        Vector3 cur_pos = transform.position;                                     // 현재 위치

        // Transform 이동과 Rigidbody 이동의 차이
        // Transform - 위치, 회전, 크기를 제어
        //           - 오브젝트 간의 '부모-자식' 관계의 상태를 저장 
        //           - Scene에 존재하는 오브젝트에 필수적인 요소들이 지님 -> Scene에 존재하는 오브젝트는 무조건 Transform 컴포넌트를 지니고 있음

        // 정리 - Transform 콤포넌트는 Scene에 존재하기 위한 필수적인 콤포넌트임!

        // Rigidbody - 오브젝트가 물리 제어로 이동하게 함
        //           - 힘과 토크를 받아서 오브젝트가 사실적으로 움직이게 함
        //           - Rigidbody가 포함된 모든 오브젝트는 중력의 영향을 받아야함 또한 스크립팅으로 인해 가해진 힘으로 움직이거나 물리 엔진을 통해 다른 오브젝트와 상호 작용해야함

        // 정리 - RIgidbody 콤포넌트는 물리적인 제어로 게임 오브젝트를 사실적으로 동작하게 만드는 콤포넌트임!

        // 차이점 - 물리적인 연산으로 행해지는 연산인가?
        //        - Transform 이동 -> 눈에만 이동하는 것처럼 보일 뿐, 사실은 해당 위치로 연속적인 순간이동하는 것임! (연산 X)
        //        - Rigidbody 이동 -> 이전 위치와 다음 위치를 빠르게 이동하는 것 (연산 O)

        //  deltaTime -> 지난 프레임이 완료되는 데까지 걸린 시간의 차이
        Vector3 next_pos = new Vector3(h, v, 0) * speed * Time.deltaTime;         // 이 다음에 이동할 위치  /  Transform 이동이기 때문에 무조건 Time.deltaTime을 곱해줘야함 (프레임에 상관 없이 같은 속도로 이동하기 위함)

        // 이동시키기
        transform.position = cur_pos + next_pos;                                  // 이동 속도에 비례해서 입력받은 대로 이동함

        // 애니매이션 실행
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal")) // 가로 이동 키를 눌렀을 때 || 가로 이동 키를 뗐을 때
        {
            anim.SetInteger("input", (int)h);                                     // input 애니메이터에 있는 파라미터(input)의 값을 h 값으로 설정함(변수 h의 값을 float형이기 때문에 형변환 시켜줌)  
        }
    }

    void Fire() // 총알 발사 함수
    {
        // 발사 시키지 않는 조건 (발사 버튼을 누르지 않음, 장전이 되어 있지 않음)
        if (!Input.GetButton("Fire1") || cur_delay < reload_delay) //  누르고 있지 않을 때 (발사 하고 있지 않을 때) || 현재 장전 시간이 재장전 시간보다 작다면 (재장전이 완료되어 있지 않다면) 
            return;                                                // 실행 시키지 않음

        switch (power)
        {
            case 1: // 플레이어의 파워가 1일 때  /  기본 총알 1개 발사

                // Instantiate() -> 매개변수 오브젝트를 생성함 (Destroy()의 반대)  /  매개변수 -> 생성할 오브젝트, 생성할 위치, 생성된 오브젝트의 방향
                // GameObject bullet = Instantiate(bullet_obj_a, transform.position, transform.rotation); 
                
                // 플레이어의 위치에 A 타입 총알을 플레이어의 방향으로 생성함
                GameObject bullet = object_manager.MakeObj("player_bullet_a");
                bullet.transform.position = transform.position;

                // 총알의 Rigidbody를 가져와서 AddForce()로 총알 발사 기능 구현
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                // 총알의 Rigidbody2D를 가져옴

                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                  // 총알을 위쪽 방향으로 이동 시킴 (플레이어는 위쪽을 바라보고 있기 때문) -> 발사
                break;

            case 2: // 플레이어의 파워가 2일 때  /  기본 총알 2개 발사
                
                // 플레이어의 오른쪽에 A 타입 총알을 플레이어의 방향으로 생성함
                GameObject bullet_r = object_manager.MakeObj("player_bullet_a");
                bullet_r.transform.position = transform.position + Vector3.right * 0.1f;
                
                // 플레이어의 왼쪽에 A 타입 총알을 플레이어의 방향으로 생성함
                GameObject bullet_l = object_manager.MakeObj("player_bullet_a");
                bullet_l.transform.position = transform.position + Vector3.left * 0.1f;

                // 총알의 Rigidbody를 가져와서 AddForce()로 총알 발사 기능 구현
                Rigidbody2D rigid_r = bullet_r.GetComponent<Rigidbody2D>();                                                     // 오른쪽 총알의 Rigidbody를 가져옴
                Rigidbody2D rigid_l = bullet_l.GetComponent<Rigidbody2D>();                                                     // 왼쪽 총알의 Rigidbody를 가져옴

                rigid_r.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                         // 오른쪽 총알을 위쪽 방향으로 이동 시킴 (플레이어는 위쪽을 바라보고 있기 때문) -> 발사
                rigid_l.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                         // 왼쪽 총알을 위쪽 방향으로 이동 시킴 (플레이어는 위쪽을 바라보고 있기 때문) -> 발사
                break;

            default: // 플레이어의 파워가 3이상일 때  /  강화 총알 1개와 기본 총알 2개 발사

                // 플레이어의 오른쪽에 A 타입 총알을 플레이어의 방향으로 생성함
                GameObject bullet_rr = object_manager.MakeObj("player_bullet_a");
                bullet_rr.transform.position = transform.position + Vector3.right * 0.3f;
                
                // 플레이어의 위치에 B 타입 총알을 플레이어의 방향으로 생성함
                GameObject bullet_cc = object_manager.MakeObj("player_bullet_b");
                bullet_cc.transform.position = transform.position;

                // 플레이어의 왼쪽에 A 타입 총알을 플레이어의 방향으로 생성함 
                GameObject bullet_ll = object_manager.MakeObj("player_bullet_a");
                bullet_ll.transform.position = transform.position + Vector3.left * 0.3f;

                // 강화 총알 및 총알의 Rigidbody를 가져와서 AddForce()로 총알 발사 기능 구현
                Rigidbody2D rigid_rr = bullet_rr.GetComponent<Rigidbody2D>();                                                     // 오른쪽 총알의 Rigidbody를 가져옴
                Rigidbody2D rigid_cc = bullet_cc.GetComponent<Rigidbody2D>();                                                     // 가운데 총알의 Rigidbody를 가져옴
                Rigidbody2D rigid_ll = bullet_ll.GetComponent<Rigidbody2D>();                                                     // 왼쪽 총알의 Rigidbody를 가져옴

                rigid_rr.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                          // 오른쪽 총알을 위쪽 방향으로 이동 시킴 (플레이어는 위쪽을 바라보고 있기 때문) -> 발사
                rigid_cc.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                          // 가운데 총알을 위쪽 방향으로 이동 시킴 (플레이어는 위쪽을 바라보고 있기 때문) -> 발사
                rigid_ll.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                          // 왼쪽 총알을 위쪽 방향으로 이동 시킴 (플레이어는 위쪽을 바라보고 있기 때문) -> 발사
                break;
        }

        cur_delay = 0;                                                                                                            // 장전 시간 초기화 (재장전을 하게 만들기 위함)

        SoundManager.instance.PlaySound("Shot");                                                                                  // 총을 쏘는 사운드 출력
    }

    void Reload() // 재장전 함수
    {
        cur_delay += Time.deltaTime; // 현재 장전 시간에 시간을 더해줌
    }

    void Boom() // 필살기 효과 함수
    {
        // 실행 시키지 않는 조건 (필살기 발사 버튼을 누르지 않음, 현재 장전되어 있는 필살기가 없음, 현재 필살기를 사용하고 있음)
        if (!Input.GetButtonUp("Fire2") || boom < 1 || is_boom) // 마우스 우클릭을 누르지 않았을 때 (발사 하지 않았을 때) || 장전되어 있는 필살기의 개수가 1보다 작을 때 (필살기가 장전되어 있지 않을 때) || 현재 필살기가 사용되고 있을 때
            return;                                             // 실행 시키지 않음

        boom--;                            // 장전된 필살기 개수에서 1을 빼고
        game_manager.UpdateBoomIcon(boom); // 필살기 UI를 업데이트해줌

        // 필살기 이펙트 관련 코드
        OnBoomEffect();              // 필살기 오브젝트를 활성화 시키고 
        Invoke("OffBoomEffect", 4f); // 4초 뒤에 비활성화 시킴
                                     // 필살기의 기능은 Update() 함수에서 실행함 (필살기를 시전 중이면 계속 필살기의 효과가 발동할 수 있게 하기 위함)
    }

    void OnBoomEffect() // 필살기 오브젝트를 활성화 시키는 함수
    {
        is_boom = true;
        boom_effect.SetActive(true);
    }

    void OffBoomEffect() // 필살기 오브젝트를 비활성화 시키는 함수
    {
        is_boom = false;
        boom_effect.SetActive(false);
    }

    void BoomAffect() // 필살기의 기능 함수
    {
        // 모든 적 및 적의 총알을 지우는 코드

        // 모든 적 오브젝트 받아오기
        GameObject[] enemies_a = object_manager.GetPool("enemy_a");
        GameObject[] enemies_b = object_manager.GetPool("enemy_b");
        GameObject[] enemies_c = object_manager.GetPool("enemy_c");

        // 모든 적 오브젝트 지우기
        for (int index = 0; index < enemies_a.Length; index++)              // 받아온 적의 개수만큼 반복
        {
            if (enemies_a[index].activeSelf)
            {
                Enemy enemy_logic = enemies_a[index].GetComponent<Enemy>(); // 받아온 적 각각의 스크립트를 가져옴

                enemy_logic.OnHit(1000);                                    // 받아온 모든 적에게 1000의 데미지를 줌 (파라미터 : 데미지)
            }
        }

        for (int index = 0; index < enemies_b.Length; index++)              // 받아온 적의 개수만큼 반복
        {
            if (enemies_b[index].activeSelf)
            {
                Enemy enemy_logic = enemies_b[index].GetComponent<Enemy>(); // 받아온 적 각각의 스크립트를 가져옴

                enemy_logic.OnHit(1000);                                    // 받아온 모든 적에게 1000의 데미지를 줌 (파라미터 : 데미지)
            }
        }

        for (int index = 0; index < enemies_c.Length; index++)              // 받아온 적의 개수만큼 반복
        {
            if (enemies_c[index].activeSelf)
            {
                Enemy enemy_logic = enemies_c[index].GetComponent<Enemy>(); // 받아온 적 각각의 스크립트를 가져옴

                enemy_logic.OnHit(1000);                                    // 받아온 모든 적에게 1000의 데미지를 줌 (파라미터 : 데미지)
            }
        }

        // 모든 적의 총알 오브젝트 받아오기
        GameObject[] bullets_a = object_manager.GetPool("enemy_bullet_a");
        GameObject[] bullets_b = object_manager.GetPool("enemy_bullet_b");

        // 모든 적의 총알 오브젝트 지우기
        for (int index = 0; index < bullets_a.Length; index++) // 받아온 적의 개수만큼 반복
        {
            if (bullets_a[index].activeSelf)
            {
                bullets_a[index].SetActive(false);             // 받아온 모든 적의 총알을 비활성화 시킴
            }
        }

        for (int index = 0; index < bullets_b.Length; index++) // 받아온 적의 개수만큼 반복
        {
            if (bullets_b[index].activeSelf)
            {
                bullets_b[index].SetActive(false);             // 받아온 모든 적의 총알을 비활성화 시킴
            }
        }
    }

    public void OnDamaged() // 플레이어가 데미지를 받는 함수 (치트키 용)
    {
        if (is_hit) // 플레이어가 이미 맞은 상태라면
            return; // 실행하지 않음

        is_hit = true;                                       // 플레이어가 맞은 상태라는 것을 알려주고

        life--;                                              // 생명을 -1 해준 뒤에
        game_manager.UpdateLifeIcon(life);                   // UI를 업데이트 해주는 함수를 실행하고

        if (life < 1)                                        // 남은 생명이 1보다 작다면 (플레이어가 죽었다면)
        {
            game_manager.GameOver();                         // 게임 오버 시키는 함수를 실행하고
        }

        else                                                 // 그게 아니라면 (남은 생명이 1 이상이라면)
        {
            game_manager.RespawnPlayer();                    // 플레이어를 리스폰 시키는 함수를 실행하고
        }

        game_manager.CallExplosion(transform.position, "P"); // 폭발을 일으키는 함수 실행 후

        gameObject.SetActive(false);                         // 플레이어를 잠시 안 보이게 함
    }

    void OnTriggerEnter2D(Collider2D collision)   // 플레이어가 다른 오브젝트가 겹쳐졌을 때 때
    {
        // 맵의 경계를 넘지 못하게 하기
        if (collision.gameObject.tag == "Border") // 플레이어와 겹쳐진 오브젝트의 태그가 "Border"이라면
        {
            switch (collision.gameObject.name)    // 플레이어와 겹쳐진 오브젝트의 이름에 따라서
            {
                // bool 변수의 값을 바꿔줌
                case "Top":
                    {
                        is_touch_top = true;
                        break;
                    }

                case "Bottom":
                    {
                        is_touch_bottom = true;
                        break;
                    }

                case "Right":
                    {
                        is_touch_right = true;
                        break;
                    }

                case "Left":
                    {
                        is_touch_left = true;
                        break;
                    }
            }
        }

        // 적에게 닿거나 적의 총알에 닿으면 피격 당하기 
        
        // 플레이어와 겹쳐진 오브젝트의 태그가 "Enemy"이라면 || 플레이어와 겹쳐진 오브젝트의 태그가 "EnemyBullet"이라면 || 플레이어와 겹쳐진 오브젝트의 태그가 "EnemySpawning"이라면
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet" || collision.gameObject.tag == "Boss" || collision.gameObject.tag == "EnemySpawning")
        {
            if (is_hit || is_respawn_time) // 플레이어가 이미 맞은 상태라면 || 플레이어가 현재 무적 시간이라면
                return;                    // 실행하지 않음

            OnDamaged();

            if (collision.gameObject.tag != "Boss" && collision.gameObject.tag != "EnemySpawning" && collision.gameObject.tag != "EnemyBullet") // 플레이어와 겹쳐진 오브젝트의 태그가 "Boss"가 아니라면 && 플레이어와 겹쳐진 오브젝트의 태그가 "EnemySpawning"가 아니라면 && 플레이어와 겹쳐진 오브젝트의 태그가 "EnemyBullet"가 아니라면
            {
                Enemy enemy_logic = collision.GetComponent<Enemy>();                                                                            // 플레이어와 겹쳐진 오브젝트의 스크립트를 가져옴

                enemy_logic.OnHit(1000);                                                                                                        // 해당 오브젝트에게 1000의 데미지를 줌
            }   
        }

        // 플레이어가 먹은 아이템에 따른 효과 실행하기
        else if (collision.gameObject.tag == "Item")               // 플레이어와 겹쳐진 오브젝트의 태그가 "Item"이라면
        {
            Item item = collision.gameObject.GetComponent<Item>(); // 플레이어와 겹쳐진 오브젝트 (Item 태그를 가진 오브젝트) 에게 Item 스크립트를 가져옴

            switch (item.type) 
            {
                case "Coin":                                          // 먹은 아이템의 타입이 "Coin"이라면
                    {
                        score += 1000;                               // 점수를 올려주고

                        SoundManager.instance.PlaySound("Get Coin"); // 점수를 획득 하는 사운드 출력
                    }
                    break;

                case "Boom":                                 // 먹은 아이템의 타입이 "Boom"(필살기)이라면
                    score += 250;                            // 점수를 쪼끔 올려주고

                    if (boom < max_boom)                     // 현재 장전되어 있는 필살기 개수가 최대 장전 개수보다 작으면 
                    {
                        SoundManager.instance.PlaySound("Get Boom"); // 필살기를 획득 하는 사운드 출력

                        boom++;                              // 1개 더 장전해주고

                        game_manager.UpdateBoomIcon(boom);   // 필살기 UI를 업데이트 해줌
                    }

                    else                                     // 그게 아니라면 (최대 장전 개수보다 크거나 같으면)
                    {
                        score += 500;                        // 점수를 조금 더 올려줌

                        SoundManager.instance.PlaySound("Get Coin"); // 점수를 획득 하는 사운드 출력
                    }
                    break;

                case "Power":                                     // 먹은 아이템의 타입이 "Power"이라면
                    score += 500;                                 // 점수를 살짝 올려주고

                    SoundManager.instance.PlaySound("Get Power"); // 회복 하는 사운드 출력 후 

                    if (power < 3)                                // power가 3보다 작으면 
                    {
                        power++;                                  // power에 1을 더해주고
                    }
                 
                    else                                          // 그게 아니라면 (3보다 크거나 같으면)
                    {
                        player_dmg++;                             // player_dmg 변수에 1을 더해줌
                    }
                    break;


                case "Follower":                                 // 먹은 아이템의 타입이 "Follower"이라면
                    score += 100;                                // 점수를 찔끔 올려주고

                    if (follower_num < 3)                        // 팔로워의 개수가 3보다 작다면
                    {
                        SoundManager.instance.PlaySound("Get Follower"); // 팔로워를 획득 하는 사운드 출력

                        followers[follower_num].SetActive(true); // follower_num에 따른 팔로워를 생성하고

                        follower_num++;                          // 팔로워의 개수를 1 더해줌
                    }

                    else                                         // 그게 아니라면 (3보다 크거나 같으면)
                    {
                        score += 500;                            // 점수를 조금 더 올려줌

                        SoundManager.instance.PlaySound("Get Coin"); // 점수를 획득 하는 사운드 출력
                    }
                    break;


                case "Heal":                                     // 먹은 아이템의 타입이 "Heal"이라면
                    if (life < 3)                                // 생명의 개수가 3보다 작다면
                    {
                        SoundManager.instance.PlaySound("Heal"); // 회복 하는 사운드 출력

                        life++;                                  // 생명의 개수를 1 더해줌

                        game_manager.UpdateLifeIcon(life);       // 생명 UI를 업데이트 해줌
                    }

                    else                                         // 그게 아니라면 (3보다 크거나 같으면)
                    {
                        score += 250;                            // 점수를 조금 올려줌

                        SoundManager.instance.PlaySound("Get Coin"); // 점수를 획득 하는 사운드 출력
                    }
                    break;
            }

            collision.gameObject.SetActive(false); // 어떤 아이템을 먹었든 아이템의 효과를 실행하고 먹은 아이템을 삭제시킴
        }
    }

    void OnTriggerExit2D(Collider2D collision)    // 플레이어와 다른 오브젝트가 겹쳐진 상태였다가 나왔을 때
    {
        if (collision.gameObject.tag == "Border") // 플레이어와 겹쳐진 상태였던 오브젝트의 태그가  "Border"이라면
        {
            switch (collision.gameObject.name)    // 플레이어와 닿은 오브젝트의 이름에 따라서
            {
                // bool 변수의 값을 바꿔줌
                case "Top":
                    {
                        is_touch_top = false;
                        break;
                    }

                case "Bottom":
                    {
                        is_touch_bottom = false;
                        break;
                    }

                case "Right":
                    {
                        is_touch_right = false;
                        break;
                    }

                case "Left":
                    {
                        is_touch_left = false;
                        break;
                    }
            }
        }
    }
}
