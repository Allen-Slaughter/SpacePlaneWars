using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    #region FIELDS
    [SerializeField] StatsBar_HUD statsBar_HUD;
    [SerializeField] bool regenerateHealth = true;
    [SerializeField] float healthRegenerateTime;
    [SerializeField, Range(0f, 1f)] float healthRegeneratePercent;

    [Header("---- INPUT ----")]
    [SerializeField] PlayerInput input;

    [Header("---- MOVE ----")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float accelerationTime = 3f;
    [SerializeField] float decelerationTime = 3f;
    [SerializeField] float moveRotationAngle = 50f;

    [Header("---- FIRE ----")]
    [SerializeField] GameObject projectile1;
    [SerializeField] GameObject projectile2;
    [SerializeField] GameObject projectile3;
    [SerializeField] GameObject projectileOverdirve;
    [SerializeField] ParticleSystem muzzleVFX;
    [SerializeField] Transform muzzleMiddle;
    [SerializeField] Transform muzzleTop;
    [SerializeField] Transform muzzleBottom;
    [SerializeField] AudioData projectileLaunchSFX;
    [SerializeField, Range(0, 2)] int weaponPower = 0;
    [SerializeField] float fireInterval = 0.2f;

    [Header("---- DODGE ----")]
    [SerializeField] AudioData dodgeSFX;
    [SerializeField, Range(0, 100)] int dodgeEnegryCost = 25;
    [SerializeField] float maxRoll = 720f;
    [SerializeField] float rollSpeed = 360f;
    [SerializeField] Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("---- OVERDIRVE ----")]
    [SerializeField] int overdirveDodgeFactor = 2;
    [SerializeField] float overdirveSpeedFactor = 1.2f;
    [SerializeField] float overdirveFireFactor = 1.2f;

    bool isDodging = false;
    bool isOverdirving = false;

    readonly float SlowMotionDuration = 1f;
    readonly float InvincibleTime = 1f;

    float paddingX;
    float paddingY;
    float currentRoll;
    float dodgeDuration;
    float t;

    Vector2 moveDirection;
    Vector2 previousVelocity;

    Quaternion previousRotation;

    WaitForSeconds waitForFireInterval;
    WaitForSeconds waitForOverdirveFireInterval;
    WaitForSeconds waitHealthRegenerateTime;
    WaitForSeconds waitDecelerationTime;
    WaitForSeconds waitInvincibleTime;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    Coroutine moveCoroutine;
    Coroutine healthRegenerateCoroutine;

    new Rigidbody2D rigidbody;
    new Collider2D collider;

    MissileSystem missile;

    #endregion

    #region UNITY EVENT FUNCTIONS
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        missile = GetComponent<MissileSystem>();

        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;

        dodgeDuration = maxRoll / rollSpeed;
        rigidbody.gravityScale = 0f;

        waitForFireInterval = new WaitForSeconds(fireInterval);
        waitForOverdirveFireInterval = new WaitForSeconds(fireInterval /= overdirveFireFactor);
        waitHealthRegenerateTime = new WaitForSeconds(healthRegenerateTime);
        waitDecelerationTime = new WaitForSeconds(decelerationTime);
        waitInvincibleTime = new WaitForSeconds(InvincibleTime);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        input.onMove += Move;
        input.onStopMove += StopMove;
        input.onFire += Fire;
        input.onStopFire += StopFire;
        input.onDodge += Dodge;
        input.onOverdirve += Overdirve;
        input.onLaunchMissile += LaunchMissile;

        PlayerOverdirve.on += OverdirveOn;
        PlayerOverdirve.off += OverdirveOff;
    }

    void OnDisable()
    {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
        input.onDodge -= Dodge;
        input.onOverdirve -= Overdirve;
        input.onLaunchMissile -= LaunchMissile;

        PlayerOverdirve.on -= OverdirveOn;
        PlayerOverdirve.off -= OverdirveOff;
    }

    void Start()
    {
        statsBar_HUD.Initialize(health, maxHealth);

        //开始操作角色时激活Game play动作表
        input.EnableGamePlayInput();
    }
    #endregion

    #region HEALTH
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        statsBar_HUD.UpdateStats(health, maxHealth);
        TimeController.Instance.BulletTime(SlowMotionDuration, SlowMotionDuration);

        if (gameObject.activeSelf)
        {
            Move(moveDirection);
            StartCoroutine(nameof(InvincibleCoroutine));

            if (regenerateHealth)
            {
                if (healthRegenerateCoroutine != null)
                {
                    StopCoroutine(healthRegenerateCoroutine);
                }

                healthRegenerateCoroutine = StartCoroutine(HealthRegenerateCoroutine(waitHealthRegenerateTime, healthRegeneratePercent));
            }
        }
    }

    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        statsBar_HUD.UpdateStats(health, maxHealth);
    }

    public override void Die()
    {
        GameManager.onGameOver.Invoke();
        GameManager.GameState = GameState.GameOver;
        statsBar_HUD.UpdateStats(0f, maxHealth);
        base.Die();
    }

    IEnumerator InvincibleCoroutine()
    {
        collider.isTrigger = true;

        yield return waitInvincibleTime;

        collider.isTrigger = false;
    }
    #endregion

    #region MOVE
    //事件处理函数
    void Move(Vector2 moveInput)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveDirection = moveInput.normalized;

        //moveInput.y在(-1,+1),产生不同方向的旋转角度，旋转轴为红色的X轴
        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveDirection * moveSpeed, Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right)));
        StopCoroutine(nameof(DecelerationCoroutine));
        StartCoroutine(nameof(MoveRangeLimatationCoroutine));
    }

    void StopMove()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveDirection = Vector2.zero;
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, moveDirection, Quaternion.identity));
        StopCoroutine(nameof(DecelerationCoroutine));
    }

    IEnumerator MoveCoroutine(float time, Vector2 moveVelocity, Quaternion moveRotation)
    {
        t = 0f;
        previousVelocity = rigidbody.velocity;
        previousRotation = transform.rotation;

        while (t < time)
        {
            t += Time.fixedDeltaTime;
            rigidbody.velocity = Vector2.Lerp(previousVelocity, moveVelocity, t / time);
            transform.rotation = Quaternion.Lerp(previousRotation, moveRotation, t / time);

            yield return waitForFixedUpdate;
        }
    }

    IEnumerator MoveRangeLimatationCoroutine()
    {
        while (true)
        {
            transform.position = Viewport.Instance.PlayerMoveablePosition(transform.position, paddingX, paddingY);

            yield return null;
        }
    }

    IEnumerator DecelerationCoroutine()
    {
        yield return decelerationTime;

        StopCoroutine(nameof(MoveRangeLimatationCoroutine));
    }
    #endregion

    #region FIRE

    void Fire()
    {
        muzzleVFX.Play();
        StartCoroutine(nameof(FireCoroutine));
    }

    void StopFire()
    {
        muzzleVFX.Stop();
        StopCoroutine(nameof(FireCoroutine));
    }

    IEnumerator FireCoroutine()
    {
        while (true)
        {
            switch (weaponPower)
            {
                case 0:
                    PoolManager.Release(isOverdirving ? projectileOverdirve : projectile1, muzzleMiddle.position);
                    break;
                case 1:
                    PoolManager.Release(isOverdirving ? projectileOverdirve : projectile1, muzzleTop.position);
                    PoolManager.Release(projectile1, muzzleBottom.position);
                    break;
                case 2:
                    PoolManager.Release(isOverdirving ? projectileOverdirve : projectile1, muzzleMiddle.position);
                    PoolManager.Release(isOverdirving ? projectileOverdirve : projectile2, muzzleTop.position);
                    PoolManager.Release(isOverdirving ? projectileOverdirve : projectile3, muzzleBottom.position);
                    break;
                default:
                    break;
            }

            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);

            yield return isOverdirving ? waitForOverdirveFireInterval : waitForFireInterval;
        }
    }

    #endregion

    #region DODGE
    void Dodge()
    {
        if (isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnegryCost)) return;

        StartCoroutine(nameof(DodgeCoroutine));
    }

    IEnumerator DodgeCoroutine()
    {
        isDodging = true;
        AudioManager.Instance.PlayRandomSFX(dodgeSFX);
        //Cost enegry   消耗能量
        PlayerEnergy.Instance.Use(dodgeEnegryCost);

        //Make player invincible    让玩家无敌
        collider.isTrigger = true;

        //Make player rotato along X axis   让玩家沿着X轴旋转
        currentRoll = 0f;

        TimeController.Instance.BulletTime(SlowMotionDuration, SlowMotionDuration);

        while (currentRoll < maxRoll)
        {
            currentRoll += rollSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);

            //Change player's scale     改变玩家大小
            transform.localScale = BezierCurve.QuadraticPoint(Vector3.one, Vector3.one, dodgeScale, currentRoll / maxRoll);

            yield return null;
        }

        collider.isTrigger = false;
        isDodging = false;
    }
    #endregion

    #region OVERDIRVE
    void Overdirve()
    {
        if (!PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX)) return;

        PlayerOverdirve.on.Invoke();
    }

    void OverdirveOn()
    {
        isOverdirving = true;
        dodgeEnegryCost *= overdirveDodgeFactor;
        moveSpeed *= overdirveSpeedFactor;
        TimeController.Instance.BulletTime(SlowMotionDuration, SlowMotionDuration);
    }

    void OverdirveOff()
    {
        isOverdirving = false;
        dodgeEnegryCost /= overdirveDodgeFactor;
        moveSpeed /= overdirveSpeedFactor;
    }

    #endregion

    void LaunchMissile()
    {
        missile.Launch(muzzleMiddle);
    }
}
