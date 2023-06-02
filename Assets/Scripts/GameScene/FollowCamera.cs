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
        if(rc.inOrbit == false)//�O���̊O
        {
            this.gameObject.transform.parent = obj.transform;
            mytransform.localPosition = new Vector3(0, up, back);
            Vector3 localAngle = mytransform.localEulerAngles;
            localAngle.x = angle; // ���[�J�����W����ɁAx�������ɂ�����]��angle�ɕύX
            localAngle.y = 0f; // ���[�J�����W����ɁAy�������ɂ�����]��0�ɕύX
            localAngle.z = 0f; // ���[�J�����W����ɁAz�������ɂ�����]��0�ɕύX
            mytransform.localEulerAngles = localAngle; // ��]�p�x��ݒ�
        }
        if (rc.inOrbit == true)//�O���̓�
        {
            this.gameObject.transform.parent = null;
            Vector3 planetPos = rc.PlanetPos;//�f���̈ʒu
            Vector3 centerDirection = planetPos * (-1);//�F�����S�����̃x�N�g��
            Vector3 nlzDirection = centerDirection.normalized;//�P�ʃx�N�g����
            Vector3 backposition = planetPos + (orbBack * nlzDirection);//�J�����̘f������ʒu
            mytransform.position = new Vector3(backposition.x, orbUp, backposition.z);//�ʒu�ύX�̎��s
            float centerAngle = Vector3.Angle(planetPos, Vector3.forward);//�J�����p�x�̌���
            mytransform.rotation = Quaternion.Euler(orbAngle, centerAngle, 0.0f);//�J�����p�x�ύX�̎��s

        }
    }
}
