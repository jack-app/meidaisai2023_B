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
        // �e�J�����I�u�W�F�N�g���擾
        mainCamera = GameObject.Find("Main Camera");
        subCamera = GameObject.Find("Sub Camera");

        // �T�u�J�����̓f�t�H���g�Ŗ����ɂ��Ă���
        subCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // ����C�L�[�������ꂽ�Ȃ�΁A
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            // �e�J�����I�u�W�F�N�g�̗L���t���O���t�](true��false,false��true)������
            mainCamera.SetActive(!mainCamera.activeSelf);
            subCamera.SetActive(!subCamera.activeSelf);
        }
    }
}
