using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jump_power = 16f;
    [SerializeField] private float double_jump_power_x = 13f;
    [SerializeField] private float double_jump_power_y = 15f;

    private float move_spd = 4f;
    public Vector2 Dir { get; private set; }

    /// <summary> 플레이어의 X축 속도 </summary>
    private float velo_x;

    /// <summary> 더블 점프 카운트 상수 </summary>
    [SerializeField] private const float double_jump_value = 1;

    /// <summary> 더블 점프 카운트 </summary>
    private float double_jump_cnt;

    private Rigidbody2D rb;
    private PlayerInput player_input;
    private Animator anim;

    private InputAction move_act;
    private InputAction jump_act;
    private InputAction skile_1_act;

    /// <summary> 공격 시 호출 </summary>
    public event Action atk_act;

    private bool isGround = false;

    /// <summary> true = 방향키로 속도 고정 제어 , false = 방향키로 속도 제어 불가  </summary>
    private bool isMove = false;

    private bool isAttack = false;

    private string anim_float_Input_X = "Input_X";
    private string anim_bool_IsGround = "IsGround";

    /// <summary> 스킬 이펙트 컨테이너 </summary>
    [SerializeField] private GameObject[] effect_arr;
    void Awake()
    {
        this.player_input = this.GetComponent<PlayerInput>();
        this.rb = this.GetComponent<Rigidbody2D>();
        this.anim = this.GetComponent<Animator>();

        this.double_jump_cnt = double_jump_value;

        this.move_act = this.player_input.actions.FindAction("Player/Move");
        this.jump_act = this.player_input.actions.FindAction("Player/Jump");
        this.skile_1_act = this.player_input.actions.FindAction("Player/Skile");


        this.Dir = new Vector2(1, 0);
    }

    void OnEnable()
    {
        move_act.Enable();
        move_act.performed += Move;
        move_act.canceled += MoveCancel;

        jump_act.Enable();
        jump_act.performed += Jump;

        skile_1_act.Enable();
        skile_1_act.performed += Skill;
    }

    void OnDisable()
    {
        move_act.Disable();
        move_act.performed -= Move;
        move_act.canceled -= MoveCancel;

        jump_act.Disable();
        jump_act.performed -= Jump;

        skile_1_act.Disable();
        skile_1_act.performed -= Skill;
    }


    void FixedUpdate()
    {
        if (this.isMove && isGround && !isAttack)
            this.rb.linearVelocityX = velo_x;
    }

    private void Move(InputAction.CallbackContext context)
    {
        // 속도 변환 배제
        this.isMove = true;

        this.anim.SetFloat(this.anim_float_Input_X, 1);

        this.Dir = context.ReadValue<Vector2>();

        if (!isAttack)
            this.transform.localScale = new Vector3(this.Dir.x > 0 ? 1 : -1, 1, 1);

        // 속도 변경
        this.velo_x = Dir.x * this.move_spd;
    }

    private void MoveCancel(InputAction.CallbackContext context)
    {
        // 속도 변환 허용
        this.isMove = false;

        this.anim.SetFloat(this.anim_float_Input_X, 0);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGround)
        {
            this.rb.AddForceY(this.jump_power, ForceMode2D.Impulse);
            // Debug.Log("점프 호출");
        }
        else if (double_jump_cnt > 0)
        {
            this.double_jump_cnt--;

            float dir_x = this.Dir.x > 0 ? 1 : -1;

            Vector2 double_jump_power = new Vector2(dir_x * this.double_jump_power_x, this.double_jump_power_y);

            JumpVFX();

            this.rb.AddForce(double_jump_power, ForceMode2D.Impulse);
        }

    }

    private void Skill(InputAction.CallbackContext context)
    {
        if (!isAttack)
            StartCoroutine(AttackRoutine(0.5f));
    }

    void JumpVFX()
    {
        // VFX 생성
        Vector3 offset = new Vector3(1.1f * -this.Dir.x, 0.25f, 0);

        GameObject vfx = Instantiate(this.effect_arr[0], this.transform.position + offset, Quaternion.identity);
        GameObject child = vfx.transform.GetChild(0).gameObject;

        Vector3 child_localScale = child.transform.localScale;

        child.transform.localScale = new Vector3(child_localScale.x * this.Dir.x, child_localScale.y, child_localScale.z);
    }

    void Skill_1_VFX()
    {
        Vector2 offset = new Vector2(0.8f, 0.6f);

        GameObject vfx = Instantiate(this.effect_arr[1], this.transform);
        vfx.transform.localPosition = offset;

        GameObject child = vfx.transform.GetChild(0).gameObject;

        Vector3 child_scale = child.transform.localScale;

        child.transform.localScale = new Vector3(child_scale.x * this.Dir.x, child_scale.y, child_scale.z);
    }

    IEnumerator AttackRoutine(float skill_delay)
    {
        this.isAttack = true;
        Skill_1_VFX();

        atk_act?.Invoke();

        yield return new WaitForSeconds(skill_delay);

        this.transform.localScale = new Vector3(this.Dir.x > 0 ? 1 : -1, 1, 1);
        this.isAttack = false;

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            this.isGround = true;

            // 더블 점프 횟수 초기화
            this.double_jump_cnt = double_jump_value;

            // 애니메이션 변환
            this.anim.SetBool(this.anim_bool_IsGround, true);
        }

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            this.isGround = false;

            // 애니메이션 변환
            this.anim.SetBool(this.anim_bool_IsGround, false);
        }
    }
}
