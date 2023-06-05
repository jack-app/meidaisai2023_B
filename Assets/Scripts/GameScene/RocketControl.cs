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
    [SerializeField] GameObject explosionPrefab;
    public bool autoCamera;
    public bool crash { get; private set; } = false;
    public bool inOrbit = false; // �O���ɏ���Ă邩�̔���
    public bool leftAround = false; // ���v���
    public bool rightAround = false; // �����v���
    public Vector3 saveVelocity; // ���x�x�N�g���̕ۑ�
    public Vector3 PlanetPos; // �f���̈ʒu
    public Vector3 delta; // ���P�b�g�̑��x�x�N�g��
    public Vector3 nowPosition { get; private set; } // ���̃��P�b�g�̈ʒu
    public float compleatEscapeTime { get; private set; }
    public GameObject orbitCenter;
    public bool escape = false; // �O������̗��E������
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
    private Vector3 prePosition; // 1�t���[���O�̃��P�b�g�̈ʒu
    private float orbitalRadius;
    private float escapeOrbitalRadius;// �O�����a
    private float mass = 1; // �f���̎���
    private float speed; // ���P�b�g�̑���
    private Vector3 relativeRocketPos;
    private float rotateSpeed;
    private float chargePower;

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
        //Debug.Log(delta.magnitude);
    }
    //�O������E�o����
    void CompleatEscape()
    {
        //Debug.Log("CompleatEscape");
        escape = false;
    }

    public void RocketDestroy()
    {
        gameManager.StartGameFinish();
        crash = true;
        mainCamera.SetActive(true);
        subCamera.SetActive(false);
        Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        this.gameObject.transform.DetachChildren();
        Destroy(ds);
        Destroy(this.gameObject);
        Destroy(this);
    }

    public void RocketUpdate()
    {
        GetDeltaPos();
        if (start == false)//�X�^�[�g�_�b�V��
        {
            if (Input.GetButtonDown("Jump"))
            {
                Vector3 StartDirection = new Vector3(0f, 0f, startDash);
                rb.AddForce(StartDirection);
                start = true;
            }
        }

        if(start)//�X�^�[�g��fule�̎��ԓ�����̏���
        {
            fuel -= Time.deltaTime;
        }
        if(fuel <= 0)
        {
            //Debug.Log("Finish!");
            RocketDestroy();
        }
        fuel = Mathf.Clamp(fuel, 0, maxFuel);//fuel�̍ŏ��l�ő�l����

        Vector3 forward = myTransform.forward;//���ʕ����̃x�N�g��
        Vector3 moveDirection = delta.normalized; //�ړ������̒P�ʃx�N�g��
        if (!inOrbit)//�O���̊O�ł̑���
        {
            Vector3 horizon = Quaternion.Euler(0, 90, 0) * moveDirection;//�������̃x�N�g��
            if (Input.GetButton("Jump") && fuel > spConsumeFuel && !spCooldown)
            {
                if (Input.GetButtonDown("Horizontal"))//���ړ�
                {
                    Vector3 horizonMove = Input.GetAxis("Horizontal") * horizon * spHorizonSpeed;                    
                    fuel -= spConsumeFuel;//fuel��10����                    
                    emergencyAvoidance = true;//���ړ�����on                    
                    spCooldown = true;//sp�N�[���_�E��on                   
                    rb.velocity += horizonMove;//���ړ��̎��s
                    Invoke("cooldown", spCooltime);                   
                    StartCoroutine("antiHorizon", horizonMove);//���ړ���~�̌Ăяo��
                }
                if (Input.GetButtonDown("Vertical") && Input.GetAxis("Vertical") > 0)
                {
                    Vector3 accelMove = moveDirection * spAccelSpeed;
                    fuel -= spConsumeFuel;//fuel��10����
                    spCooldown = true;//sp�N�[���_�E��on
                    rb.velocity += accelMove;//���ړ��̎��s
                    Invoke("cooldown", spCooltime);
                }
            }
            else
            {
                if (Input.GetButton("Horizontal"))//横移動
                {
                    float rocketAngle = Vector3.Angle(moveDirection, myTransform.position);
                    float rocketAxis = Vector3.Cross(moveDirection, myTransform.position).y < 0 ? -1 : 1;
                    float angle = rocketAngle * rocketAxis;
                    Vector3 horizonSize = horizon * horizonSpeed;
                    if(angle < 90 && Input.GetAxis("Horizontal") < 0)//左
                    {
                        rb.AddForce(-horizonSize);
                    }
                    if(angle > -90 && Input.GetAxis("Horizontal") > 0)//右
                    {
                        rb.AddForce(horizonSize);
                    }
                }
                if (Input.GetAxis("Vertical") < 0 && rb.velocity.magnitude > 100)
                {
                    rb.AddForce(-moveDirection * breakSpeed);
                }
            }
        }

        if (inOrbit)//�O���̒��ł̑���
        {
            //Debug.Log("inOrbit");

            chargePower = System.Math.Min(charge, 1);//charge�̍ő�l����
            float escapeSpeed = saveVelocity.magnitude;//�O�����E���̑���
            if (Input.GetButton("Jump"))
            {
                Debug.Log("charge");
                charge += chargeSpeed;//�X�y�[�X�������charge�𑝉�
            }
            if (Input.GetButtonUp("Jump"))//�X�y�[�X�L�[�𗣂����Ƃ�
            {
                Debug.Log("Escape");
                if (autoCamera)
                {
                    mainCamera.SetActive(true);
                    subCamera.SetActive(false);
                }
                rb.velocity = forward * (escapeSpeed + (maxCharge * chargePower));//���ʕ����ɑ��x��^����           
                charge = 0f;//charge�̃��Z�b�g
                escape = true;//escape������on
                compleatEscapeTime = Mathf.Pow((Mathf.Pow(escapeOrbitalRadius, 2) - Mathf.Pow(relativeRocketPos.magnitude, 2)), 0.5f) / escapeSpeed + escapeTime;
                Invoke("CompleatEscape",compleatEscapeTime);//escape�����̌Ăяo��
            }
            if(charge > exprosionCharge)//charge���������Ƃ����j
            {
                //Debug.Log("Explosion!");//��
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
        //���P�b�g�̊p�x����
        if (!emergencyAvoidance)//���ړ����͌�����ς��Ȃ�
        {
            var rocketAngle = Vector3.Angle(delta, Vector3.forward);
            var rocketAxis = Vector3.Cross(delta, Vector3.forward).y < 0 ? -1 : 1;
            var normalizedAngle = Mathf.Repeat(-rocketAngle * rocketAxis, 360);
            myTransform.rotation = Quaternion.Euler(0, normalizedAngle, 0);
            //Debug.Log(normalizedAngle);
        }
    }

    private void OnCollisionEnter(Collision collision) //�Ԃ������Ƃ�
    {
        //Debug.Log("CrashExprosion!");//��
        RocketDestroy();
    }

    private void OnTriggerEnter(Collider collider) //�d�͌��ɓ������Ƃ�
    {
        //�߂Â����f���̏��̎擾
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

    private void OnTriggerStay(Collider collider) //�d�͌��ɂ���Ƃ�
    {
        if (!escape && !emergencyAvoidance)//�O�����E��������
        {
            if (collider.gameObject.tag == "Planet")
            {
                foreach (GameObject planetObject in colList)
                {
                    Debug.Log(planetObject);
                    planet = planetObject.GetComponent<Planet>();
                    mass = planet.mass;
                    orbitalRadius = planet.orbitLevel1;
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

                        if (angle > 90 && !leftAround && !rightAround) //�����v���
                        {
                            escapeOrbitalRadius = orbitalRadius;
                            relativeRocketPos = -gravityDirection;//�O���̔��a�ƂȂ�x�N�g���̎擾
                            speed = delta.magnitude;//�����̌v�Z
                            saveVelocity = rb.velocity;//���x�̕ۑ�
                            rb.velocity = Vector3.zero;//Rigidbody�̋@�\��~
                            rightAround = true;//�����v��蔻��ON
                            orbitCenter = planetObject;
                            fuel += fuelRecovery;
                            if (autoCamera)
                            {
                                mainCamera.SetActive(false);
                                subCamera.SetActive(true);
                            }
                        }

                        if (angle < -90 && !leftAround && !rightAround) //���v���
                        {
                            escapeOrbitalRadius = orbitalRadius;
                            relativeRocketPos = -gravityDirection;//�O���̔��a�ƂȂ�x�N�g���̎擾
                            speed = delta.magnitude;//�����̌v�Z
                            saveVelocity = rb.velocity;//���x�̕ۑ�
                            rb.velocity = Vector3.zero;//Rigidbody�̋@�\��~
                            leftAround = true;//���v��蔻��ON
                            orbitCenter = planetObject;
                            fuel += fuelRecovery;
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
        yield return new WaitForSeconds(0.15f);
        rb.velocity -= horizonMove;//���ړ��̒�~�̎��s
        yield return new WaitForSeconds(0.05f);
        emergencyAvoidance = false;//���ړ�����off
    }
    private void cooldown()//sp����N�[���_�E�����
    {
        spCooldown = false;
    }
}
