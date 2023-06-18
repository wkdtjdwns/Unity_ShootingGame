using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 스테이지에 대한 변수
    public int stage;

    public Animator stage_anim;  // 스테이지 시작 텍스트 애니메이터
    public Animator clear_anim;  // 스테이지 클리어 텍스트 애니메이터

    public Animator fade_anim;   // 암막 애니메이터

    public Transform player_pos; // 플레이어의 위치 (원위치 시키기 위함)

    public RuntimeAnimatorController[] ac_array; // 적의 애니매이터을 변경하기 위한 함수

    // 적 생성에 대한 변수
    public string[] enemy_objs;      // 적 프리펩 배열 (적을 생성하기 위해서 GameObject가 아닌 string형으로 선언함)
    public Transform[] spawn_points; // 적을 생성할 위치 배열

    public float spawn_delay;        // 적 생성에 필요한 딜레이
    public float cur_spawn_delay;    // 현재 생성 딜레이 시간

    // 적의 총알이 플레이어를 향하게 하기 위한 변수 (프리펩은 이미 Scene에 올라인 오브젝트에 접근 할 수 없음)
    public GameObject player;

    // UI에 대한 변수
    public Text score_text; // 플레이어의 점수 텍스트
    public Image[] life_images; // 플레이어의 생명 UI 배열 
    public Image[] boom_images; // 플레이어의 필살기 UI 배열 

    public GameObject game_over_group; // 게임 오버 되었을 때 활성화 할 UI

    public Text game_over_text;        // 게임 오버 되었을 때 활성화 할 텍스트

    public ObjectManager object_manager;

    // 파일을 읽기 위한 변수
    public List<Spawn> spawn_list; // 적 생성에 관련된 정보가 들어 있는 리스트 
    public int spawn_index;        // 어떤 적을 생성할 지에 대한 인덱스
    public bool spawn_end;         // 모든 적 생성이 끝났는지 여부

    // 옵션 창 변수
    public Image option_obj; // 옵션 오브젝트를 가져 오기 위한 변수
    bool is_option;          // ESC 창이 뜨게 할지 말지 여부
    public bool is_live;     // 게임의 활성화 / 비활성화 상태를 구분

    // bgm에 대한 변수
    AudioSource bgm_player;
    public Button[] bgm_button;

    void Awake() // 시작하자 마자
    {
        // enemy_objs 배열의 경우의 수를 할당함
        enemy_objs = new string[] { "enemy_a", "enemy_b", "enemy_c", "enemy_boss" };

        spawn_list = new List<Spawn>();

        is_live = true;

        bgm_player = GameObject.Find("Bgm Player").GetComponent<AudioSource>();

        // 스테이지를 시작힘
        stage = 1;
        StageStart();
    }

    public void StageStart() // 스테이지를 시작하는 함수
    {
        // 스테이지 UI(Text) 설정
        // UI(Text)도 애니매이션이 가능함
        stage_anim.SetTrigger("on"); // 애니매이션 실행

        // 텍스트 내용 변경하기
        stage_anim.GetComponent<Text>().text = "Stage " + stage + "\nStart";
        clear_anim.GetComponent<Text>().text = "Stage " + stage + "\nClear";

        // 현재 스테이지에 해당하는 텍스트 파일을 읽음
        ReadSpawnFile();

        // Fade In - 밝아지게 함
        fade_anim.SetTrigger("in");

        if (stage != 1)
        {
            SoundManager.instance.PlaySound("Start");
        }
    }

    public void StageEnd() // 스테이지를 끝내는 함수
    {
        // 클리어 UI(Text) 설정
        clear_anim.SetTrigger("on");

        // Fade Out - 어두워지게 함
        fade_anim.SetTrigger("out");

        // 사운드 출력
        SoundManager.instance.PlaySound("Clear");

        // 플레이어를 원위치 하게 함
        player.transform.position = player_pos.position;

        // 다음 스테이지 시작
        stage++;
        if (stage > 3) 
        {
            game_over_text.text = "Game Clear!";

            Invoke("GameOver",4);
        }

        else
        {
            Invoke("StageStart", 5);
        }
    }

    void ReadSpawnFile() // 임의로 설정한 텍스트 파일을 읽는 함수
    {
        // 변수 초기화
        spawn_list.Clear();
        spawn_index = 0;
        spawn_end = false;

        // 파일을 한 줄씩 읽기
        // TextAsset -> 텍스트 파일 에셋 클래스  /  폴더이름.Load("파일이름") -> 폴더이름에 해당하는 폴더 안에 있는 파일이름에 해당하는 파일을 불러옴  /  as -> 파일을 불러왔는데 타입이 앞에서 선언한 타입 (TextAsset)의 파일이 아니라면 null 값으로 처리함
        TextAsset text_file = Resources.Load("Stage " + stage) as TextAsset;

        // StringReader -> 파일 내의 문자열 데이터를 읽는 클래스 (System.IO)
        StringReader string_reader = new StringReader(text_file.text);

        while (string_reader != null) // 위에서 읽은 데이터가 null 값이 아니라면
        {
            // ReadLine() -> 텍스트 데이터를 한 줄씩 반환함 (자동 줄 바꿈)
            string line = string_reader.ReadLine(); // 텍스트 데이터를 한 줄씩 반환하고
            Debug.Log(line);

            if (line == null) // 한 줄씩 반환한 값이 null 값 이라면
                break;        // 멈춤

            // 위에서 읽은 데이터들을 배열에 저장함
            Spawn spawn_data = new Spawn();

            // 먼저 배열에 저장할 데이터들을 준비하고
            // Split('구분자') -> 지정한 구분자를 기준으로 문자열을 나누는 함수
            spawn_data.delay = float.Parse(line.Split(',')[0]); // Spawn 스크립트의 delay 변수에 위에서 한 줄씩 읽은 값을 ','를 기준으로 자른 뒤의 값중 1번째 값을 저장함 (float 값을 string 값으로 변환한 뒤에 저장함)
            spawn_data.type = line.Split(',')[1];               // Spawn 스크립트의 type 변수에 위에서 한 줄씩 읽은 값을 ','를 기준으로 자른 뒤의 값중 2번째 값을 저장함
            spawn_data.point = int.Parse(line.Split(',')[2]);   // Spawn 스크립트의 point 변수에 위에서 한 줄씩 읽은 값을 ','를 기준으로 자른 뒤의 값중 3번째 값을 저장함 (int 값을 string 값으로 변환한 뒤에 저장함)

            // 준비된 데이터들을 배열에 저장함
            spawn_list.Add(spawn_data);
        }

        string_reader.Close(); // StringReader로 열어둔 파일은 작업이 모두 끝난 후에는 꼭 닫아야함

        // 첫번째 스폰 딜레이 적용하기
        spawn_delay = spawn_list[0].delay;
    }

    public void ReadBossFile(int boss_spawn) // 보스의 패턴 텍스트 파일을 읽는 함수
    {
        // 변수 초기화
        spawn_list.Clear();
        spawn_index = 0;
        spawn_end = false;

        // 파일을 한 줄씩 읽기
        // TextAsset -> 텍스트 파일 에셋 클래스  /  폴더이름.Load("파일이름") -> 폴더이름에 해당하는 폴더 안에 있는 파일이름에 해당하는 파일을 불러옴  /  as -> 파일을 불러왔는데 타입이 앞에서 선언한 타입 (TextAsset)의 파일이 아니라면 null 값으로 처리함
        TextAsset text_file = Resources.Load("Boss " + boss_spawn) as TextAsset;

        // StringReader -> 파일 내의 문자열 데이터를 읽는 클래스 (System.IO)
        StringReader string_reader = new StringReader(text_file.text);

        while (string_reader != null) // 위에서 읽은 데이터가 null 값이 아니라면
        {
            // ReadLine() -> 텍스트 데이터를 한 줄씩 반환함 (자동 줄 바꿈)
            string line = string_reader.ReadLine(); // 텍스트 데이터를 한 줄씩 반환하고
            Debug.Log(line);

            if (line == null) // 한 줄씩 반환한 값이 null 값 이라면
                break;        // 멈춤

            // 위에서 읽은 데이터들을 배열에 저장함
            Spawn spawn_data = new Spawn();

            // 먼저 배열에 저장할 데이터들을 준비하고
            // Split('구분자') -> 지정한 구분자를 기준으로 문자열을 나누는 함수
            spawn_data.delay = float.Parse(line.Split(',')[0]); // Spawn 스크립트의 delay 변수에 위에서 한 줄씩 읽은 값을 ','를 기준으로 자른 뒤의 값중 1번째 값을 저장함 (float 값을 string 값으로 변환한 뒤에 저장함)
            spawn_data.type = line.Split(',')[1];               // Spawn 스크립트의 type 변수에 위에서 한 줄씩 읽은 값을 ','를 기준으로 자른 뒤의 값중 2번째 값을 저장함
            spawn_data.point = int.Parse(line.Split(',')[2]);   // Spawn 스크립트의 point 변수에 위에서 한 줄씩 읽은 값을 ','를 기준으로 자른 뒤의 값중 3번째 값을 저장함 (int 값을 string 값으로 변환한 뒤에 저장함)

            // 준비된 데이터들을 배열에 저장함
            spawn_list.Add(spawn_data);
        }

        string_reader.Close(); // StringReader로 열어둔 파일은 작업이 모두 끝난 후에는 꼭 닫아야함

        // 첫번째 스폰 딜레이 적용하기
        spawn_delay = spawn_list[0].delay;
    }

    public void Option() // 옵션 창을 활성화 하는 함수
    {
        SoundManager.instance.PlaySound("Button");  // 버튼을 클릭하는 사운드를 출력하고

        is_option = !is_option;                     // 옵션을 반대 상태로 만들어준다 (true 상태였으면 false로, false 상태였으면 true로 바꿈)
        is_live = !is_live;                         // 라이브를 반대 상태로 만들어 준다 (true 상태였으면 false로, false 상태였으면 true로 바꿈)

        option_obj.gameObject.SetActive(is_option); // 그 다음 옵션이 true 상태로 바뀌었다면 비활성화 되어 있던 ESC 창 UI를 활성화 시킴 
        Time.timeScale = is_option == true ? 0 : 1; // ESC 창이 열려 있다면 시간을 멈추고 아니라면 시간을 흐르게 함
    }

    void Update() // 매 프라임마다
    {
        // 적 생성에 관한 코드
        cur_spawn_delay += Time.deltaTime;               // 현재 생성 딜레이 시간에 시간을 더해줌

        if (cur_spawn_delay > spawn_delay && !spawn_end) // 현재 생성 딜레이 시간이 적 생성에 필요한 딜레이 보다 크면 (적을 생성할 수 있다면) && 모든 생성이 다 끝나지 않았다면
        {
            SpawnEnemy();                                // 적을 생성함

            cur_spawn_delay = 0;                         // 딜레이 초기화 (다시 딜레이만큼 기다릴 수 있게 함)
        }

        // UI 정보 업데이트
        Player player_logic = player.GetComponent<Player>();
        score_text.text = string.Format("{0:n0}", player_logic.score); // int 형인 score의 값을 ###,###,###( {0:n0} )의 형태인 string 형 값으로 바꿔줌

        // 옵션 창 띄우기
        if (Input.GetButtonDown("Cancel")) // ESC 버튼을 눌렀을 때 (ESC를 누르면 true를 반환)
        {
            Option();                      // 옵션 창을 올린다
        }

        // 치트키 코드
        if (Input.GetKeyDown(KeyCode.Q)) // Q 키를 누르면
        {
            player_logic.OnDamaged();    // 플레이어가 데미지를 받음
        }

        if (Input.GetKeyDown(KeyCode.E)) // E 키를 누르면
        {
            SpawnEnemy();                // 적을 소환함
        }

        if (Input.GetKeyDown(KeyCode.P)) // P 키를 누르면
        {
            // Power 아이템을 하나 먹은 효과 발동
            player_logic.score += 500;

            SoundManager.instance.PlaySound("Get Power");

            if (player_logic.power < 3)
                player_logic.power++;

            else 
                player_logic.player_dmg++;
        }

        if (Input.GetKeyDown(KeyCode.B)) // B 키를 누르면
        {
            // Boom 아이템을 하나 먹은 효과 발동
           player_logic.score += 250;

            if (player_logic.boom < player_logic.max_boom)
            {
                SoundManager.instance.PlaySound("Get Boom");

                player_logic.boom++;

                UpdateBoomIcon(player_logic.boom);
            }

            else
            {
                player_logic.score += 500;

                SoundManager.instance.PlaySound("Get Score");
            }
        }

        if (Input.GetKeyDown(KeyCode.O)) // O 키를 누르면
        {
            GameOver();                  // 게임 오버 시킴
        }

        if (Input.GetKeyDown(KeyCode.F)) // F 키를 누르면
        {
            // Follower 아이템을 하나 먹은 효과 발동
            player_logic.score += 100;

            if (player_logic.follower_num < 3)
            {
                SoundManager.instance.PlaySound("Get Follower");

                player_logic.followers[player_logic.follower_num].SetActive(true);

                player_logic.follower_num++;
            }

            else
            {
                player_logic.score += 500;

                SoundManager.instance.PlaySound("Get Score");
            }
        }

        if (Input.GetKeyDown(KeyCode.H)) // H 키를 누르면
        {
            // Heal 아이템을 하나 먹은 효과 발동
            if (player_logic.life < 3)
            {
                SoundManager.instance.PlaySound("Heal");

                player_logic.life++;

                UpdateLifeIcon(player_logic.life);
            }

            else
            {
                player_logic.score += 250;

                SoundManager.instance.PlaySound("Get Score");
            }
        }
    }

    public void SpawnEnemy() // 적을 생성하는 함수
    {
        // 어떤 적을 소환할 지 읽어온 텍스트 데이터를 사용해서 결정함

        int enemt_index = 0; // 어떤 적을 소환할 지 결정하는 변수
        
        switch (spawn_list[spawn_index].type) // 현재 소환할 적의 타입에 따라서
        {
            // 소환할 적을 다르게 함 
            case "A":
                enemt_index = 0;
                break;

            case "B":
                enemt_index = 1;
                break;

            case "C":
                enemt_index = 2;
                break;

            case "Boss":
                enemt_index = 3;
                break;
        }

        int enemy_point = spawn_list[spawn_index].point; // 현재 소환할 적의 위치도 다르게 해줌

        // 정해진 적을 정해진 위치에 생성함
        GameObject enemy = object_manager.MakeObj(enemy_objs[enemt_index]); // 소환해야 하는 적을 생성하고
        enemy.transform.position = spawn_points[enemy_point].position;      // 소환해야 하는 위치에 위치하게 함

        // 생성된 적의 콤포넌트를 가져옴
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>(); // 생성된 적의 Rigidbody2D를 가져옴
        Enemy enemy_logic = enemy.GetComponent<Enemy>();       // 생성된 적의 Enemy 스크립트를 가져옴
        enemy_logic.player = player;                           // 생성된 적에게 플레이어 변수를 넘겨줌 (총알을 플레이어의 방향으로 쏘기 위함)
        enemy_logic.game_manager = this;                       // 생성된 적에게 이 스크립트를 넘겨줌
        enemy_logic.object_manager = object_manager;           // 생성된 적에게 object_manager 변수를 넘겨줌

        // 적이 어떤 방향으로 움직일 지 정함
        if (enemy_point <= 8 || enemy_point >= 17)                                                // ran_point의 값(랜덤 값)이 8보다 작거나 같을 때 -> 적의 생성 위치가 화면 위쪽에 있는 스폰 포인트일 때 || ran_point의 값(랜덤 값)이 17보다 크거나 같을 때 -> 적의 생성 위치가 화면 중간에 있는 스폰 포인트일 때
        {
            rigid.velocity = new Vector2(0, enemy_logic.speed * (-0.75f));                        // 생성된 적을 아래쪽으로 움직이게 함
        }

        else if (enemy_point <= 12)                                                               // ran_point의 값(랜덤 값)이 12보다 작거나 같을 때 -> 적의 생성 위치가 화면 왼쪽에 있는 스폰 포인트일 때
        {
            // Vector3.forward -> z축 + 1 (반시계 방향)  /   Vector3.back -> z축 - 1 (시계 방향)
            enemy.transform.Rotate(Vector3.forward * 45);                                         // 생성된 적의 방향을 반시계 방향으로 45°돌림 (적이 알맞은 방향을 보기 위함)
            rigid.velocity = new Vector2(enemy_logic.speed, enemy_logic.speed * (-0.75f));        // 생성된 적을 오른쪽 아래로 움직이게 함
        }

        else if (enemy_point <= 16)                                                               // ran_point의 값(랜덤 값)이 16보다 작거나 같을 때 -> 적의 생성 위치가 화면 오른쪽에 있는 스폰 포인트일 때
        {
            enemy.transform.Rotate(Vector3.back * 45);                                            // 생성된 적의 방향을 시계 방향으로 45°돌림 (적이 알맞은 방향을 보기 위함)
            rigid.velocity = new Vector2(enemy_logic.speed * (-1), enemy_logic.speed * (-0.75f)); // 생성된 적을 왼쪽 아래로 움직이게 함
        }

        // 리스폰 인덱스 증가
        spawn_index++;
        // 리스트의 숫자를 가져오는 것은 Length가 아닌 Count
        if (spawn_index >= spawn_list.Count) // 모든 적을 소환했다면
        {
            spawn_end = true; // 모든 소환이 끝났다는 사실을 알려주고
            return;           // 실행을 종료함
        }

        // 적 생성이 완료 되면 리스폰 딜레이 갱신
        spawn_delay = spawn_list[spawn_index].delay;

        SoundManager.instance.PlaySound("Spawn"); // 터치 하는 사운드 출력
    }

    public void UpdateLifeIcon(int life) // 플레이어의 남은 생명 아이콘을 업데이트 해주는 함수  /   매개변수 -> 플레이어의 생명
    {
        // 모든 생명 UI를 투명한 색으로 만든 다음
        for (int index = 0; index < life_images.Length; index++) // 최대 생명의 수 만큼 반복
        {
            life_images[index].color = new Color(1, 1, 1, 0);
        }

        // 남은 목숨의 개수대로 반투명으로 바꿔줌
        for (int index=0; index<life; index++) // 현재 가지고 있는 생명의 수 만큼 반복
        {
            // Image 변수이기 때문에 SetActive()를 사용하지 않고 색상을 다르게 바꿔줌
            life_images[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoomIcon(int boom) // 플레이어의 남은 필살기 아이콘을 업데이트 해주는 함수  /   매개변수 -> 플레이어의 필살기 개수
    {
        // 모든 생명 UI를 투명한 색으로 만든 다음
        for (int index = 0; index < boom_images.Length; index++) // 최대 필살기의 수 만큼 반복
        {
            boom_images[index].color = new Color(1, 1, 1, 0);
        }

        // 남은 목숨의 개수대로 반투명으로 바꿔줌
        for (int index = 0; index < boom; index++) // 현재 가지고 있는 필살기의 수 만큼 반복
        {
            // Image 변수이기 때문에 SetActive()를 사용하지 않고 색상을 다르게 바꿔줌
            boom_images[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void RespawnPlayer() // 플레이어를 리스폰 시키는 함수
    {
        Invoke("RespawnPlayerExe", 1.5f); // 1.5초 뒤에 플레이어를 리스폰 시킴
    }

    void RespawnPlayerExe() // 실질적으로 플레이어를 리스폰 시키는 함수
    {
        player.transform.position = Vector3.down * 4f;  // 플레이어의 위치를 (0, -4, 0)으로 잡아주고
        player.SetActive(true);                         // 플레이어를 보이게 함

        Player player_logic = player.GetComponent<Player>(); 
        player_logic.is_hit = false;                    // 플레이어가 맞았다는 사실을 초기화 시킴
    }

    public void CallExplosion(Vector3 pos, string type) // 다른 곳에서 오브젝트 폭발 이펙트를 불러올 수 있게 하는 함수  /  파라미터 -> pos = 폭발하는 위치 (해당 오브젝트의 현재 위치), type = 폭발하는 오브젝트의 이름
    {
        GameObject explosion = object_manager.MakeObj("explosion");
        Explosion explosion_logic = explosion.GetComponent<Explosion>();

        explosion.transform.position = pos;   // 폭발 이펙트를 불러올 위치 설정 후

        explosion_logic.StartExplosion(type); // 폭발 시킴
    }

    public void ChangeAc(Animator anim, int index) // Animator 변경을 관리하는 함수
    {
        anim.runtimeAnimatorController = ac_array[index];
    }

    public void ChangeBgm(int bgm_button) // 배경음 변경 버튼을 눌렀을 때
    {
        SoundManager.instance.PlaySound("Button");                     // 버튼을 누르는 사운드를 출력하고

        bgm_player.clip = SoundManager.instance.bgm_clips[bgm_button]; // 배경음 플레이어에 방금 받은 bgm_button 값에 해당하는 배경음으로 설정 후
        bgm_player.Play();                                             // 배경음을 플레이함
    }

    public void ClickStartBtn() // 게임을 시작하는 함수
    {
        SoundManager.instance.PlaySound("Button"); // 버튼을 클릭하는 사운드를 출력하고

        SceneManager.LoadScene(0);                 // 게임 씬을 불러옴
    }

    public void ClickExitBtn() // 게임을 종료하는 함수
    {
        SoundManager.instance.PlaySound("Exit"); // 게임을 종료하는 사운드를 출력하고

        Application.Quit();                      // 게임을 종료함
    }

    public void GameOver() // 게임 오버 시키는 함수
    {
        game_over_group.SetActive(true); // 게임 오버 되었을 때 활성화 시키는 오브젝트를 활성화 시킴
    }

    public void GameRetry() // 게임을 다시 시작하는 함수
    {
        SceneManager.LoadScene(0); // 게임 씬을 불러옴
    }

    void LoadGameScene() // 게임 씬을 불러오는 함수
    {
        SoundManager.instance.PlaySound("Button"); // 버튼을 클릭하는 사운드를 출력하고

        SceneManager.LoadScene(0);                 // 게임 씬을 불러옴
    }
}