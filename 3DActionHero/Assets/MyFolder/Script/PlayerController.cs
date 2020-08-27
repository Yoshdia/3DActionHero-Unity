using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Rigidbody による物理特性を使用しないプレイヤー制御
    /// </summary>
    protected CharacterController characterController;
    /// <summary>
    /// 移動力
    /// </summary>
    protected Vector3 movement;
    /// <summary>
    /// 前方に移動する速度
    /// </summary>
    protected float forwardSpeed;
    /// <summary>
    /// プレイヤーを映すカメラ
    /// </summary>
    protected MainCamera mainCamera;
    /// <summary>
    /// プレーヤーの現在の回転とプレーヤーの希望する回転の差
    /// </summary>
    protected float angleDiff;
    /// <summary>
    /// プレイヤーが次に進行する角度
    /// </summary>
    protected Quaternion targetRotation;
    /// <summary>
    /// 移動の入力情報
    /// </summary>
    protected Vector2 moveInput;
    /// <summary>
    /// 移動したい速度
    /// </summary>
    protected float desiredForwardSpeed;
    /// <summary>
    /// 移動速度
    /// </summary>
    protected float maxForwardSpeed = 8f;
    /// <summary>
    /// 地面と接触している状態での加速度
    /// </summary>
    const float groundAcceleration = 20f;
    /// <summary>
    /// 地面と接触している状態での減速度
    /// </summary>
    const float groundDeceleration = 25f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = FindObjectOfType<MainCamera>();
    }

    void Update()
    {
        InputUpdate();

        CalculateForwardMovement();

        SetTargetRotation();

        if (movement != Vector3.zero)
        {
            characterController.Move(movement);
            transform.rotation = targetRotation;
        }
    }

    /// <summary>
    /// 入力情報を初期化
    /// </summary>
    void InputUpdate()
    {
        moveInput = Vector2.zero;
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }
    }

    /// <summary>
    /// 前方への移動量を計算
    /// </summary>
    void CalculateForwardMovement()
    {
        desiredForwardSpeed = moveInput.magnitude * maxForwardSpeed;
        ///加速度
        float acceleration = Mathf.Approximately(moveInput.sqrMagnitude, 0f) ? groundAcceleration : groundDeceleration;
        //移動量を上限を付け計算
        forwardSpeed = Mathf.MoveTowards(forwardSpeed, desiredForwardSpeed, acceleration * Time.deltaTime);
        //移動量を前方方向に曲げる
        movement = forwardSpeed * transform.forward * Time.deltaTime;
    }

    /// <summary>
    /// カメラが向いている方向にプレイヤーを向ける前の計算
    /// </summary>
    void SetTargetRotation()
    {
        ///2次元の入力情報を3次元に
        Vector3 localMovementDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        ///カメラにおける前方
        Vector3 forward = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f) * Vector3.forward;
        forward.y = 0f;
        forward.Normalize();
        ///後にrotationに代入する方向
        Quaternion forwardRotation;

        //移動方向がカメラにおける前方と反対の場合前方を反転させる
        if (Mathf.Approximately(Vector3.Dot(localMovementDirection, Vector3.forward), -1.0f))
        {
            forwardRotation = Quaternion.LookRotation(-forward);
        }
        else
        {
            //z軸をカメラの入力で曲げカメラにおける前方方向に曲げる
            Quaternion cameraToInputOffset = Quaternion.FromToRotation(Vector3.forward, localMovementDirection);
            forwardRotation = Quaternion.LookRotation(cameraToInputOffset * forward);
        }

        // 回転したい方向
        Vector3 desiredForward = forwardRotation * Vector3.forward;

        // プレーヤーの現在の回転とプレーヤーの希望する回転の差をラジアンで求める
        float angleCurrent = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
        float targetAngle = Mathf.Atan2(desiredForward.x, desiredForward.z) * Mathf.Rad2Deg;
        angleDiff = Mathf.DeltaAngle(angleCurrent, targetAngle);

        targetRotation = forwardRotation;

        //Vector3 localInput = new Vector3(moveInput.x, 0f, moveInput.y);
        //float groundedTurnSpeed = Mathf.Lerp(1200f, 400f, forwardSpeed / desiredForwardSpeed);
        //float actualTurnSpeed = m_IsGrounded ? groundedTurnSpeed : Vector3.Angle(transform.forward, localInput) * k_InverseOneEighty * k_AirborneTurnSpeedProportion * groundedTurnSpeed;
        targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 2000 * Time.deltaTime);
    }
}
