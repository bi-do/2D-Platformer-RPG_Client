using System.Collections;
using System.Threading;
using UnityEngine;

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
            StartCoroutine(AttackRoutine());
    }

    protected override void Death()
    {

    }

    protected override void HIt()
    {

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
            this.dir_x = Random.Range(-1, 1) < 0 ? -1f : 1f;
            this.isMove = true;

            this.transform.localScale = new Vector3(this.dir_x, 1, 1);
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

        this.dir_x = (target_tf.position - this.transform.position).x > 0 ? 1 : -1;
        this.transform.localScale = new Vector3(this.dir_x, 1, 1);

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

    IEnumerator AttackRoutine()
    {
        this.isAttack = true;
        this.isMove = false;

        this.anim.SetTrigger(anim_trigger_Attack);
        yield return null;

        yield return new WaitUntil(() =>
        {
            return !anim.IsInTransition(0) // 현재 전환중인가?
            && this.anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Goblin_Attack"); // 현재 진행되는 애니메이션이 Attack이 맞는가?

        });

        // 현재 애니메이션 클립이 끝날 때까지 대기
        yield return new WaitUntil(() =>
        {
            return this.anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Goblin_Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f;
        });

        ChangeState(MonsterFSMState.TRACE);
        this.isAttack = false;
    }
}