using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float maxHealth;

    private AudioSource _audioSource;
    
    public InputSettings InputSettings;
    private CharacterController _characterController;
    private Rigidbody _rigidbody;

    public float moveSpeed;
    public float rotateSpeed;

    public GameObject PlayerSprite;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private bool _isRight;
    private bool _isShooting;
    
    private GameObject _bulletPrefab;        // 子弹预制体
    public float bulletSpeed = 10f;        // 子弹速度
    public float detectionRange = 15f;     // 侦测范围
    public float coneAngle = 45f;          // 锥形角度
    
    private CinemachineImpulseSource _impulseSource;

    public GameObject shootLightRight;
    public GameObject shootLightLeft;

    private bool isDead;
    private float currentHealth;
    
    void Start()
    {
        currentHealth = maxHealth;
        InputSettings.OnFrontPortalPress += OpenFrontPortal;
        InputSettings.OnBackPortalPress += OpenBackPortal;
        _bulletPrefab = Resources.Load<GameObject>("Bullet");
        _characterController = GetComponent<CharacterController>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = PlayerSprite.GetComponent<Animator>();
        _spriteRenderer = PlayerSprite.GetComponent<SpriteRenderer>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        PlayerMovement(InputSettings.MoveDir());

        if (!MyEventManager.Instance.isDisplayDialogue && Input.GetMouseButtonDown(0)&&!_isShooting&&!isDead)
        {
            Shoot();
        }
    }
    
    Vector3 _moveDirection;
    /// <summary>
    /// 玩家移动
    /// </summary>
    /// <param name="inputVector"></param>
    private void PlayerMovement(Vector2 inputVector)
    {
        //以摄像机为参考
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        //现在将输入的xy分量应用到right和forward
        if (!MyEventManager.Instance.isDisplayDialogue && !_isShooting && !isDead)
        {
            _moveDirection = (right * inputVector.x + forward * inputVector.y);
            // 通过 CharacterController 移动角色
            PlayAniSwitch(inputVector);
        }
        else
        {
            _moveDirection = Vector3.zero;
            //_animator.Play("Idle");
        }

        if (MyEventManager.Instance.isDisplayDialogue)
        {
            _animator.Play("Idle");
        }
        _characterController.SimpleMove(_moveDirection * moveSpeed);
    }

    public void PlayAniSwitch(Vector2 inputVector)
    {
        float horizontalInput = inputVector.x;
        float verticalInput = inputVector.y;

        // 首先处理左右方向的动画（Side动画）
        if (Mathf.Abs(horizontalInput) > 0)
        {
            // 播放Side动画
            _animator.Play("Run_Side");

            // 如果输入为正（向右），不翻转；如果输入为负（向左），翻转角色
            _spriteRenderer.flipX = horizontalInput >  0;
            _isRight = _spriteRenderer.flipX;
        }
        // 处理上下方向的动画
        else if (verticalInput > 0)
        {
            // 播放向上（Up）的动画
            _animator.Play("Run_Back");
        }
        else if (verticalInput < 0)
        {
            // 播放向下（Down）的动画
            _animator.Play("Run_Front");
        }
        else
        {
            // 如果没有输入，播放Idle动画
            _animator.Play("Idle");
        }
    }
    
    void Shoot()
    {
        _isShooting = true;
        ShootEffect();
        StartCoroutine(ShootCold(0.3f));
        // 获取点击位置
        Vector3 clickPosition = Input.mousePosition;

        // 将点击位置从屏幕坐标转换为世界坐标
        Ray ray = Camera.main.ScreenPointToRay(clickPosition);
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        float rayDistance;
        Vector3 worldClickPosition = Vector3.zero;

        if (playerPlane.Raycast(ray, out rayDistance))
        {
            worldClickPosition = ray.GetPoint(rayDistance);
        }

        // 判断点击点在玩家左侧还是右侧
        Vector3 toClick =worldClickPosition-transform.position;
        float side = Vector3.Dot(transform.forward, toClick);

        // 定义搜索方向
        Vector3 searchDirection = side >= 0 ? transform.forward : -transform.forward;

        // 在锥形范围内检测敌人
        Transform targetEnemy = FindEnemyInCone(searchDirection);

        // 发射跟踪子弹
        FireHomingBullet(targetEnemy, searchDirection);
        
        PlayShootAnimation(side >= 0);
    }
    
    /// <summary>
    /// 旋转角色（暂时无用）
    /// </summary>
    /// <param name="moveDirection"></param>
    private void RotatePlayer(Vector3 moveDirection)
    {
        if (moveDirection != Vector3.zero)
        {
            // 计算目标旋转的四元数
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            // 平滑旋转角色
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }
    
    private void OpenFrontPortal()
    {
        Debug.Log("front");
    }

    private void OpenBackPortal()
    {
        Debug.Log("back");
    }
    
    void PlayShootAnimation(bool isRightSide)
    {
        if (_animator != null)
        {
            _animator.Play("ShootRight");
            if (isRightSide)
            {
                _spriteRenderer.flipX = false;

                // 开启右边的灯
                shootLightRight.SetActive(true);

                // 在 0.2 秒后关闭右边的灯
                StartCoroutine(TurnOffLightAfterDelay(shootLightRight, 0.2f));
            }
            else
            {
                _spriteRenderer.flipX = true;

                // 开启左边的灯
                shootLightLeft.SetActive(true);

                // 在 0.2 秒后关闭左边的灯
                StartCoroutine(TurnOffLightAfterDelay(shootLightLeft, 0.2f));
            }
        }
    }
    
    // 在锥形范围内查找敌人
    Transform FindEnemyInCone(Vector3 direction)
    {
        // 获取所有带有"Enemy"标签的敌人
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            Vector3 toEnemy = enemy.transform.position - transform.position;
            float distance = toEnemy.magnitude;

            if (distance <= detectionRange)
            {
                // 计算角度
                float angle = Vector3.Angle(direction, toEnemy);

                if (angle <= coneAngle / 2)
                {
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestEnemy = enemy.transform;
                    }
                }
            }
        }

        return nearestEnemy;
    }

    // 发射跟踪子弹

    void FireHomingBullet(Transform target, Vector3 direction)
    {
        // 生成子弹
        GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);

        // 获取子弹的脚本并设置目标或方向
        HomingBullet homingBullet = bullet.GetComponent<HomingBullet>();
        if (homingBullet != null)
        {
            if (target != null)
            {
                homingBullet.SetTarget(target);
            }
            else
            {
                homingBullet.SetDirection(direction);
            }

            homingBullet.SetSpeed(bulletSpeed);
        }
    }

    IEnumerator ShootCold(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isShooting = false;
    }

    private void ShootEffect()
    {
        _impulseSource.GenerateImpulse();
        _audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/射击2"));
    }
    
    IEnumerator TurnOffLightAfterDelay(GameObject lightObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        lightObject.SetActive(false);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead)
            return;

        currentHealth -= damageAmount;
        Debug.Log("Hit");
        // 检查血量下限
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die(); // 调用死亡方法
        }
        else
        {

            // 可以添加其他受击反馈，例如屏幕闪红、摄像机震动等
        }

        // 更新血量 UI（如果有）
        // UIManager.Instance.UpdateHealth(currentHealth);
    }

    private void Die()
    {
        isDead = true;
        Image image = GameObject.FindWithTag("AutoMask").GetComponentInChildren<Image>();
        Sequence q = DOTween.Sequence();
        q.Append( image.DOFade(1, 0.5f).OnComplete(() =>
        {
            Instantiate(Resources.Load<GameObject>("UIDeadText"));
        }));
        q.AppendInterval(1.5f).OnComplete(()=>SceneManager.LoadScene("StoryWriting"));
                
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(2);
        }
    }
}
