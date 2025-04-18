using UnityEngine;
using UnityEngine.UI;

// 최적화

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
    // 여기서 num 값이 judgementSprites 배열 범위를 벗어나면 런타임 오류가 발생합니다.
    // 팁: Unity에서 외부 입력(int num)으로 배열에 접근할 때는 항상 범위 체크를 습관화하세요!
    
}
