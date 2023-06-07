using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel = null!;
    [SerializeField] private GameObject resultPanel = null!;
    [SerializeField] private TextMeshProUGUI scoreText = null!;
    [SerializeField] private TextMeshProUGUI resultScoreText = null!;
    [SerializeField] private TextMeshProUGUI resultCauseOfDeath;
    [SerializeField] private TextMeshProUGUI resultTotalTime;
    [SerializeField] private TextMeshProUGUI resultMaxSpeed;
    [SerializeField] private TextMeshProUGUI resultPlanetCount;
    [SerializeField] private TextMeshProUGUI resultSpCount;
    [SerializeField] private TextMeshProUGUI resultComment;
    [SerializeField] private TextMeshProUGUI speedText = null!;
    [SerializeField] private Slider fuelSlider = null!;
    [SerializeField] private Slider ChargeSliderL = null!;
    [SerializeField] private Slider ChargeSliderR = null!;
    [SerializeField] private Image ChargeSliderLImg;
    [SerializeField] private Image ChargeSliderRImg;
    [SerializeField] private Image fuleColor;
    [SerializeField] Color32 fuelEnough = new Color32(255, 255, 255, 255);
    [SerializeField] Color32 fuelLess = new Color32(255, 255, 255, 255);
    [SerializeField] Color32 chargeStartColor = new Color32(255, 255, 255, 255);
    [SerializeField] Color32 chargeEndColor = new Color32(255, 255, 255, 255);
    private string causeOfDeathText;
    private bool pause = false;
    private bool result = false;

    private void Pause()
    {
        Time.timeScale = 0f;
        pause = true;
        pausePanel.gameObject.SetActive(true);
    }

    private void Resume()
    {
        pausePanel.gameObject.SetActive(false);
        pause = false;
        Time.timeScale = 1f;
    }

    private void Continue()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    private void Finish()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    public void Result(long score, int CauseOfDeath, float TotalTime, float MaxSpeed, int PlanetCount, int SpCount)
    {
        int minutes = (int)TotalTime / 60;
        float seconds = TotalTime - minutes * 60;
        Time.timeScale = 0f;
        Destroy(fuelSlider.gameObject);
        Destroy(ChargeSliderL.gameObject);
        Destroy(ChargeSliderR.gameObject);
        Destroy(scoreText);
        Destroy(speedText);

        resultPanel.gameObject.SetActive(true);
        resultScoreText.text = score.ToString("N0") + "km";
        resultTotalTime.text = "�o�ߎ���:" + minutes.ToString("D2") + ":" + ((int)seconds).ToString("D2");
        resultMaxSpeed.text = "�ō����x:" + MaxSpeed.ToString("F") + "km/s";
        resultPlanetCount.text = "�K�ꂽ���̐�:" + PlanetCount.ToString() + "��";
        resultSpCount.text = "�ً}����̉�:" + SpCount.ToString() + "��";
        if(CauseOfDeath == 1)
        {
            causeOfDeathText = "�G�l���M�[���؂�Ĕ�������";
        }
        else if (CauseOfDeath == 2)
        {
            causeOfDeathText = "���ɏՓ˂��Ĕ�������";
        }
        else if (CauseOfDeath == 3)
        {
            causeOfDeathText = "�͂𒙂߂����Ĕ�������";
        }
        else if (CauseOfDeath == 4)
        {
            causeOfDeathText = "���̔����Ɋ������܂ꂽ";
        }
        else if (CauseOfDeath == 5)
        {
            causeOfDeathText = "�u���b�N�z�[���Ɉ��܂ꂽ";
        }
        else
        {
            causeOfDeathText = "�Ȃɂ��N�����H";
        }
        resultCauseOfDeath.text = causeOfDeathText;

        if(score <= 100)
        {
            resultComment.text = "���ĂȂ���";
        }
        else if(score > 100 && score <= 2000)
        {
            resultComment.text = "���S�Ҋ��}�ł�";
        }
        else if (score > 2000 && score <= 5000)
        {
            resultComment.text = "�܂��܂�����";
        }
        else if (score > 5000 && score <= 15000)
        {
            resultComment.text = "�����܂ł͗��K";
        }
        else if (score > 15000 && score <= 30000)
        {
            resultComment.text = "��邶���";
        }
        else if (score > 30000 && score <= 50000)
        {
            resultComment.text = "�i�C�X�t���C�g!";
        }
        else if (score > 50000 && score <= 70000)
        {
            resultComment.text = "�������炪�{��";
        }
        else if (score > 70000 && score <= 90000)
        {
            resultComment.text = "���������āF���l";
        }
        else if (score > 90000 && score <= 100000)
        {
            resultComment.text = "�ɂ�����";
        }
        else if (score > 100000 && score <= 200000)
        {
            resultComment.text = "You are Astrunner!";
        }
        else if (score > 100000 && score <= 200000 && SpCount == 0)
        {
            resultComment.text = "Perfect Game!";
        }
        else if (score >= 200000)
        {
            resultComment.text = "����ȂɊy����?";
        }



        result = true;
    }

    public void UIUpdate(long score, float fuel, float speed, float charge)
    {
        fuelSlider.value = fuel;
        ChargeSliderL.value = charge;
        ChargeSliderR.value = charge;
        scoreText.text = score.ToString("N0") + "km";
        speedText.text = speed.ToString("F") + "km/s";
        if(charge >= 1)
        {
            ChargeSliderLImg.color = Color.Lerp(chargeStartColor, chargeEndColor, Mathf.PingPong(Time.time / 0.15f, 1.0f));
            ChargeSliderRImg.color = Color.Lerp(chargeStartColor, chargeEndColor, Mathf.PingPong(Time.time / 0.15f, 1.0f));
        }
        else
        {
            ChargeSliderLImg.color = chargeStartColor;
            ChargeSliderRImg.color = chargeStartColor;
        }
        if(fuel <= 20)
        {
            fuleColor.color = fuelLess;
        }
        else
        {
            fuleColor.color = fuelEnough;
        }

        if (Input.GetKey(KeyCode.P))
        {
            Pause();
        }
        if (pause && Input.GetKey(KeyCode.R))
        {
            Resume();
        }
        if (result && Input.GetKey(KeyCode.C))
        {
            Continue();
        }
        if ((result || pause)&& Input.GetKey(KeyCode.F))
        {
            Finish();
        }
    }
}
