using UnityEngine;

namespace NpcSystem
{
    public class RobotVacuumNpc : BaseNpc
    {
        private static readonly int _moveX = Animator.StringToHash("MoveX");
        private static readonly int _moveY = Animator.StringToHash("MoveY");
        
        [SerializeField] private Animator anim;
        
        protected override void Tick()
        {
            Patrol();
            
            anim.SetFloat(_moveX, aiPath.velocity.x);
            anim.SetFloat(_moveY, aiPath.velocity.y);
        }
    }
}
