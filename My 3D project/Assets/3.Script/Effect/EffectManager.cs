using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] Animator noteHitAnimator;
    [SerializeField] Animator judgementAnimator;
    [SerializeField] UnityEngine.UI.Image judgementImage;
    [SerializeField] Sprite[] judgementSprites;
    
    
    private static readonly int HIT = Animator.StringToHash("Hit");
    
    
    public void NoteHitEffect()
    {
        noteHitAnimator.SetTrigger(HIT);
    }

    public void JudgementHitEffect(int num)
    {
        judgementImage.sprite = judgementSprites[num];
        judgementAnimator.SetTrigger(HIT);
    }
    
}
