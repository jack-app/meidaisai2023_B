using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityControl : MonoBehaviour
{
    [SerializeField] private float GravityCoefficient;//���L���͒萔
    [SerializeField] private float orbitSpeedBounus;//�O���˓����̃X�s�[�h��UP���o�̔{��
    private float orbitalRadius;//�O�����a
    private Rigidbody rb;
    private Transform myTransform; 
    private Vector3 moveDirection;
    private float Mass = 1;//�f���̎���
    public Vector3 PlanetPos;//�f���̈ʒu
    public Vector3 speedVector;//���P�b�g�̑��x�x�N�g��
    private float speed;//���P�b�g�̑���
    public bool InOrbit = false;//�O���ɏ���Ă邩�̔���
    public bool leftAround = false;//���v���
    public bool rightAround = false;//�����v���
    public Vector3 saveVelocity;//���x�x�N�g���̕ۑ�
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
        //�����W���O�ɌŒ�
        Vector3 pos = myTransform.position;
        pos.y = 0.0f;
        transform.position = pos;
        //���x�x�N�g���擾
        speedVector = jetControl.delta;
        //Debug.Log(speedVector.magnitude);
        //�O���ɏ���Ă邩����
        if(leftAround ==true || rightAround == true)
        {
            InOrbit = true;
        }
        else
        {
            InOrbit = false;
        }

        if(jetControl.escape == true)//�O�����E��������
        {
            leftAround = false;
            rightAround = false;
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        //�߂Â����f���̏��̎擾
        if (collider.gameObject.tag == "Planet")
        {
            planetInfo = collider.GetComponent<PlanetInfo>();
            Mass = planetInfo.mass;
            orbitalRadius = planetInfo.orbitLevel1;
        }

    }

    void OnTriggerStay(Collider collider)
    {
        if (jetControl.escape == false)//�O�����E��������
        {
            if (collider.gameObject.tag == "Planet")
            {
                //Debug.Log(collider.gameObject.name);
                //�d��
                //�߂Â����f���̍��W�̎擾
                PlanetPos = collider.gameObject.transform.position;
                //���P�b�g����f���̒��S�֌������x�N�g���̎擾
                Vector3 GravityDirection = PlanetPos - myTransform.position;
                //���P�b�g�Ƙf���̋����̎擾
                float GravityLength = GravityDirection.magnitude;
                Debug.Log(GravityLength);
                //�P�ʃx�N�g���̎擾
                Vector3 GravityNlz = GravityDirection.normalized;
                //�d�͂̌v�Z
                Vector3 GravityForth = GravityNlz * GravityCoefficient * Mass / Mathf.Pow(GravityLength, 2);

                if (GravityLength < orbitalRadius)
                {
                    //�����̏ꍇ
                    Debug.Log("Inside");
                    //���x�x�N�g���Ƙf���Ɍ����������̃x�N�g���̂Ȃ��p                
                    var axis = Vector3.Cross(GravityDirection, speedVector).y < 0 ? -1 : 1;//�O�όv�Z(�Ȃ��p��-180����180�ɂ���̂ɕK�v)
                    var angle = Vector3.Angle(GravityDirection, speedVector) * (axis);//�Ȃ��p
                                                                                      //�Ȃ��p��_�E0����180_��0����-180                          
                                                                                      //Debug.Log(angle);

                    if (angle > 90 && rightAround == false) //�����v���
                    {
                        speed = 10 * speedVector.magnitude * orbitSpeedBounus;//�����̌v�Z
                        saveVelocity = rb.velocity;//���x�̕ۑ�
                        rb.velocity = Vector3.zero;//Rigidbody�̋@�\��~
                        rightAround = true;//�����v��蔻��ON
                    }
                    if (rightAround == true)//�����v�����s
                    {
                        float rotateSpeed = Mathf.Rad2Deg * (speed / GravityLength);//�p���x�̌v�Z
                        myTransform.RotateAround(PlanetPos, Vector3.up, -rotateSpeed * Time.deltaTime);//��]�̎��s
                        myTransform.position += planetInfo.plaDelta;
                    }

                    if (angle < -90 && leftAround == false) //���v���
                    {
                        speed = 10 * speedVector.magnitude * orbitSpeedBounus;//�����̌v�Z
                        saveVelocity = rb.velocity;//���x�̕ۑ�
                        rb.velocity = Vector3.zero;//Rigidbody�̋@�\��~
                        leftAround = true;//���v��蔻��ON
                    }
                    if (leftAround == true)
                    {
                        float rotateSpeed = Mathf.Rad2Deg * (speed / GravityLength);//�p���x�̌v�Z
                        myTransform.RotateAround(PlanetPos, Vector3.up, rotateSpeed * Time.deltaTime);//��]�̎��s
                        myTransform.position += planetInfo.plaDelta;
                    }
                }
                else
                {
                    //�O���̏ꍇ
                    Debug.Log("Outside");                
                    //�d�͂�^����
                    rb.AddForce(GravityForth);
                }

            }
        }
    }
}
