using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    //追従するオブジェクト
    public GameObject player;
    //追従させるカメラオブジェクト
    public GameObject mainCamera;
    //カメラを動かすキー(右
    const int RotateBotton = 1;
    //回転角度の最小最大値
    const float AngleLimitMax = 40f;
    const float AngleLimitMin = -40f;
    //回転速度
    [SerializeField]
    private float rotate_speed = 1.0f;

    //マウスホイールの入力状態を保存
    private float scroll;
    //カメラのズーム速度
    [SerializeField]
    private float scrollSpeed = 5;
    [SerializeField]
    private const float CanZoomMax = 20;
    [SerializeField]
    private const float CanZoomMin = 3;


    void Awake()
    {
        mainCamera = Camera.main.gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 beforeCameraPos = mainCamera.transform.position;
        Vector3 playerPos = player.transform.position;

        mainCamera.transform.position += transform.forward * scroll * scrollSpeed;
        float dis = Vector3.Distance(playerPos, mainCamera.transform.position);
        if (dis <= CanZoomMin || dis >= CanZoomMax)
        {
            mainCamera.transform.position = beforeCameraPos;
        }

        //ターゲットの座標を追従
        transform.position = playerPos;

        //移動用キーが押されていたら
        if (Input.GetMouseButton(RotateBotton))
        {
            rotateCameraAngle();
        }

        float angle_x = 180f <= transform.eulerAngles.x ? transform.eulerAngles.x - 360 : transform.eulerAngles.x;
        transform.eulerAngles = new Vector3(
            Mathf.Clamp(angle_x, AngleLimitMin, AngleLimitMax),
            transform.eulerAngles.y,
            transform.eulerAngles.z
        );
    }
    private void rotateCameraAngle()
    {
        Vector3 angle = new Vector3(
            Input.GetAxis("Mouse X") * rotate_speed,
            Input.GetAxis("Mouse Y") * -rotate_speed,
            0
        );

        transform.eulerAngles += new Vector3(angle.y, angle.x);
    }
}
