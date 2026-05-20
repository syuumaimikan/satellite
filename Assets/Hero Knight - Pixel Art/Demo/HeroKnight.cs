using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class HeroKnight : MonoBehaviour
{
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;
    [SerializeField] float m_diveAttackForce = 18f;
    [SerializeField] private GameObject m_shockwaveEffect;
    [SerializeField] private int m_maxJumps = 2;
    [SerializeField] private float m_rollCooldown = 0.8f;
    [SerializeField] private float m_invincibleDuration = 0.35f;
    private SpriteRenderer m_spriteRenderer;

    private PlayerStats m_stats;
    private bool m_isInvincible = false;
    private float m_invincibleTimer = 0f;
    private float m_rollCooldownTimer = 0f;
    private int m_jumpCount = 0;
    private bool m_isDiveAttacking = false;
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;
    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;


    // Use this for initialization
    void Start()
    {
        m_stats = GetComponent<PlayerStats>();
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    // Update is called once per frame
    void Update()
    {
        // Invincible timer
        if (m_isInvincible)
        {
            m_invincibleTimer -= Time.deltaTime;

            // 点滅
            Color c = m_spriteRenderer.color;
            c.a = Mathf.PingPong(
                Time.time * 10f,
                0.5f
            ) + 0.5f;

            m_spriteRenderer.color = c;

            if (m_invincibleTimer <= 0f)
            {
                m_isInvincible = false;

                Color reset =
                    m_spriteRenderer.color;
                reset.a = 1f;

                m_spriteRenderer.color = reset;
            }
        }
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;
        // Roll cooldown timer
        if (m_rollCooldownTimer > 0)
        {
            m_rollCooldownTimer -= Time.deltaTime;
        }
        // Increase timer that checks roll duration
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
        {
            m_rolling = false;

            // Dodge後の加速を止める
            m_body2d.linearVelocity =
                new Vector2(
                    m_body2d.linearVelocity.x * 0.2f,
                    m_body2d.linearVelocity.y
                );
        }

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;

            m_jumpCount = 0;

            // DiveAttack終了
            if (m_isDiveAttacking)
            {
                m_isDiveAttacking = false;

                // 衝撃波生成
                if (m_shockwaveEffect != null)
                {
                    Instantiate(
                        m_shockwaveEffect,
                        transform.position +
                        new Vector3(0f, -0.8f, 0f),
                        Quaternion.identity
                    );
                }

                // カメラシェイク
                CameraShake.Instance.Shake(0.15f, 0.15f);
            }

            m_animator.SetBool("Grounded", true);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = 0f;

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            inputX = -1f;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            inputX = 1f;
        }

        float inputY = 0f;

        if (Keyboard.current.sKey.isPressed ||
            Keyboard.current.downArrowKey.isPressed)
        {
            inputY = -1f;
        }

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }

        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        if (!m_rolling)
            m_body2d.linearVelocity =
            new Vector2(
                inputX * m_stats.moveSpeed,
                m_body2d.linearVelocity.y
            );

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        //Damage
        if (Keyboard.current.kKey
            .wasPressedThisFrame)
        {
            GetComponent<PlayerHealth>()
                .TakeDamage(10);
        }

        //Death
        if (Keyboard.current.eKey.wasPressedThisFrame && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }

        //Hurt
        else if (Keyboard.current.qKey.wasPressedThisFrame && !m_rolling)
            m_animator.SetTrigger("Hurt");

        // Dive Attack (空中 ↓ + 左クリック)
        if (!m_grounded &&
            inputY < 0 &&
            Mouse.current.leftButton.wasPressedThisFrame &&
            !m_rolling &&
            !m_isDiveAttacking)
        {
            m_isDiveAttacking = true;

            // アニメ
            m_animator.SetTrigger("DiveAttack");

            // 一気に下へ落下
            m_body2d.linearVelocity =
                new Vector2(
                    m_body2d.linearVelocity.x * 0.5f,
                    -m_diveAttackForce
                );
        }

        //Attack
        else if (Mouse.current.leftButton.wasPressedThisFrame &&
                 m_timeSinceAttack > 0.25f &&
                 !m_rolling)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }

        // Guard
        else if (Mouse.current.rightButton.wasPressedThisFrame &&
                 !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Mouse.current.rightButton.wasReleasedThisFrame)
            m_animator.SetBool("IdleBlock", false);

        // Roll
        else if (Keyboard.current.leftShiftKey.wasPressedThisFrame
         && !m_rolling
         && !m_isWallSliding
         && m_rollCooldownTimer <= 0f)
        {
            m_rolling = true;
            m_rollCurrentTime = 0f;

            // クールタイム
            m_rollCooldownTimer = m_rollCooldown;

            // 無敵開始
            m_isInvincible = true;
            m_invincibleTimer = m_invincibleDuration;

            m_animator.SetTrigger("Roll");

            m_body2d.linearVelocity =
                new Vector2(
                    m_facingDirection * m_rollForce,
                    m_body2d.linearVelocity.y
                );
        }


        //Jump
        else if (Keyboard.current.spaceKey.wasPressedThisFrame
         && m_jumpCount < m_maxJumps
         && !m_rolling)
        {
            m_animator.SetTrigger("Jump");

            m_grounded = false;
            m_animator.SetBool("Grounded", false);

            // 上方向速度をリセットして綺麗に飛ぶ
            m_body2d.linearVelocity =
                new Vector2(
                    m_body2d.linearVelocity.x,
                    0f
                );

            // ジャンプ
            m_body2d.AddForce(
                Vector2.up * m_stats.jumpForce,
                ForceMode2D.Impulse
            );

            m_jumpCount++;

            // 地面センサー誤反応防止
            if (m_jumpCount == 1)
            {
                m_groundSensor.Disable(0.2f);
            }
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
    public void SetMaxJumps(int jumps)
    {
        m_maxJumps = jumps;
    }

    public void SetRollCooldown(float cooldown)
    {
        m_rollCooldown = cooldown;
    }
}
