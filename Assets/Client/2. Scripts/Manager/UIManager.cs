using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image player_hp_bar;

    void OnEnable()
    {
        GameManager.Instance.player_hp_change_act += HpBarUpdate;
    }

    void OnDisable()
    {
        GameManager.Instance.player_hp_change_act -= HpBarUpdate;
    }

    void HpBarUpdate(float max_hp, float cur_hp)
    {
        player_hp_bar.fillAmount = cur_hp / max_hp;
    }

}
