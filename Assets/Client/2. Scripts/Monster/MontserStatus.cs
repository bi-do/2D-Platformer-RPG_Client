using System;
using UnityEngine;
using UnityEngine.UI;

public class MontserStatus : MonoBehaviour, IDamageable
{
    private float max_hp = 50f;
    [SerializeField] private float cur_hp;

    public float Cur_hp
    {
        get
        {
            return cur_hp;
        }
        private set
        {
            cur_hp = value;

            this.hp_UI.fillAmount = cur_hp / max_hp;
            if (value <= 0)
            {
                Death();
            }
        }
    }
    private int cuf_dff;
    private int cur_atk = 5;

    private bool isDead = false;

    MonsterController monster_control;
    [SerializeField] private Image hp_UI;

    public event Action<float, float> Hp_change_act;

    void Awake()
    {
        this.monster_control = this.GetComponent<MonsterController>();
    }

    void Start()
    {
        this.Cur_hp = max_hp;
    }

    void LateUpdate()
    {
        this.hp_UI.rectTransform.localScale = new Vector3(monster_control.Dir_x > 0 ? 1 : -1, 1, 1);
    }

    public void Dealing()
    {
        // 순간적인 콜라이더 탐지
        Vector2 center = this.transform.position + new Vector3(0, 0.9f, 0);
        Collider2D[] target_col = Physics2D.OverlapBoxAll(center, new Vector2(3.5f, 1.8f), 0f, 1 << 10);

        foreach (Collider2D element in target_col)
        {
            element.GetComponent<IDamageable>().HIT(cur_atk);
        }
    }

    public void HIT(int param_dmg)
    {
        if (isDead)
            return;

        Cur_hp -= param_dmg;
        monster_control.HitAlert();
    }

    private void Death()
    {
        this.isDead = true;
        monster_control.DeathAlert();
    }
}
