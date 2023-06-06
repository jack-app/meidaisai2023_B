using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCamera : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] private float back = 100f;
    [SerializeField] private float up = 200f;
    [SerializeField] private float angle;
    [SerializeField] private RocketControl rc;
    private bool timeReset;
    private bool eTimeReset;
    private float time;
    private float eTime;
    private Vector3 planetforcasPos;//位置変更の実行
    private Quaternion planetAngle;


    public void CameraUpdate()
    {
        Vector3 rToCenterDirection = obj.transform.position * (-1);//ロケットからの宇宙中心方向のベクトル
        Vector3 rNlzDirection = rToCenterDirection.normalized;//単位ベクトル化
        Vector3 rBackposition = obj.transform.position + (back * rNlzDirection);//カメラのロケット後方位置
        Vector3 rocketforcasPos = new Vector3(rBackposition.x, up, rBackposition.z);//位置変更の実行

        float rCenterAngle = Vector3.Angle(obj.transform.position, Vector3.forward);//カメラ角度の決定
        var rCameraAxis = Vector3.Cross(obj.transform.position, Vector3.forward).y < 0 ? -1 : 1;
        var rNormalizedAngle = Mathf.Repeat(-rCenterAngle * rCameraAxis, 360);
        Quaternion rocketAngle = Quaternion.Euler(angle, rNormalizedAngle, 0f);


        time += Time.deltaTime;
        eTime += Time.deltaTime;

        if (rc.inOrbit)
        {
            eTimeReset = false;
            if (!timeReset)
            {
                time = 0f;
                timeReset = true;
            }
            else
            {
                Vector3 planetPos = rc.orbitCenter.transform.position;//惑星の位置
                Vector3 pToCenterDirection = planetPos * (-1);//惑星からの宇宙中心方向のベクトル
                Vector3 pNlzDirection = pToCenterDirection.normalized;//単位ベクトル化
                Vector3 pBackposition = planetPos + (back * pNlzDirection);//カメラの惑星後方位置
                planetforcasPos = new Vector3(pBackposition.x, up, pBackposition.z);//位置変更の実行

                float pCenterAngle = Vector3.Angle(planetPos, Vector3.forward);//カメラ角度の決定
                var pCameraAxis = Vector3.Cross(planetPos, Vector3.forward).y < 0 ? -1 : 1;
                var pNormalizedAngle = Mathf.Repeat(-pCenterAngle * pCameraAxis, 360);
                planetAngle = Quaternion.Euler(angle, pNormalizedAngle, 0f);

                float inot = System.Math.Min(time / 1, 1);
                transform.position = Vector3.Lerp(rocketforcasPos, planetforcasPos, inot);
                transform.rotation = Quaternion.Lerp(rocketAngle, planetAngle, inot);
            }
        }
        else if (rc.escape)
        {
            timeReset = false;
            if (!eTimeReset)
            {
                time = 0f;
                eTimeReset = true;
            }
            else
            {
                float esct = System.Math.Min(time / rc.compleatEscapeTime, 1);
                transform.position = Vector3.Lerp(planetforcasPos, rocketforcasPos, esct);
                transform.rotation = Quaternion.Lerp(planetAngle, rocketAngle, esct);
            }
        }
        else
        {
            timeReset = false;
            eTimeReset = false;
            transform.position = rocketforcasPos;
            this.transform.rotation = Quaternion.Euler(angle, rNormalizedAngle, 0.0f);//カメラ角度変更の実行
        }
    }
}
    
