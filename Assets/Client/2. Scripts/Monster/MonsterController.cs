using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class MonsterController : MonoBehaviour
{
    protected enum MonsterFSMState
    {
        IDLE,
        PATROL,
        TRACE,
        ATTACK,
        HIT,
        DEATH
    }
    [SerializeField] protected MonsterFSMState Fsm_type { get; private set; }

    public float Dir_x { get; protected set; }

    protected float move_spd;

    protected bool isMove = false;
    protected bool isAttack = false;
    protected bool isHit = false;
    protected bool isDead = false;

    /// <summary> 추적할 거리 </summary>
    protected float trace_dist;

    /// <summary> 공격 범위 </summary>
    protected float atk_dist;

    /// <summary> 상태 관리용 타이머 </summary>
    protected float cur_timer;

    /// <summary> 랜덤 상태 유지 시간 </summary>
    protected float ran_timer;

    /// <summary> 플레이어 추적 트랜스폼 ( 멀티 플레이 시 리스트로 변경 ) </summary>
    protected Transform target_tf;

    protected Animator anim;
    protected Rigidbody2D rb;

    /// <summary> 상태 룩업 배열 </summary>
    public Action[] act_arr = new Action[5];

    protected virtual void Awake()
    {
        this.target_tf = GameObject.FindGameObjectWithTag("Player").transform;
        this.anim = this.transform.GetComponent<Animator>();
        this.rb = this.GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        act_arr[0] = Idle;
        act_arr[1] = Patrol;
        act_arr[2] = Trace;
        act_arr[3] = Attack;
        act_arr[4] = HIt;

        this.Fsm_type = MonsterFSMState.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
            this.act_arr[(int)this.Fsm_type]?.Invoke();
    }

    void FixedUpdate()
    {
        if (this.isMove)
            this.rb.linearVelocityX = this.move_spd * this.Dir_x;
    }

    protected void ChangeState(MonsterFSMState state)
    {
        if (this.Fsm_type != state)
        {
            this.isMove = false;
            this.rb.linearVelocityX = 0;

            // 타이머 초기화
            this.cur_timer = 0;
            this.ran_timer = 0;

            this.Fsm_type = state;
            Debug.Log($"상태변환 : {state}");
        }
    }

    protected abstract void Idle();

    protected abstract void Patrol();

    protected abstract void Trace();

    protected abstract void Attack();

    protected abstract void HIt();

    protected abstract void Death();

    public void HitAlert()
    {
        if (this.Fsm_type != MonsterFSMState.ATTACK)
            ChangeState(MonsterFSMState.HIT);
    }

    public void DeathAlert()
    {
        this.isDead = true;
        Death();
    }

    protected bool CheckEnemy()
    {
        Vector3 my_pos = this.transform.position;

        Vector2 my_dir = this.transform.right * this.Dir_x;

        float dist = (target_tf.position - my_pos).magnitude;

        float fov = Vector2.Dot((target_tf.position - my_pos).normalized, my_dir);

        // 적과의 거리 확인 + 층고 확인 + 적이 시야에 들어왔느냐 ( 보고있는 방향에 적 출현 )
        if (this.trace_dist < dist || target_tf.position.y < my_pos.y - 0.5f || target_tf.position.y > my_pos.y + 2f || fov < 0)
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    /// <summary>
    /// 애니메이션 트리거 활성화 및 해당 애니메이션이 끝날 때까지 대기 
    /// ( 실행할 애니메이션 트리거 , 체크할 애니메이션 상태 , 대기 후 바꿀 몬스터 상태 , 대기 후 실행할 콜백 )
    /// </summary>
    protected IEnumerator AnimRoutine(string trigger_str, string check_str, MonsterFSMState state, Action end_callback = null)
    {
        this.isMove = false;

        this.anim.SetTrigger(trigger_str);
        yield return null;

        yield return new WaitUntil(() =>
        {
            return !anim.IsInTransition(0) // 현재 전환중인가?
            && this.anim.GetCurrentAnimatorStateInfo(0).IsName(check_str); // 현재 진행되는 애니메이션이 Attack이 맞는가?

        });

        // 현재 애니메이션 클립이 끝날 때까지 대기
        yield return new WaitUntil(() =>
        {
            return this.anim.GetCurrentAnimatorStateInfo(0).IsName(check_str) &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f;
        });

        ChangeState(state);

        // 끝날 시점에 콜백
        end_callback?.Invoke();
    }

}
