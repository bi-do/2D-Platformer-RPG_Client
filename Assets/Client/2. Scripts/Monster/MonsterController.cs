using System;
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
    protected MonsterFSMState Fsm_type { get; private set; }

    protected float dir_x;

    protected float move_spd;

    protected bool isMove = false;
    protected bool isAttack = false;

    /// <summary> 추적할 거리 </summary>
    protected float trace_dist;

    /// <summary> 공격 범위 </summary>
    protected float atk_dist;

    /// <summary> 상태 관리용 타이머 </summary>
    protected float cur_timer;

    /// <summary> 랜덤 상태 유지 시간 </summary>
    protected float ran_timer;
    //  추적용 트랜스폼
    protected Transform target_tf;



    protected Animator anim;
    protected Rigidbody2D rb;

    /// <summary> 플레이어가 몬스터에게 피격 시 호출 </summary>
    public Action attack_act;

    /// <summary> 상태 룩업 배열 </summary>
    public Action[] act_arr = new Action[6];

    protected virtual void Awake()
    {
        this.target_tf = GameObject.FindGameObjectWithTag("Player").transform;
        this.anim = this.GetComponent<Animator>();
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
        act_arr[5] = Death;

        this.Fsm_type = MonsterFSMState.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        this.act_arr[(int)this.Fsm_type]?.Invoke();
    }

    void FixedUpdate()
    {
        if (this.isMove)
            this.rb.linearVelocityX = this.move_spd * this.dir_x;
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

    protected bool CheckEnemy()
    {
        Vector3 my_pos = this.transform.position;

        Vector2 my_dir = this.transform.right * this.dir_x;

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

}
