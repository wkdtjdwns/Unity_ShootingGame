using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    // ������Ʈ Ǯ�� : �̸� �����ص� Ǯ���� Ȱ��ȭ/��Ȱ��ȭ�� ���
    // ���ӿ��� ����� ������ Instantiate()�� Destroy() �Լ��� ����, �� ������ �ϸ鼭 �ʿ���� ������ �޸𸮸� ������ (�������� ������ ���� ���ϰ� �ɸ�)
    // -> �׷��� �������÷�Ʈ(GC)�� �����  /  GC : ���� ������ �޸𸮸� ���� ���

    // ��Ÿ �ٸ� ���ӵ��� ù �ε� �ð��鵵 ��� ��ġ �� ������Ʈ Ǯ ���� ������ �ʿ���!

    // ������ ���� ������Ʈ�� �� ���̱� ������ public���� �������� ����

    // �������� �����Ͽ� ������ �迭��
    // ������ ������ �迭
    GameObject[] enemy_a;
    GameObject[] enemy_b;
    GameObject[] enemy_c;
    GameObject[] enemy_boss;

    // �����۵��� ������ �迭
    GameObject[] item_boom;
    GameObject[] item_coin;
    GameObject[] item_power;
    GameObject[] item_follower;
    GameObject[] item_heal;

    // �÷��̾��� �Ѿ��� ������ �迭
    GameObject[] player_bullet_a;
    GameObject[] player_bullet_b;

    // �ȷο��� �Ѿ��� ������ �迭
    GameObject[] follower_bullet;

    // ���� �Ѿ��� ������ �迭
    GameObject[] enemy_bullet_a;
    GameObject[] enemy_bullet_b;

    // ������ �Ѿ��� ������ �迭
    GameObject[] boss_bullet_a;
    GameObject[] boss_bullet_b;

    // ������Ʈ ���� ����Ʈ�� ������ �迭
    GameObject[] explosion;

    // ������ ������
    // �� ������ ����
    public GameObject enemy_a_prefab;
    public GameObject enemy_b_prefab;
    public GameObject enemy_c_prefab;
    public GameObject enemy_boss_prefab;

    // ������ ������ ����
    public GameObject item_boom_prefab;
    public GameObject item_coin_prefab;
    public GameObject item_power_prefab;
    public GameObject item_follower_prefab;
    public GameObject item_heal_prefab;

    // �÷��̾��� �Ѿ� ������ ����
    public GameObject player_bullet_a_prefab;
    public GameObject player_bullet_b_prefab;

    // �ȷο��� �Ѿ� ������ ����
    public GameObject follower_bullet_prefab;

    // ���� �Ѿ� ������ ����
    public GameObject enemy_bullet_a_prefab;
    public GameObject enemy_bullet_b_prefab;

    // ������ �Ѿ� ������ ����
    public GameObject boss_bullet_a_prefab;
    public GameObject boss_bullet_b_prefab;

    // ������Ʈ ���� ����Ʈ ������ ����
    public GameObject explosion_prefab;

    // �ڵ� �۾��� ���� �� ���� �� �� �ְ� �ϴ� �迭 ����
    GameObject[] target_pool;

