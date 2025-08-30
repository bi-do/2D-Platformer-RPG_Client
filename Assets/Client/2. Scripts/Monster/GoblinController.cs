using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GoblinController : MonsterController
{
    private string anim_bool_Run = "Run";
    private string anim_trigger_Attack = "Attack";
    private string anim_trigger_Hit = "Hit";
    private string anim_trigger_Death = "Death";

    protected override void Awake()
    {
        base.Awake();

        // 나중에 SO로 초기화 할 예정
        this.move_spd = 3f;
        this.trace_dist = 5f;
        this.atk_dist = 2f;
    }

    protected override void Attack()
    {
        if (!isAttack)
        {
            this.isAttack = true;
            StartCoroutine(AnimRoutine(this.anim_trigger_Attack, "Base Layer.Goblin_Attack", MonsterFSMState.TRACE, () => this.isAttack = false));
        }
    }

    protected override void Death()
    {
        this.anim.SetTrigger(anim_trigger_Death);
        Destroy(this.gameObject, 5f);
    }

    protected override void HIt()
    {
        if (!isHit)
        {
            this.isHit = true;
            StartCoroutine(AnimRoutine(this.anim_trigger_Hit, "Base Layer.Goblin_Hit", MonsterFSMState.TRACE, () => this.isHit = false));
        }
    }

    protected override void Idle()
    {
        // 첫 호출
        if (this.cur_timer == 0)
        {
            this.ran_timer = Random.Range(1, 4);
            this.anim.SetBool(anim_bool_Run, false);
            this.isMove = false;
        }

        this.cur_timer += Time.deltaTime;
        if (this.cur_timer >= this.ran_timer)
        {
            ChangeState(MonsterFSMState.PATROL);
        }
        if (CheckEnemy())
        {
            ChangeState(MonsterFSMState.TRACE);
        }

    }

    protected override void Patrol()
    {
        // 첫 호출
        if (this.cur_timer == 0)
        {
            this.ran_timer = Random.Range(1, 4);
            this.Dir_x = Random.Range(-1, 1) < 0 ? -1f : 1f;
            this.isMove = true;

            this.transform.localScale = new Vector3(this.Dir_x, 1, 1);
            this.anim.SetBool(anim_bool_Run, true);
        }

        this.cur_timer += Time.deltaTime;

        if (this.cur_timer >= this.ran_timer)
        {
            ChangeState(MonsterFSMState.IDLE);
        }
        if (CheckEnemy())
        {
            ChangeState(MonsterFSMState.TRACE);
        }
    }

    protected override void Trace()
    {
        if (this.cur_timer == 0)
            this.isMove = true;

        this.Dir_x = (target_tf.position - this.transform.position).x > 0 ? 1 : -1;
        this.transform.localScale = new Vector3(this.Dir_x, 1, 1);

        this.anim.SetBool(anim_bool_Run, true);

        float dist = (target_tf.position - this.transform.position).magnitude;

        if (dist <= this.atk_dist)
        {
            ChangeState(MonsterFSMState.ATTACK);
        }
        else if (dist > this.trace_dist)
        {
            ChangeState(MonsterFSMState.IDLE);
        }
    }


}
