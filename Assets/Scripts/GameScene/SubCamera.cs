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

    public void CameraUpdate()
    {
        if (rc.inOrbit == false)//軌道の外
        {
            Vector3 centerDirection = obj.transform.position * (-1);//ロケットからの宇宙中心方向のベクトル
            Vector3 nlzDirection = centerDirection.normalized;//単位ベクトル化
            Vector3 backposition = obj.transform.position + (back * nlzDirection);//カメラのロケット後方位置
            this.transform.position = new Vector3(backposition.x, up, backposition.z);//位置変更の実行
            float　centerAngle = Vector3.Angle(obj.transform.position, Vector3.forward);//カメラ角度の決定
            var cameraAxis = Vector3.Cross(obj.transform.position, Vector3.forward).y < 0 ? -1 : 1;
            var normalizedAngle = Mathf.Repeat(-centerAngle * cameraAxis, 360);
            this.transform.rotation = Quaternion.Euler(angle, normalizedAngle, 0.0f);//カメラ角度変更の実行
            

        }
        if (rc.inOrbit == true)//軌道の内
        {
            Debug.Log(rc.orbitCenter.name);
            Vector3 planetPos = rc.orbitCenter.transform.position;//惑星の位置
            Vector3 centerDirection = planetPos * (-1);//惑星からの宇宙中心方向のベクトル
            Vector3 nlzDirection = centerDirection.normalized;//単位ベクトル化
            Vector3 backposition = planetPos + (back * nlzDirection);//カメラの惑星後方位置
            this.transform.position = new Vector3(backposition.x, up, backposition.z);//位置変更の実行
            float centerAngle = Vector3.Angle(planetPos, Vector3.forward);//カメラ角度の決定
            var cameraAxis = Vector3.Cross(planetPos, Vector3.forward).y < 0 ? -1 : 1;
            var normalizedAngle = Mathf.Repeat(-centerAngle * cameraAxis, 360);
            this.transform.rotation = Quaternion.Euler(angle, normalizedAngle, 0.0f);//カメラ角度変更の実行

        }

    }
}
    