void Awake() // �������� ����
    {
        // ȭ�鿡 �ѹ��� ������ ������ ����ؼ� �迭�� ���̸� �Ҵ�����
        // ������ ������ �迭 ���� �Ҵ�
        enemy_a = new GameObject[35];
        enemy_b = new GameObject[35];
        enemy_c = new GameObject[35];
        enemy_boss = new GameObject[1];

        // �����۵��� ������ �迭 ���� �Ҵ�
        item_boom = new GameObject[10];
        item_coin = new GameObject[20];
        item_power = new GameObject[10];
        item_follower = new GameObject[10];
        item_heal = new GameObject[10];

        // �÷��̾��� �Ѿ��� ������ �迭 ���� �Ҵ�
        player_bullet_a = new GameObject[100];
        player_bullet_b = new GameObject[100];
        
        // �ȷο��� �Ѿ��� ������ �迭 ���� �Ҵ�
        follower_bullet = new GameObject[100];

        // ���� �Ѿ��� ������ �迭 ���� �Ҵ�
        enemy_bullet_a = new GameObject[100];
        enemy_bullet_b  = new GameObject[100];

        // ������ �Ѿ��� ������ �迭 ���� �Ҵ�
        boss_bullet_a = new GameObject[50];
        boss_bullet_b = new GameObject[1000];

        // ������Ʈ ���� ����Ʈ�� ������ �迭 ���� �Ҵ�
        explosion = new GameObject[50];

        Generate();
    }

    void Generate()
    {
        // ������ �����ϰ� �迭�� ������
        for (int index = 0; index < enemy_a.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            enemy_a[index] = Instantiate(enemy_a_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            enemy_a[index].SetActive(false); 
        }

        for (int index = 0; index < enemy_b.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            enemy_b[index] = Instantiate(enemy_b_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            enemy_b[index].SetActive(false);
        }

        for (int index = 0; index < enemy_c.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            enemy_c[index] = Instantiate(enemy_c_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            enemy_c[index].SetActive(false);
        }

        for (int index = 0; index < enemy_boss.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            enemy_boss[index] = Instantiate(enemy_boss_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            enemy_boss[index].SetActive(false);
        }

        // �����۵��� �����ϰ� �迭�� ������
        for (int index = 0; index < item_boom.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            item_boom[index] = Instantiate(item_boom_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            item_boom[index].SetActive(false);
        }

        for (int index = 0; index < item_coin.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            item_coin[index] = Instantiate(item_coin_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            item_coin[index].SetActive(false);
        }

        for (int index = 0; index < item_power.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            item_power[index] = Instantiate(item_power_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            item_power[index].SetActive(false);
        }

        for (int index = 0; index < item_follower.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            item_follower[index] = Instantiate(item_follower_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            item_follower[index].SetActive(false);
        }

        for (int index = 0; index < item_heal.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            item_heal[index] = Instantiate(item_heal_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            item_heal[index].SetActive(false);
        }

        // �÷��̾��� �Ѿ��� �����ϰ� �迭�� ������
        for (int index = 0; index < player_bullet_a.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            player_bullet_a[index] = Instantiate(player_bullet_a_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            player_bullet_a[index].SetActive(false);
        }

        for (int index = 0; index < player_bullet_b.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            player_bullet_b[index] = Instantiate(player_bullet_b_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            player_bullet_b[index].SetActive(false);
        }
        
        // �ȷο��� �Ѿ��� �����ϰ� �迭�� ������
        for (int index = 0; index < follower_bullet.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            follower_bullet[index] = Instantiate(follower_bullet_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            follower_bullet[index].SetActive(false);
        }

        // ���� �Ѿ��� �����ϰ� �迭�� ������
        for (int index = 0; index < enemy_bullet_a.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            enemy_bullet_a[index] = Instantiate(enemy_bullet_a_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            enemy_bullet_a[index].SetActive(false);
        }

        for (int index = 0; index < enemy_bullet_b.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            enemy_bullet_b[index] = Instantiate(enemy_bullet_b_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            enemy_bullet_b[index].SetActive(false);
        }

        for (int index = 0; index < boss_bullet_a.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            boss_bullet_a[index] = Instantiate(boss_bullet_a_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            boss_bullet_a[index].SetActive(false);
        }

        for (int index = 0; index < boss_bullet_b.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            boss_bullet_b[index] = Instantiate(boss_bullet_b_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            boss_bullet_b[index].SetActive(false);
        }

        // ������Ʈ ���� ����Ʈ�� �����ϰ� �迭�� ������
        for (int index = 0; index < explosion.Length; index++)
        {
            // Instantiate() �� ������ �������� �迭�� ������
            explosion[index] = Instantiate(explosion_prefab);

            // Instantiate() �� ������ �ڿ��� �ٷ� ��Ȱ��ȭ ������ (ȭ�鿡 ��� ������Ʈ�� ���� ����� �ȵǱ� ����)
            explosion[index].SetActive(false);
        }
    }
    public GameObject MakeObj(string type) // ������Ʈ���� ����� �Լ�  /  ��ȯ�� -> GameObject
    {
        // ���� ������Ʈ�� ���� ������ ���� 
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

        // ������ ������ ������Ʈ�� ������
        for (int index = 0; index < target_pool.Length; index++) // ������ ������
        {
            // activeSelf -> ���� ������Ʈ�� ����
            if (!target_pool[index].activeSelf)     // ���� ������ ������Ʈ�� Ȱ��ȭ �Ǿ����� �ʴٸ�
            {
                target_pool[index].SetActive(true); // ������ ������Ʈ�� Ȱ��ȭ �����ְ�
                return target_pool[index];          // ������ ������Ʈ�� ��ȯ��
            }
        }

        // �ƹ��͵� �������� �ʾҴٸ�
        return null; // null ���� ��ȯ��
    }

    public GameObject[] GetPool(string type) // ������ ������Ʈ�� Ǯ�� �������� �Լ�  / ��ȯ�� -> GameObject �迭
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
