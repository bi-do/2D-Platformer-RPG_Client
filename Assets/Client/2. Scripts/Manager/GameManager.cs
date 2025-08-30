using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    /// <summary> 플레이어 HP 변경 시 Alert </summary>
    public event Action<float, float> player_hp_change_act;

    public void PlayerHPChangeAlert(float max_hp, float cur_hp)
    {
        player_hp_change_act?.Invoke(max_hp, cur_hp);
    }

}
