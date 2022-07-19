using DG.Tweening;
using System.Collections;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    public bool isPlayer;
    public ParticleSystem slapParticle;



    public void SlapFinished()
    {
        GameController.instance.SlapDone();
    }

    public void SlapMoment()
    {
        GameController.instance.SlapMomentRagac();
        if (isPlayer)
        {
            GameController.instance.opponentAnim.GetComponent<AnimEvents>().slapParticle.Play();
            GameController.instance.opponentAnim.SetTrigger(GameController.instance.loseWithSuper ? "GetSuperSlap" : (GameController.instance.playerSelectedPower > GameController.instance.startValueForSuperSlap ? "GetSlapDidi" : "GetSlap"));
        }
        else
        {
            GameController.instance.playerAnim.GetComponent<AnimEvents>().slapParticle.Play();
            GameController.instance.playerAnim.SetTrigger(GameController.instance.loseWithSuper ? "GetSuperSlap" : (GameController.instance.damageByOpponent > GameController.instance.startValueForSuperSlap ? "GetSlapDidi" : "GetSlap"));
        }

        if ((!isPlayer && GameController.instance.damageByOpponent > GameController.instance.startValueForSuperSlap) || (isPlayer && GameController.instance.playerSelectedPower > GameController.instance.startValueForSuperSlap))
        {
            GameController.instance.superSlapSound.Play();
            StartCoroutine(CameraShake());
            Vibration.Vibrate(100);
        }
        else
        {
            GameController.instance.slapSound.Play();
            Vibration.Vibrate(70);
        }
    }

    IEnumerator CameraShake()
    {
        var defaultRot = Camera.main.transform.localEulerAngles;
        var desRot = defaultRot;
        desRot.z += 5;
        Camera.main.transform.DORotate(desRot, 0.1f);
        yield return new WaitForSecondsRealtime(0.1f);
        desRot.z -= 10;
        Camera.main.transform.DORotate(desRot, 0.1f);
        yield return new WaitForSecondsRealtime(0.2f);
        Camera.main.transform.DORotate(defaultRot, 0.1f);
    }

    public void CameraZoomIn()
    {
        StartCoroutine(CameraZoomingRoutine(0, true));
    }

    public void CameraZoomOut()
    {
        StartCoroutine(CameraZoomingRoutine(0.3f, false));
    }

    IEnumerator CameraZoomingRoutine(float seconds, bool zoomIn)
    {
        yield return new WaitForSecondsRealtime(seconds);
        GameController.instance.CameraZooming(zoomIn);
    }

    #region Other
    public void DisableThisGameObject()
    {
        gameObject.SetActive(false);
    }
    #endregion //Other
}