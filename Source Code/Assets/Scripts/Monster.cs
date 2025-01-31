using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public float moveSpeed = 2f;          // 移动速度
    public float detectionRange = 0.1f;    // 侦测范围
    public int maxHealth = 100;           // 最大血量
    private int currentHealth;            // 当前血量

    private Transform player;             // 玩家的位置
    public Animator animator;            // 动画组件
    private Rigidbody rb;                 // 刚体组件
    private SpriteRenderer spriteRenderer;
    
    private bool facingRight;

    private bool _isDead;

    private AudioSource _audioSource;
    void Start()
    {
        // 初始化血量
        currentHealth = maxHealth;
        _audioSource = GetComponent<AudioSource>();
        spriteRenderer = animator.gameObject.GetComponent<SpriteRenderer>();

        // 查找标签为"player"的玩家对象
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!_isDead)
        {

            if (player != null && !MyEventManager.Instance.isDisplayDialogue)
            {
                // 计算与玩家的距离
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (distanceToPlayer <= detectionRange)
                {
                    // 向玩家移动
                    MoveTowardsPlayer();
                }
                else
                {
                    // 原地待机
                    Idle();
                }
            }
            else
            {
                // 原地待机
                Idle();
            }
        }
    }

    // 向玩家移动的方法
    void MoveTowardsPlayer()
    {
        // 播放移动动画
        animator.Play("Walk"); // 确保有名为"Move"的动画

        // 计算移动方向
        Vector3 direction = (player.position - transform.position).normalized;

        if ((player.position.x < transform.position.x && facingRight) ||
            (player.position.x > transform.position.x && !facingRight))
        {
            facingRight = !facingRight;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        // 移动怪物
        //rb.MovePosition(transform.position + direction.normalized * moveSpeed * Time.deltaTime);
        rb.velocity = direction * moveSpeed;
    }

    // 待机方法
    void Idle()
    {
        // 播放待机动画
        animator.Play("Idle"); // 确保有名为"Idle"的动画
    }

    // 改变血量的方法
    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        // 限制血量范围
        //currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            _isDead = true;
        }
    }

    // 死亡方法
    void Die()
    {
        // 播放死亡动画或销毁怪物对象
        animator.Play("Dead");
        StartCoroutine(WaitToDie()); // 确保有名为"Die"的动画
        // Destroy(gameObject); // 如需销毁怪物对象，请取消注释
    }

    // 受击方法
    public void OnAttacked(int damage, Vector3 knockbackDirection, float knockbackForce)
    {
        // 减少血量
        ChangeHealth(-damage);
        
        _audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/玩家受伤"));

        if (!_isDead)
        {

            // 播放受击动画
            animator.Play("Hit"); // 确保有名为"Hit"的动画

            // 施加击退力
            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);
        }
        else
        {
            Die();
        }
    }

    IEnumerator WaitToDie()
    {
        yield return new WaitForSeconds(0.76f);
        Destroy(gameObject);
    }
    
}
