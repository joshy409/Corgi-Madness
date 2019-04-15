using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;


public class DogAgent : Agent {

    [HideInInspector]
    // The target the dog will run towards.
    public Transform target; 

    // These items should be set in the inspector
    [Header("Body Parts")] 
    public Transform mouthPosition;
    public Transform body;
    public Transform leg0_upper;
    public Transform leg1_upper;
    public Transform leg2_upper;
    public Transform leg3_upper;
    public Transform leg0_lower;
    public Transform leg1_lower;
    public Transform leg2_lower;
    public Transform leg3_lower;

    // These determine how the dog should be able to rotate around the y axis
    [Header("Body Rotation")] 
    public float maxTurnSpeed;
    public ForceMode turningForceMode;

    [Header("Sounds")]
    // If true, the dog will bark. 
    // Note : This should be turned off during training...unless you want to hear a dozen dogs barking for hours
    public bool canBark;
    // The clips to use for the barks of the dog
    public List<AudioClip> barkSounds = new List <AudioClip>();
	AudioSource audioSourceSFX;

    JointDriveController jdController;
    
    [HideInInspector]
    // This vector gives the position of the target relative to the position of the dog
	public Vector3 dirToTarget;
    // This float determines how much the dog will be rotating around the y axis
    float rotateBodyActionValue; 
    // Counts the number of steps until the next agent's decision will be made
    int decisionCounter;

    // [HideInInspector]
    public bool runningToItem;
    // [HideInInspector]
    public bool returningItem;

    void Awake()
    {
        // Audio Setup
        audioSourceSFX = body.gameObject.AddComponent<AudioSource>();
        audioSourceSFX.spatialBlend = .75f;
        audioSourceSFX.minDistance = .7f;
        audioSourceSFX.maxDistance = 5;
        if(canBark)
        {
            StartCoroutine(BarkBarkGame());
        }

        //Joint Drive Setup
        jdController = GetComponent<JointDriveController>();
        jdController.SetupBodyPart(body);
        jdController.SetupBodyPart(leg0_upper);
        jdController.SetupBodyPart(leg0_lower);
        jdController.SetupBodyPart(leg1_upper);
        jdController.SetupBodyPart(leg1_lower);
        jdController.SetupBodyPart(leg2_upper);
        jdController.SetupBodyPart(leg2_lower);
        jdController.SetupBodyPart(leg3_upper);
        jdController.SetupBodyPart(leg3_lower);

    }

    /// <summary>
    /// Add relevant information on each body part to observations.
    /// </summary>
    public void CollectObservationBodyPart(BodyPart bp)
    {
        var rb = bp.rb;
        AddVectorObs(bp.groundContact.touchingGround ? 1 : 0); // Is this bp touching the ground
        if(bp.rb.transform != body)
        {
            AddVectorObs(bp.currentXNormalizedRot);
            AddVectorObs(bp.currentYNormalizedRot);
            AddVectorObs(bp.currentZNormalizedRot);
            AddVectorObs(bp.currentStrength/jdController.maxJointForceLimit);
        }
    }

    /// <summary>
    /// The method the agent uses to collect information about the environment
    /// </summary>
    public override void CollectObservations()
    {
        AddVectorObs(dirToTarget.normalized);
        AddVectorObs(body.localPosition);
        AddVectorObs(jdController.bodyPartsDict[body].rb.velocity);
        AddVectorObs(jdController.bodyPartsDict[body].rb.angularVelocity);
        AddVectorObs(body.forward); //the capsule is rotated so this is local forward
        AddVectorObs(body.up); //the capsule is rotated so this is local forward
        foreach (var bodyPart in jdController.bodyPartsDict.Values)
        {
            CollectObservationBodyPart(bodyPart);
        }
    }


    /// <summary>
    /// Rotates the body of the agent around the y axis
    /// </summary>
    /// <param name="act"> The amount by which the agent must rotate</param>
    void RotateBody(float act)
    {
        float speed = Mathf.Lerp(0, maxTurnSpeed, Mathf.Clamp(act, 0, 1));
        Vector3 rotDir = dirToTarget; 
        rotDir.y = 0;
        // Adds a force on the front of the body
        jdController.bodyPartsDict[body].rb.AddForceAtPosition(
            rotDir.normalized * speed * Time.deltaTime, body.forward, turningForceMode); 
        // Adds a force on the back od the body
        jdController.bodyPartsDict[body].rb.AddForceAtPosition(
            -rotDir.normalized * speed * Time.deltaTime, -body.forward, turningForceMode); 
    }

