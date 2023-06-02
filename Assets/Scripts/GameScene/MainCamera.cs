using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private Transform mytransform;
    [SerializeField] private float back = 100f;
    [SerializeField] private float up = 200f;
    [SerializeField] private float angle;
    void Start()
    {
        mytransform = this.transform;
        mytransform.localPosition = new Vector3(0, up, back);
        Vector3 localAngle = mytransform.localEulerAngles;
        localAngle.x = angle; // ���[�J�����W����ɁAx�������ɂ�����]��angle�ɕύX
        localAngle.y = 0f; // ���[�J�����W����ɁAy�������ɂ�����]��0�ɕύX
        localAngle.z = 0f; // ���[�J�����W����ɁAz�������ɂ�����]��0�ɕύX
        mytransform.localEulerAngles = localAngle; // ��]�p�x��ݒ�

    }
}
