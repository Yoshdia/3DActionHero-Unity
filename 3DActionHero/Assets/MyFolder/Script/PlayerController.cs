using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    protected CharacterController characterController;
    protected Vector3 movement;
    protected float forwardSpeed;
    protected MainCamera mainCamera;
    protected float m_AngleDiff;
    protected Quaternion m_TargetRotation;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = FindObjectOfType<MainCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        //入力
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }

        float desiredForwardSpeed = moveInput.magnitude * 5;

        float acceleration = Mathf.Approximately(moveInput.sqrMagnitude, 0f) ? 20 : 25;

        forwardSpeed = Mathf.MoveTowards(forwardSpeed, desiredForwardSpeed, acceleration * 5 * Time.deltaTime);
        movement = forwardSpeed * transform.forward * Time.deltaTime;

        Vector3 localMovementDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        Vector3 forward = Quaternion.Euler(0f, /*cameraSettings.Current.m_XAxis.Value*/mainCamera.transform.eulerAngles.y, 0f) * Vector3.forward;
        forward.y = 0f;
        forward.Normalize();

        Quaternion targetRotation;

        // ローカルの移動方向が前方と反対の場合、ターゲットの回転はカメラの方向になります。
        if (Mathf.Approximately(Vector3.Dot(localMovementDirection, Vector3.forward), -1.0f))
        {
            targetRotation = Quaternion.LookRotation(-forward);
        }
        else
        {
            // それ以外の場合、回転はカメラの前方からの入力のオフセットでなければなりません。向きを入力方向へ回転
            Quaternion cameraToInputOffset = Quaternion.FromToRotation(Vector3.forward, localMovementDirection);
            targetRotation = Quaternion.LookRotation(cameraToInputOffset * forward);
        }

        // エレンの望ましい前方方向。
        Vector3 resultingForward = targetRotation * Vector3.forward;

        // プレーヤーの現在の回転とプレーヤーの希望する回転の差をラジアンで求めます。
        float angleCurrent = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
        float targetAngle = Mathf.Atan2(resultingForward.x, resultingForward.z) * Mathf.Rad2Deg;

        m_AngleDiff = Mathf.DeltaAngle(angleCurrent, targetAngle);
        m_TargetRotation = targetRotation;

        characterController.Move(movement);

        Vector3 localInput = new Vector3(moveInput.x, 0f, moveInput.y);
        float groundedTurnSpeed = Mathf.Lerp(1200f, 400f, forwardSpeed / desiredForwardSpeed);
        //float actualTurnSpeed = m_IsGrounded ? groundedTurnSpeed : Vector3.Angle(transform.forward, localInput) * k_InverseOneEighty * k_AirborneTurnSpeedProportion * groundedTurnSpeed;
        m_TargetRotation = Quaternion.RotateTowards(transform.rotation, m_TargetRotation, 2000 * Time.deltaTime);

        if (movement != Vector3.zero)
        {
            transform.rotation = m_TargetRotation;
        }

    }
}