    /// <summary>
    /// Allows the dog to bark
    /// </summary>
    /// <returns></returns>
    public IEnumerator BarkBarkGame()
    {   

        while(true)
        {
            //When we're returning the stick we should not bark because we have
            //a stick in our mouth :|>
            if(!returningItem)
            {
                //Choose one of the barking clips at random and play it.
                audioSourceSFX.PlayOneShot(barkSounds[Random.Range( 0, barkSounds.Count)], 1);
            }
            //Wait for a random amount of time (between 1 & 10 sec) until we bark again.
            yield return new WaitForSeconds(Random.Range(1, 10)); 
        }
    }

    /// <summary>
    /// The agent's action method. Is called at each decision and allows the agent to move
    /// </summary>
    /// <param name="vectorAction"> The actions that were determined by the policy</param>
    /// <param name="textAction"> The text action given by the policy</param>
	public override void AgentAction(float[] vectorAction, string textAction)
    {

        var bpDict = jdController.bodyPartsDict;
      
        // Update joint drive target rotation
        bpDict[leg0_upper].SetJointTargetRotation(vectorAction[0], vectorAction[1], 0);
        bpDict[leg1_upper].SetJointTargetRotation(vectorAction[2], vectorAction[3], 0);
        bpDict[leg2_upper].SetJointTargetRotation(vectorAction[4], vectorAction[5], 0);
        bpDict[leg3_upper].SetJointTargetRotation(vectorAction[6], vectorAction[7], 0);
        bpDict[leg0_lower].SetJointTargetRotation(vectorAction[8], 0, 0);
        bpDict[leg1_lower].SetJointTargetRotation(vectorAction[9], 0, 0);
        bpDict[leg2_lower].SetJointTargetRotation(vectorAction[10], 0, 0);
        bpDict[leg3_lower].SetJointTargetRotation(vectorAction[11], 0, 0);

        // Update joint drive strength
        bpDict[leg0_upper].SetJointStrength(vectorAction[12]);
        bpDict[leg1_upper].SetJointStrength(vectorAction[13]);
        bpDict[leg2_upper].SetJointStrength(vectorAction[14]);
        bpDict[leg3_upper].SetJointStrength(vectorAction[15]);
        bpDict[leg0_lower].SetJointStrength(vectorAction[16]);
        bpDict[leg1_lower].SetJointStrength(vectorAction[17]);
        bpDict[leg2_lower].SetJointStrength(vectorAction[18]);
        bpDict[leg3_lower].SetJointStrength(vectorAction[19]);

        rotateBodyActionValue = vectorAction[20];
  
    }

    /// <summary>
    /// Update the direction vector to the current target;
    /// </summary>
    public void UpdateDirToTarget()
    {
        dirToTarget = target.position - jdController.bodyPartsDict[body].rb.position;

    }

    void FixedUpdate()
    {
        UpdateDirToTarget();

        if (decisionCounter == 0)
        {
            decisionCounter = 3;
            RequestDecision();
        }
        else
        {
            decisionCounter--;
        }

        RotateBody(rotateBodyActionValue); 

        // Energy Conservation
        // The dog is penalized by how strongly it rotates towards the target.
        // Without this penalty the dog tries to rotate as fast as it can at all times.
        var bodyRotationPenalty = -0.001f * rotateBodyActionValue;
        AddReward(bodyRotationPenalty);

        // Reward for moving towards the target
        RewardFunctionMovingTowards();
        // Penalty for time
        RewardFunctionTimePenalty();
    }
	
    /// <summary>
    /// Reward moving towards target & Penalize moving away from target.
    /// This reward incentivizes the dog to run as fast as it can towards the target,
    /// and decentivizes running away from the target.
    /// </summary>
    void RewardFunctionMovingTowards()
    {

		float movingTowardsDot = Vector3.Dot(
		    jdController.bodyPartsDict[body].rb.velocity, dirToTarget.normalized); 
        AddReward(0.01f * movingTowardsDot);
    }


    /// <summary>
    /// Time penalty
    /// The dog gets a pentalty each step so that it tries to finish 
    /// as quickly as possible.
    /// </summary>
    void RewardFunctionTimePenalty()
    {
        AddReward(- 0.001f);  //-0.001f chosen by experimentation.
    }

}