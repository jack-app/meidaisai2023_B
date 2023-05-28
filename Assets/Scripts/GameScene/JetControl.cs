using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetControl : MonoBehaviour
{
    [SerializeField] private float startDash;//初速度
    [SerializeField] private float escapeTime;//離脱完了までの時間
    [SerializeField] private float maxCharge;
    [SerializeField] private float charge;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float Air;
    [SerializeField] private float AirDecrease;
    [SerializeField] private float AirIncrease;
    [SerializeField] private float HorizonSpeed;
    private Rigidbody rb;
    GravityControl gc;
    private bool inOrbit = false;
    private bool start = false;//スタートしたかの判定
    public bool escape = false;//軌道からの離脱中判定
    private Transform myTransform;
    private Vector3 prePosition;
    private Vector3 nowPosition;
    public Vector3 delta;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gc = GetComponent<GravityControl>();
        myTransform = this.transform;
        prePosition = myTransform.position;
        InvokeRepeating("Delta", 0.0f, 0.1f);//速度ベクトルの計算頻度_注_変更した場合GravityControlの角速度計算の変更も必要
    }

    //速度ベクトル(delta)の取得
    void Delta()
    {
        nowPosition = myTransform.position;
        delta = nowPosition - prePosition;
         prePosition = nowPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(start ==false)//スタートダッシュ
        {
            if (Input.GetButtonDown("Jump"))
            {
                Vector3 StartDirection = new Vector3(0f, 0f, startDash);
                rb.AddForce(StartDirection);
                start = true;
            }
        }
        inOrbit = gc.InOrbit;
        Vector3 forward = myTransform.forward;//正面方向のベクトル
        Vector3 moveDirection = delta.normalized; //移動方向の単位ベクトル      
        var result = Quaternion.Euler(0, 90, 0) * moveDirection;//横方向のベクトル
        Vector3 HorizonMove = result * HorizonSpeed;
        if(Input.GetButton("Horizontal"))//横移動
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
            float escapeSpeed = gc.saveVelocity.magnitude;//軌道離脱時の速さ
            
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
    }
    void CompleatEscape()
    {
        escape = false;
    }
}
