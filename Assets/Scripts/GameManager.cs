using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ���������� ���� ����
    public int stage;

    public Animator stage_anim;  // �������� ���� �ؽ�Ʈ �ִϸ�����
    public Animator clear_anim;  // �������� Ŭ���� �ؽ�Ʈ �ִϸ�����

    public Animator fade_anim;   // �ϸ� �ִϸ�����

    public Transform player_pos; // �÷��̾��� ��ġ (����ġ ��Ű�� ����)

    public RuntimeAnimatorController[] ac_array; // ���� �ִϸ������� �����ϱ� ���� �Լ�

    // �� ������ ���� ����
    public string[] enemy_objs;      // �� ������ �迭 (���� �����ϱ� ���ؼ� GameObject�� �ƴ� string������ ������)
    public Transform[] spawn_points; // ���� ������ ��ġ �迭

    public float spawn_delay;        // �� ������ �ʿ��� ������
    public float cur_spawn_delay;    // ���� ���� ������ �ð�

    // ���� �Ѿ��� �÷��̾ ���ϰ� �ϱ� ���� ���� (�������� �̹� Scene�� �ö��� ������Ʈ�� ���� �� �� ����)
    public GameObject player;

    // UI�� ���� ����
    public Text score_text; // �÷��̾��� ���� �ؽ�Ʈ
    public Image[] life_images; // �÷��̾��� ���� UI �迭 
    public Image[] boom_images; // �÷��̾��� �ʻ�� UI �迭 

    public GameObject game_over_group; // ���� ���� �Ǿ��� �� Ȱ��ȭ �� UI

    public Text game_over_text;        // ���� ���� �Ǿ��� �� Ȱ��ȭ �� �ؽ�Ʈ

    public ObjectManager object_manager;

    // ������ �б� ���� ����
    public List<Spawn> spawn_list; // �� ������ ���õ� ������ ��� �ִ� ����Ʈ 
    public int spawn_index;        // � ���� ������ ���� ���� �ε���
    public bool spawn_end;         // ��� �� ������ �������� ����

    // �ɼ� â ����
    public Image option_obj; // �ɼ� ������Ʈ�� ���� ���� ���� ����
    bool is_option;          // ESC â�� �߰� ���� ���� ����
    public bool is_live;     // ������ Ȱ��ȭ / ��Ȱ��ȭ ���¸� ����

    // bgm�� ���� ����
    AudioSource bgm_player;
    public Button[] bgm_button;

    void Awake() // �������� ����
    {
        // enemy_objs �迭�� ����� ���� �Ҵ���
        enemy_objs = new string[] { "enemy_a", "enemy_b", "enemy_c", "enemy_boss" };

        spawn_list = new List<Spawn>();

        is_live = true;

        bgm_player = GameObject.Find("Bgm Player").GetComponent<AudioSource>();

        // ���������� ������
        stage = 1;
        StageStart();
    }

    public void StageStart() // ���������� �����ϴ� �Լ�
    {
        // �������� UI(Text) ����
        // UI(Text)�� �ִϸ��̼��� ������
        stage_anim.SetTrigger("on"); // �ִϸ��̼� ����

        // �ؽ�Ʈ ���� �����ϱ�
        stage_anim.GetComponent<Text>().text = "Stage " + stage + "\nStart";
        clear_anim.GetComponent<Text>().text = "Stage " + stage + "\nClear";

        // ���� ���������� �ش��ϴ� �ؽ�Ʈ ������ ����
        ReadSpawnFile();

        // Fade In - ������� ��
        fade_anim.SetTrigger("in");

        if (stage != 1)
        {
            SoundManager.instance.PlaySound("Start");
        }
    }

    public void StageEnd() // ���������� ������ �Լ�
    {
        // Ŭ���� UI(Text) ����
        clear_anim.SetTrigger("on");

        // Fade Out - ��ο����� ��
        fade_anim.SetTrigger("out");

        // ���� ���
        SoundManager.instance.PlaySound("Clear");

        // �÷��̾ ����ġ �ϰ� ��
        player.transform.position = player_pos.position;

        // ���� �������� ����
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

    void ReadSpawnFile() // ���Ƿ� ������ �ؽ�Ʈ ������ �д� �Լ�
    {
        // ���� �ʱ�ȭ
        spawn_list.Clear();
        spawn_index = 0;
        spawn_end = false;

        // ������ �� �پ� �б�
        // TextAsset -> �ؽ�Ʈ ���� ���� Ŭ����  /  �����̸�.Load("�����̸�") -> �����̸��� �ش��ϴ� ���� �ȿ� �ִ� �����̸��� �ش��ϴ� ������ �ҷ���  /  as -> ������ �ҷ��Դµ� Ÿ���� �տ��� ������ Ÿ�� (TextAsset)�� ������ �ƴ϶�� null ������ ó����
        TextAsset text_file = Resources.Load("Stage " + stage) as TextAsset;

        // StringReader -> ���� ���� ���ڿ� �����͸� �д� Ŭ���� (System.IO)
        StringReader string_reader = new StringReader(text_file.text);

        while (string_reader != null) // ������ ���� �����Ͱ� null ���� �ƴ϶��
        {
            // ReadLine() -> �ؽ�Ʈ �����͸� �� �پ� ��ȯ�� (�ڵ� �� �ٲ�)
            string line = string_reader.ReadLine(); // �ؽ�Ʈ �����͸� �� �پ� ��ȯ�ϰ�
            Debug.Log(line);

            if (line == null) // �� �پ� ��ȯ�� ���� null �� �̶��
                break;        // ����

            // ������ ���� �����͵��� �迭�� ������
            Spawn spawn_data = new Spawn();

            // ���� �迭�� ������ �����͵��� �غ��ϰ�
            // Split('������') -> ������ �����ڸ� �������� ���ڿ��� ������ �Լ�
            spawn_data.delay = float.Parse(line.Split(',')[0]); // Spawn ��ũ��Ʈ�� delay ������ ������ �� �پ� ���� ���� ','�� �������� �ڸ� ���� ���� 1��° ���� ������ (float ���� string ������ ��ȯ�� �ڿ� ������)
            spawn_data.type = line.Split(',')[1];               // Spawn ��ũ��Ʈ�� type ������ ������ �� �پ� ���� ���� ','�� �������� �ڸ� ���� ���� 2��° ���� ������
            spawn_data.point = int.Parse(line.Split(',')[2]);   // Spawn ��ũ��Ʈ�� point ������ ������ �� �پ� ���� ���� ','�� �������� �ڸ� ���� ���� 3��° ���� ������ (int ���� string ������ ��ȯ�� �ڿ� ������)

            // �غ�� �����͵��� �迭�� ������
            spawn_list.Add(spawn_data);
        }

        string_reader.Close(); // StringReader�� ����� ������ �۾��� ��� ���� �Ŀ��� �� �ݾƾ���

        // ù��° ���� ������ �����ϱ�
        spawn_delay = spawn_list[0].delay;
    }

    public void ReadBossFile(int boss_spawn) // ������ ���� �ؽ�Ʈ ������ �д� �Լ�
    {
        // ���� �ʱ�ȭ
        spawn_list.Clear();
        spawn_index = 0;
        spawn_end = false;

        // ������ �� �پ� �б�
        // TextAsset -> �ؽ�Ʈ ���� ���� Ŭ����  /  �����̸�.Load("�����̸�") -> �����̸��� �ش��ϴ� ���� �ȿ� �ִ� �����̸��� �ش��ϴ� ������ �ҷ���  /  as -> ������ �ҷ��Դµ� Ÿ���� �տ��� ������ Ÿ�� (TextAsset)�� ������ �ƴ϶�� null ������ ó����
        TextAsset text_file = Resources.Load("Boss " + boss_spawn) as TextAsset;

        // StringReader -> ���� ���� ���ڿ� �����͸� �д� Ŭ���� (System.IO)
        StringReader string_reader = new StringReader(text_file.text);

        while (string_reader != null) // ������ ���� �����Ͱ� null ���� �ƴ϶��
        {
            // ReadLine() -> �ؽ�Ʈ �����͸� �� �پ� ��ȯ�� (�ڵ� �� �ٲ�)
            string line = string_reader.ReadLine(); // �ؽ�Ʈ �����͸� �� �پ� ��ȯ�ϰ�
            Debug.Log(line);

            if (line == null) // �� �پ� ��ȯ�� ���� null �� �̶��
                break;        // ����

            // ������ ���� �����͵��� �迭�� ������
            Spawn spawn_data = new Spawn();

            // ���� �迭�� ������ �����͵��� �غ��ϰ�
            // Split('������') -> ������ �����ڸ� �������� ���ڿ��� ������ �Լ�
            spawn_data.delay = float.Parse(line.Split(',')[0]); // Spawn ��ũ��Ʈ�� delay ������ ������ �� �پ� ���� ���� ','�� �������� �ڸ� ���� ���� 1��° ���� ������ (float ���� string ������ ��ȯ�� �ڿ� ������)
            spawn_data.type = line.Split(',')[1];               // Spawn ��ũ��Ʈ�� type ������ ������ �� �پ� ���� ���� ','�� �������� �ڸ� ���� ���� 2��° ���� ������
            spawn_data.point = int.Parse(line.Split(',')[2]);   // Spawn ��ũ��Ʈ�� point ������ ������ �� �پ� ���� ���� ','�� �������� �ڸ� ���� ���� 3��° ���� ������ (int ���� string ������ ��ȯ�� �ڿ� ������)

            // �غ�� �����͵��� �迭�� ������
            spawn_list.Add(spawn_data);
        }

        string_reader.Close(); // StringReader�� ����� ������ �۾��� ��� ���� �Ŀ��� �� �ݾƾ���

        // ù��° ���� ������ �����ϱ�
        spawn_delay = spawn_list[0].delay;
    }

    public void Option() // �ɼ� â�� Ȱ��ȭ �ϴ� �Լ�
    {
        SoundManager.instance.PlaySound("Button");  // ��ư�� Ŭ���ϴ� ���带 ����ϰ�

        is_option = !is_option;                     // �ɼ��� �ݴ� ���·� ������ش� (true ���¿����� false��, false ���¿����� true�� �ٲ�)
        is_live = !is_live;                         // ���̺긦 �ݴ� ���·� ����� �ش� (true ���¿����� false��, false ���¿����� true�� �ٲ�)

        option_obj.gameObject.SetActive(is_option); // �� ���� �ɼ��� true ���·� �ٲ���ٸ� ��Ȱ��ȭ �Ǿ� �ִ� ESC â UI�� Ȱ��ȭ ��Ŵ 
        Time.timeScale = is_option == true ? 0 : 1; // ESC â�� ���� �ִٸ� �ð��� ���߰� �ƴ϶�� �ð��� �帣�� ��
    }

    void Update() // �� �����Ӹ���
    {
        // �� ������ ���� �ڵ�
        cur_spawn_delay += Time.deltaTime;               // ���� ���� ������ �ð��� �ð��� ������

        if (cur_spawn_delay > spawn_delay && !spawn_end) // ���� ���� ������ �ð��� �� ������ �ʿ��� ������ ���� ũ�� (���� ������ �� �ִٸ�) && ��� ������ �� ������ �ʾҴٸ�
        {
            SpawnEnemy();                                // ���� ������

            cur_spawn_delay = 0;                         // ������ �ʱ�ȭ (�ٽ� �����̸�ŭ ��ٸ� �� �ְ� ��)
        }

        // UI ���� ������Ʈ
        Player player_logic = player.GetComponent<Player>();
        score_text.text = string.Format("{0:n0}", player_logic.score); // int ���� score�� ���� ###,###,###( {0:n0} )�� ������ string �� ������ �ٲ���

        // �ɼ� â ����
        if (Input.GetButtonDown("Cancel")) // ESC ��ư�� ������ �� (ESC�� ������ true�� ��ȯ)
        {
            Option();                      // �ɼ� â�� �ø���
        }

        // ġƮŰ �ڵ�
        if (Input.GetKeyDown(KeyCode.Q)) // Q Ű�� ������
        {
            player_logic.OnDamaged();    // �÷��̾ �������� ����
        }

        if (Input.GetKeyDown(KeyCode.E)) // E Ű�� ������
        {
            SpawnEnemy();                // ���� ��ȯ��
        }

        if (Input.GetKeyDown(KeyCode.P)) // P Ű�� ������
        {
            // Power �������� �ϳ� ���� ȿ�� �ߵ�
            player_logic.score += 500;

            SoundManager.instance.PlaySound("Get Power");

            if (player_logic.power < 3)
                player_logic.power++;

            else 
                player_logic.player_dmg++;
        }

        if (Input.GetKeyDown(KeyCode.B)) // B Ű�� ������
        {
            // Boom �������� �ϳ� ���� ȿ�� �ߵ�
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

        if (Input.GetKeyDown(KeyCode.O)) // O Ű�� ������
        {
            GameOver();                  // ���� ���� ��Ŵ
        }

        if (Input.GetKeyDown(KeyCode.F)) // F Ű�� ������
        {
            // Follower �������� �ϳ� ���� ȿ�� �ߵ�
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

        if (Input.GetKeyDown(KeyCode.H)) // H Ű�� ������
        {
            // Heal �������� �ϳ� ���� ȿ�� �ߵ�
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

    public void SpawnEnemy() // ���� �����ϴ� �Լ�
    {
        // � ���� ��ȯ�� �� �о�� �ؽ�Ʈ �����͸� ����ؼ� ������

        int enemt_index = 0; // � ���� ��ȯ�� �� �����ϴ� ����
        
        switch (spawn_list[spawn_index].type) // ���� ��ȯ�� ���� Ÿ�Կ� ����
        {
            // ��ȯ�� ���� �ٸ��� �� 
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

        int enemy_point = spawn_list[spawn_index].point; // ���� ��ȯ�� ���� ��ġ�� �ٸ��� ����

        // ������ ���� ������ ��ġ�� ������
        GameObject enemy = object_manager.MakeObj(enemy_objs[enemt_index]); // ��ȯ�ؾ� �ϴ� ���� �����ϰ�
        enemy.transform.position = spawn_points[enemy_point].position;      // ��ȯ�ؾ� �ϴ� ��ġ�� ��ġ�ϰ� ��

        // ������ ���� ������Ʈ�� ������
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>(); // ������ ���� Rigidbody2D�� ������
        Enemy enemy_logic = enemy.GetComponent<Enemy>();       // ������ ���� Enemy ��ũ��Ʈ�� ������
        enemy_logic.player = player;                           // ������ ������ �÷��̾� ������ �Ѱ��� (�Ѿ��� �÷��̾��� �������� ��� ����)
        enemy_logic.game_manager = this;                       // ������ ������ �� ��ũ��Ʈ�� �Ѱ���
        enemy_logic.object_manager = object_manager;           // ������ ������ object_manager ������ �Ѱ���

        // ���� � �������� ������ �� ����
        if (enemy_point <= 8 || enemy_point >= 17)                                                // ran_point�� ��(���� ��)�� 8���� �۰ų� ���� �� -> ���� ���� ��ġ�� ȭ�� ���ʿ� �ִ� ���� ����Ʈ�� �� || ran_point�� ��(���� ��)�� 17���� ũ�ų� ���� �� -> ���� ���� ��ġ�� ȭ�� �߰��� �ִ� ���� ����Ʈ�� ��
        {
            rigid.velocity = new Vector2(0, enemy_logic.speed * (-0.75f));                        // ������ ���� �Ʒ������� �����̰� ��
        }

        else if (enemy_point <= 12)                                                               // ran_point�� ��(���� ��)�� 12���� �۰ų� ���� �� -> ���� ���� ��ġ�� ȭ�� ���ʿ� �ִ� ���� ����Ʈ�� ��
        {
            // Vector3.forward -> z�� + 1 (�ݽð� ����)  /   Vector3.back -> z�� - 1 (�ð� ����)
            enemy.transform.Rotate(Vector3.forward * 45);                                         // ������ ���� ������ �ݽð� �������� 45�Ƶ��� (���� �˸��� ������ ���� ����)
            rigid.velocity = new Vector2(enemy_logic.speed, enemy_logic.speed * (-0.75f));        // ������ ���� ������ �Ʒ��� �����̰� ��
        }

        else if (enemy_point <= 16)                                                               // ran_point�� ��(���� ��)�� 16���� �۰ų� ���� �� -> ���� ���� ��ġ�� ȭ�� �����ʿ� �ִ� ���� ����Ʈ�� ��
        {
            enemy.transform.Rotate(Vector3.back * 45);                                            // ������ ���� ������ �ð� �������� 45�Ƶ��� (���� �˸��� ������ ���� ����)
            rigid.velocity = new Vector2(enemy_logic.speed * (-1), enemy_logic.speed * (-0.75f)); // ������ ���� ���� �Ʒ��� �����̰� ��
        }

        // ������ �ε��� ����
        spawn_index++;
        // ����Ʈ�� ���ڸ� �������� ���� Length�� �ƴ� Count
        if (spawn_index >= spawn_list.Count) // ��� ���� ��ȯ�ߴٸ�
        {
            spawn_end = true; // ��� ��ȯ�� �����ٴ� ����� �˷��ְ�
            return;           // ������ ������
        }

        // �� ������ �Ϸ� �Ǹ� ������ ������ ����
        spawn_delay = spawn_list[spawn_index].delay;

        SoundManager.instance.PlaySound("Spawn"); // ��ġ �ϴ� ���� ���
    }

    public void UpdateLifeIcon(int life) // �÷��̾��� ���� ���� �������� ������Ʈ ���ִ� �Լ�  /   �Ű����� -> �÷��̾��� ����
    {
        // ��� ���� UI�� ������ ������ ���� ����
        for (int index = 0; index < life_images.Length; index++) // �ִ� ������ �� ��ŭ �ݺ�
        {
            life_images[index].color = new Color(1, 1, 1, 0);
        }

        // ���� ����� ������� ���������� �ٲ���
        for (int index=0; index<life; index++) // ���� ������ �ִ� ������ �� ��ŭ �ݺ�
        {
            // Image �����̱� ������ SetActive()�� ������� �ʰ� ������ �ٸ��� �ٲ���
            life_images[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoomIcon(int boom) // �÷��̾��� ���� �ʻ�� �������� ������Ʈ ���ִ� �Լ�  /   �Ű����� -> �÷��̾��� �ʻ�� ����
    {
        // ��� ���� UI�� ������ ������ ���� ����
        for (int index = 0; index < boom_images.Length; index++) // �ִ� �ʻ���� �� ��ŭ �ݺ�
        {
            boom_images[index].color = new Color(1, 1, 1, 0);
        }

        // ���� ����� ������� ���������� �ٲ���
        for (int index = 0; index < boom; index++) // ���� ������ �ִ� �ʻ���� �� ��ŭ �ݺ�
        {
            // Image �����̱� ������ SetActive()�� ������� �ʰ� ������ �ٸ��� �ٲ���
            boom_images[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void RespawnPlayer() // �÷��̾ ������ ��Ű�� �Լ�
    {
        Invoke("RespawnPlayerExe", 1.5f); // 1.5�� �ڿ� �÷��̾ ������ ��Ŵ
    }

    void RespawnPlayerExe() // ���������� �÷��̾ ������ ��Ű�� �Լ�
    {
        player.transform.position = Vector3.down * 4f;  // �÷��̾��� ��ġ�� (0, -4, 0)���� ����ְ�
        player.SetActive(true);                         // �÷��̾ ���̰� ��

        Player player_logic = player.GetComponent<Player>(); 
        player_logic.is_hit = false;                    // �÷��̾ �¾Ҵٴ� ����� �ʱ�ȭ ��Ŵ
    }

    public void CallExplosion(Vector3 pos, string type) // �ٸ� ������ ������Ʈ ���� ����Ʈ�� �ҷ��� �� �ְ� �ϴ� �Լ�  /  �Ķ���� -> pos = �����ϴ� ��ġ (�ش� ������Ʈ�� ���� ��ġ), type = �����ϴ� ������Ʈ�� �̸�
    {
        GameObject explosion = object_manager.MakeObj("explosion");
        Explosion explosion_logic = explosion.GetComponent<Explosion>();

        explosion.transform.position = pos;   // ���� ����Ʈ�� �ҷ��� ��ġ ���� ��

        explosion_logic.StartExplosion(type); // ���� ��Ŵ
    }

    public void ChangeAc(Animator anim, int index) // Animator ������ �����ϴ� �Լ�
    {
        anim.runtimeAnimatorController = ac_array[index];
    }

    public void ChangeBgm(int bgm_button) // ����� ���� ��ư�� ������ ��
    {
        SoundManager.instance.PlaySound("Button");                     // ��ư�� ������ ���带 ����ϰ�

        bgm_player.clip = SoundManager.instance.bgm_clips[bgm_button]; // ����� �÷��̾ ��� ���� bgm_button ���� �ش��ϴ� ��������� ���� ��
        bgm_player.Play();                                             // ������� �÷�����
    }

    public void ClickStartBtn() // ������ �����ϴ� �Լ�
    {
        SoundManager.instance.PlaySound("Button"); // ��ư�� Ŭ���ϴ� ���带 ����ϰ�

        SceneManager.LoadScene(0);                 // ���� ���� �ҷ���
    }

    public void ClickExitBtn() // ������ �����ϴ� �Լ�
    {
        SoundManager.instance.PlaySound("Exit"); // ������ �����ϴ� ���带 ����ϰ�

        Application.Quit();                      // ������ ������
    }

    public void GameOver() // ���� ���� ��Ű�� �Լ�
    {
        game_over_group.SetActive(true); // ���� ���� �Ǿ��� �� Ȱ��ȭ ��Ű�� ������Ʈ�� Ȱ��ȭ ��Ŵ
    }

    public void GameRetry() // ������ �ٽ� �����ϴ� �Լ�
    {
        SceneManager.LoadScene(0); // ���� ���� �ҷ���
    }

    void LoadGameScene() // ���� ���� �ҷ����� �Լ�
    {
        SoundManager.instance.PlaySound("Button"); // ��ư�� Ŭ���ϴ� ���带 ����ϰ�

        SceneManager.LoadScene(0);                 // ���� ���� �ҷ���
    }
}