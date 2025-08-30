using UnityEngine;

public class PlayerStatus : MonoBehaviour, IDamageable
{
    private float max_hp = 100f;
    [SerializeField] private float cur_hp;

    private float Cur_hp
    {
        get
        {
            return cur_hp;
        }
        set
        {
            cur_hp = value;
            GameManager.Instance.PlayerHPChangeAlert(max_hp, cur_hp);
            Debug.Log($"플레이어 피격 {value} 데미지");
        }
    }

    private float cur_mp;
    private int cur_atk = 10;
    private float cur_dff;
    private float move_spd;
    private int LV;

    private PlayerController player_control;

    void Awake()
    {
        this.player_control = this.GetComponent<PlayerController>();
        this.Cur_hp = max_hp;
    }

    void OnEnable()
    {
        this.player_control.atk_act += Dealing;
    }

    void OnDisable()
    {
        this.player_control.atk_act -= Dealing;
    }

    public void HIT(int param_dmg)
    {
        this.Cur_hp -= param_dmg;
    }

    public void Dealing()
    {
        // 순간적인 콜라이더 탐지
        Vector2 center = this.transform.position + new Vector3(player_control.Dir.x > 0 ? 1 : -1 * 1.2f, 0.5f, 0f);
        Collider2D[] target_col = Physics2D.OverlapBoxAll(center, new Vector2(1.4f, 1.75f), 0f, 1 << 11);

        foreach (Collider2D element in target_col)
        {
            element.GetComponent<IDamageable>().HIT(cur_atk);
        }
    }
}
