using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class DogAcademy : Academy
{

    public override void AcademyReset()
    {
        Monitor.verticalOffset = 1f;
        Physics.defaultSolverIterations = 12;
        Physics.defaultSolverVelocityIterations = 12;
        Time.captureFramerate = 0;
    }

    public override void AcademyStep()
    {
    }

}
