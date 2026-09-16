using UnityEngine;

/// <summary>
/// プレイヤーの移動、走り、スタミナ、マグネット、アニメーションを管理します。
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats; // プレイヤーのステータス

    private Rigidbody rb;                       // 物理移動用
    private Animator anim;                      // アニメーション制御
    private Vector3 moveDirection;              // 入力から計算した移動方向
    private bool isRunning;                     // 走り入力中か
    private bool isExhausted;                   // スタミナ切れで疲労状態か
    private Vector2 defaultScale;               // 向き反転用の初期スケール

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        defaultScale = transform.localScale;
    }

    void Update()
    {
        // メニュー開いている間は完全停止
        if (UpgradeMenuController.isOpen)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        // 経過時間を加算（ゴール判定に使用）
        playerStats.timeElapsed += Time.deltaTime;

        GetInput();      // 入力処理（移動・向き・アニメ）
        HandleStamina(); // スタミナ消費・回復・疲労
        HandleMagnet();  // マグネット発動・持続・クールタイム
    }

    void FixedUpdate()
    {
        // 疲労状態なら走れない
        if (isRunning && !isExhausted)
            Run();
        else
            Walk();
    }

    /// <summary>
    /// 入力処理：移動方向、走り、向き反転、歩きアニメ
    /// </summary>
    private void GetInput()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        moveDirection = new Vector3(x, 0, z).normalized;
        isRunning = Input.GetKey(KeyCode.Space);
        
        // 左右の向き反転
        if (moveDirection.x > 0)
        {
            transform.localScale = defaultScale;
        }
        else if (moveDirection.x < 0)
        {
            transform.localScale = new Vector2(-defaultScale.x,defaultScale.y);
        }

        // 歩きアニメのON/OFF
        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            anim.SetBool("Walk",true);
        }
        else
        {
            anim.SetBool("Walk",false);
        }
    }

    /// <summary>
    /// マグネットの発動・持続・クールタイム管理
    /// </summary>
    void HandleMagnet()
    {
        // 未取得なら使えない
        if (!playerStats.hasMagnet) return;

        // クールタイム中
        if (playerStats.magnetCooldownTimer > 0f)
        {
            playerStats.magnetCooldownTimer -= Time.deltaTime;
            return;
        }

        // 発動中
        if (playerStats.magnetActive)
        {
            playerStats.magnetTimer -= Time.deltaTime;

            // 時間切れからクールタイムへ
            if (playerStats.magnetTimer <= 0f)
            {
                playerStats.magnetActive = false;
                playerStats.magnetCooldownTimer = playerStats.magnetCooldown;
            }

            return;
        }

        // 発動キー（F）でマグネット開始
        if (Input.GetKeyDown(KeyCode.F))
        {
            playerStats.magnetActive = true;
            playerStats.magnetTimer = playerStats.magnetDuration;
        }
    }

    /// <summary>
    /// スタミナ消費・回復・疲労状態の管理
    /// </summary>
    private void HandleStamina()
    {
        bool isMoving = moveDirection.magnitude > 0.1f;
        float runCost = playerStats.runStaminaCost; // 走りの消費は一定

        // 走っている → スタミナ消費
        if (isRunning && isMoving && !isExhausted)
        {
            playerStats.stamina -= runCost * Time.deltaTime;

            // スタミナ切れ → 疲労状態へ
            if (playerStats.stamina <= 0f)
            {
                playerStats.stamina = 0f;
                isExhausted = true; 
            }
        }
        else
        {
            
            // 歩いているor止まっている → 回復
            float recovery = playerStats.staminaRecovery;

            if (playerStats.stamina < playerStats.maxStamina)
            {
                playerStats.stamina += recovery * Time.deltaTime;
            }

            // MAXまで回復したら疲労解除
            if (playerStats.stamina >= playerStats.maxStamina && isExhausted)
            {
                playerStats.stamina = playerStats.maxStamina;
                isExhausted = false;
            }
        }
    }

    /// <summary> 
    /// 歩き移動（疲労時は速度低下）
    /// </summary>
    private void Walk()
    {
        float speed = isExhausted ? playerStats.tiredWalkSpeed : playerStats.moveSpeed;

        rb.linearVelocity = new Vector3(
            moveDirection.x * speed,
            rb.linearVelocity.y,
            moveDirection.z * speed
        );
        
        
    }

    /// <summary>
    /// 走り移動
    /// </summary>
    private void Run()
    {
        rb.linearVelocity = new Vector3(
            moveDirection.x * playerStats.runSpeed,
            rb.linearVelocity.y,
            moveDirection.z * playerStats.runSpeed
        );
    }
}
