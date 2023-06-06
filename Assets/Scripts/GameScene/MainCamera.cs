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
        localAngle.x = angle; // ローカル座標を基準に、x軸を軸にした回転をangleに変更
        localAngle.y = 0f; // ローカル座標を基準に、y軸を軸にした回転を0に変更
    }
    void Update()
    {
        localAngle.z = -rc.rotationZ; // ローカル座標を基準に、z軸を軸にした回転を0に変更
        mytransform.localEulerAngles = localAngle; // 回転角度を設定
    }
}
