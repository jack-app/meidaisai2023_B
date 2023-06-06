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
    private bool timeReset;
    private bool eTimeReset;
    private float time;
    private float eTime;
    private Vector3 planetforcasPos;//�ʒu�ύX�̎��s
    private Quaternion planetAngle;


    public void CameraUpdate()
    {
        Vector3 rToCenterDirection = obj.transform.position * (-1);//���P�b�g����̉F�����S�����̃x�N�g��
        Vector3 rNlzDirection = rToCenterDirection.normalized;//�P�ʃx�N�g����
        Vector3 rBackposition = obj.transform.position + (back * rNlzDirection);//�J�����̃��P�b�g����ʒu
        Vector3 rocketforcasPos = new Vector3(rBackposition.x, up, rBackposition.z);//�ʒu�ύX�̎��s

        float rCenterAngle = Vector3.Angle(obj.transform.position, Vector3.forward);//�J�����p�x�̌���
        var rCameraAxis = Vector3.Cross(obj.transform.position, Vector3.forward).y < 0 ? -1 : 1;
        var rNormalizedAngle = Mathf.Repeat(-rCenterAngle * rCameraAxis, 360);
        Quaternion rocketAngle = Quaternion.Euler(angle, rNormalizedAngle, 0f);


        time += Time.deltaTime;
        eTime += Time.deltaTime;

        if (rc.inOrbit)
        {
            eTimeReset = false;
            if (!timeReset)
            {
                time = 0f;
                timeReset = true;
            }
            else
            {
                Vector3 planetPos = rc.orbitCenter.transform.position;//�f���̈ʒu
                Vector3 pToCenterDirection = planetPos * (-1);//�f������̉F�����S�����̃x�N�g��
                Vector3 pNlzDirection = pToCenterDirection.normalized;//�P�ʃx�N�g����
                Vector3 pBackposition = planetPos + (back * pNlzDirection);//�J�����̘f������ʒu
                planetforcasPos = new Vector3(pBackposition.x, up, pBackposition.z);//�ʒu�ύX�̎��s

                float pCenterAngle = Vector3.Angle(planetPos, Vector3.forward);//�J�����p�x�̌���
                var pCameraAxis = Vector3.Cross(planetPos, Vector3.forward).y < 0 ? -1 : 1;
                var pNormalizedAngle = Mathf.Repeat(-pCenterAngle * pCameraAxis, 360);
                planetAngle = Quaternion.Euler(angle, pNormalizedAngle, 0f);

                float inot = System.Math.Min(time / 1, 1);
                transform.position = Vector3.Lerp(rocketforcasPos, planetforcasPos, inot);
                transform.rotation = Quaternion.Lerp(rocketAngle, planetAngle, inot);
            }
        }
        else if (rc.escape)
        {
            timeReset = false;
            if (!eTimeReset)
            {
                time = 0f;
                eTimeReset = true;
            }
            else
            {
                float esct = System.Math.Min(time / rc.compleatEscapeTime, 1);
                transform.position = Vector3.Lerp(planetforcasPos, rocketforcasPos, esct);
                transform.rotation = Quaternion.Lerp(planetAngle, rocketAngle, esct);
            }
        }
        else
        {
            timeReset = false;
            eTimeReset = false;
            transform.position = rocketforcasPos;
            this.transform.rotation = Quaternion.Euler(angle, rNormalizedAngle, 0.0f);//�J�����p�x�ύX�̎��s
        }
    }
}
    
