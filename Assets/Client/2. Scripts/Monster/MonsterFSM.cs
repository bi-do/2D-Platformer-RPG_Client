using System;
using UnityEngine;

public class MonsterFSM : MonoBehaviour
{
    private enum MonsterFSMState
    {
        IDLE,
        PATROL,
        TRACE,
        ATTACK,
        HIT,
        DEATH
    }
    private MonsterFSMState fsm_type;

    private Animator anim;

    public event Action idle_act, patrol_act, trace_act, attack_act, hit_act, death_act;

    public Action[] act_arr = new Action[6];

    void Awake()
    {
        this.anim = this.GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        act_arr[0] = idle_act;
        act_arr[1] = patrol_act;
        act_arr[2] = trace_act;
        act_arr[3] = attack_act;
        act_arr[4] = hit_act;
        act_arr[5] = death_act;

        this.fsm_type = MonsterFSMState.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        this.act_arr[(int)this.fsm_type]?.Invoke();
    }

    private void Idle()
    {
        
    }

    private void Patrol()
    {

    }

    private void Trace()
    {

    }

    private void Attack()
    {

    }

    private void HIt()
    {

    }

    private void Death()
    {
        
    }
}
