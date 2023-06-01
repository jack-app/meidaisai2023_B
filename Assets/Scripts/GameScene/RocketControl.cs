using UnityEngine;

public class RocketControl : MonoBehaviour
{
    [SerializeField] private float GravityCoefficient;//���L���͒萔
    [SerializeField] private float orbitSpeedBounus;//�O���˓����̃X�s�[�h��UP���o�̔{��
    [SerializeField] private float startDash;//�����x
    [SerializeField] private float escapeTime;//���E�����܂ł̎���
    [SerializeField] private float maxCharge;
    [SerializeField] private float charge;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float Air;
    [SerializeField] private float AirDecrease;
    [SerializeField] private float AirIncrease;
    [SerializeField] private float HorizonSpeed;
    public bool inOrbit = false; // �O���ɏ���Ă邩�̔���
    public bool leftAround = false; // ���v���
    public bool rightAround = false; // �����v���
    public Vector3 saveVelocity; // ���x�x�N�g���̕ۑ�
    public Vector3 PlanetPos; // �f���̈ʒu
    public Vector3 delta; // ���P�b�g�̑��x�x�N�g��
    public Vector3 NowPosition { get; private set; } // ���̃��P�b�g�̈ʒu
    private Rigidbody rb;
    private Transform myTransform;
    private GameObject planetObject;
    private Planet planet;
    private bool start = false; // �X�^�[�g�������̔���
    private bool escape = false; // �O������̗��E������
    private Vector3 prePosition; // 1�t���[���O�̃��P�b�g�̈ʒu
    private float orbitalRadius; // �O�����a
    private float mass = 1; // �f���̎���
    private float speed; // ���P�b�g�̑���
    private Vector3 relativeRocketPos;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        myTransform = transform;
    }    
    
    //���x�x�N�g��(delta)�̎擾
    private void GetDeltaPos()
    {
        NowPosition = myTransform.position;
        delta = (NowPosition - prePosition) / Time.deltaTime;
        prePosition = NowPosition;
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
        Vector3 forward = myTransform.forward;//���ʕ����̃x�N�g��
        Vector3 moveDirection = delta.normalized; //�ړ������̒P�ʃx�N�g��      
        var result = Quaternion.Euler(0, 90, 0) * moveDirection;//�������̃x�N�g��
        Vector3 HorizonMove = result * HorizonSpeed;
        if (Input.GetButton("Horizontal"))//���ړ�
        {
            if (Air > 0)
            {
                Air -= AirDecrease;
                rb.AddForce(Input.GetAxis("Horizontal") * HorizonMove);
            }
        }
        if (inOrbit == true)//�O���̒��ł̑���
        {
            charge = System.Math.Min(charge, maxCharge);//charge�̍ő�l����
            float escapeSpeed = saveVelocity.magnitude;//�O�����E���̑���

            if (Input.GetButton("Jump"))
            {
                charge += chargeSpeed;//�X�y�[�X��������charge�𑝉�
            }

            if (Input.GetButtonUp("Jump"))
            {
                Debug.Log("escape");
                rb.velocity = forward * escapeSpeed * charge;//���ʕ����ɑ��x��^����           
                charge = 1;
                escape = true;
                Invoke("CompleatEscape", escapeTime);
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
        var rocketAngle = Vector3.Angle(delta, Vector3.forward);
        var rocketAxis = Vector3.Cross(delta, Vector3.forward).y < 0 ? -1 : 1;
        var normalizedAngle = Mathf.Repeat(-rocketAngle * rocketAxis, 360);
        myTransform.rotation = Quaternion.Euler(0, normalizedAngle, 0);
        //Debug.Log(normalizedAngle);
    }

    private void RocketCrash()
    {
        
    }

    private void OnCollisionEnter(Collision collision) //�Ԃ������Ƃ�
    {
        RocketCrash();
    }

    private void OnTriggerEnter(Collider collider) //�d�͌��ɓ������Ƃ�
    {
        //�߂Â����f���̏��̎擾
        if (collider.gameObject.tag == "Planet")
        {
            planet = collider.GetComponent<Planet>();
            mass = planet.mass;
            orbitalRadius = planet.orbitLevel1;
        }
        planetObject = collider.gameObject;
    }

    private void OnTriggerStay(Collider collider) //�d�͌��ɂ���Ƃ�
    {
        if (escape == false)//�O�����E��������
        {
            if (collider.gameObject.tag == "Planet")
            {
                //Debug.Log(collider.gameObject.name);
                //�d��
                //�߂Â����f���̍��W�̎擾
                PlanetPos = collider.gameObject.transform.position;
                //���P�b�g����f���̒��S�֌������x�N�g���̎擾
                Vector3 gravityDirection = PlanetPos - myTransform.position;
                //���P�b�g�Ƙf���̋����̎擾
                float GravityLength = gravityDirection.magnitude;
                Debug.Log(GravityLength);
                //�P�ʃx�N�g���̎擾
                Vector3 GravityNlz = gravityDirection.normalized;
                //�d�͂̌v�Z
                Vector3 GravityForth = GravityNlz * GravityCoefficient * mass / Mathf.Pow(GravityLength, 2);

                if (GravityLength < orbitalRadius)
                {
                    //�����̏ꍇ
                    //Debug.Log("Inside");
                    //���x�x�N�g���Ƙf���Ɍ����������̃x�N�g���̂Ȃ��p                
                    var axis = Vector3.Cross(gravityDirection, delta).y < 0 ? -1 : 1;//�O�όv�Z(�Ȃ��p��-180����180�ɂ���̂ɕK�v)
                    var angle = Vector3.Angle(gravityDirection, delta) * (axis);//�Ȃ��p
                                                                                      //�Ȃ��p��_�E0����180_��0����-180                          
                                                                                      //Debug.Log(angle);

                    if (angle > 90 && rightAround == false) //�����v���
                    {
                        relativeRocketPos = this.gameObject.transform.position - planetObject.transform.position;//�O���̔��a�ƂȂ�x�N�g���̎擾
                        speed = delta.magnitude * orbitSpeedBounus;//�����̌v�Z
                        saveVelocity = rb.velocity;//���x�̕ۑ�
                        rb.velocity = Vector3.zero;//Rigidbody�̋@�\��~
                        rightAround = true;//�����v��蔻��ON
                    }
                    if (rightAround == true)//�����v�����s
                    {
                        float rotateSpeed = Mathf.Rad2Deg * (speed / GravityLength);//�p���x�̌v�Z
                        relativeRocketPos = Quaternion.Euler(0, -rotateSpeed * Time.deltaTime, 0) * relativeRocketPos;//�x�N�g���̉�]
                        Vector3 rocketPos = relativeRocketPos + planetObject.transform.position;//���P�b�g�̈ʒu����
                        this.gameObject.transform.position = rocketPos;//��]�̎��s
                        NowPosition = rocketPos;
                    }

                    if (angle < -90 && leftAround == false) //���v���
                    {
                        relativeRocketPos = this.gameObject.transform.position - planetObject.transform.position;//�O���̔��a�ƂȂ�x�N�g���̎擾
                        speed = delta.magnitude * orbitSpeedBounus;//�����̌v�Z
                        saveVelocity = rb.velocity;//���x�̕ۑ�
                        rb.velocity = Vector3.zero;//Rigidbody�̋@�\��~
                        leftAround = true;//���v��蔻��ON
                    }
                    if (leftAround == true)
                    {
                        float rotateSpeed = Mathf.Rad2Deg * (speed / GravityLength);//�p���x�̌v�Z
                        relativeRocketPos = Quaternion.Euler(0, rotateSpeed * Time.deltaTime, 0) * relativeRocketPos;//�x�N�g���̉�]
                        Vector3 rocketPos = relativeRocketPos + planetObject.transform.position;//���P�b�g�̈ʒu����
                        this.gameObject.transform.position = rocketPos;//��]�̎��s
                        NowPosition = rocketPos;
                    }
                }
                else
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
