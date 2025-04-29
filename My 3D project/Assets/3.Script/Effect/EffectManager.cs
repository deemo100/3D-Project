using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    [SerializeField] Animator noteHitEffectAnimator;   
    [SerializeField] Animator judgementEffectAnimator; 
    [SerializeField] Image judgementImage;             
    [SerializeField] Sprite[] judgementSprites;        
    
    
    private static readonly int HIT = Animator.StringToHash("Hit");
    
    
    public void NoteHitEffect()                   
    {                                             
        noteHitEffectAnimator.SetTrigger(HIT);    
    }                                             
    
    public void JudgementHitEffect(int num)                          
    {                                                                
        if (num >= 0 && num < judgementSprites.Length)               
        {                                                            
            judgementImage.sprite = judgementSprites[num];           
            judgementEffectAnimator.SetTrigger(HIT);                 
        }                                                            
        else                                                         
        {                                                            
            Debug.LogWarning($"Invalid judgement index: {num}");     
        }                                                            
    }                                                                

}
