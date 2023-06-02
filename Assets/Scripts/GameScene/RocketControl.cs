using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketControl : MonoBehaviour
{
    List<GameObject> colList = new List<GameObject>();
    [SerializeField] private float GravityCoefficient;//万有引力定数
    [SerializeField] private float orbitSpeedBounus;//軌道突入時のスピード感UP演出の倍率
    [SerializeField] private float startDash;//初速度
    [SerializeField] private float escapeTime;//離脱完了までの時間
    [SerializeField] private float maxCharge;//chargeの最大値
    [SerializeField] private float charge;//chargeの値
    [SerializeField] private float chargeSpeed;//chargeの増加速度
    [SerializeField] private float exprosionCharge;//超えたら爆破するcharge
    [SerializeField] private float fuel;//横移動で消費する値
    [SerializeField] private float maxFuel;//横移動回数の最大値
    [SerializeField] private float spCooltime;//sp操作(横移動)のクールタイム
    [SerializeField] private float horizonSpeed;//横移動速度
    [SerializeField] private float spFuel;//横移動速度での燃料消費量
    [SerializeField] GameObject explosionPrefab;
    public bool inOrbit = false; // 軌道に乗ってるかの判定
    public bool leftAround = false; // 時計回り
    public bool rightAround = false; // 反時計回り
    public Vector3 saveVelocity; // 速度ベクトルの保存
    public Vector3 PlanetPos; // 惑星の位置
    public Vector3 delta; // ロケットの速度ベクトル
    private Rigidbody rb;
    private Transform myTransform;
    private GameObject planetObject;
    public GameObject orbitCenter;
    private Planet planet;
    private bool start = false; // スタートしたかの判定
    private bool escape = false; // 軌道からの離脱中判定
    private bool emergencyAvoidance = false;//横移動中判定
    private bool spCooldown = false;//sp操作のクールダウン判定
    private Vector3 prePosition; // 1フレーム前のロケットの位置
    private Vector3 nowPosition; // 今のロケットの位置
    private float orbitalRadius; // 軌道半径
    private float mass = 1; // 惑星の質量
    private float speed; // ロケットの速さ
    private Vector3 relativeRocketPos;

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
        //Debug.Log(delta.magnitude);
    }
    //軌道から脱出完了
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

        if(start)//スタート後fuleの時間当たりの消費
        {
            fuel -= Time.deltaTime;
        }
        if(fuel <= 0)
        {
            Debug.Log("Finish!");
        }
        fuel = Mathf.Clamp(fuel, 0, maxFuel);//fuelの最小値最大値制限

        Vector3 forward = myTransform.forward;//正面方向のベクトル
        Vector3 moveDirection = delta.normalized; //移動方向の単位ベクトル
        if (!inOrbit)//軌道の外での操作
        {
            Vector3 horizon = Quaternion.Euler(0, 90, 0) * moveDirection;//横方向のベクトル
            Vector3 horizonSize = horizon * horizonSpeed;
            if (Input.GetButtonDown("Horizontal"))//横移動
            {
                if (fuel > spFuel && !spCooldown)
                {
                    Vector3 horizonMove = Input.GetAxis("Horizontal") * horizonSize;
                    fuel -= spFuel;//fuelを10消費
                    emergencyAvoidance = true;//横移動判定on
                    spCooldown = true;//spクールダウンon
                    rb.AddForce(horizonMove);//横移動の実行
                    Invoke("cooldown", spCooltime);
                    StartCoroutine("antiHorizon", horizonMove);//横移動停止の呼び出し
                }
            }
        }

        if (inOrbit)//軌道の中での操作
        {
            Debug.Log("inOrbit");

            float chargePower = System.Math.Min(charge, maxCharge);//chargeの最大値制限
            float escapeSpeed = saveVelocity.magnitude;//軌道離脱時の速さ

            if (Input.GetButton("Jump"))
            {
                Debug.Log("charge");
                charge += chargeSpeed;//スペースを押す間chargeを増加
            }
            if (Input.GetButtonUp("Jump"))//スペースキーを離したとき
            {
                Debug.Log("Escape");
                rb.velocity = forward * escapeSpeed * chargePower;//正面方向に速度を与える           
                charge = 1.05f;//chargeのリセット
                escape = true;//escape中判定on
                Invoke("CompleatEscape", escapeTime);//escape完了の呼び出し
            }
            if(charge > exprosionCharge)//chargeしすぎたとき爆破
            {
                Debug.Log("Explosion!");//仮
                this.gameObject.transform.DetachChildren();
                Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
                Destroy(this.gameObject);
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
        //ロケットの角度調整
        if (!emergencyAvoidance)//横移動中は向きを変えない
        {
            var rocketAngle = Vector3.Angle(delta, Vector3.forward);
            var rocketAxis = Vector3.Cross(delta, Vector3.forward).y < 0 ? -1 : 1;
            var normalizedAngle = Mathf.Repeat(-rocketAngle * rocketAxis, 360);
            myTransform.rotation = Quaternion.Euler(0, normalizedAngle, 0);
            //Debug.Log(normalizedAngle);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        //近づいた惑星の情報の取得
        if (collider.gameObject.tag == "Planet" && !colList.Contains(collider.gameObject))
        {
            colList.Add(collider.gameObject);
            /*planet = collider.GetComponent<Planet>();
            mass = planet.mass;
            orbitalRadius = planet.orbitLevel1;
            */
        }
        //planetObject = collider.gameObject;
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Planet" && colList.Contains(collider.gameObject))
        {
            colList.Remove(collider.gameObject);
        }
    }
    void OnTriggerStay(Collider collider)
    {
        if (escape == false)//軌道離脱中か判定
        {
            if (collider.gameObject.tag == "Planet")
            {
                foreach (GameObject planetObject in colList)
                {
                    planet = planetObject.GetComponent<Planet>();
                    mass = planet.mass;
                    orbitalRadius = planet.orbitLevel1;
                    Debug.Log(planetObject.name);
                    //重力
                    //近づいた惑星の座標の取得
                    PlanetPos = planetObject.transform.position;
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
                        //Debug.Log(planet.name);
                        //速度ベクトルと惑星に向かう方向のベクトルのなす角                
                        var axis = Vector3.Cross(gravityDirection, delta).y < 0 ? -1 : 1;//外積計算(なす角を-180から180にするのに必要)
                        var angle = Vector3.Angle(gravityDirection, delta) * (axis);//なす角
                                                                                    //なす角は_右0から180_左0から-180                          
                                                                                    //Debug.Log(angle);

                        if (angle > 90 && inOrbit == false) //反時計回り
                        {
                            relativeRocketPos = -gravityDirection;//軌道の半径となるベクトルの取得
                            speed = delta.magnitude * orbitSpeedBounus;//速さの計算
                            saveVelocity = rb.velocity;//速度の保存
                            rb.velocity = Vector3.zero;//Rigidbodyの機能停止
                            rightAround = true;//反時計回り判定ON
                            orbitCenter = planetObject;
                        }
                        if (rightAround == true)//反時計回り実行
                        {
                            Debug.Log(relativeRocketPos.magnitude);
                            float rotateSpeed = Mathf.Rad2Deg * (speed / relativeRocketPos.magnitude);//角速度の計算                         
                            relativeRocketPos = Quaternion.Euler(0, -rotateSpeed * Time.deltaTime , 0) * relativeRocketPos;//ベクトルの回転
                            Vector3 rocketPos = relativeRocketPos + orbitCenter.transform.position;//ロケットの位置決定
                            this.gameObject.transform.position = rocketPos;//回転の実行
                            nowPosition = rocketPos;
                            /*
                            myTransform.RotateAround(PlanetPos, Vector3.up, -rotateSpeed * Time.deltaTime);//回転の実行
                            myTransform.position += planet.plaDelta;
                            */
                        }

                        if (angle < -90 && inOrbit == false) //時計回り
                        {
                            relativeRocketPos = -gravityDirection;//軌道の半径となるベクトルの取得
                            speed = delta.magnitude * orbitSpeedBounus;//速さの計算
                            saveVelocity = rb.velocity;//速度の保存
                            rb.velocity = Vector3.zero;//Rigidbodyの機能停止
                            leftAround = true;//時計回り判定ON
                            orbitCenter = planetObject;
                        }
                        if (leftAround == true)
                        {
                            float rotateSpeed = Mathf.Rad2Deg * (speed / relativeRocketPos.magnitude);//角速度の計算
                            relativeRocketPos = Quaternion.Euler(0, rotateSpeed * Time.deltaTime, 0) * relativeRocketPos;//ベクトルの回転
                            Vector3 rocketPos = relativeRocketPos + orbitCenter.transform.position;//ロケットの位置決定
                            this.gameObject.transform.position = rocketPos;//回転の実行
                            nowPosition = rocketPos;
                            /*
                            myTransform.RotateAround(PlanetPos, Vector3.up, rotateSpeed * Time.deltaTime);//回転の実行
                            myTransform.position += planet.plaDelta;
                            */
                        }
                    }
                    else if (!inOrbit)
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
    private IEnumerator antiHorizon(Vector3 horizonMove)//横移動の停止 
    {
        yield return new WaitForSeconds(0.5f);
        rb.AddForce(-horizonMove);//横移動の停止の実行
        yield return new WaitForSeconds(0.1f);
        emergencyAvoidance = false;//横移動判定off
    }
    private void cooldown()//sp操作クールダウン解除
    {
        spCooldown = false;
    }

    void OnCollisionEnter(Collision collision)//衝突判定
    {
        Debug.Log("CrashExprosion!");//仮
        Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        this.gameObject.transform.DetachChildren();
        Destroy(this.gameObject);
    }
}
