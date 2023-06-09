using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel = null!;
    [SerializeField] private GameObject resultPanel = null!;
    [SerializeField] private GameObject settingPanel = null!;
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
    [SerializeField] private AudioSource resultMusic = null!;
    [SerializeField] private Slider ChargeSliderL = null!;
    [SerializeField] private Slider ChargeSliderR = null!;
    [SerializeField] private Image ChargeSliderLImg;
    [SerializeField] private Image ChargeSliderRImg;
    [SerializeField] private Image fuleColor;
    [SerializeField] Color32 fuelEnough = new Color32(255, 255, 255, 255);
    [SerializeField] Color32 fuelLess = new Color32(255, 255, 255, 255);
    [SerializeField] Color32 chargeStartColor = new Color32(255, 255, 255, 255);
    [SerializeField] Color32 chargeEndColor = new Color32(255, 255, 255, 255);
    [SerializeField] private GameObject sceneChangePanel;
    [SerializeField] private AudioSource BGMSource;
    private Image fadeAlpha;
    private string causeOfDeathText;
    private bool pause = false;
    private bool result = false;
    private bool setting = false;
    private bool fadeinFlag = true;
    private bool fadeoutFlag = false;
    private float alpha = 2.0f;
    private string nextScene = "StartScene";

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
        nextScene = "GameScene";
        fadeoutFlag = true;
    }

    private void Finish()
    {
        nextScene = "StartScene";
        fadeoutFlag = true;
    }

    public void Result(long score, int CauseOfDeath, float TotalTime, float MaxSpeed, int PlanetCount, int SpCount)
    {
        pausePanel.gameObject.SetActive(false);
        pause = false;
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
        resultTotalTime.text = "経過時間:" + minutes.ToString("D2") + ":" + ((int)seconds).ToString("D2");
        resultMaxSpeed.text = "最高速度:" + MaxSpeed.ToString("F") + "km/s";
        resultPlanetCount.text = "訪れた星の数:" + PlanetCount.ToString() + "個";
        resultSpCount.text = "緊急回避の回数:" + SpCount.ToString() + "回";
        if(CauseOfDeath == 1)
        {
            causeOfDeathText = "エネルギーが切れて爆発した";
        }
        else if (CauseOfDeath == 2)
        {
            causeOfDeathText = "星に衝突して爆発した";
        }
        else if (CauseOfDeath == 3)
        {
            causeOfDeathText = "力を貯めすぎて爆発した";
        }
        else if (CauseOfDeath == 4)
        {
            causeOfDeathText = "星の爆発に巻き込まれた";
        }
        else if (CauseOfDeath == 5)
        {
            causeOfDeathText = "ブラックホールに飲まれた";
        }
        else if (CauseOfDeath == 6)
        {
            causeOfDeathText = "自爆した";
        }
        else
        {
            causeOfDeathText = "なにが起きた？";
        }
        resultCauseOfDeath.text = causeOfDeathText;

        if(CauseOfDeath == 6)
        {
            resultComment.text = "諦めんなよ";
        }
        else if(score <= 1000)
        {
            resultComment.text = "ついてないね";
        }
        else if(score > 1000 && score <= 2000)
        {
            resultComment.text = "初心者歓迎です";
        }
        else if (score > 2000 && score <= 5000)
        {
            resultComment.text = "まだまだだね";
        }
        else if (score > 5000 && score <= 15000)
        {
            resultComment.text = "ここまでは練習";
        }
        else if (score > 15000 && score <= 30000)
        {
            resultComment.text = "やるじゃん";
        }
        else if (score > 30000 && score <= 50000)
        {
            resultComment.text = "ナイスフライト!";
        }
        else if (score > 50000 && score <= 70000)
        {
            resultComment.text = "ここからが本番";
        }
        else if (score > 70000 && score <= 80000)
        {
            resultComment.text = "もしかして：玄人";
        }
        else if (score > 80000 && score <= 90000)
        {
            resultComment.text = "Great!";
        }
        else if (score > 90000 && score <= 100000)
        {
            resultComment.text = "惜しい!";
        }
        else if (score > 100000 && score <= 200000)
        {
            resultComment.text = "You are Astrunner!";
        }
        else if (score > 100000 && score <= 200000 && SpCount == 0)
        {
            resultComment.text = "Perfect Astrunner!";
        }
        else if (score >= 200000)
        {
            resultComment.text = "そんなに楽しい?";
        }



        result = true;
    }

    private void Fade()
    {
        if (fadeinFlag)
        {
            alpha -= 0.02f;
            if (alpha <= 0)
            {
                fadeinFlag = false;
                alpha = 0;
            }
            fadeAlpha.color = new Color(0, 0, 0, alpha);
        }
        else if (fadeoutFlag)
        {

            alpha += 0.02f;
            if (alpha >= 1)
            {
                Time.timeScale = 1.0f;
                SceneManager.LoadScene(nextScene);
                fadeoutFlag = false;
                alpha = 1;
            }
            BGMSource.volume = BGMSource.volume - 0.02f;
            fadeAlpha.color = new Color(0, 0, 0, alpha);
        }
    }

    public void UIStart()
    {
        fadeAlpha = sceneChangePanel.GetComponent<Image>();
        alpha = fadeAlpha.color.a;
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
        if(fuel <= 15)
        {
            fuleColor.color = fuelLess;
        }
        else
        {
            fuleColor.color = fuelEnough;
        }
    }

    public void KeyCheck()
    {
        Fade();
        if (KeyManager.p.down && !result)
        {
            Pause();
        }
        if (pause && KeyManager.s.keep)
        {
            setting = true;
            settingPanel.gameObject.SetActive(true);
        }
        if (setting && KeyManager.b.keep)
        {
            setting = false;
            settingPanel.gameObject.SetActive(false);
        }
        if (pause && !setting && KeyManager.r.keep)
        {
            Resume();
        }
        if (result && KeyManager.c.down)
        {
            Continue();
        }
        if ((result || pause) && !setting && KeyManager.f.keep)
        {
            Finish();
        }
    }
}
