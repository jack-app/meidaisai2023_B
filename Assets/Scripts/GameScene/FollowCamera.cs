using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private GameObject obj;
    private Transform mytransform;
    [SerializeField] private float back = 100f;
    [SerializeField] private float up = 200f;
    [SerializeField] private float orbBack = 100f;
    [SerializeField] private float orbUp = 200f;
    RocketControl rc;
    // Start is called before the first frame update
    void Start()
    {
        obj = GameObject.Find("Rocket");
        mytransform = this.transform;
        rc = obj.GetComponent<RocketControl>();
    }

    
    void Update()
    {
        if(rc.inOrbit == false)//軌道の外
        {
            //パターン1
            //Vector3 centerDirection = obj.transform.position * (-1);//宇宙中心方向のベクトル
            //Vector3 nlzDirection = centerDirection.normalized;//単位ベクトル化
            //Vector3 backposition = obj.transform.position + (back * nlzDirection);//カメラのロケット後方位置
            //mytransform.position = new Vector3(backposition.x, up, backposition.z);//位置変更の実行
            //float angle = Vector3.Angle(obj.transform.position, Vector3.forward);//カメラ角度の決定
            //mytransform.rotation = Quaternion.Euler(5f, angle, 0.0f);//カメラ角度変更の実行

            //パターン２
            Vector3 Direction2 = -obj.transform.forward;//正面方向の逆ベクトル
            Vector3 backposition2 = obj.transform.position + (back * Direction2);//カメラのロケット後方位置
            mytransform.position = new Vector3(backposition2.x, up, backposition2.z);//位置変更の実行
            float angle2 = Vector3.Angle(Direction2 * (-1), Vector3.forward);//カメラ角度の決定
            mytransform.rotation = Quaternion.Euler(5f, angle2, 0.0f);//カメラ角度変更の実行
        }
        if (rc.inOrbit == true)//軌道の内
        {
            Vector3 planetPos = rc.PlanetPos;//惑星の位置
            Vector3 centerDirection = planetPos * (-1);//宇宙中心方向のベクトル
            Vector3 nlzDirection = centerDirection.normalized;//単位ベクトル化
            Vector3 backposition = planetPos + (orbBack * nlzDirection);//カメラの惑星後方位置
            mytransform.position = new Vector3(backposition.x, orbUp, backposition.z);//位置変更の実行
            float angle = Vector3.Angle(planetPos, Vector3.forward);//カメラ角度の決定
            mytransform.rotation = Quaternion.Euler(60f, angle, 0.0f);//カメラ角度変更の実行

        }
    }
}
