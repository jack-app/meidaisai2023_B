using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeCamera : MonoBehaviour
{
    private GameObject mainCamera;
    private GameObject subCamera;
    // Start is called before the first frame update
    void Start()
    {
        // 各カメラオブジェクトを取得
        mainCamera = GameObject.Find("Main Camera");
        subCamera = GameObject.Find("Sub Camera");

        // サブカメラはデフォルトで無効にしておく
        subCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // もしCキーが押されたならば、
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            // 各カメラオブジェクトの有効フラグを逆転(true→false,false→true)させる
            mainCamera.SetActive(!mainCamera.activeSelf);
            subCamera.SetActive(!subCamera.activeSelf);
        }
    }
}
