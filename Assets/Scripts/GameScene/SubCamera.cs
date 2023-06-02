using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCamera : MonoBehaviour
{
    private GameObject obj;
    private Transform mytransform;
    [SerializeField] private float back = 100f;
    [SerializeField] private float up = 200f;
    [SerializeField] private float angle;
    RocketControl rc;

    // Start is called before the first frame update
    void Start()
    {
        obj = GameObject.Find("Rocket");
        mytransform = this.transform;
        rc = obj.GetComponent<RocketControl>();

    }

    // Update is called once per frame
    void Update()
    {
        if (rc.inOrbit == false)//�O���̊O
        {
            Vector3 centerDirection = obj.transform.position * (-1);//���P�b�g����̉F�����S�����̃x�N�g��
            Vector3 nlzDirection = centerDirection.normalized;//�P�ʃx�N�g����
            Vector3 backposition = obj.transform.position + (back * nlzDirection);//�J�����̃��P�b�g����ʒu
            mytransform.position = new Vector3(backposition.x, up, backposition.z);//�ʒu�ύX�̎��s
            float�@centerAngle = Vector3.Angle(obj.transform.position, Vector3.forward);//�J�����p�x�̌���
            mytransform.rotation = Quaternion.Euler(angle, centerAngle, 0.0f);//�J�����p�x�ύX�̎��s
            

        }
        if (rc.inOrbit == true)//�O���̓�
        {
            Vector3 planetPos = rc.orbitCenter.transform.position;//�f���̈ʒu
            Vector3 centerDirection = planetPos * (-1);//�f������̉F�����S�����̃x�N�g��
            Vector3 nlzDirection = centerDirection.normalized;//�P�ʃx�N�g����
            Vector3 backposition = planetPos + (back * nlzDirection);//�J�����̘f������ʒu
            mytransform.position = new Vector3(backposition.x, up, backposition.z);//�ʒu�ύX�̎��s
            float centerAngle = Vector3.Angle(planetPos, Vector3.forward);//�J�����p�x�̌���
            mytransform.rotation = Quaternion.Euler(angle, centerAngle, 0.0f);//�J�����p�x�ύX�̎��s

        }

    }
}
    
