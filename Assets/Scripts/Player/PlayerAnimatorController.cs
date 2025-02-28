using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Animator anim;
    
    readonly int SpeedAnimRef = Animator.StringToHash("Speed");
    readonly int SprintAnimRef = Animator.StringToHash("Sprint");
    readonly int JumpAnimRef = Animator.StringToHash("Jump");
    PlayerMove playerMove;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();
    }
    
    void Update()
    {
        anim.SetFloat(SpeedAnimRef, playerMove.GetAnimSpeed());
        anim.SetBool(SprintAnimRef, playerMove.IsSprinting());
    }
    
    public void OnJump()
    {
        anim.SetTrigger(JumpAnimRef);
    }
}