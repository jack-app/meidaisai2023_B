using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private GameObject obj;
    private Transform mytransform;
    [SerializeField] private float back = 100f;
    [SerializeField] private float up = 200f;
    [SerializeField] private float orbBack = 100f;
    [SerializeField] private float orbUp = 200f;
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
            //�p�^�[��1
            //Vector3 centerDirection = obj.transform.position * (-1);//�F�����S�����̃x�N�g��
            //Vector3 nlzDirection = centerDirection.normalized;//�P�ʃx�N�g����
            //Vector3 backposition = obj.transform.position + (back * nlzDirection);//�J�����̃��P�b�g����ʒu
            //mytransform.position = new Vector3(backposition.x, up, backposition.z);//�ʒu�ύX�̎��s
            //float angle = Vector3.Angle(obj.transform.position, Vector3.forward);//�J�����p�x�̌���
            //mytransform.rotation = Quaternion.Euler(5f, angle, 0.0f);//�J�����p�x�ύX�̎��s

            //�p�^�[���Q
            Vector3 Direction2 = -obj.transform.forward;//���ʕ����̋t�x�N�g��
            Vector3 backposition2 = obj.transform.position + (back * Direction2);//�J�����̃��P�b�g����ʒu
            mytransform.position = new Vector3(backposition2.x, up, backposition2.z);//�ʒu�ύX�̎��s
            float angle2 = Vector3.Angle(Direction2 * (-1), Vector3.forward);//�J�����p�x�̌���
            mytransform.rotation = Quaternion.Euler(5f, angle2, 0.0f);//�J�����p�x�ύX�̎��s
        }
        if (rc.inOrbit == true)//�O���̓�
        {
            Vector3 planetPos = rc.PlanetPos;//�f���̈ʒu
            Vector3 centerDirection = planetPos * (-1);//�F�����S�����̃x�N�g��
            Vector3 nlzDirection = centerDirection.normalized;//�P�ʃx�N�g����
            Vector3 backposition = planetPos + (orbBack * nlzDirection);//�J�����̘f������ʒu
            mytransform.position = new Vector3(backposition.x, orbUp, backposition.z);//�ʒu�ύX�̎��s
            float angle = Vector3.Angle(planetPos, Vector3.forward);//�J�����p�x�̌���
            mytransform.rotation = Quaternion.Euler(60f, angle, 0.0f);//�J�����p�x�ύX�̎��s

        }
    }
}
