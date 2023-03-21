using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace Solvers
{
    public class FaceUser : Solver
    {
        public override void SolverUpdate()
        {
            if (SolverHandler != null && SolverHandler.TransformTarget != null)
            {
                var target = SolverHandler.TransformTarget;
                GoalRotation = Quaternion.LookRotation(target.position, Vector3.up) * Quaternion.Euler(0,180,0);
                
            }
        }
    }
}