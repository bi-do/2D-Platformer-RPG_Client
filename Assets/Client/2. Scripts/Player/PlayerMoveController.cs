using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    private float move_spd = 4f;
    [SerializeField] private float jump_power = 16f;
    [SerializeField] private float double_jump_power_x = 15f;
    [SerializeField] private float double_jump_power_y = 15f;

    Vector2 dir;

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

    private bool isGround = false;

    /// <summary> true = 방향키로 속도 고정 제어 , false = 방향키로 속도 제어 불가  </summary>
    private bool isMove = false;

    private string anim_float_Input_X = "Input_X";
    private string anim_bool_IsGround = "IsGround";


    void Awake()
    {
        this.player_input = this.GetComponent<PlayerInput>();
        this.rb = this.GetComponent<Rigidbody2D>();
        this.anim = this.GetComponent<Animator>();

        this.double_jump_cnt = double_jump_value;

        this.move_act = this.player_input.actions.FindAction("Player/Move");
        this.jump_act = this.player_input.actions.FindAction("Player/Jump");
    }

    void OnEnable()
    {
        move_act.Enable();
        move_act.performed += Move;
        move_act.canceled += MoveCancel;

        jump_act.Enable();
        jump_act.performed += Jump;
    }

    void OnDisable()
    {
        move_act.Disable();
        move_act.performed -= Move;
        move_act.canceled -= MoveCancel;

        jump_act.Disable();
        jump_act.performed -= Jump;
    }



    private void Move(InputAction.CallbackContext context)
    {
        // 속도 변환 배제
        this.isMove = true;

        this.anim.SetFloat(this.anim_float_Input_X , 1);

        this.dir = context.ReadValue<Vector2>();

        // 방향 전환
        this.transform.localScale = new Vector3(this.dir.x > 0 ? 1 : -1, 1, 1);

        // 속도 변경
        this.velo_x = dir.x * this.move_spd;
    }

    private void MoveCancel(InputAction.CallbackContext context)
    {
        // 속도 변환 허용
        this.isMove = false;

        this.anim.SetFloat(this.anim_float_Input_X , 0);
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
            
            float dir_x = this.dir.x > 0 ? 1 : -1;

            Vector2 double_jump_power = new Vector2(dir_x * this.double_jump_power_x, this.double_jump_power_y);

            this.rb.AddForce(double_jump_power, ForceMode2D.Impulse);
            // Debug.Log("더블 점프 호출");
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (this.isMove && isGround)
            this.rb.linearVelocityX = velo_x;
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
