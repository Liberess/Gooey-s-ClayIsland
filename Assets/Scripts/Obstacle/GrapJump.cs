using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hun.Obstacle
{
    public class GrapJump : Grapple
    {
        protected override void Complete()
        {
            
        }

        protected override void Initialize()
        {
            IsActive = true;
            PullTimer = 0.0f;
            grapState = Player.PlayerGrappler.GrapState.Jump;
        }
    }
}
