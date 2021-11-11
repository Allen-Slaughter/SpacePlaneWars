using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] PlayerInput input;

    [SerializeField] float moveSpeed = 10f;
    new Rigidbody2D rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        input.onMove += Move;
        input.onStopMove += StopMove;
    }

    void OnDisable()
    {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
    }

    void Start()
    {
        rigidbody.gravityScale = 0f;
        //开始操作角色时激活Gamoplay动作表
        input.EnableGamePlayInput();
    }

    //事件处理函数
    void Move(Vector2 moveInput)
    {
        rigidbody.velocity = moveInput * moveSpeed;
    }

    void StopMove()
    {
        rigidbody.velocity = Vector2.zero;
    }
}
