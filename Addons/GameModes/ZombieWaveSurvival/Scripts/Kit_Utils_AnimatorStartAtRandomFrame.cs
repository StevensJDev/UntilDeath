using UnityEngine;
using UnityEngine.Animations;

namespace MarsFPSKit
{
    public class Kit_Utils_AnimatorStartAtRandomFrame : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.normalizedTime < 0.1f)
            {
                animator.CrossFade(stateInfo.fullPathHash, 0.3f, layerIndex, Random.Range(0.1f, 1f));
            }
        }
    }
}