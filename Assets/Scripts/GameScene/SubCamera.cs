using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCamera : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] private float back = 100f;
    [SerializeField] private float up = 200f;
    [SerializeField] private float angle;
    [SerializeField] private RocketControl rc;

    public void CameraUpdate()
    {
        if (rc.inOrbit == false)//�O���̊O
        {
            Vector3 centerDirection = obj.transform.position * (-1);//���P�b�g����̉F�����S�����̃x�N�g��
            Vector3 nlzDirection = centerDirection.normalized;//�P�ʃx�N�g����
            Vector3 backposition = obj.transform.position + (back * nlzDirection);//�J�����̃��P�b�g����ʒu
            this.transform.position = new Vector3(backposition.x, up, backposition.z);//�ʒu�ύX�̎��s
            float�@centerAngle = Vector3.Angle(obj.transform.position, Vector3.forward);//�J�����p�x�̌���
            var cameraAxis = Vector3.Cross(obj.transform.position, Vector3.forward).y < 0 ? -1 : 1;
            var normalizedAngle = Mathf.Repeat(-centerAngle * cameraAxis, 360);
            this.transform.rotation = Quaternion.Euler(angle, normalizedAngle, 0.0f);//�J�����p�x�ύX�̎��s
            

        }
        if (rc.inOrbit == true)//�O���̓�
        {
            Debug.Log(rc.orbitCenter.name);
            Vector3 planetPos = rc.orbitCenter.transform.position;//�f���̈ʒu
            Vector3 centerDirection = planetPos * (-1);//�f������̉F�����S�����̃x�N�g��
            Vector3 nlzDirection = centerDirection.normalized;//�P�ʃx�N�g����
            Vector3 backposition = planetPos + (back * nlzDirection);//�J�����̘f������ʒu
            this.transform.position = new Vector3(backposition.x, up, backposition.z);//�ʒu�ύX�̎��s
            float centerAngle = Vector3.Angle(planetPos, Vector3.forward);//�J�����p�x�̌���
            var cameraAxis = Vector3.Cross(planetPos, Vector3.forward).y < 0 ? -1 : 1;
            var normalizedAngle = Mathf.Repeat(-centerAngle * cameraAxis, 360);
            this.transform.rotation = Quaternion.Euler(angle, normalizedAngle, 0.0f);//�J�����p�x�ύX�̎��s

        }

    }
}
    
