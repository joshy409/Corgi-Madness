using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingTarget : MonoBehaviour
{
	[Header("General")] 
	public DogAgent trainee;
	public Transform spawnArea;
	
	private Bounds spawnAreaBounds;
	
	
	
	// Use this for initialization
	void Start () {
		spawnAreaBounds = spawnArea.GetComponent<Collider>().bounds;
		SpawnItemTraining();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (trainee.dirToTarget.magnitude < 1)
		{
			TouchedTargetTraining();
		}
	}
	
	/// <summary>
	/// Use the ground's bounds to pick a random spawn position.
	/// </summary>
    public void SpawnItemTraining()
    {
        Vector3 randomSpawnPos = Vector3.zero;
        float randomPosX = Random.Range(-spawnAreaBounds.extents.x, spawnAreaBounds.extents.x);
        float randomPosZ = Random.Range(-spawnAreaBounds.extents.z, spawnAreaBounds.extents.z);
        transform.position = spawnArea.transform.position + new Vector3(randomPosX, 1f, randomPosZ);
    }

	/// <summary>
    /// Agent touched the target
    /// </summary>
	public void TouchedTargetTraining()
	{
		trainee.AddReward(1); //Dog Fetch
		SpawnItemTraining();
		trainee.Done();
	}
	
}
