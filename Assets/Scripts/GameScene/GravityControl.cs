using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityControl : MonoBehaviour
{
    [SerializeField] private float GravityCoefficient;//万有引力定数
    [SerializeField] private float orbitSpeedBounus;//軌道突入時のスピード感UP演出の倍率
    private float orbitalRadius;//軌道半径
    private Rigidbody rb;
    private Transform myTransform; 
    private Vector3 moveDirection;
    private float Mass = 1;//惑星の質量
    public Vector3 PlanetPos;//惑星の位置
    public Vector3 speedVector;//ロケットの速度ベクトル
    private float speed;//ロケットの速さ
    public bool InOrbit = false;//軌道に乗ってるかの判定
    public bool leftAround = false;//時計回り
    public bool rightAround = false;//反時計回り
    public Vector3 saveVelocity;//速度ベクトルの保存
    PlanetInfo planetInfo;
    JetControl jetControl;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jetControl = GetComponent<JetControl>();
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        //ｙ座標を０に固定
        Vector3 pos = myTransform.position;
        pos.y = 0.0f;
        transform.position = pos;
        //速度ベクトル取得
        speedVector = jetControl.delta;
        //Debug.Log(speedVector.magnitude);
        //軌道に乗ってるか判定
        if(leftAround ==true || rightAround == true)
        {
            InOrbit = true;
        }
        else
        {
            InOrbit = false;
        }

        if(jetControl.escape == true)//軌道離脱中か判定
        {
            leftAround = false;
            rightAround = false;
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        //近づいた惑星の情報の取得
        if (collider.gameObject.tag == "Planet")
        {
            planetInfo = collider.GetComponent<PlanetInfo>();
            Mass = planetInfo.mass;
            orbitalRadius = planetInfo.orbitLevel1;
        }

    }

    void OnTriggerStay(Collider collider)
    {
        if (jetControl.escape == false)//軌道離脱中か判定
        {
            if (collider.gameObject.tag == "Planet")
            {
                //Debug.Log(collider.gameObject.name);
                //重力
                //近づいた惑星の座標の取得
                PlanetPos = collider.gameObject.transform.position;
                //ロケットから惑星の中心へ向かうベクトルの取得
                Vector3 GravityDirection = PlanetPos - myTransform.position;
                //ロケットと惑星の距離の取得
                float GravityLength = GravityDirection.magnitude;
                Debug.Log(GravityLength);
                //単位ベクトルの取得
                Vector3 GravityNlz = GravityDirection.normalized;
                //重力の計算
                Vector3 GravityForth = GravityNlz * GravityCoefficient * Mass / Mathf.Pow(GravityLength, 2);

                if (GravityLength < orbitalRadius)
                {
                    //内側の場合
                    Debug.Log("Inside");
                    //速度ベクトルと惑星に向かう方向のベクトルのなす角                
                    var axis = Vector3.Cross(GravityDirection, speedVector).y < 0 ? -1 : 1;//外積計算(なす角を-180から180にするのに必要)
                    var angle = Vector3.Angle(GravityDirection, speedVector) * (axis);//なす角
                                                                                      //なす角は_右0から180_左0から-180                          
                                                                                      //Debug.Log(angle);

                    if (angle > 90 && rightAround == false) //反時計回り
                    {
                        speed = 10 * speedVector.magnitude * orbitSpeedBounus;//速さの計算
                        saveVelocity = rb.velocity;//速度の保存
                        rb.velocity = Vector3.zero;//Rigidbodyの機能停止
                        rightAround = true;//反時計回り判定ON
                    }
                    if (rightAround == true)//反時計回り実行
                    {
                        float rotateSpeed = Mathf.Rad2Deg * (speed / GravityLength);//角速度の計算
                        myTransform.RotateAround(PlanetPos, Vector3.up, -rotateSpeed * Time.deltaTime);//回転の実行
                        myTransform.position += planetInfo.plaDelta;
                    }

                    if (angle < -90 && leftAround == false) //時計回り
                    {
                        speed = 10 * speedVector.magnitude * orbitSpeedBounus;//速さの計算
                        saveVelocity = rb.velocity;//速度の保存
                        rb.velocity = Vector3.zero;//Rigidbodyの機能停止
                        leftAround = true;//時計回り判定ON
                    }
                    if (leftAround == true)
                    {
                        float rotateSpeed = Mathf.Rad2Deg * (speed / GravityLength);//角速度の計算
                        myTransform.RotateAround(PlanetPos, Vector3.up, rotateSpeed * Time.deltaTime);//回転の実行
                        myTransform.position += planetInfo.plaDelta;
                    }
                }
                else
                {
                    //外側の場合
                    Debug.Log("Outside");                
                    //重力を与える
                    rb.AddForce(GravityForth);
                }

            }
        }
    }
}
