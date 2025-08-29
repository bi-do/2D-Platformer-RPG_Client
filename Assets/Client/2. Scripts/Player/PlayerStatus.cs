using UnityEngine;

public class PlayerStatus : MonoBehaviour, IDamageable
{
    [SerializeField] private float cur_hp = 100f;

    private float Cur_hp
    {
        get
        {
            return cur_hp;
        }
        set
        {
            cur_hp = value;
            Debug.Log($"플레이어 피격 {value} 데미지");
        }
    }

    private float cur_mp;
    private float cur_atk;
    private float cur_dff;
    private float move_spd;

    [SerializeField] private PlayerController player_control;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HIT(int param_dmg)
    {
        this.Cur_hp -= param_dmg;
    }
}
