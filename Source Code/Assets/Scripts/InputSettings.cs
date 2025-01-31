using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 输入数据预处理
/// </summary>
public class InputSettings : MonoBehaviour
{
    private PlayerInput _playerInput;
    private InputActionAsset _inputActionAsset;
    private InputAction _frontPortalAction;
    private InputAction _backPortalAction;
    private InputAction _moveAction;
    
    public event Action OnFrontPortalPress;
    public event Action OnBackPortalPress;
    private void Awake()
    {
        //输入模块获取
        try
        {
            _playerInput = GetComponent<PlayerInput>();
            _inputActionAsset = _playerInput.actions;
            _frontPortalAction = _inputActionAsset.FindAction("FrontPortal");
            _backPortalAction = _inputActionAsset.FindAction("BackPortal");
            _moveAction = _inputActionAsset.FindAction("Move");
            
            _frontPortalAction.performed += OnFrontPortalActionPerformed;
            _backPortalAction.performed += OnBackPortalActionPerformed;
        }
        catch (Exception e)
        {
            Debug.LogWarning("无法获取输入配置文件"+e);
            throw;
        }
    }

    /// <summary>
    /// 获取移动输入指向
    /// </summary>
    /// <returns></returns>
    public Vector2 MoveDir()
    {
        Vector2 inputDir = _moveAction.ReadValue<Vector2>();
        return inputDir.normalized;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    private void OnFrontPortalActionPerformed(InputAction.CallbackContext context)
    {
        // 当事件被触发时，通知所有订阅者
        if (OnFrontPortalPress != null)
        {
            OnFrontPortalPress.Invoke();
        }
    } 
    
    private void OnBackPortalActionPerformed(InputAction.CallbackContext context)
    {
        // 当事件被触发时，通知所有订阅者
        if (OnBackPortalPress != null)
        {
            OnBackPortalPress.Invoke();
        }
    }
}
