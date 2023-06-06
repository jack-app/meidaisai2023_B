using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private Transform mytransform;
    [SerializeField] private float back = 100f;
    [SerializeField] private float up = 200f;
    [SerializeField] private float angle;
    private GameObject obj;
    private RocketControl rc;
    private Vector3 localAngle;
    void Start()
    {
        obj = GameObject.Find("Rocket");
        rc = obj.GetComponent<RocketControl>();
        mytransform = this.transform;
        mytransform.localPosition = new Vector3(0, up, back);
        localAngle = mytransform.localEulerAngles;
        localAngle.x = angle; // ���[�J�����W����ɁAx�������ɂ�����]��angle�ɕύX
        localAngle.y = 0f; // ���[�J�����W����ɁAy�������ɂ�����]��0�ɕύX
    }
    void Update()
    {
        localAngle.z = -rc.rotationZ; // ���[�J�����W����ɁAz�������ɂ�����]��0�ɕύX
        mytransform.localEulerAngles = localAngle; // ��]�p�x��ݒ�
    }
}
