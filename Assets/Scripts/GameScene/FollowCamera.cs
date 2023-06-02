using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private GameObject obj;
    private Transform mytransform;
    [SerializeField] private float back = 100f;
    [SerializeField] private float up = 200f;
    [SerializeField] private float angle;
    [SerializeField] private float orbBack = 100f;
    [SerializeField] private float orbUp = 200f;
    [SerializeField] private float orbAngle = 60f;
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
            this.gameObject.transform.parent = obj.transform;
            mytransform.localPosition = new Vector3(0, up, back);
            Vector3 localAngle = mytransform.localEulerAngles;
            localAngle.x = angle; // ローカル座標を基準に、x軸を軸にした回転をangleに変更
            localAngle.y = 0f; // ローカル座標を基準に、y軸を軸にした回転を0に変更
            localAngle.z = 0f; // ローカル座標を基準に、z軸を軸にした回転を0に変更
            mytransform.localEulerAngles = localAngle; // 回転角度を設定
        }
        if (rc.inOrbit == true)//軌道の内
        {
            this.gameObject.transform.parent = null;
            Vector3 planetPos = rc.PlanetPos;//惑星の位置
            Vector3 centerDirection = planetPos * (-1);//宇宙中心方向のベクトル
            Vector3 nlzDirection = centerDirection.normalized;//単位ベクトル化
            Vector3 backposition = planetPos + (orbBack * nlzDirection);//カメラの惑星後方位置
            mytransform.position = new Vector3(backposition.x, orbUp, backposition.z);//位置変更の実行
            float centerAngle = Vector3.Angle(planetPos, Vector3.forward);//カメラ角度の決定
            mytransform.rotation = Quaternion.Euler(orbAngle, centerAngle, 0.0f);//カメラ角度変更の実行

        }
    }
}
