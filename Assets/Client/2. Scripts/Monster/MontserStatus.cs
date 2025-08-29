using UnityEngine;

public class MontserStatus : MonoBehaviour
{
    private int cur_hp;
    private int cuf_dff;
    private int cur_atk = 5;

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
}