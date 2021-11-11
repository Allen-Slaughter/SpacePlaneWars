using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Player Input")]
public class PlayerInput : ScriptableObject, InputActions.IGameplayActions
{
    //开始移动事件
    public event UnityAction<Vector2> onMove = delegate { };
    //停止移动事件
    public event UnityAction onStopMove = delegate { };
    InputActions inputActions;

    void OnEnable()
    {
        inputActions = new InputActions();

        inputActions.Gameplay.SetCallbacks(this);
    }

    void OnDisable()
    {
        DisableAllInput();
    }

    //禁用所有输入
    public void DisableAllInput()
    {
        inputActions.Gameplay.Disable();
    }

    //调用Gameplay动作表
    public void EnableGamePlayInput()
    {
        inputActions.Gameplay.Enable();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //开始移动事件的触发
        if (context.phase == InputActionPhase.Performed)
        {
            onMove.Invoke(context.ReadValue<Vector2>());
        }
        //停止移动事件的触发
        if (context.phase == InputActionPhase.Canceled)
        {
            onStopMove.Invoke();
        }
    }
}
