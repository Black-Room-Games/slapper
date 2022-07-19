using UnityEngine;
using DG.Tweening;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; private set; }

    [HideInInspector] public bool gameStarted;
    [HideInInspector] public bool gameOver;
    [HideInInspector] public bool loseWithSuper;

    public Animator powerIndicatorAnim;
    public Transform powerIndicatorArrow;

    [Header("General")]
    public CharController playerChar;
    public CharController opponentChar;
    public Animator playerAnim;
    public Animator opponentAnim;
    [Space(10)]
    public AnimationClip[] superSlapClips;
    public AnimationClip[] warmUpSlapClips;
    public AnimatorOverrideController animatorOverrideController;
    [Space(10)]
    public bool playerTurn;
    public int playerMinPower;
    public int playerMaxPower;
    public float startValueForSuperSlap;
    public int maxPowerIncreaserBy;
    [Space(5)]
    public int opponentHealth;
    int opponentMaxHealth;

    [HideInInspector] public int playerHealth;
    public int playerHealthIncreaser;

    //temps
    [HideInInspector] public int damageByOpponent;

    [HideInInspector] public int playerSelectedPower;

    [Header("Char Customisation")]
    public Material[] kanisFerebi;
    public Material[] clothes;
    public Material[] naskebi;
    public Material[] tmebi;
    public Material[] fexsacmlebi;

    [Header("Sounds")]
    public AudioSource slapSound;
    public AudioSource superSlapSound;

    [Header("CameraThings")]
    public float cameraMoveSpeed;
    public Camera mainCamera;
    public Transform playerViewCamPos;
    public Transform opponentViewCamPos;
    [Space(10)]
    public float cameraZoomTime;
    public float cameraZoomOutValue;
    public float cameraZoomInValue;

    private void Awake() => instance = this;

    private void Start()
    {
        Application.targetFrameRate = 60;

        playerHealth = playerHealthIncreaser * Player.instance.maxHpStatus;
        UIController.instance.playerHealthCountTxt.text = playerHealth.ToString();

        opponentHealth += (30 * (Player.instance.level - 1));
        opponentMaxHealth = opponentHealth;
        UIController.instance.opponentHealthCountTxt.text = opponentHealth.ToString();

        opponentAnim.Play("WaitSlap", -1, 0);

        UIController.instance.powerIndicatorTxt.text = (playerMaxPower + ((Player.instance.maxPowerStatus - 1) * maxPowerIncreaserBy)).ToString();

        RandCharVisual(opponentChar);

        YsoCorp.GameUtils.YCManager.instance.OnGameStarted(Player.instance.level);
    }

    private void OnApplicationQuit()
    {
        //AppMetricaEvents.instance.LevelFinish_Event(Player.instance.level, "Leave");
    }

    private void Update()
    {
        if (Application.isEditor) //TESTING
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                ChangeCameraView(true);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                ChangeCameraView(false);
        }
    }

    #region General
    public void MakeGameOver(bool win)
    {
        if (!gameOver)
        {
            gameOver = true;
            powerIndicatorAnim.gameObject.SetActive(false);
            StartCoroutine(FinishGameOver(win));
        }
    }

    IEnumerator FinishGameOver(bool win)
    {
        yield return new WaitForSecondsRealtime(1.2f);
        UIController.instance.gameOverPanel.SetActive(true);

        YsoCorp.GameUtils.YCManager.instance.OnGameFinished(win);

        if (win)
        {
            playerAnim.SetTrigger("Win");
            //if (!loseWithSuper)
            opponentAnim.SetTrigger("Lose");

            Player.instance.coins += ((Player.instance.level - 1) * 50) + 100;
            Player.instance.level++;
        }
        else
        {
            opponentAnim.SetTrigger("Win");
            //if (!loseWithSuper)
            playerAnim.SetTrigger("Lose");
        }

        Player.instance.SavePlayer();
    }

    public void UpdateCoins(int value)
    {
        Player.instance.coins += value;
        UIController.instance.coinsTxt.text = Player.instance.coins.ToString();
    }

    public void SlapMomentRagac()
    {
        if (playerTurn)
            UpdatePlayerHealth(-damageByOpponent);
        else
            UpdateOpponentHealth(-playerSelectedPower);
    }

    public void SlapDone()
    {
        ChangeCameraView(playerTurn);
        if (playerTurn)
        {
            if (!gameOver)
            {
                powerIndicatorAnim.gameObject.SetActive(true);
                UIController.instance.powerIndicatorTxt.text = (playerMaxPower + ((Player.instance.maxPowerStatus - 1) * maxPowerIncreaserBy)).ToString();
                UIController.instance.ResetMobingCircleAnim();
                playerAnim.SetTrigger("BeforeSlap");
            }
        }
        else
        {
            if (!gameOver)
            {
                powerIndicatorAnim.gameObject.SetActive(false);
                opponentAnim.SetTrigger("BeforeSlap");
                StartCoroutine(SlapByOpponentRoutine());
            }
        }
    }

    IEnumerator SlapByOpponentRoutine()
    {
        float waitScs = Random.Range(1f, 2f);
        yield return new WaitForSeconds(waitScs);
        damageByOpponent = Random.Range(10, 30) + ((Player.instance.level - 1) * 10);

        if (damageByOpponent > startValueForSuperSlap)
        {
            animatorOverrideController["Girl_Attack01"] = superSlapClips[Random.Range(0, superSlapClips.Length)];
            animatorOverrideController["Girl_idle01"] = warmUpSlapClips[Random.Range(0, warmUpSlapClips.Length)];
            opponentAnim.runtimeAnimatorController = animatorOverrideController;
        }

        opponentAnim.SetTrigger(damageByOpponent > startValueForSuperSlap ? "SuperSlap" : "Slap");
        playerTurn = true;
    }

    public void UpdatePlayerHealth(int value)
    {
        if (playerHealth + value < 0)
            value -= (playerHealth + value);

        UIController.instance.playerHpMinusTxt.text = "-" + Mathf.Abs(value);
        UIController.instance.playerHpMinusTxt.gameObject.SetActive(true);

        playerHealth += value;
        UIController.instance.playerHealthCountTxt.text = playerHealth.ToString();

        var desScale = UIController.instance.playerHealthBarFillerImg.transform.localScale;
        desScale.x = (float)playerHealth / (playerHealthIncreaser * Player.instance.maxHpStatus);
        UIController.instance.playerHealthBarFillerImg.transform.localScale = desScale;

        //UIController.instance.playerHealthBarFillerImg.fillAmount = (float)playerHealth / (playerHealthIncreaser * Player.instance.maxHpStatus);
        playerChar.charSkinMesh.material.SetFloat("Vector1_AAA45276", 0.25f * (((playerHealthIncreaser * Player.instance.maxHpStatus) - (float)playerHealth) / (playerHealthIncreaser * Player.instance.maxHpStatus)));

        if (playerHealth <= 0)
        {
            if (startValueForSuperSlap < Mathf.Abs(value))
                loseWithSuper = true;
            MakeGameOver(false);
        }
    }

    public void UpdateOpponentHealth(int value)
    {
        if (opponentHealth + value < 0)
            value -= (opponentHealth + value);

        UIController.instance.opponentHpMinusTxt.text = "-" + Mathf.Abs(value);
        UIController.instance.opponentHpMinusTxt.gameObject.SetActive(true);

        opponentHealth += value;
        UIController.instance.opponentHealthCountTxt.text = opponentHealth.ToString();

        var desScale = UIController.instance.opponentHealthBarFillerImg.transform.localScale;
        desScale.x = (float)opponentHealth / opponentMaxHealth;
        UIController.instance.opponentHealthBarFillerImg.transform.localScale = desScale;

        //UIController.instance.opponentHealthBarFillerImg.fillAmount = (float)opponentHealth / opponentMaxHealth;
        opponentChar.charSkinMesh.material.SetFloat("Vector1_AAA45276", 0.25f * ((opponentMaxHealth - (float)opponentHealth) / opponentMaxHealth));

        if (opponentHealth <= 0)
        {
            if (startValueForSuperSlap < Mathf.Abs(value))
                loseWithSuper = true;
            MakeGameOver(true);
        }
    }
    #endregion //General

    #region CharCustomisation
    void RandCharVisual(CharController charC)
    {
        int randomTma = Random.Range(0, tmebi.Length);

        /*var tempMatArray = charC.charSkinMesh.materials;
        tempMatArray = new Material[7] {
            clothes[Random.Range(0, clothes.Length)],
            kanisFerebi[Random.Range(0, kanisFerebi.Length)],
            naskebi[Random.Range(0, naskebi.Length)],
            fexsacmlebi[Random.Range(0, fexsacmlebi.Length)],
            tmebi[randomTma],
            charC.charSkinMesh.materials[5],
            tmebi[randomTma]};

        charC.charSkinMesh.materials = tempMatArray;*/
        //charC.charSkinMesh_forProfile.materials = tempMatArray;

        /*charC.charSkinMesh.materials[0] = clothes[Random.Range(0, clothes.Length)];
        charC.charSkinMesh.materials[1] = kanisFerebi[Random.Range(0, kanisFerebi.Length)];
        charC.charSkinMesh.materials[2] = naskebi[Random.Range(0, naskebi.Length)];
        charC.charSkinMesh.materials[3] = fexsacmlebi[Random.Range(0, fexsacmlebi.Length)];
        charC.charSkinMesh.materials[4] = tmebi[randomTma];
        charC.charSkinMesh.materials[6] = tmebi[randomTma];*/

        charC.charSkinMesh.material = kanisFerebi[Random.Range(0, kanisFerebi.Length)];
        charC.charSkinMesh_profile.material = charC.charSkinMesh.material;

        charC.swimSuitMesh.material = clothes[Random.Range(0, clothes.Length)];
        charC.braMesh.material = clothes[Random.Range(0, clothes.Length)];
        charC.shortsMesh.material = clothes[Random.Range(0, clothes.Length)];

        int randomClothes = Random.Range(0, 2);
        if (randomClothes == 0)
            charC.swimSuitMesh.gameObject.SetActive(false);
        else
        {
            charC.braMesh.gameObject.SetActive(false);
            charC.shortsMesh.gameObject.SetActive(false);
        }

        //charC.swimSuitMesh_profile.material = charC.swimSuitMesh.materials[0];

        charC.tma_0_mesh.material = tmebi[randomTma];
        charC.tma_0_mesh_profile.material = charC.tma_0_mesh.materials[0];
        charC.tma_1_mesh.material = tmebi[randomTma];
        charC.tma_1_mesh_profile.material = charC.tma_1_mesh.materials[0];
        charC.tma_2_mesh.material = tmebi[randomTma];
        charC.tma_2_mesh_profile.material = charC.tma_2_mesh.materials[0];

        if (Random.Range(0, 2) == 1)
        {
            charC.tma_1_mesh.gameObject.SetActive(false);
            charC.tma_1_mesh_profile.gameObject.SetActive(false);
        }
        if (Random.Range(0, 2) == 1)
        {
            charC.tma_2_mesh.gameObject.SetActive(false);
            charC.tma_2_mesh_profile.gameObject.SetActive(false);
        }
    }
    #endregion //CharCustomisation

    #region CameraThings
    public void ChangeCameraView(bool toPlayer)
    {
        mainCamera.transform.DOMove(toPlayer ? playerViewCamPos.position : opponentViewCamPos.position, cameraMoveSpeed);
        mainCamera.transform.DORotateQuaternion(toPlayer ? playerViewCamPos.rotation : opponentViewCamPos.rotation, cameraMoveSpeed);
    }

    public void CameraZooming(bool cameraZoomIn)
    {
        DOTween.To(() => mainCamera.fieldOfView, x => mainCamera.fieldOfView = x, cameraZoomIn ? cameraZoomInValue : cameraZoomOutValue, cameraZoomTime);
    }
    #endregion //CameraThings
}