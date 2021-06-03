using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class AnimateTurnController : MonoBehaviour
{
    [SerializeField] GameObject ShieldPrefab;
    [SerializeField] GameObject ReloadPrefab;
    [SerializeField] GameObject BulletPrefab;
    [SerializeField] float speed = 1;

    private void OnEnable()
    {
        TurnController.AnimatingTurn.EnterState += Animate;
    }
    private void OnDisable()
    {
        TurnController.AnimatingTurn.EnterState -= Animate;
    }

    List<GameObject> prefabsToDestroy = new List<GameObject>();

    void Animate()
    {
        Sequence mySequence = DOTween.Sequence();
        Debug.Log("player decisions .count" +TurnController.CurrentTurn.playersDecision.Count);
        foreach (var item in TurnController.CurrentTurn.playersDecision)
        {
            Debug.Log(item.owner.Player.NickName);
        }
        foreach (var characterAction in TurnController.CurrentTurn.playersDecision)
        {
            if (characterAction.action == 0)
            {
                GameObject reload = Instantiate(ReloadPrefab, characterAction.owner.CharacterPosition, Quaternion.identity);
                Tween tween = reload.transform.DOMoveY(0, 5 * speed);
                mySequence.Insert(0, tween);
                prefabsToDestroy.Add(reload);
            }

            if (characterAction.action == 1)
            {
                GameObject shield = Instantiate(ShieldPrefab, characterAction.owner.CharacterPosition, Quaternion.identity);
                Tween tween = shield.transform.DOMoveY(0, 5 * speed);
                mySequence.Insert(0, tween);
                prefabsToDestroy.Add(shield);
            }

            if (characterAction.action == 2)
            {
                GameObject bullet = Instantiate(BulletPrefab,characterAction.owner.CharacterPosition,Quaternion.identity);

                Tween tween = bullet.transform.DOMove(characterAction.target.CharacterPosition, 1 * speed).SetEase(Ease.Linear);
                mySequence.Insert(0, tween);
                tween.OnComplete(()=> { Destroy(bullet);});
            }
        }
        mySequence.OnComplete(() =>
        {
            foreach (var prefab in prefabsToDestroy)
            {
                Destroy(prefab);
            }
            prefabsToDestroy.Clear();
            TurnController.stateMachine.currentState = TurnController.EndingTurn;
        });

        mySequence.Play();

    }
}
