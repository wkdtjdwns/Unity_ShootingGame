using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // �÷��̾��� ����
    public float speed;          // �÷��̾��� �̵� �ӵ�
    public float power;          // �÷��̾��� �Ŀ�
    public int life;             // �÷��̾��� ü��
    public int player_dmg;       // �÷��̾��� ������

    public bool is_respawn_time; // ���� �÷��̾ ���� �ð����� ����

    // �÷��̾� �ȷο쿡 ���� ����
    public int follower_num;       // ���� �ȷο��� ����
    public GameObject[] followers; // �ȷο���� ������ �迭 ����

    // �ʻ�� ���� ����
    public int boom;       // ���� �����Ǿ� �ִ� �ʻ�� ����
    public int max_boom;   // �ִ�� �����ϰ� ���� �� �ִ� �ʻ�� ����

    // ���ӿ� ���� ����
    public int score;   // ����
    public bool is_hit; // �÷��̾ �¾Ҵ� �� ����

    // �Ѿ��� �߻��ϴ� ������ (������)
    public float cur_delay;    // ���� ���� �ð�
    public float reload_delay; // ������ �ð�

    // �÷��̾� �� ���
    public bool is_touch_top;
    public bool is_touch_bottom;
    public bool is_touch_right;
    public bool is_touch_left;

    // �Ѿ� ����
    public GameObject bullet_obj_a; // A Ÿ�� �Ѿ�
    public GameObject bullet_obj_b; // B Ÿ�� �Ѿ�

    // �ʻ�� ����
    public GameObject boom_effect;  // �ʻ�� �Ѿ�
    public bool is_boom;            // �ʻ�� ��� ����

    public GameManager game_manager;
    public ObjectManager object_manager;
    
    Animator anim;
    SpriteRenderer sprite_renderer;

    void Awake() // �������� ����
    {
        // �ʱ�ȭ ��
        anim = GetComponent<Animator>();
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        Unbeatable();

        Invoke("Unbeatable", 3);
    }
    
    void Unbeatable() // ���� ���� �ð����� �Ǵ��ϴ� �Լ� 
    {
        is_respawn_time = !is_respawn_time; // ����Ǹ� ���� �ð��� ���θ� ���ݰ� �ݴ�� ������ְ� �Ǵ���

        if (is_respawn_time) // ���� ���� �ð��̶��
        {
            sprite_renderer.color = new Color(1, 1, 1, 0.5f); // �÷��̾��� ������ ��¦ �����ϰ� �ٲ��ְ�

            // �ȷο��� �Ȱ��� �������
            for (int index = 0; index < followers.Length; index++)
            {
                followers[index].GetComponent<SpriteRenderer>().color = sprite_renderer.color = new Color(1, 1, 1, 0.5f);
            }
        }

        else                 // ���� ���� �ð��� �ƴ϶��
        {
            sprite_renderer.color = new Color(1, 1, 1, 1);   // �÷��̾��� ������ ������� ��������

            // �ȷο��� �Ȱ��� �������
            for (int index = 0; index < followers.Length; index++)
            {
                followers[index].GetComponent<SpriteRenderer>().color = sprite_renderer.color = new Color(1, 1, 1, 1);
            }
        }
    }

    void Update() // �� �����Ӹ���
    {
        Move();   // �÷��̾� �̵� (�ܹ����� Ű �Է��� Update()���� ����)

        Fire();   // �Ѿ� �߻� (�ܹ����� Ű �Է��� Update()���� ����)
        
        // �ʻ���� ȿ�� �ߵ�
        Boom();           // �ʻ�� ������Ʈ Ȱ��ȭ
        if (is_boom)      // �ʻ�⸦ �����ϰ� �ִٸ�
            BoomAffect(); // �ʻ���� ��� �Լ��� ȣ����

        Reload(); // �Ѿ� ���� (�ܹ����� Ű �Է��� Update()���� ����)
    }

    void Move() // �÷��̾� �̵� �Լ�
    {
        // �Է¹ޱ�
        float h = Input.GetAxisRaw("Horizontal"); // Ű �Է¿� ���� 1, 0, -1�� ���� ������ (����)
        if ((is_touch_right && h == 1) || (is_touch_left && h == -1)) // ������ ��� ������Ʈ�� ����� �� && ���������� �̵� �� �϶� || ���� ��� ������Ʈ�� ����� �� && �������� �̵� �� �϶�
            h = 0;

        float v = Input.GetAxisRaw("Vertical");   // Ű �Է¿� ���� 1, 0, -1�� ���� ������ (����)
        if ((is_touch_top && v == 1) || (is_touch_bottom && v == -1)) // ���� ��� ������Ʈ�� ����� �� && �������� �̵� �� �϶� || �Ʒ��� ��� ������Ʈ�� ����� �� && �Ʒ������� �̵� �� �϶�
            v = 0;

        // �Է� ���� ��� �̵���Ű��
        Vector3 cur_pos = transform.position;                                     // ���� ��ġ

        // Transform �̵��� Rigidbody �̵��� ����
        // Transform - ��ġ, ȸ��, ũ�⸦ ����
        //           - ������Ʈ ���� '�θ�-�ڽ�' ������ ���¸� ���� 
        //           - Scene�� �����ϴ� ������Ʈ�� �ʼ����� ��ҵ��� ���� -> Scene�� �����ϴ� ������Ʈ�� ������ Transform ������Ʈ�� ���ϰ� ����

        // ���� - Transform ������Ʈ�� Scene�� �����ϱ� ���� �ʼ����� ������Ʈ��!

        // Rigidbody - ������Ʈ�� ���� ����� �̵��ϰ� ��
        //           - ���� ��ũ�� �޾Ƽ� ������Ʈ�� ��������� �����̰� ��
        //           - Rigidbody�� ���Ե� ��� ������Ʈ�� �߷��� ������ �޾ƾ��� ���� ��ũ�������� ���� ������ ������ �����̰ų� ���� ������ ���� �ٸ� ������Ʈ�� ��ȣ �ۿ��ؾ���

        // ���� - RIgidbody ������Ʈ�� �������� ����� ���� ������Ʈ�� ��������� �����ϰ� ����� ������Ʈ��!

        // ������ - �������� �������� �������� �����ΰ�?
        //        - Transform �̵� -> ������ �̵��ϴ� ��ó�� ���� ��, ����� �ش� ��ġ�� �������� �����̵��ϴ� ����! (���� X)
        //        - Rigidbody �̵� -> ���� ��ġ�� ���� ��ġ�� ������ �̵��ϴ� �� (���� O)

        //  deltaTime -> ���� �������� �Ϸ�Ǵ� ������ �ɸ� �ð��� ����
        Vector3 next_pos = new Vector3(h, v, 0) * speed * Time.deltaTime;         // �� ������ �̵��� ��ġ  /  Transform �̵��̱� ������ ������ Time.deltaTime�� ��������� (�����ӿ� ��� ���� ���� �ӵ��� �̵��ϱ� ����)

        // �̵���Ű��
        transform.position = cur_pos + next_pos;                                  // �̵� �ӵ��� ����ؼ� �Է¹��� ��� �̵���

        // �ִϸ��̼� ����
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal")) // ���� �̵� Ű�� ������ �� || ���� �̵� Ű�� ���� ��
        {
            anim.SetInteger("input", (int)h);                                     // input �ִϸ����Ϳ� �ִ� �Ķ����(input)�� ���� h ������ ������(���� h�� ���� float���̱� ������ ����ȯ ������)  
        }
    }

    void Fire() // �Ѿ� �߻� �Լ�
    {
        // �߻� ��Ű�� �ʴ� ���� (�߻� ��ư�� ������ ����, ������ �Ǿ� ���� ����)
        if (!Input.GetButton("Fire1") || cur_delay < reload_delay) //  ������ ���� ���� �� (�߻� �ϰ� ���� ���� ��) || ���� ���� �ð��� ������ �ð����� �۴ٸ� (�������� �Ϸ�Ǿ� ���� �ʴٸ�) 
            return;                                                // ���� ��Ű�� ����

        switch (power)
        {
            case 1: // �÷��̾��� �Ŀ��� 1�� ��  /  �⺻ �Ѿ� 1�� �߻�

                // Instantiate() -> �Ű����� ������Ʈ�� ������ (Destroy()�� �ݴ�)  /  �Ű����� -> ������ ������Ʈ, ������ ��ġ, ������ ������Ʈ�� ����
                // GameObject bullet = Instantiate(bullet_obj_a, transform.position, transform.rotation); 
                
                // �÷��̾��� ��ġ�� A Ÿ�� �Ѿ��� �÷��̾��� �������� ������
                GameObject bullet = object_manager.MakeObj("player_bullet_a");
                bullet.transform.position = transform.position;

                // �Ѿ��� Rigidbody�� �����ͼ� AddForce()�� �Ѿ� �߻� ��� ����
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                // �Ѿ��� Rigidbody2D�� ������

                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                  // �Ѿ��� ���� �������� �̵� ��Ŵ (�÷��̾�� ������ �ٶ󺸰� �ֱ� ����) -> �߻�
                break;

            case 2: // �÷��̾��� �Ŀ��� 2�� ��  /  �⺻ �Ѿ� 2�� �߻�
                
                // �÷��̾��� �����ʿ� A Ÿ�� �Ѿ��� �÷��̾��� �������� ������
                GameObject bullet_r = object_manager.MakeObj("player_bullet_a");
                bullet_r.transform.position = transform.position + Vector3.right * 0.1f;
                
                // �÷��̾��� ���ʿ� A Ÿ�� �Ѿ��� �÷��̾��� �������� ������
                GameObject bullet_l = object_manager.MakeObj("player_bullet_a");
                bullet_l.transform.position = transform.position + Vector3.left * 0.1f;

                // �Ѿ��� Rigidbody�� �����ͼ� AddForce()�� �Ѿ� �߻� ��� ����
                Rigidbody2D rigid_r = bullet_r.GetComponent<Rigidbody2D>();                                                     // ������ �Ѿ��� Rigidbody�� ������
                Rigidbody2D rigid_l = bullet_l.GetComponent<Rigidbody2D>();                                                     // ���� �Ѿ��� Rigidbody�� ������

                rigid_r.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                         // ������ �Ѿ��� ���� �������� �̵� ��Ŵ (�÷��̾�� ������ �ٶ󺸰� �ֱ� ����) -> �߻�
                rigid_l.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                         // ���� �Ѿ��� ���� �������� �̵� ��Ŵ (�÷��̾�� ������ �ٶ󺸰� �ֱ� ����) -> �߻�
                break;

            default: // �÷��̾��� �Ŀ��� 3�̻��� ��  /  ��ȭ �Ѿ� 1���� �⺻ �Ѿ� 2�� �߻�

                // �÷��̾��� �����ʿ� A Ÿ�� �Ѿ��� �÷��̾��� �������� ������
                GameObject bullet_rr = object_manager.MakeObj("player_bullet_a");
                bullet_rr.transform.position = transform.position + Vector3.right * 0.3f;
                
                // �÷��̾��� ��ġ�� B Ÿ�� �Ѿ��� �÷��̾��� �������� ������
                GameObject bullet_cc = object_manager.MakeObj("player_bullet_b");
                bullet_cc.transform.position = transform.position;

                // �÷��̾��� ���ʿ� A Ÿ�� �Ѿ��� �÷��̾��� �������� ������ 
                GameObject bullet_ll = object_manager.MakeObj("player_bullet_a");
                bullet_ll.transform.position = transform.position + Vector3.left * 0.3f;

                // ��ȭ �Ѿ� �� �Ѿ��� Rigidbody�� �����ͼ� AddForce()�� �Ѿ� �߻� ��� ����
                Rigidbody2D rigid_rr = bullet_rr.GetComponent<Rigidbody2D>();                                                     // ������ �Ѿ��� Rigidbody�� ������
                Rigidbody2D rigid_cc = bullet_cc.GetComponent<Rigidbody2D>();                                                     // ��� �Ѿ��� Rigidbody�� ������
                Rigidbody2D rigid_ll = bullet_ll.GetComponent<Rigidbody2D>();                                                     // ���� �Ѿ��� Rigidbody�� ������

                rigid_rr.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                          // ������ �Ѿ��� ���� �������� �̵� ��Ŵ (�÷��̾�� ������ �ٶ󺸰� �ֱ� ����) -> �߻�
                rigid_cc.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                          // ��� �Ѿ��� ���� �������� �̵� ��Ŵ (�÷��̾�� ������ �ٶ󺸰� �ֱ� ����) -> �߻�
                rigid_ll.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                                          // ���� �Ѿ��� ���� �������� �̵� ��Ŵ (�÷��̾�� ������ �ٶ󺸰� �ֱ� ����) -> �߻�
                break;
        }

        cur_delay = 0;                                                                                                            // ���� �ð� �ʱ�ȭ (�������� �ϰ� ����� ����)

        SoundManager.instance.PlaySound("Shot");                                                                                  // ���� ��� ���� ���
    }

    void Reload() // ������ �Լ�
    {
        cur_delay += Time.deltaTime; // ���� ���� �ð��� �ð��� ������
    }

    void Boom() // �ʻ�� ȿ�� �Լ�
    {
        // ���� ��Ű�� �ʴ� ���� (�ʻ�� �߻� ��ư�� ������ ����, ���� �����Ǿ� �ִ� �ʻ�Ⱑ ����, ���� �ʻ�⸦ ����ϰ� ����)
        if (!Input.GetButtonUp("Fire2") || boom < 1 || is_boom) // ���콺 ��Ŭ���� ������ �ʾ��� �� (�߻� ���� �ʾ��� ��) || �����Ǿ� �ִ� �ʻ���� ������ 1���� ���� �� (�ʻ�Ⱑ �����Ǿ� ���� ���� ��) || ���� �ʻ�Ⱑ ���ǰ� ���� ��
            return;                                             // ���� ��Ű�� ����

        boom--;                            // ������ �ʻ�� �������� 1�� ����
        game_manager.UpdateBoomIcon(boom); // �ʻ�� UI�� ������Ʈ����

        // �ʻ�� ����Ʈ ���� �ڵ�
        OnBoomEffect();              // �ʻ�� ������Ʈ�� Ȱ��ȭ ��Ű�� 
        Invoke("OffBoomEffect", 4f); // 4�� �ڿ� ��Ȱ��ȭ ��Ŵ
                                     // �ʻ���� ����� Update() �Լ����� ������ (�ʻ�⸦ ���� ���̸� ��� �ʻ���� ȿ���� �ߵ��� �� �ְ� �ϱ� ����)
    }

    void OnBoomEffect() // �ʻ�� ������Ʈ�� Ȱ��ȭ ��Ű�� �Լ�
    {
        is_boom = true;
        boom_effect.SetActive(true);
    }

    void OffBoomEffect() // �ʻ�� ������Ʈ�� ��Ȱ��ȭ ��Ű�� �Լ�
    {
        is_boom = false;
        boom_effect.SetActive(false);
    }

    void BoomAffect() // �ʻ���� ��� �Լ�
    {
        // ��� �� �� ���� �Ѿ��� ����� �ڵ�

        // ��� �� ������Ʈ �޾ƿ���
        GameObject[] enemies_a = object_manager.GetPool("enemy_a");
        GameObject[] enemies_b = object_manager.GetPool("enemy_b");
        GameObject[] enemies_c = object_manager.GetPool("enemy_c");

        // ��� �� ������Ʈ �����
        for (int index = 0; index < enemies_a.Length; index++)              // �޾ƿ� ���� ������ŭ �ݺ�
        {
            if (enemies_a[index].activeSelf)
            {
                Enemy enemy_logic = enemies_a[index].GetComponent<Enemy>(); // �޾ƿ� �� ������ ��ũ��Ʈ�� ������

                enemy_logic.OnHit(1000);                                    // �޾ƿ� ��� ������ 1000�� �������� �� (�Ķ���� : ������)
            }
        }

        for (int index = 0; index < enemies_b.Length; index++)              // �޾ƿ� ���� ������ŭ �ݺ�
        {
            if (enemies_b[index].activeSelf)
            {
                Enemy enemy_logic = enemies_b[index].GetComponent<Enemy>(); // �޾ƿ� �� ������ ��ũ��Ʈ�� ������

                enemy_logic.OnHit(1000);                                    // �޾ƿ� ��� ������ 1000�� �������� �� (�Ķ���� : ������)
            }
        }

        for (int index = 0; index < enemies_c.Length; index++)              // �޾ƿ� ���� ������ŭ �ݺ�
        {
            if (enemies_c[index].activeSelf)
            {
                Enemy enemy_logic = enemies_c[index].GetComponent<Enemy>(); // �޾ƿ� �� ������ ��ũ��Ʈ�� ������

                enemy_logic.OnHit(1000);                                    // �޾ƿ� ��� ������ 1000�� �������� �� (�Ķ���� : ������)
            }
        }

        // ��� ���� �Ѿ� ������Ʈ �޾ƿ���
        GameObject[] bullets_a = object_manager.GetPool("enemy_bullet_a");
        GameObject[] bullets_b = object_manager.GetPool("enemy_bullet_b");

        // ��� ���� �Ѿ� ������Ʈ �����
        for (int index = 0; index < bullets_a.Length; index++) // �޾ƿ� ���� ������ŭ �ݺ�
        {
            if (bullets_a[index].activeSelf)
            {
                bullets_a[index].SetActive(false);             // �޾ƿ� ��� ���� �Ѿ��� ��Ȱ��ȭ ��Ŵ
            }
        }

        for (int index = 0; index < bullets_b.Length; index++) // �޾ƿ� ���� ������ŭ �ݺ�
        {
            if (bullets_b[index].activeSelf)
            {
                bullets_b[index].SetActive(false);             // �޾ƿ� ��� ���� �Ѿ��� ��Ȱ��ȭ ��Ŵ
            }
        }
    }

    public void OnDamaged() // �÷��̾ �������� �޴� �Լ� (ġƮŰ ��)
    {
        if (is_hit) // �÷��̾ �̹� ���� ���¶��
            return; // �������� ����

        is_hit = true;                                       // �÷��̾ ���� ���¶�� ���� �˷��ְ�

        life--;                                              // ������ -1 ���� �ڿ�
        game_manager.UpdateLifeIcon(life);                   // UI�� ������Ʈ ���ִ� �Լ��� �����ϰ�

        if (life < 1)                                        // ���� ������ 1���� �۴ٸ� (�÷��̾ �׾��ٸ�)
        {
            game_manager.GameOver();                         // ���� ���� ��Ű�� �Լ��� �����ϰ�
        }

        else                                                 // �װ� �ƴ϶�� (���� ������ 1 �̻��̶��)
        {
            game_manager.RespawnPlayer();                    // �÷��̾ ������ ��Ű�� �Լ��� �����ϰ�
        }

        game_manager.CallExplosion(transform.position, "P"); // ������ ����Ű�� �Լ� ���� ��

        gameObject.SetActive(false);                         // �÷��̾ ��� �� ���̰� ��
    }

    void OnTriggerEnter2D(Collider2D collision)   // �÷��̾ �ٸ� ������Ʈ�� �������� �� ��
    {
        // ���� ��踦 ���� ���ϰ� �ϱ�
        if (collision.gameObject.tag == "Border") // �÷��̾�� ������ ������Ʈ�� �±װ� "Border"�̶��
        {
            switch (collision.gameObject.name)    // �÷��̾�� ������ ������Ʈ�� �̸��� ����
            {
                // bool ������ ���� �ٲ���
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

        // ������ ��ų� ���� �Ѿ˿� ������ �ǰ� ���ϱ� 
        
        // �÷��̾�� ������ ������Ʈ�� �±װ� "Enemy"�̶�� || �÷��̾�� ������ ������Ʈ�� �±װ� "EnemyBullet"�̶�� || �÷��̾�� ������ ������Ʈ�� �±װ� "EnemySpawning"�̶��
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet" || collision.gameObject.tag == "Boss" || collision.gameObject.tag == "EnemySpawning")
        {
            if (is_hit || is_respawn_time) // �÷��̾ �̹� ���� ���¶�� || �÷��̾ ���� ���� �ð��̶��
                return;                    // �������� ����

            OnDamaged();

            if (collision.gameObject.tag != "Boss" && collision.gameObject.tag != "EnemySpawning" && collision.gameObject.tag != "EnemyBullet") // �÷��̾�� ������ ������Ʈ�� �±װ� "Boss"�� �ƴ϶�� && �÷��̾�� ������ ������Ʈ�� �±װ� "EnemySpawning"�� �ƴ϶�� && �÷��̾�� ������ ������Ʈ�� �±װ� "EnemyBullet"�� �ƴ϶��
            {
                Enemy enemy_logic = collision.GetComponent<Enemy>();                                                                            // �÷��̾�� ������ ������Ʈ�� ��ũ��Ʈ�� ������

                enemy_logic.OnHit(1000);                                                                                                        // �ش� ������Ʈ���� 1000�� �������� ��
            }   
        }

        // �÷��̾ ���� �����ۿ� ���� ȿ�� �����ϱ�
        else if (collision.gameObject.tag == "Item")               // �÷��̾�� ������ ������Ʈ�� �±װ� "Item"�̶��
        {
            Item item = collision.gameObject.GetComponent<Item>(); // �÷��̾�� ������ ������Ʈ (Item �±׸� ���� ������Ʈ) ���� Item ��ũ��Ʈ�� ������

            switch (item.type) 
            {
                case "Coin":                                          // ���� �������� Ÿ���� "Coin"�̶��
                    {
                        score += 1000;                               // ������ �÷��ְ�

                        SoundManager.instance.PlaySound("Get Coin"); // ������ ȹ�� �ϴ� ���� ���
                    }
                    break;

                case "Boom":                                 // ���� �������� Ÿ���� "Boom"(�ʻ��)�̶��
                    score += 250;                            // ������ �ɲ� �÷��ְ�

                    if (boom < max_boom)                     // ���� �����Ǿ� �ִ� �ʻ�� ������ �ִ� ���� �������� ������ 
                    {
                        SoundManager.instance.PlaySound("Get Boom"); // �ʻ�⸦ ȹ�� �ϴ� ���� ���

                        boom++;                              // 1�� �� �������ְ�

                        game_manager.UpdateBoomIcon(boom);   // �ʻ�� UI�� ������Ʈ ����
                    }

                    else                                     // �װ� �ƴ϶�� (�ִ� ���� �������� ũ�ų� ������)
                    {
                        score += 500;                        // ������ ���� �� �÷���

                        SoundManager.instance.PlaySound("Get Coin"); // ������ ȹ�� �ϴ� ���� ���
                    }
                    break;

                case "Power":                                     // ���� �������� Ÿ���� "Power"�̶��
                    score += 500;                                 // ������ ��¦ �÷��ְ�

                    SoundManager.instance.PlaySound("Get Power"); // ȸ�� �ϴ� ���� ��� �� 

                    if (power < 3)                                // power�� 3���� ������ 
                    {
                        power++;                                  // power�� 1�� �����ְ�
                    }
                 
                    else                                          // �װ� �ƴ϶�� (3���� ũ�ų� ������)
                    {
                        player_dmg++;                             // player_dmg ������ 1�� ������
                    }
                    break;


                case "Follower":                                 // ���� �������� Ÿ���� "Follower"�̶��
                    score += 100;                                // ������ ��� �÷��ְ�

                    if (follower_num < 3)                        // �ȷο��� ������ 3���� �۴ٸ�
                    {
                        SoundManager.instance.PlaySound("Get Follower"); // �ȷο��� ȹ�� �ϴ� ���� ���

                        followers[follower_num].SetActive(true); // follower_num�� ���� �ȷο��� �����ϰ�

                        follower_num++;                          // �ȷο��� ������ 1 ������
                    }

                    else                                         // �װ� �ƴ϶�� (3���� ũ�ų� ������)
                    {
                        score += 500;                            // ������ ���� �� �÷���

                        SoundManager.instance.PlaySound("Get Coin"); // ������ ȹ�� �ϴ� ���� ���
                    }
                    break;


                case "Heal":                                     // ���� �������� Ÿ���� "Heal"�̶��
                    if (life < 3)                                // ������ ������ 3���� �۴ٸ�
                    {
                        SoundManager.instance.PlaySound("Heal"); // ȸ�� �ϴ� ���� ���

                        life++;                                  // ������ ������ 1 ������

                        game_manager.UpdateLifeIcon(life);       // ���� UI�� ������Ʈ ����
                    }

                    else                                         // �װ� �ƴ϶�� (3���� ũ�ų� ������)
                    {
                        score += 250;                            // ������ ���� �÷���

                        SoundManager.instance.PlaySound("Get Coin"); // ������ ȹ�� �ϴ� ���� ���
                    }
                    break;
            }

            collision.gameObject.SetActive(false); // � �������� �Ծ��� �������� ȿ���� �����ϰ� ���� �������� ������Ŵ
        }
    }

    void OnTriggerExit2D(Collider2D collision)    // �÷��̾�� �ٸ� ������Ʈ�� ������ ���¿��ٰ� ������ ��
    {
        if (collision.gameObject.tag == "Border") // �÷��̾�� ������ ���¿��� ������Ʈ�� �±װ�  "Border"�̶��
        {
            switch (collision.gameObject.name)    // �÷��̾�� ���� ������Ʈ�� �̸��� ����
            {
                // bool ������ ���� �ٲ���
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
