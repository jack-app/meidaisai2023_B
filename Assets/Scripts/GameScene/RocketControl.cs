using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RocketControl : MonoBehaviour
{
    [SerializeField] private GameManager gameManager = null!;
    [SerializeField] private float GravityCoefficient;//���L���͒萔
    [SerializeField] private float startDash;//�����x
    [SerializeField] private float escapeTime;//���E�����܂ł̎���
    [SerializeField] private float maxCharge;//charge�̍ő�l
    [SerializeField] private float charge;//charge�̒l
    [SerializeField] private float chargeSpeed;//charge�̑������x
    [SerializeField] private float exprosionCharge;//�������甚�j����charge
    [SerializeField] private float fuel;//���ړ��ŏ����l
    [SerializeField] private float maxFuel;//���ړ��񐔂̍ő�l
    [SerializeField] private float fuelRecovery;
    [SerializeField] private float spCooltime;//sp����(���ړ�)�̃N�[���^�C��
    [SerializeField] private float spHorizonSpeed;//���ړ����x
    [SerializeField] private float spConsumeFuel;//���ړ����x�ł̔R�������
    [SerializeField] private float horizonSpeed;//���ړ����x
    [SerializeField] private float breakSpeed;//���ړ����x
    [SerializeField] private float spAccelSpeed;
    [SerializeField] private float maxAngleZ;
    [SerializeField] private float spMaxAngleZ;
    [SerializeField] private float rotationSpeedZ;
    [SerializeField] private float antiRotationSpeedZ;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private ParticleSystem rightParticle;
    [SerializeField] private ParticleSystem leftParticle;
    [SerializeField] private ParticleSystem frontParticle;
    [SerializeField] private MainCamera mainCameraCon;
    public bool autoCamera;
    public bool crash { get; private set; } = false;
    public bool inOrbit = false; // �O���ɏ���Ă邩�̔���
    public bool leftAround = false; // ���v���
    public bool rightAround = false; // �����v���
    public bool escape = false; // �O������̗��E������
    public Vector3 PlanetPos; // �f���̈ʒu
    public Vector3 delta; // ���P�b�g�̑��x�x�N�g��
    private Vector3 saveVelocity; // ���x�x�N�g���̕ۑ�
    public Vector3 nowPosition { get; private set; } // ���̃��P�b�g�̈ʒu
    public float compleatEscapeTime { get; private set; }
    public GameObject orbitCenter;
    private List<GameObject> colList = new List<GameObject>();
    private Rigidbody rb;
    private Transform myTransform;
    private GameObject mainCamera;
    private GameObject subCamera;
    private GameObject ds;
    private Planet planet;
    private bool start = false; // �X�^�[�g�������̔���
    private bool emergencyAvoidance = false;//���ړ�������
    private bool spCooldown = false;//sp����̃N�[���_�E������
    private bool spAngleCount = false;
    private Vector3 prePosition; // 1�t���[���O�̃��P�b�g�̈ʒu
    private float orbitalRadius;
    private float escapeOrbitalRadius;// �O�����a
    private float mass = 1; // �f���̎���
    private float speed; // ���P�b�g�̑���
    private Vector3 relativeRocketPos;
    private float rotateSpeed;
    public float chargePower { get; private set; }
    private float angleChangeTime;
    public float rotationZ { get; private set; }
    private float carrentRotationZ;
    private float spAngleChangeTime;
    private float spAngleChangeRatio;
    private float spAngleZ;
    private float horizonInput;

    private float resultMaxSpeed = 0;//最高速度
    private float resultTime; //経過時間
    private int resultPlanetCount;//訪れた星の数
    private int resultSpCount;//緊急回避した回数
    public int resultCauseOfDeath;//死因

    public bool InGravity()
    {
        return colList.Any();
    }
    public void RemovePlanetFromColList(GameObject obj)
    {
        colList.Remove(obj);
        Debug.Log("called Remove Planet From ColList");
    }
    public float FuelAmount()
    {
        return fuel;
    }

    // Start is called before the first frame update
    private void Start()
    {
        ds = GameObject.Find("DirectionSign");
        mainCamera = GameObject.Find("Main Camera");
        subCamera = GameObject.Find("Sub Camera");      
        rb = GetComponent<Rigidbody>();
        myTransform = transform;
    }    
    
    //���x�x�N�g��(delta)�̎擾
    private void GetDeltaPos()
    {
        nowPosition = myTransform.position;
        delta = (nowPosition - prePosition) / Time.deltaTime;
        prePosition = nowPosition;
        if (start)
        { 
            resultMaxSpeed = System.Math.Max(resultMaxSpeed, delta.magnitude); 
        }
    }
    //�O������E�o����
    void CompleatEscape()
    {
        //Debug.Log("CompleatEscape");
        escape = false;
    }

    public void RocketDestroy()
    {
        gameManager.StartGameFinish(resultCauseOfDeath, resultTime, resultMaxSpeed, resultPlanetCount, resultSpCount);
        crash = true;
        mainCamera.SetActive(true);
        subCamera.SetActive(false);
        Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        Destroy(mainCameraCon);
        this.gameObject.transform.DetachChildren();
        Destroy(ds);
        Destroy(this.gameObject);
        Destroy(this);
    }

    public void RocketUpdate()
    {
        if(Input.GetKey(KeyCode.Return))
        {
            resultCauseOfDeath = 6;
            RocketDestroy();
        }
        autoCamera = Setting.autoCamera;
        GetComponent<TrailRenderer>().enabled = Setting.rocketTrajectoryExist;
        GetDeltaPos();
        if (start == false)//�X�^�[�g�_�b�V��
        {
            angleChangeTime = 0;
            if (Input.GetKey(KeyCode.Space))
            {
                Vector3 StartDirection = new Vector3(0f, 0f, startDash);
                rb.AddForce(StartDirection);
                start = true;
            }
        }

        if(start)//�X�^�[�g��fule�̎��ԓ�����̏���
        {
            fuel -= Time.deltaTime;
            resultTime += Time.deltaTime;
        }
        if(fuel <= 0)
        {
            //Debug.Log("Finish!");
            resultCauseOfDeath = 1;//死因：燃料がなくなった。
            RocketDestroy();
        }
        fuel = Mathf.Clamp(fuel, 0, maxFuel);//fuel�̍ŏ��l�ő�l����

        spAngleChangeTime += Time.deltaTime;
        spAngleChangeRatio = spAngleChangeTime / 0.2f;
        spAngleChangeRatio = Mathf.Clamp(spAngleChangeRatio, 0, 1);
        if(emergencyAvoidance)
        {
            
            if(spAngleCount)
            {
                rotationZ = Mathf.Lerp(carrentRotationZ, spAngleZ, spAngleChangeRatio);
                if (spAngleChangeRatio >= 1)
                {
                    spAngleCount = false;
                    spAngleChangeTime = 0;
                }
            }
            else
            {
                rotationZ = Mathf.Lerp(spAngleZ, 0, spAngleChangeRatio);
            }
            myTransform.rotation = Quaternion.Euler(0, myTransform.localEulerAngles.y, rotationZ);
            
        }
        else
        {
            rotationZ = maxAngleZ * angleChangeTime;//ロケットの傾き
            var rocketAngle = Vector3.Angle(delta, Vector3.forward);
            var rocketAxis = Vector3.Cross(delta, Vector3.forward).y < 0 ? -1 : 1;
            var normalizedAngle = Mathf.Repeat(-rocketAngle * rocketAxis, 360);
            myTransform.rotation = Quaternion.Euler(0, normalizedAngle, rotationZ);
            //Debug.Log(normalizedAngle);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            horizonInput = 1;
        }
        else if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            horizonInput = -1;
        }
        else
        {
            horizonInput = 0;
        }

        Vector3 forward = myTransform.forward;//���ʕ����̃x�N�g��
        Vector3 moveDirection = delta.normalized; //�ړ������̒P�ʃx�N�g��
        if (!inOrbit && start)//�O���̊O�ł̑���
        {
            Vector3 horizon = Quaternion.Euler(0, 90, 0) * moveDirection;//�������̃x�N�g��
            if (KeyManager.space.keep && fuel > spConsumeFuel && !spCooldown)
            {
                if (!(horizonInput == 0))//���ړ�緊急回避作動
                {
                    gameManager.RocketMoveAudio();
                    resultSpCount += 1;
                    ParticleSystem particle = horizonInput > 0 ? rightParticle : leftParticle;
                    particle.Play();
                    Vector3 horizonMove = horizonInput * horizon * spHorizonSpeed;
                    fuel -= spConsumeFuel;//fuel��10����
                    carrentRotationZ = rotationZ;
                    spAngleZ = -spMaxAngleZ * horizonInput;
                    emergencyAvoidance = true;                                       
                    spCooldown = true;//sp�N�[���_�E��on
                    spAngleCount = true;
                    angleChangeTime = 0;
                    spAngleChangeTime = 0;
                    rb.velocity += horizonMove;//���ړ��̎��s
                    Invoke("cooldown", spCooltime);                   
                    StartCoroutine("antiHorizon", horizonMove);//���ړ���~�̌Ăяo��
                }
                if (KeyManager.vertical.keep && KeyManager.vertical.axis > 0)
                {
                    gameManager.RocketMoveAudio();
                    frontParticle.Play();
                    Vector3 accelMove = moveDirection * spAccelSpeed;
                    fuel -= spConsumeFuel;//fuel��10����
                    spCooldown = true;//sp�N�[���_�E��on
                    rb.velocity += accelMove;//���ړ��̎��s
                    Invoke("cooldown", spCooltime);
                }
            }
            else if(!emergencyAvoidance)
            {
                if (KeyManager.horizontal.keep)//横移動
                {
                    float rocketAngle = Vector3.Angle(moveDirection, myTransform.position);
                    float rocketAxis = Vector3.Cross(moveDirection, myTransform.position).y < 0 ? -1 : 1;
                    float angle = rocketAngle * rocketAxis;
                    Vector3 horizonSize = horizon * horizonSpeed;
                    angleChangeTime = Mathf.Clamp(angleChangeTime, -1, 1);
                    if(angle < 90 && KeyManager.horizontal.axis < 0)//左
                    {
                        rb.AddForce(-horizonSize);
                        if(angleChangeTime >= 0)
                        {
                            angleChangeTime += rotationSpeedZ;
                        }
                        if (angleChangeTime < 0)
                        {
                            angleChangeTime += antiRotationSpeedZ;
                        }

                    }
                    if (angle > -90 && KeyManager.horizontal.axis > 0)//右
                    {
                        rb.AddForce(horizonSize);
                        if (angleChangeTime <= 0)
                        {
                            angleChangeTime -= rotationSpeedZ;
                        }
                        if (angleChangeTime > 0)
                        {
                            angleChangeTime -= antiRotationSpeedZ;
                        }
                    }
                }
                else//横移動していないとき角度を元に戻す
                {
                    if(angleChangeTime < 0)
                    {
                        angleChangeTime += antiRotationSpeedZ;
                        angleChangeTime = System.Math.Min(angleChangeTime, 0);    
                    }
                    if(angleChangeTime > 0)
                    {
                        angleChangeTime -= antiRotationSpeedZ;
                        angleChangeTime = System.Math.Max(angleChangeTime, 0);
                    }
                }

                if (KeyManager.vertical.axis < 0 && rb.velocity.magnitude > 100f)//ブレーキ
                {
                    rb.AddForce(-moveDirection * breakSpeed);
                }
            }
        }

        if (inOrbit)//�O���̒��ł̑���
        {
            Debug.Log("inOrbit");
            if (angleChangeTime < 0)//角度Zを0に戻す
            {
                angleChangeTime += antiRotationSpeedZ;
                angleChangeTime = System.Math.Min(angleChangeTime, 0);
            }
            if (angleChangeTime > 0)
            {
                angleChangeTime -= antiRotationSpeedZ;
                angleChangeTime = System.Math.Max(angleChangeTime, 0);
            }

            chargePower = System.Math.Min(charge, 1);//charge�̍ő�l����
            float escapeSpeed = saveVelocity.magnitude;//�O�����E���̑���
            if (KeyManager.space.keep)
            {
                Debug.Log("charge");
                charge += chargeSpeed;//�X�y�[�X�������charge�𑝉�
            }
            else if (chargePower > 0)//�X�y�[�X�L�[�𗣂����Ƃ�
            {
                Debug.Log("Escape");
                if (autoCamera)
                {
                    mainCamera.SetActive(true);
                    subCamera.SetActive(false);
                }
                gameManager.RocketMoveAudio();
                frontParticle.Play();
                rb.velocity = forward * (escapeSpeed + (maxCharge * chargePower));//���ʕ����ɑ��x��^����           
                charge = 0f;//charge�̃��Z�b�g
                escape = true;//escape������on
                compleatEscapeTime = Mathf.Pow((Mathf.Pow(escapeOrbitalRadius, 2) - Mathf.Pow(relativeRocketPos.magnitude, 2)), 0.5f) / escapeSpeed + escapeTime;
                Invoke("CompleatEscape",compleatEscapeTime);//escape�����̌Ăяo��
            }
            if(charge > exprosionCharge)//charge���������Ƃ����j
            {
                //Debug.Log("Explosion!");//��
                resultCauseOfDeath = 3;//死因：力を貯めすぎた。
                RocketDestroy();
            }
        }
        //Debug.Log(speedVector.magnitude);
        //�O���ɏ���Ă邩����
        if(leftAround == true || rightAround == true)
        {
            inOrbit = true;
        }
        else
        {
            inOrbit = false;
        }

        if(escape == true)//�O�����E��������
        {
            leftAround = false;
            rightAround = false;
        }
    }

    private void OnCollisionEnter(Collision collision) //�Ԃ������Ƃ�
    {
        //Debug.Log("CrashExprosion!");//��
        resultCauseOfDeath = 2;//死因：星に衝突した。
        RocketDestroy();
    }

    private void OnTriggerEnter(Collider collider) //�d�͌��ɓ������Ƃ�
    {
        //�߂Â����f���̏��̎擾
        if (collider.gameObject.tag == "Planet" && !colList.Contains(collider.gameObject))
        {
            colList.Add(collider.gameObject);
        }
        if(collider.gameObject.tag == "BlackHole")
        {
            resultCauseOfDeath = 5;
            gameManager.StartGameFinish(resultCauseOfDeath, resultTime, resultMaxSpeed, resultPlanetCount, resultSpCount);
            crash = true;
            mainCamera.SetActive(true);
            subCamera.SetActive(false);
            this.gameObject.transform.DetachChildren();
            Destroy(ds);
            Destroy(this.gameObject);
            Destroy(this);
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Planet" && colList.Contains(collider.gameObject))
        {
            colList.Remove(collider.gameObject);
        }
    }

    private void OnTriggerStay(Collider collider) //�d�͌��ɂ���Ƃ�
    {
        if (!escape && !emergencyAvoidance)//�O�����E��������
        {
            if (collider.gameObject.tag == "Planet")
            {
                foreach (GameObject planetObject in colList)
                {
                    planet = planetObject.GetComponent<Planet>();
                    mass = planet.mass;
                    orbitalRadius = planet.planetRadius * 2;
                    //Debug.Log(colList.Count);
                    //�d��
                    //�߂Â����f���̍��W�̎擾
                    PlanetPos = planetObject.transform.position;
                    //���P�b�g����f���̒��S�֌������x�N�g���̎擾
                    Vector3 gravityDirection = PlanetPos - myTransform.position;
                    //���P�b�g�Ƙf���̋����̎擾
                    float GravityLength = gravityDirection.magnitude;
                    //Debug.Log(GravityLength);
                    //�P�ʃx�N�g���̎擾
                    Vector3 GravityNlz = gravityDirection.normalized;
                    //�d�͂̌v�Z
                    Vector3 GravityForth = GravityNlz * GravityCoefficient * mass / Mathf.Pow(GravityLength, 2);

                    if (GravityLength < orbitalRadius)
                    {
                        //����̏ꍇ
                        Debug.Log("Inside");
                        //Debug.Log(planet.name);
                        //���x�x�N�g���Ƙf���Ɍ����������̃x�N�g���̂Ȃ��p                
                        var axis = Vector3.Cross(gravityDirection, delta).y < 0 ? -1 : 1;//�O�όv�Z(�Ȃ��p��-180����180�ɂ���̂ɕK�v)
                        var angle = Vector3.Angle(gravityDirection, delta) * (axis);//�Ȃ��p
                                                                                    //�Ȃ��p��_�E0����180_��0����-180                          
                                                                                    //Debug.Log(angle);

                        if (angle > 85 && !leftAround && !rightAround) //�����v���
                        {
                            escapeOrbitalRadius = orbitalRadius;
                            relativeRocketPos = -gravityDirection;//�O���̔��a�ƂȂ�x�N�g���̎擾
                            speed = delta.magnitude;//�����̌v�Z
                            saveVelocity = rb.velocity;//���x�̕ۑ�
                            rb.velocity = Vector3.zero;//Rigidbody�̋@�\��~
                            rightAround = true;//�����v��蔻��ON
                            orbitCenter = planetObject;
                            fuel += fuelRecovery;
                            resultPlanetCount += 1;
                            if (autoCamera)
                            {
                                mainCamera.SetActive(false);
                                subCamera.SetActive(true);
                            }
                        }

                        if (angle < -85 && !leftAround && !rightAround) //���v���
                        {
                            escapeOrbitalRadius = orbitalRadius;
                            relativeRocketPos = -gravityDirection;//�O���̔��a�ƂȂ�x�N�g���̎擾
                            speed = delta.magnitude;//�����̌v�Z
                            saveVelocity = rb.velocity;//���x�̕ۑ�
                            rb.velocity = Vector3.zero;//Rigidbody�̋@�\��~
                            leftAround = true;//���v��蔻��ON
                            orbitCenter = planetObject;
                            fuel += fuelRecovery;
                            resultPlanetCount += 1;
                            if (autoCamera)
                            {
                                mainCamera.SetActive(false);
                                subCamera.SetActive(true);
                            }
                        }
                    }

                    if (rightAround)//�����v�����s
                    {
                        rotateSpeed = Mathf.Rad2Deg * ((speed + (maxCharge * chargePower)) / relativeRocketPos.magnitude);//�p���x�̌v�Z
                        relativeRocketPos = Quaternion.Euler(0, -rotateSpeed * Time.deltaTime / Mathf.Pow(colList.Count, 2), 0) * relativeRocketPos;//�x�N�g���̉�]
                        Vector3 rocketPos = relativeRocketPos + orbitCenter.transform.position;//���P�b�g�̈ʒu����
                        this.gameObject.transform.position = rocketPos;//��]�̎��s
                        nowPosition = rocketPos;
                        /*
                        myTransform.RotateAround(PlanetPos, Vector3.up, -rotateSpeed * Time.deltaTime);//��]�̎��s
                        myTransform.position += planet.plaDelta;
                        */
                    }
                    else if (leftAround)
                    {
                        rotateSpeed = Mathf.Rad2Deg * ((speed + (maxCharge * chargePower)) / relativeRocketPos.magnitude);//�p���x�̌v�Z
                        relativeRocketPos = Quaternion.Euler(0, rotateSpeed * Time.deltaTime / Mathf.Pow(colList.Count, 2), 0) * relativeRocketPos;//�x�N�g���̉�]
                        Vector3 rocketPos = relativeRocketPos + orbitCenter.transform.position;//���P�b�g�̈ʒu����
                        this.gameObject.transform.position = rocketPos;//��]�̎��s
                        nowPosition = rocketPos;
                        /*
                        myTransform.RotateAround(PlanetPos, Vector3.up, rotateSpeed * Time.deltaTime);//��]�̎��s
                        myTransform.position += planet.plaDelta;
                        */
                    }
                    else if (!inOrbit)
                    {
                        //�O���̏ꍇ
                        //Debug.Log("Outside");
                        //�d�͂�^����
                        rb.AddForce(GravityForth);
                    }
                }

            }
        }
    }
    private IEnumerator antiHorizon(Vector3 horizonMove)//���ړ��̒�~ 
    {
        yield return new WaitForSeconds(0.35f);
        rb.velocity -= horizonMove;//���ړ��̒�~�̎��s
        yield return new WaitForSeconds(0.05f);
        emergencyAvoidance = false;//���ړ�����off
    }
    private void cooldown()//sp����N�[���_�E�����
    {
        spCooldown = false;
    }
}
