using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCamera : MonoBehaviour
{
    private GameObject obj;
    private Transform mytransform;
    [SerializeField] private float back = 100f;
    [SerializeField] private float up = 200f;
    [SerializeField] private float angle;
    RocketControl rc;

    // Start is called before the first frame update
    void Start()
    {
        obj = GameObject.Find("Rocket");
        mytransform = this.transform;
        rc = obj.GetComponent<RocketControl>();

    }

    // Update is called once per frame
    void Update()
    {
        if (rc.inOrbit == false)//軌道の外
        {
            Vector3 centerDirection = obj.transform.position * (-1);//ロケットからの宇宙中心方向のベクトル
            Vector3 nlzDirection = centerDirection.normalized;//単位ベクトル化
            Vector3 backposition = obj.transform.position + (back * nlzDirection);//カメラのロケット後方位置
            mytransform.position = new Vector3(backposition.x, up, backposition.z);//位置変更の実行
            float　centerAngle = Vector3.Angle(obj.transform.position, Vector3.forward);//カメラ角度の決定
            mytransform.rotation = Quaternion.Euler(angle, centerAngle, 0.0f);//カメラ角度変更の実行
            

        }
        if (rc.inOrbit == true)//軌道の内
        {
            Vector3 planetPos = rc.orbitCenter.transform.position;//惑星の位置
            Vector3 centerDirection = planetPos * (-1);//惑星からの宇宙中心方向のベクトル
            Vector3 nlzDirection = centerDirection.normalized;//単位ベクトル化
            Vector3 backposition = planetPos + (back * nlzDirection);//カメラの惑星後方位置
            mytransform.position = new Vector3(backposition.x, up, backposition.z);//位置変更の実行
            float centerAngle = Vector3.Angle(planetPos, Vector3.forward);//カメラ角度の決定
            mytransform.rotation = Quaternion.Euler(angle, centerAngle, 0.0f);//カメラ角度変更の実行

        }

    }
}
    
