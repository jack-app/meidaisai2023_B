using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Setting : MonoBehaviour
{
    public static bool rocketTrajectoryExist = true;
    public static bool planetTrajectoryExist = true;
    public static bool visualGravityExist = true;
    public static bool visualOrbitExist = true;
    public static bool autoCamera = false;
    [SerializeField] private TextMeshProUGUI rocketTrajectoryExistGUI;
    [SerializeField] private TextMeshProUGUI planetTrajectoryExistGUI;
    [SerializeField] private TextMeshProUGUI visualGravityExistGUI;
    [SerializeField] private TextMeshProUGUI visualOrbitExistGUI;
    [SerializeField] private TextMeshProUGUI autoCameraGUI;
    private Color32 On = new Color32(0, 255, 0, 255);
    private Color32 Off = new Color32(255, 0, 0, 255);

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            rocketTrajectoryExist = !rocketTrajectoryExist;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            planetTrajectoryExist = !planetTrajectoryExist;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            visualGravityExist = !visualGravityExist;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            visualOrbitExist = !visualOrbitExist;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            autoCamera = !autoCamera;
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            autoCamera = false;
            visualGravityExist = true;
            visualOrbitExist = true;
            rocketTrajectoryExist = true;
            planetTrajectoryExist = true;
        }
        
        if(rocketTrajectoryExist)
        {
            rocketTrajectoryExistGUI.color = On;
        }
        else
        {
            rocketTrajectoryExistGUI.color = Off;
        }

        if (planetTrajectoryExist)
        {
            planetTrajectoryExistGUI.color = On;
        }
        else
        {
            planetTrajectoryExistGUI.color = Off;
        }

        if (visualGravityExist)
        {
            visualGravityExistGUI.color = On;
        }
        else
        {
            visualGravityExistGUI.color = Off;
        }

        if (visualOrbitExist)
        {
            visualOrbitExistGUI.color = On;
        }
        else
        {
            visualOrbitExistGUI.color = Off;
        }

        if (autoCamera)
        {
            autoCameraGUI.color = On;
        }
        else
        {
            autoCameraGUI.color = Off;
        }


    }
}
