using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    private Transform target;   // 目标
    private Vector3 direction;  // 移动方向
    private float speed;        // 子弹速度

    private Vector3 myDir;
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        //myDir = direction;
    }

    public void SetSpeed(float bulletSpeed)
    {
        speed = bulletSpeed;
    }

    void Start()
    {
        // 如果没有目标且方向未设置，默认向前
        if (target == null && direction == Vector3.zero)
        {
            direction = transform.forward;
        }
        Invoke("DestoryBullet",0.5f);
    }

    void Update()
    {
        if (target != null)
        {
            // 移动子弹朝向目标
            Vector3 toTarget = (target.position - transform.position).normalized;
            transform.position += toTarget * speed * Time.deltaTime;
        }
        else
        {
            // 沿着指定方向移动
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    // 碰撞检测（可选）
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Monster monster= other.gameObject.GetComponent<Monster>();
            myDir = transform.position - other.transform.position;
            Debug.Log(myDir);
            monster.OnAttacked(1,-myDir,2.5f);
            // 对敌人造成伤害（根据你的游戏逻辑）
            Destroy(gameObject);
        }
    }

    void DestoryBullet()
    {
        Destroy(gameObject);
    }
}
