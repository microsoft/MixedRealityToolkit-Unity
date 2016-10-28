//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class DehydrationDeactivation : StateMachineBehaviour
    {
        /// <summary>
        /// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        /// </summary>
        /// <param name="animator">Animator that triggered OnStateEnter.</param>
        /// <param name="stateInfo">Animator state info.</param>
        /// <param name="layerIndex">Layer index.</param>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.transform.gameObject.SetActive(false);
        }
    }
}