using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketControl : MonoBehaviour
{
    [SerializeField] private float GravityCoefficient;//万有引力定数
    [SerializeField] private float orbitSpeedBounus;//軌道突入時のスピード感UP演出の倍率
    [SerializeField] private float startDash;//初速度
    [SerializeField] private float escapeTime;//離脱完了までの時間
    [SerializeField] private float maxCharge;
    [SerializeField] private float charge;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float Air;
    [SerializeField] private float AirDecrease;
    [SerializeField] private float AirIncrease;
    [SerializeField] private float HorizonSpeed;
    public bool inOrbit = false; // 軌道に乗ってるかの判定
    public bool leftAround = false; // 時計回り
    public bool rightAround = false; // 反時計回り
    public Vector3 saveVelocity; // 速度ベクトルの保存
    public Vector3 PlanetPos; // 惑星の位置
    public Vector3 delta; // ロケットの速度ベクトル
    private Rigidbody rb;
    private Transform myTransform;
    private GameObject planetObject;
    private Planet planet;
    private bool start = false; // スタートしたかの判定
    private bool escape = false; // 軌道からの離脱中判定
    private Vector3 prePosition; // 1フレーム前のロケットの位置
    private Vector3 nowPosition; // 今のロケットの位置
    private float orbitalRadius; // 軌道半径
    private float mass = 1; // 惑星の質量
    private float speed; // ロケットの速さ

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        myTransform = transform;
    }    
    
    //速度ベクトル(delta)の取得
    void GetDeltaPos()
    {
        nowPosition = myTransform.position;
        delta = (nowPosition - prePosition) / Time.deltaTime;
        prePosition = nowPosition;
    }

    void CompleatEscape()
    {
        escape = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetDeltaPos();
        if (start == false)//スタートダッシュ
        {
            if (Input.GetButtonDown("Jump"))
            {
                Vector3 StartDirection = new Vector3(0f, 0f, startDash);
                rb.AddForce(StartDirection);
                start = true;
            }
        }
        Vector3 forward = myTransform.forward;//正面方向のベクトル
        Vector3 moveDirection = delta.normalized; //移動方向の単位ベクトル      
        var result = Quaternion.Euler(0, 90, 0) * moveDirection;//横方向のベクトル
        Vector3 HorizonMove = result * HorizonSpeed;
        if (Input.GetButton("Horizontal"))//横移動
        {
            if (Air > 0)
            {
                Air -= AirDecrease;
                rb.AddForce(Input.GetAxis("Horizontal") * HorizonMove);
            }
        }
        if (inOrbit == true)//軌道の中での操作
        {
            charge = System.Math.Min(charge, maxCharge);//chargeの最大値制限
            float escapeSpeed = saveVelocity.magnitude;//軌道離脱時の速さ

            if (Input.GetButton("Jump"))
            {
                charge += chargeSpeed;//スペースを押す間chargeを増加
            }


            if (Input.GetButtonUp("Jump"))
            {
                rb.velocity = forward * escapeSpeed * charge;//正面方向に速度を与える           
                charge = 1;
                escape = true;
                Invoke("CompleatEscape", escapeTime);
            }
        }
        //Debug.Log(speedVector.magnitude);
        //軌道に乗ってるか判定
        if(leftAround == true || rightAround == true)
        {
            inOrbit = true;
        }
        else
        {
            inOrbit = false;
        }

        if(escape == true)//軌道離脱中か判定
        {
            leftAround = false;
            rightAround = false;
        }        
        var rocketAngle = Vector3.Angle(delta, Vector3.forward);
        var rocketAxis = Vector3.Cross(delta, Vector3.forward).y < 0 ? -1 : 1;
        var normalizedAngle = Mathf.Repeat(-rocketAngle * rocketAxis, 360);
        myTransform.rotation = Quaternion.Euler(0, normalizedAngle, 0);
        Debug.Log(normalizedAngle);
    }

    void OnTriggerEnter(Collider collider)
    {
        //近づいた惑星の情報の取得
        if (collider.gameObject.tag == "Planet")
        {
            planet = collider.GetComponent<Planet>();
            mass = planet.mass;
            orbitalRadius = planet.orbitLevel1;
        }
        planetObject = collider.gameObject;
    }

    void OnTriggerStay(Collider collider)
    {
        if (escape == false)//軌道離脱中か判定
        {
            if (collider.gameObject.tag == "Planet")
            {
                //Debug.Log(collider.gameObject.name);
                //重力
                //近づいた惑星の座標の取得
                PlanetPos = collider.gameObject.transform.position;
                //ロケットから惑星の中心へ向かうベクトルの取得
                Vector3 gravityDirection = PlanetPos - myTransform.position;
                //ロケットと惑星の距離の取得
                float GravityLength = gravityDirection.magnitude;
                //Debug.Log(GravityLength);
                //単位ベクトルの取得
                Vector3 GravityNlz = gravityDirection.normalized;
                //重力の計算
                Vector3 GravityForth = GravityNlz * GravityCoefficient * mass / Mathf.Pow(GravityLength, 2);

                if (GravityLength < orbitalRadius)
                {
                    //内側の場合
                    Debug.Log("Inside");
                    //速度ベクトルと惑星に向かう方向のベクトルのなす角                
                    var axis = Vector3.Cross(gravityDirection, delta).y < 0 ? -1 : 1;//外積計算(なす角を-180から180にするのに必要)
                    var angle = Vector3.Angle(gravityDirection, delta) * (axis);//なす角
                                                                                      //なす角は_右0から180_左0から-180                          
                                                                                      //Debug.Log(angle);

                    if (angle > 90 && rightAround == false) //反時計回り
                    {
                        speed = delta.magnitude * orbitSpeedBounus;//速さの計算
                        saveVelocity = rb.velocity;//速度の保存
                        rb.velocity = Vector3.zero;//Rigidbodyの機能停止
                        rightAround = true;//反時計回り判定ON
                    }
                    if (rightAround == true)//反時計回り実行
                    {
                        Vector3 relativeRocketPos = this.gameObject.transform.position - planetObject.transform.position;
                        float rotateSpeed = Mathf.Rad2Deg * (speed / GravityLength);//角速度の計算
                        Vector3 movedVector = Quaternion.Euler(0, -rotateSpeed * Time.deltaTime, 0) * relativeRocketPos;
                        Vector3 rocketPos = movedVector + planetObject.transform.position;
                        this.gameObject.transform.position = rocketPos;
                        nowPosition = rocketPos;
                        
                        /*
                        myTransform.RotateAround(PlanetPos, Vector3.up, -rotateSpeed * Time.deltaTime);//回転の実行
                        myTransform.position += planet.plaDelta;
                        */
                    }

                    if (angle < -90 && leftAround == false) //時計回り
                    {
                        speed = delta.magnitude * orbitSpeedBounus;//速さの計算
                        saveVelocity = rb.velocity;//速度の保存
                        rb.velocity = Vector3.zero;//Rigidbodyの機能停止
                        leftAround = true;//時計回り判定ON
                    }
                    if (leftAround == true)
                    {
                        Vector3 relativeRocketPos = this.gameObject.transform.position - planetObject.transform.position;
                        float rotateSpeed = Mathf.Rad2Deg * (speed / GravityLength);//角速度の計算
                        Vector3 movedVector = Quaternion.Euler(0, rotateSpeed * Time.deltaTime, 0) * relativeRocketPos;
                        Vector3 rocketPos = movedVector + planetObject.transform.position;
                        this.gameObject.transform.position = rocketPos;
                        nowPosition = rocketPos;
                        /*
                        myTransform.RotateAround(PlanetPos, Vector3.up, rotateSpeed * Time.deltaTime);//回転の実行
                        myTransform.position += planet.plaDelta;
                        */
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
