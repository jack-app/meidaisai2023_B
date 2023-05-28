using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetControl : MonoBehaviour
{
    [SerializeField] private float startDash;//�����x
    [SerializeField] private float escapeTime;//���E�����܂ł̎���
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
    private bool start = false;//�X�^�[�g�������̔���
    public bool escape = false;//�O������̗��E������
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
        InvokeRepeating("Delta", 0.0f, 0.1f);//���x�x�N�g���̌v�Z�p�x_��_�ύX�����ꍇGravityControl�̊p���x�v�Z�̕ύX���K�v
    }

    //���x�x�N�g��(delta)�̎擾
    void Delta()
    {
        nowPosition = myTransform.position;
        delta = nowPosition - prePosition;
         prePosition = nowPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(start ==false)//�X�^�[�g�_�b�V��
        {
            if (Input.GetButtonDown("Jump"))
            {
                Vector3 StartDirection = new Vector3(0f, 0f, startDash);
                rb.AddForce(StartDirection);
                start = true;
            }
        }
        inOrbit = gc.InOrbit;
        Vector3 forward = myTransform.forward;//���ʕ����̃x�N�g��
        Vector3 moveDirection = delta.normalized; //�ړ������̒P�ʃx�N�g��      
        var result = Quaternion.Euler(0, 90, 0) * moveDirection;//�������̃x�N�g��
        Vector3 HorizonMove = result * HorizonSpeed;
        if(Input.GetButton("Horizontal"))//���ړ�
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
            float escapeSpeed = gc.saveVelocity.magnitude;//�O�����E���̑���
            
            if (Input.GetButton("Jump"))
            {
                charge += chargeSpeed;//�X�y�[�X��������charge�𑝉�
            }


            if (Input.GetButtonUp("Jump"))
            {
                rb.velocity = forward * escapeSpeed * charge;//���ʕ����ɑ��x��^����           
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
