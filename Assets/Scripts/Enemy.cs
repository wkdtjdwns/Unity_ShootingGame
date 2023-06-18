using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // ���� ����
    public float speed;       // ���� �̵� �ӵ�
    public float health;      // ���� ü��
    public string enemy_name; // ���� �̸�
    public int enemy_score;   // ������ ���� ������ ����

    public int stage;

    // ������ ���� ������ �ʿ��� ����
    public int pattern_index;       // ������ ���� �ε��� ����
    public int cur_pattern_count;   // ���� ���� Ƚ�� ����
    public int[] max_pattern_count; // �ش� ������ �ִ� ���� Ƚ�� �迭 ���� (�������� ������ �����ϱ� ������ �迭 ������ ����)

    public float ran_num;           // ���Ͽ� �ʿ��� ������ ��

    // �Ѿ� ����
    public GameObject bullet_obj_a; // A Ÿ�� �Ѿ�
    public GameObject bullet_obj_b; // B Ÿ�� �Ѿ�

    // �Ѿ��� �߻��ϴ� ������ (������)
    public float cur_delay;    // ���� ���� �ð�
    public float reload_delay; // ������ �ð�

    // ������ ����
    public GameObject item_boom;
    public GameObject item_coin;
    public GameObject item_power;
    public GameObject item_follower;

    public Sprite[] sprites; // ���� ��������Ʈ�� ������ �迭 ����

    // �������� ������Ʈ�� ���� ��Ű�� ���� (�������� �̹� Scene�� �ö��� ������Ʈ�� ���� �� �� ���� -> GameManager���� ������Ʈ�� ������ ����)
    public GameObject player;
    public GameManager game_manager;
    public ObjectManager object_manager;

    // �ʱ�ȭ �� ����
    SpriteRenderer sprite_renderer;
    Animator anim;
    Animator spawn_anim;

    void Awake() // �������� ����
    {
        // �ʱ�ȭ ��
        sprite_renderer = GetComponent<SpriteRenderer>();

        stage = 1;

        if (enemy_name == "Boss") // �� �̸��� ���� �� ����
        {
            // �ִϸ��̼� ���
            anim = GetComponent<Animator>();
        }
    }

    void OnEnable() // ������Ʈ�� Ȱ��ȭ �Ǿ��� ��
    {
        // �Ҹ�Ǵ� ������ Ȱ��ȭ �ɶ� �ٽ� �ʱ�ȭ �ؾ���
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

    void Stop() // ���� ���߰� �ϴ� �Լ�
    {
        // Stop() �Լ��� 2�� ������� �ʰ� ��
        if (!gameObject.activeSelf) // �ش� ������Ʈ�� �ش� ������Ʈ�� Ȱ��ȭ �Ǿ����� ������
            return;                 // �����Ű�� ����

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 0.75f);
    }

    void Think() // ���� �����ϰ� �ϴ� �Լ�
    {
        // ���� �±� �� �ִϸ����� �ʱ�ȭ
        gameObject.tag = "Boss";
        game_manager.ChangeAc(anim, 0);

        // ���� ü�¿� ���� ������ ������ �ٸ��� ����
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

        Debug.Log("��ų ��� " +  pattern_index);

        cur_pattern_count = 0; // ���� ���� Ƚ�� �ʱ�ȭ

        // ������ ������ ���� �ε����� ������ ������ �ٸ��� ��
        switch (pattern_index)
        {
            case 0:
                max_pattern_count[pattern_index] = Random.Range(2, 4); // �ش� ������ �ִ� ���� Ƚ���� 2 ~ 3 ������ �� �� �������� ���ϰ�

                FireForward();                                         // ���� ����
                break; 
            
            case 1:
                max_pattern_count[pattern_index] = Random.Range(3, 7); // �ش� ������ �ִ� ���� Ƚ���� 3 ~ 6 ������ �� �� �������� ���ϰ�

                FireShot();                                            // ���� ����
                break;

            case 2:
                max_pattern_count[pattern_index] = Random.Range(80, 101); // �ش� ������ �ִ� ���� Ƚ���� 80 ~ 100 ������ �� �� �������� ���ϰ�

                if (max_pattern_count[pattern_index] % 2 == 0)            // ������ ���� ¦����
                {
                    max_pattern_count[pattern_index]++;                   // 1�� ���ؼ� Ȧ���� ����� �� (�÷��̾ ������ �־ �������� ���� ����)
                }

                ran_num = Random.Range(4.5f, 10f);                      // ���Ͽ� �ʿ��� ������ ���� ���� �ڿ�

                FireArc();                                                // ���� ����
                break;

            case 3:
                max_pattern_count[pattern_index] = Random.Range(8, 13); // �ش� ������ �ִ� ���� Ƚ���� 8 ~ 12 ������ �� �� �������� ���ϰ�

                FireAround();                                           // ���� ����
                break;

            case 4:
                int boss_spawn = Random.Range(0, 3);

                Spawn(boss_spawn);
                break;

            case 5:
                break;
        }
    }

    // ���� �ε��� ���� ���� ���ϵ�
    void FireForward()
    {
        // ������ �Ѿ� 8�� �߻�

        if (health <= 0) // ������ �׾��ٸ�
            return;      // �����Ű�� ����

        // ���� �����ʿ� ������ A Ÿ�� �Ѿ��� ���� �������� ������
        GameObject bullet_r = object_manager.MakeObj("boss_bullet_a");
        bullet_r.transform.position = transform.position + Vector3.right * 0.25f;

        // ���� �������ʿ� ������ A Ÿ�� �Ѿ��� ���� �������� ������
        GameObject bullet_rr = object_manager.MakeObj("boss_bullet_a");
        bullet_rr.transform.position = transform.position + Vector3.right * 0.6f;

        // ���� ���������ʿ� ������ A Ÿ�� �Ѿ��� ���� �������� ������
        GameObject bullet_rrr = object_manager.MakeObj("boss_bullet_a");
        bullet_rrr.transform.position = transform.position + Vector3.right * 0.85f;
        
        // ���� �����������ʿ� ������ A Ÿ�� �Ѿ��� ���� �������� ������
        GameObject bullet_rrrr = object_manager.MakeObj("boss_bullet_a");
        bullet_rrrr.transform.position = transform.position + Vector3.right * 1.15f;

        // ���� ���ʿ� ������ A Ÿ�� �Ѿ��� ���� �������� ������
        GameObject bullet_l = object_manager.MakeObj("boss_bullet_a");
        bullet_l.transform.position = transform.position + Vector3.left * 0.25f;

        // ���� �޿��ʿ� ������ A Ÿ�� �Ѿ��� ���� �������� ������
        GameObject bullet_ll = object_manager.MakeObj("boss_bullet_a");
        bullet_ll.transform.position = transform.position + Vector3.left * 0.6f;

        // ���� �޿޿��ʿ� ������ A Ÿ�� �Ѿ��� ���� �������� ������
        GameObject bullet_lll = object_manager.MakeObj("boss_bullet_a");
        bullet_lll.transform.position = transform.position + Vector3.left * 0.85f;

        // ���� �޿޿޿��ʿ� ������ A Ÿ�� �Ѿ��� ���� �������� ������
        GameObject bullet_llll = object_manager.MakeObj("boss_bullet_a");
        bullet_llll.transform.position = transform.position + Vector3.left * 1.15f;

        // �Ѿ��� Rigidbody�� �����ͼ� AddForce()�� �Ѿ� �߻� ��� ����
        Rigidbody2D rigid_r = bullet_r.GetComponent<Rigidbody2D>();       // ������ �Ѿ��� Rigidbody�� ������
        Rigidbody2D rigid_rr = bullet_rr.GetComponent<Rigidbody2D>();     // �������� �Ѿ��� Rigidbody�� ������
        Rigidbody2D rigid_rrr = bullet_rrr.GetComponent<Rigidbody2D>();   // ������ �Ѿ��� Rigidbody�� ������
        Rigidbody2D rigid_rrrr = bullet_rrrr.GetComponent<Rigidbody2D>(); // �������� �Ѿ��� Rigidbody�� ������

        Rigidbody2D rigid_l = bullet_l.GetComponent<Rigidbody2D>();       // ���� �Ѿ��� Rigidbody�� ������
        Rigidbody2D rigid_ll = bullet_ll.GetComponent<Rigidbody2D>();     // �޿��� �Ѿ��� Rigidbody�� ������
        Rigidbody2D rigid_lll = bullet_lll.GetComponent<Rigidbody2D>();   // �޿޿��� �Ѿ��� Rigidbody�� ������
        Rigidbody2D rigid_llll = bullet_llll.GetComponent<Rigidbody2D>(); // �޿޿޿��� �Ѿ��� Rigidbody�� ������

        // �Ʒ� ������ �Ѿ˵� �߻�
        rigid_r.AddForce(Vector2.down * 8, ForceMode2D.Impulse);    // ������ �Ѿ��� �÷��̾� �������� �̵� ��Ŵ-> �߻�
        rigid_rr.AddForce(Vector2.down * 8, ForceMode2D.Impulse);   // �������� �Ѿ��� �÷��̾� �������� �̵� ��Ŵ-> �߻�
        rigid_rrr.AddForce(Vector2.down * 8, ForceMode2D.Impulse);  // ���������� �Ѿ��� �÷��̾� �������� �̵� ��Ŵ-> �߻�
        rigid_rrrr.AddForce(Vector2.down * 8, ForceMode2D.Impulse); // ������������ �Ѿ��� �÷��̾� �������� �̵� ��Ŵ-> �߻�

        rigid_l.AddForce(Vector2.down * 8, ForceMode2D.Impulse);    // ���� �Ѿ��� �÷��̾� �������� �̵� ��Ŵ-> �߻�
        rigid_ll.AddForce(Vector2.down * 8, ForceMode2D.Impulse);   // �޿��� �Ѿ��� �÷��̾� �������� �̵� ��Ŵ-> �߻�
        rigid_lll.AddForce(Vector2.down * 8, ForceMode2D.Impulse);  // �޿޿��� �Ѿ��� �÷��̾� �������� �̵� ��Ŵ-> �߻�
        rigid_llll.AddForce(Vector2.down * 8, ForceMode2D.Impulse); // �޿޿޿��� �Ѿ��� �÷��̾� �������� �̵� ��Ŵ-> �߻�

        // ���� ī��Ʈ
        cur_pattern_count++;                                      // ���� ���� Ƚ���� 1 �ø���

        SoundManager.instance.PlaySound("Enemy Shot");            // ���� ��� ���� ���

        if (cur_pattern_count < max_pattern_count[pattern_index]) // ���� ���� Ƚ���� ������ ������ �ִ� ���� Ƚ���� �ѱ��� �ʾҴٸ�
        {
            Invoke("FireForward", 2);                             // �ش� ���� �ٽ� ����
        }

        else                                                      // �װ� �ƴ϶�� (���� ���� Ƚ���� �ִ� ���� Ƚ���� �Ѱ�ٸ�)
        {
            Invoke("Think", 2.5f);                                // ���� ������ ������
        }
    }

    void FireShot()
    {
        // �÷��̾� �������� ���� �߻�

        if (health <= 0) // ������ �׾��ٸ�
            return;      // �����Ű�� ����

        // ���� ��ġ�� B Ÿ�� �Ѿ��� ���� �������� 20�� ������
        for (int index = 0; index < 20; index++)
        {
            GameObject bullet = object_manager.MakeObj("enemy_bullet_b");
            bullet.transform.position = transform.position;

            // �Ѿ��� Rigidbody�� �����ͼ� AddForce()�� �Ѿ� �߻� ��� ����
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();           // �Ѿ��� Rigidbody2D�� ������

            Vector3 dir_vec = player.transform.position - transform.position; // ��ǥ�� (�÷��̾�)���� ���� (�÷��̾� ��ġ - �ڽ��� ��ġ)�� ������ �������� ��
            
            // ��ġ�� ��ġ�� �ʰ� ������ ���� ���� ���� �ڿ� �̵� ��Ŵ (x���� -1 ~ 1 ������ �� �� �ϳ�  /  y���� 0 ~ 3�� ������ �� �� �ϳ�)
            Vector3 ran_vec = new Vector2(Random.Range(-0.75f, 0.75f), Random.Range(0f, 3f));
            dir_vec += ran_vec;

            // ���� ���ͷ� ����� (dir_vec_r, dir_vec_l�� ���� ��ǥ ���̱� ������ �ظ��ϸ� 1�� �ѱ�� ����) -> normalized (���Ͱ� ���� ��(1)�� ��ȯ�� ����) ���
            rigid.AddForce(dir_vec.normalized * 6.5f, ForceMode2D.Impulse);   // �Ѿ��� �÷��̾� �������� �̵� ��Ŵ -> �߻�
        }

        // ���� ī��Ʈ
        cur_pattern_count++;                                      // ���� ���� Ƚ���� 1 �ø���

        SoundManager.instance.PlaySound("Enemy Shot");            // ���� ��� ���� ���

        if (cur_pattern_count < max_pattern_count[pattern_index]) // ���� ���� Ƚ���� ������ ������ �ִ� ���� Ƚ���� �ѱ��� �ʾҴٸ�
        {
            Invoke("FireShot", 1.5f);                             // �ش� ���� �ٽ� ����
        }

        else                                                      // �װ� �ƴ϶�� (���� ���� Ƚ���� �ִ� ���� Ƚ���� �Ѱ�ٸ�)
        {
            Invoke("Think", 2.5f);                                // ���� ������ ������
        }
    }

    void FireArc()
    {
        // ��ä ������� �Ѿ� �߻�

        if (health <= 0) // ������ �׾��ٸ�
            return;      // �����Ű�� ����

        // ���� ��ġ�� A Ÿ�� �Ѿ��� ���� �������� ������
        GameObject bullet = object_manager.MakeObj("enemy_bullet_a");

        // �Ѿ��� ��ġ �� ȸ�� �ʱ�ȭ
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        // �Ѿ��� Rigidbody�� �����ͼ� AddForce()�� �Ѿ� �߻� ��� ����
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();           // �Ѿ��� Rigidbody2D�� ������

        // Mathf.Sin() -> �ﰢ �Լ��� SIn (Mathf.Cos()�� ����ص� ���� ������ �ٸ� ������, �߻� ����� ����)  /  Mathf.PI -> ������ PI(��)  /  PI * ? -> �󸶳� ������ ������ �׸��� ����  /  cur_pattern_count / max_pattern_count[pattern_index] -> 0 ~ 1������ ���� ����� ����
        Vector3 dir_vec = new Vector2(Mathf.Sin( Mathf.PI * ran_num * cur_pattern_count / max_pattern_count[pattern_index] ), -1);

        // ���� ���ͷ� ����� (dir_vec_r, dir_vec_l�� ���� ��ǥ ���̱� ������ �ظ��ϸ� 1�� �ѱ�� ����) -> normalized (���Ͱ� ���� ��(1)�� ��ȯ�� ����) ���
        rigid.AddForce(dir_vec.normalized * 6.5f, ForceMode2D.Impulse);   // �Ѿ��� �÷��̾� �������� �̵� ��Ŵ -> �߻�

        // ���� ī��Ʈ
        cur_pattern_count++;                                      // ���� ���� Ƚ���� 1 �ø���

        SoundManager.instance.PlaySound("Enemy Shot");            // ���� ��� ���� ���

        if (cur_pattern_count < max_pattern_count[pattern_index]) // ���� ���� Ƚ���� ������ ������ �ִ� ���� Ƚ���� �ѱ��� �ʾҴٸ�
        {
            Invoke("FireArc", 0.15f);                             // �ش� ���� �ٽ� ����
        }

        else                                                      // �װ� �ƴ϶�� (���� ���� Ƚ���� �ִ� ���� Ƚ���� �Ѱ�ٸ�)
        {
            Invoke("Think", 2.5f);                                // ���� ������ ������
        }
    }

    void FireAround()
    {
        // �� ���·� �Ѿ� �߻�

        if (health <= 0) // ������ �׾��ٸ�
            return;      // �����Ű�� ����

        // ���� ��ġ�� A Ÿ�� �Ѿ��� ���� �������� ���� ������ ����ŭ ������
        int round_num_a = 50;
        int round_num_b = 37;

        int round_num = cur_pattern_count % 2 == 0 ? round_num_a : round_num_b;

        for (int index = 0; index < round_num; index++)
        {
            GameObject bullet = object_manager.MakeObj("boss_bullet_b");

            // �Ѿ��� ��ġ �� ȸ�� �ʱ�ȭ
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            // �Ѿ��� Rigidbody�� �����ͼ� AddForce()�� �Ѿ� �߻� ��� ����
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();           // �Ѿ��� Rigidbody2D�� ������

            // Mathf.Cos() -> �ﰢ �Լ��� Cos (Mathf.Sin()�� ����ص� ���� ������ �ٸ� ������, �߻� ����� ����)  /  Mathf.PI -> ������ PI(��)  /  PI * ? -> �󸶳� ������ ������ �׸��� ����  /  index / round_num -> 0 ~ 1������ ���� ����� ����
            Vector3 dir_vec = new Vector2(Mathf.Sin( Mathf.PI * 2 * index / round_num)
                                         ,Mathf.Cos( Mathf.PI * 2 * index / round_num) * (-1)); // x��� y�࿡ ���ϴ� ���� ���� Sin, Cos�� ���� �ٸ��� �����ؾ� �� (�׷��� ������ x��, y���� ��� �Ȱ��� ���� �Ǿ ���������� �Ѿ��� ���ư��� ����)

            // ���� ���ͷ� ����� (dir_vec_r, dir_vec_l�� ���� ��ǥ ���̱� ������ �ظ��ϸ� 1�� �ѱ�� ����) -> normalized (���Ͱ� ���� ��(1)�� ��ȯ�� ����) ���
            rigid.AddForce(dir_vec.normalized * 3, ForceMode2D.Impulse);   // �Ѿ��� �÷��̾� �������� �̵� ��Ŵ -> �߻�

            // �Ѿ˵��� �ڽ��� ������ �ٶ󺸰� ��
            Vector3 rot_vec = Vector3.forward * 360 * index / round_num;
            bullet.transform.Rotate(rot_vec);
        }

        // ���� ī��Ʈ
        cur_pattern_count++;                                      // ���� ���� Ƚ���� 1 �ø���

        SoundManager.instance.PlaySound("Enemy Shot");            // ���� ��� ���� ���

        if (cur_pattern_count < max_pattern_count[pattern_index]) // ���� ���� Ƚ���� ������ ������ �ִ� ���� Ƚ���� �ѱ��� �ʾҴٸ�
        {
            Invoke("FireAround", 0.7f);                           // �ش� ���� �ٽ� ����
        }

        else                                                      // �װ� �ƴ϶�� (���� ���� Ƚ���� �ִ� ���� Ƚ���� �Ѱ�ٸ�)
        {
            Invoke("Think", 2.5f);                                // ���� ������ ������
        }
    }

    void Spawn(int boss_spawn)
    {
        // ������ ���� ������

        if (health <= 0) // ������ �׾��ٸ�
            return;      // �����Ű�� ����

        // ���� ���
        SoundManager.instance.PlaySound("Spawning");

        // ������ �±� �ٲٱ� (�ǰ� ���� �ʱ� ����)
        gameObject.tag = "EnemySpawning";

        // ���� ���� �ٲٱ� (�ִϸ��̼����� �ص� ������ �� / �� �����Ҷ� ������ �������°� ����)
        game_manager.ChangeAc(anim, 1); // �ִϸ����͸� ������

        // ���� ��ȯ��
        game_manager.ReadBossFile(boss_spawn);

        // ��� ���� ��ȯ�ߴ��� �Ǵ�
        while (game_manager.cur_spawn_delay > game_manager.spawn_delay && !game_manager.spawn_end)
        {
            game_manager.SpawnEnemy();

            game_manager.cur_spawn_delay = 0;
        }

        Invoke("EndBossSpawn", 20f); // 20
    }

    void EndBossSpawn()
    {
        // ���� ���
        SoundManager.instance.PlaySound("End Spawning");

        // ������ �ٲ� ���� ������� �������� (�ִϸ��̼� ����)
        game_manager.ChangeAc(anim, 2); // �ִϸ����͸� ������

        Invoke("Think", 1.5f);
    }

    void Update() // �� �����Ӹ���
    {
        // ġƮŰ �κ�
        if (Input.GetKeyDown(KeyCode.G)) // G Ű�� ������
        {
            OnHit(1000);                 // ���� 1000�������� ���� -> ����
        }

        if (enemy_name == "Boss")            // �� �̸��� ������ �̿��߸� ������
        {
            if (Input.GetKeyDown(KeyCode.T)) // T Ű�� ������
            {
                cur_pattern_count = max_pattern_count[pattern_index];

                CancelInvoke();              // ��� Invoke() �Լ��� ���߰�

                Think();                     // �ٽ� ������
            }
        }

        // �ڵ� �κ�
        stage = game_manager.stage;

        // ���� ü�¿� ���� ������ ������ �ٸ��� ��
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

        // �Ϲ����� ������ ����
        Fire();   // �Ѿ� �߻�

        Reload(); // �Ѿ� ����

    }

    void Fire() // �Ѿ� �߻� �Լ�
    {
        // �߻� ��Ű�� �ʴ� ���� -> ������ �Ǿ� ���� ����
        if (cur_delay < reload_delay) // ���� ���� �ð��� ������ �ð����� �۴ٸ� (�������� �Ϸ�Ǿ� ���� �ʴٸ�) 
            return;                   // ���� ��Ű�� ����

        if(enemy_name == "A")       // ���� �̸��� A��� (���� Ÿ���� A Ÿ���̶��)
        {
            // ���� ��ġ�� A Ÿ�� �Ѿ��� ���� �������� ������
            GameObject bullet = object_manager.MakeObj("enemy_bullet_a");
            bullet.transform.position = transform.position;

            // �Ѿ��� Rigidbody�� �����ͼ� AddForce()�� �Ѿ� �߻� ��� ����
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                // �Ѿ��� Rigidbody2D�� ������

            Vector3 dir_vec = player.transform.position - transform.position;                      // ��ǥ�� (�÷��̾�)���� ���� (�÷��̾� ��ġ - �ڽ��� ��ġ)�� ������ �������� ��

            // ���� ���ͷ� ����� (dir_vec_r, dir_vec_l�� ���� ��ǥ ���̱� ������ �ظ��ϸ� 1�� �ѱ�� ����) -> normalized (���Ͱ� ���� ��(1)�� ��ȯ�� ����) ���
            rigid.AddForce(dir_vec.normalized * 3, ForceMode2D.Impulse);                           // �Ѿ��� �÷��̾� �������� �̵� ��Ŵ -> �߻�
        }

        else if (enemy_name == "C") // ���� �̸��� C��� (���� Ÿ���� C Ÿ���̶��)
        {
            // ���� �����ʿ� B Ÿ�� �Ѿ��� ���� �������� ������
            GameObject bullet_r = object_manager.MakeObj("enemy_bullet_b");
            bullet_r.transform.position = transform.position + Vector3.right * 0.3f;

            // ���� ���ʿ� B Ÿ�� �Ѿ��� ���� �������� ������
            GameObject bullet_l = object_manager.MakeObj("enemy_bullet_b");
            bullet_l.transform.position = transform.position + Vector3.left * 0.3f;

            // �Ѿ��� Rigidbody�� �����ͼ� AddForce()�� �Ѿ� �߻� ��� ����
            Rigidbody2D rigid_r = bullet_r.GetComponent<Rigidbody2D>();                                                     // ������ �Ѿ��� Rigidbody�� ������
            Rigidbody2D rigid_l = bullet_l.GetComponent<Rigidbody2D>();                                                     // ���� �Ѿ��� Rigidbody�� ������

            Vector3 dir_vec_r = player.transform.position - (transform.position + Vector3.right * 0.3f);                    // ��ǥ�� (�÷��̾�)���� ���� (�÷��̾� ��ġ - �ڽ��� ��ġ)�� ������ �������� ��
            Vector3 dir_vec_l = player.transform.position - (transform.position + Vector3.right * 0.3f);                    // ��ǥ�� (�÷��̾�)���� ���� (�÷��̾� ��ġ - �ڽ��� ��ġ)�� ������ �������� ��

            // ���� ���ͷ� ����� (dir_vec_r, dir_vec_l�� ���� ��ǥ ���̱� ������ �ظ��ϸ� 1�� �ѱ�� ����) -> normalized (���Ͱ� ���� ��(1)�� ��ȯ�� ����) ���
            rigid_r.AddForce(dir_vec_r.normalized * 4, ForceMode2D.Impulse);                                                // ������ �Ѿ��� �÷��̾� �������� �̵� ��Ŵ-> �߻�
            rigid_l.AddForce(dir_vec_l.normalized * 4, ForceMode2D.Impulse);                                                // ���� �Ѿ��� �÷��̾� �������� �̵� ��Ŵ-> �߻�
        }
    
        cur_delay = 0;                                                                                                      // ���� �ð� �ʱ�ȭ (�������� �ϰ� ����� ����)

        SoundManager.instance.PlaySound("Enemy Shot");                                                                      // ���� ��� ���� ���
    }

    void Reload() // ������ �Լ�
    {
        cur_delay += Time.deltaTime; // ���� ���� �ð��� �ð��� ������
    }

    public void OnHit(float dmg) // ���� �¾��� �� ������ �Լ�  /  �Ű������� �������� ����
    {
        if (health <= 0) // ���� �̹� ���� �����̸�
            return;      // ���� ��Ű�� ���� (�������� ������ ��������� �ʰ� �ϱ� ����)

        Player player_logic = player.GetComponent<Player>(); // �÷��̾��� ������Ʈ�� ������ �Ŀ�

        health -= dmg + player_logic.player_dmg;             // ���� ü���� �� ��������ŭ ����
        Debug.Log("health = " + health.ToString());

        SoundManager.instance.PlaySound("Enemy Hit");       // ���� �ǰ� ���ϴ� ���� ���

        if (enemy_name == "Boss")                // �� �̸��� �������
        {
            anim.SetTrigger("on_hit");           // �¾��� �� �����ϴ� �ִϸ��̼� ���� 
        }

        else                                     // �װ� �ƴ϶�� (�Ϲ����� �����̶��)
        {
            sprite_renderer.sprite = sprites[1]; // �ǰ� ���� ��������Ʈ�� �ٲ�

            Invoke("ReturnSprite", 0.1f);            // 0.1�� �ڿ� ��������Ʈ�� ������� �ٲ�
        }
          
        if (health <= 0)                         // ü���� 0���� �۰ų� ���� �� (�׾��� ��)
        {
            // ���� ���
            // �÷��̾��� ��ũ��Ʈ�� �����ͼ� ������ �ο���
            player_logic.score += enemy_score;   // �÷��̾� ��ũ��Ʈ�� �ִ� score ������ enemy_score�� ���� ����

            // ������ ����ϱ�
            int ran = enemy_name == "Boss" ? -1 : Random.Range(0, 13); // �� �̸��� Boss�̸� -1, �ƴϸ� 0 ~ 11 �� ������ �� (�� ������ ����� �������� ������ ������)

            // ������ ������ ���
            if (ran < 0)       // ran  = -1  /  ������ �׾��� ���� ����
            {
                Debug.Log("Not Item");
            }

            else if (ran < 5)  // ran = 0, 1, 2, 3, 4  /  Ȯ�� = �� 41.67%
            {
                // Coin�� ���� ��ġ�� �����
                GameObject item_coin = object_manager.MakeObj("item_coin");
                item_coin.transform.position = transform.position;

                Item item_coin_logic = item_coin.GetComponent<Item>();
                item_coin_logic.player = player;
            }

            else if (ran < 8)  // ran =  5, 6, 7  /  Ȯ�� = 30%
            {
                // Power�� ���� ��ġ�� �����
                GameObject item_power = object_manager.MakeObj("item_power");
                item_power.transform.position = transform.position;

                Item item_power_logic = item_power.GetComponent<Item>();
                item_power_logic.player = player;
            }

            else if (ran < 10) // ran = 8, 9  /  Ȯ�� = 20%
            {
                // Boom�� ���� ��ġ�� �����
                GameObject item_boom = object_manager.MakeObj("item_boom");
                item_boom.transform.position = transform.position;

                Item item_boom_logic = item_boom.GetComponent<Item>();
                item_boom_logic.player = player;
            }

            else if (ran < 12) // ran = 10, 11  /  Ȯ�� = 20%
            {
                GameObject item_follower = object_manager.MakeObj("item_follower");
                item_follower.transform.position = transform.position;

                Item item_follower_logic = item_follower.GetComponent<Item>();
                item_follower_logic.player = player;
            }

            else                // ran = 12  /  Ȯ�� = 
            {
                GameObject item_heal = object_manager.MakeObj("item_heal");
                item_heal.transform.position = transform.position;

                Item item_heal_logic = item_heal.GetComponent<Item>();
                item_heal_logic.player = player;
            }

            gameObject.SetActive(false); // ���� ��Ȱ��ȭ ��Ŵ

            // Quaternion.identity -> �⺻ ȸ���� -> 0
            transform.rotation = Quaternion.identity;

            game_manager.CallExplosion(transform.position, enemy_name); // ���� �̸��� ���� ũ��� ���� �� �ڸ����� ���� ��Ŵ

            if (enemy_name == "Boss")    // ������ �׿��� �� 
            {
                game_manager.StageEnd(); // ���� ���������� �Ѿ
            }
        }
    }

    void ReturnSprite() // ���� ��������Ʈ�� ������� �������� �Լ�
    {
        sprite_renderer.sprite = sprites[0]; // �ǰ� ���ϱ� ���� ��������Ʈ�� �ٲ�
    }

    void OnTriggerEnter2D(Collider2D collision) // ���� �ٸ� ������Ʈ�� �������� ��
    {
        if (collision.gameObject.tag == "BorderBullet" && enemy_name != "Boss") // ���� ������ ������Ʈ�� �±װ� "BorderBullet"�̶�� && ���� �̸��� "Boss"�� �ƴ϶��
        {
            gameObject.SetActive(false);                                        // ���� ��Ȱ��ȭ ��Ŵ
            transform.rotation = Quaternion.identity;
        }

        else if (collision.gameObject.tag == "PlayerBullet" && gameObject.tag != "EnemySpawning") // ���� ������ ������Ʈ�� �±װ� "PlayerBullet"�̶�� && ���� �±װ� "EnemySpawning"�� �ƴ϶��
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();                          // ���� ������ ������Ʈ���� Bullet ������Ʈ�� �ο��ϰ�
            OnHit(bullet.dmg);                                                                    // ���� �ǰݴ����� �� �����ϴ� �Լ��� ������

            collision.gameObject.SetActive(false);                                                // �Ѿ��� ��Ȱ��ȭ ��
        }
    }
}