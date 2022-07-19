using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController instance { get; private set; }

    [Header("Panels")]
    public GameObject inGamePanel;
    public GameObject settingsPanel;
    public GameObject startPanel;
    [Space(10)]
    public GameObject gameOverPanel;

    [Header("InGamePanel")]
    public TextMeshProUGUI coinsTxt;
    public TextMeshProUGUI levelTxt;
    [Space(20)]
    public Image playerProfileIcon;
    public Image opponentProfileIcon;
    public TextMeshProUGUI playerHpMinusTxt;
    public TextMeshProUGUI opponentHpMinusTxt;
    [Space(5)]
    public TextMeshProUGUI playerHealthCountTxt;
    public TextMeshProUGUI opponentHealthCountTxt;
    [Space(5)]
    public Image playerHealthBarFillerImg;
    public Image opponentHealthBarFillerImg;
    [Space(20)]
    public float maxCircleDistance;
    public TextMeshProUGUI powerIndicatorTxt;

    [Header("StartPanel")]
    public int minUpgradePrice;
    [Space(5)]
    public TextMeshProUGUI maxHpStatusTxt;
    public TextMeshProUGUI maxPowerStatusTxt;
    [Space(5)]
    public TextMeshProUGUI maxHpPriceTxt;
    public TextMeshProUGUI maxPowerPriceTxt;

    private void Awake() => instance = this;

    private void Start()
    {
        coinsTxt.text = Player.instance.coins.ToString();
        levelTxt.text = "LEVEL " + Player.instance.level;

        maxHpStatusTxt.text = $"Health ({Player.instance.maxHpStatus})";
        maxPowerStatusTxt.text = $"Power ({Player.instance.maxPowerStatus})";

        maxHpPriceTxt.text = (Player.instance.maxHpStatus * minUpgradePrice).ToString();
        maxPowerPriceTxt.text = (Player.instance.maxPowerStatus * minUpgradePrice).ToString();
    }

    private void Update()
    {
        if (Application.isEditor) //TESTING
            if (Input.GetKeyDown(KeyCode.R))
                ResetMobingCircleAnim();
    }

    #region InGamePanel
    public void OnClickSettingsBtn(bool activate)
    {
        settingsPanel.SetActive(activate);
    }

    public void ResetMobingCircleAnim()
    {
        if (!GameController.instance.powerIndicatorAnim.enabled)
        {
            GameController.instance.powerIndicatorAnim.enabled = true;
            //GameController.instance.powerIndicatorAnim.Play("MovingCircle", -1, 0);
        }
    }

    public void OnDownSwingStopBtn()
    {
        if (GameController.instance.playerTurn)
        {
            if (!GameController.instance.gameStarted)
            {
                GameController.instance.gameStarted = true;
                startPanel.SetActive(false);
            }

            if (GameController.instance.powerIndicatorAnim.enabled)
            {
                GameController.instance.playerTurn = false;
                GameController.instance.powerIndicatorAnim.enabled = false;
                var distance = Vector3.Distance(Vector3.zero + Vector3.up * GameController.instance.powerIndicatorArrow.localPosition.y, GameController.instance.powerIndicatorArrow.localPosition - Vector3.forward * 0.015f);
                var reversePercent = (distance / maxCircleDistance) * 100;
                var originalPercent = 100 - reversePercent;
                GameController.instance.playerSelectedPower = (int)((((GameController.instance.playerMaxPower + ((Player.instance.maxPowerStatus - 1) * GameController.instance.maxPowerIncreaserBy)) - GameController.instance.playerMinPower) * originalPercent) / 100);
                if (GameController.instance.playerSelectedPower > 30 && GameController.instance.playerSelectedPower < GameController.instance.playerMaxPower + ((Player.instance.maxPowerStatus - 1) * GameController.instance.maxPowerIncreaserBy) - 5)
                    GameController.instance.playerSelectedPower += 5;
                powerIndicatorTxt.text = GameController.instance.playerSelectedPower.ToString();

                if (GameController.instance.playerSelectedPower > GameController.instance.startValueForSuperSlap)
                {
                    GameController.instance.animatorOverrideController["Girl_Attack01"] = GameController.instance.superSlapClips[Random.Range(0, GameController.instance.superSlapClips.Length)];
                    GameController.instance.animatorOverrideController["Girl_idle01"] = GameController.instance.warmUpSlapClips[Random.Range(0, GameController.instance.warmUpSlapClips.Length)];
                    GameController.instance.playerAnim.runtimeAnimatorController = GameController.instance.animatorOverrideController;
                }

                GameController.instance.playerAnim.SetTrigger(GameController.instance.playerSelectedPower > GameController.instance.startValueForSuperSlap ? "SuperSlap" : "Slap");
            }
        }
    }
    #endregion //InGamePanel

    #region settingsPanel
    public void OnClickSoundsBtn()
    {

    }

    public void OnClickVibrationBtn()
    {

    }
    #endregion //settingsPanel

    #region startPanel
    public void OnClickUpgradeHealthBtn()
    {
        if (Player.instance.coins >= Player.instance.maxHpStatus * minUpgradePrice)
        {
            GameController.instance.UpdateCoins(-(Player.instance.maxHpStatus * minUpgradePrice));
            Player.instance.maxHpStatus++;
            Player.instance.SavePlayer();

            GameController.instance.playerHealth = GameController.instance.playerHealthIncreaser * Player.instance.maxHpStatus;
            playerHealthCountTxt.text = GameController.instance.playerHealth.ToString();

            maxHpStatusTxt.text = $"Health ({Player.instance.maxHpStatus})";
            maxHpPriceTxt.text = (Player.instance.maxHpStatus * minUpgradePrice).ToString();
        }
        else
            Debug.Log("Not enought coins!");
    }

    public void OnClickUpgradePowerBtn()
    {
        if (Player.instance.coins >= Player.instance.maxPowerStatus * minUpgradePrice)
        {
            GameController.instance.UpdateCoins(-(Player.instance.maxPowerStatus * minUpgradePrice));
            Player.instance.maxPowerStatus++;
            Player.instance.SavePlayer();

            powerIndicatorTxt.text = (GameController.instance.playerMaxPower + ((Player.instance.maxPowerStatus - 1) * GameController.instance.maxPowerIncreaserBy)).ToString();

            maxPowerStatusTxt.text = $"Power ({Player.instance.maxPowerStatus})";
            maxPowerPriceTxt.text = (Player.instance.maxPowerStatus * minUpgradePrice).ToString();
        }
        else
            Debug.Log("Not enought coins!");
    }
    #endregion //startPanel

    #region GameOverPanel
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion //GameOverPanel
}