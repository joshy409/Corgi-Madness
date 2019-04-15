using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {

	[Header("GENERAL")]
	// When true, the player can throw the stick
	public bool canThrow; 
	// A reference to the item the player is allowed to throw
	public Transform item;
	// A reference to the stick in the dog's mouth when the game starts
	public GameObject stickTitleScreen;

	// The position of the player. This is where the dog will return the stick
	public Transform returnPoint; 

	[HideInInspector]
	// The rigidbody of item
	public Rigidbody itemRB; 
	[HideInInspector]
	// Collider of item
	public Collider itemCol; 
	[HideInInspector]
	// The dog the player is playing with
	public DogAgent dogAgent; 

	[Header("HOLDING ITEM SETTINGS")]
	// The offset postion relative to the player
	public Vector3 holdingPositionOffset; 
	// The position of the stick relative to the player when holding
	Vector3 holdingPos; 
	// Velocity to use when holding the stick
	public float holdingItemTargetVelocity; 
	// Max velocity change allowed for the stick when being held
	public float holdingItemMaxVelocityChange; 

	[Header("THROWING PARAMETERS")]
	// Defines the strength of the player
	public float throwSpeed; 
	// Direction the player is thowing towards
	private Vector3 throwDir; 

	[Header("SOUND")]
	// List of throwing sounds clips
	public List<AudioClip> throwSounds = new List <AudioClip>();

    [Header("STICK")]
    public VRTK.VRTK_InteractableObject stick;

    [Header("Object Auto Grab Script")]
    public VRTK.VRTK_ObjectAutoGrab autoGrabScript;

    [Header("Controller Grab Button")]
    public VRTK.VRTK_InteractGrab rightController;

    //SFX audio source
    AudioSource audioSourceSFX; 
	// Starting position of touch/mouse click
	Vector3 startingPos; 
	// Current position of touch/mouse click
	Vector3 currentPos; 
	// Is true when the player is currently touching/clicking the screen
	bool currentlyTouching; 
	// Current Touch detected
	Touch currentTouch; 
	// Is true when touch input detected
	bool usingTouchInput; 
	// Is true when mouse input detected
	bool usingMouseInput;
	// Camera in the scene
	Camera cam; 

	/// <summary>
	/// Initialization iof the Throw component
	/// </summary>
	void Awake ()
	{
		cam = Camera.main;
		dogAgent = FindObjectOfType<DogAgent>();
		itemRB = item.GetComponent<Rigidbody>();
		itemCol = item.GetComponent<Collider>();
		audioSourceSFX = gameObject.AddComponent<AudioSource>();
		dogAgent.target = returnPoint;
        canThrow = true;
	}
	
	/// <summary>
	/// Is called when the player swipes the screen
	/// </summary>
	void StartSwipe()
	{
		startingPos = cam.ScreenToViewportPoint(Input.mousePosition) - new Vector3(0.5f, 0.5f, 0.0f);
		usingTouchInput = true;
		currentlyTouching = true;
		currentTouch = Input.GetTouch(0);
		if(!dogAgent.returningItem)
		{
			dogAgent.target = item;
		}
	}

	/// <summary>
	/// Is called when the player drags the mouse on the screen
	/// </summary>
	void StartMouseDrag()
	{
		startingPos = cam.ScreenToViewportPoint(Input.mousePosition) - new Vector3(0.5f, 0.5f, 0.0f);
		usingMouseInput = true;
		currentlyTouching = true;
		if(!dogAgent.returningItem)
		{
			dogAgent.target = item;
		}
	}

	/// <summary>
	/// This method is called when the player is throwing the item
	/// </summary>
	void ThrowItem()
	{
		canThrow = false;
		audioSourceSFX.PlayOneShot(throwSounds[Random.Range( 0, throwSounds.Count)], .25f);
		itemRB.velocity *= .5f;
		throwDir = (currentPos - startingPos);
		var dir = cam.transform.TransformDirection(throwDir) + cam.transform.forward;
		dir.y = 0;
		itemRB.AddForce(dir * throwSpeed, ForceMode.VelocityChange);
		StartCoroutine(DelayedThrow());
	}

	/// <summary>
	/// This Coroutine ensures the dog will wait a moment before going toward the target
	/// </summary>
	/// <returns></returns>
	IEnumerator DelayedThrow()
	{
		float elapsed = 0;
		while(elapsed < 2)
		{
			elapsed += Time.deltaTime;
			yield return null;
		}
		StartCoroutine(GoGetItemGame());
	}

	void FixedUpdate()
	{
		if(currentlyTouching)
		{
			if(usingTouchInput)
			{
				currentTouch = Input.GetTouch(0);

				currentPos = cam.ScreenToViewportPoint(currentTouch.position) - new Vector3(0.5f, 0.5f, 0.0f);
			}
			if(usingMouseInput)
			{
				currentPos = cam.ScreenToViewportPoint(Input.mousePosition) - new Vector3(0.5f, 0.5f, 0.0f);
			}
			holdingPos = cam.transform.TransformPoint(holdingPositionOffset + (currentPos * 2));
			Vector3 moveToPos = holdingPos - itemRB.position;  //cube needs to go to the standard Pos
			Vector3 velocityTarget = moveToPos * holdingItemTargetVelocity * Time.deltaTime; //not sure of the logic here, but it modifies velTarget
            itemRB.velocity = Vector3.MoveTowards(itemRB.velocity, velocityTarget, holdingItemMaxVelocityChange);
		}
	}

    private bool isgrabbed = false;
	void Update()
	{
		if(canThrow)
		{
			if (Input.touchCount > 0 && !currentlyTouching)
			{
				currentTouch = Input.GetTouch(0);
				if(currentTouch.phase == TouchPhase.Began)
				{
					StartSwipe();
				}
			}

			if(usingTouchInput && currentlyTouching)
			{
				currentTouch = Input.GetTouch(0);
				if(currentTouch.phase == TouchPhase.Ended)
				{
					currentlyTouching = false;
					ThrowItem();
				}
			}

			if (Input.GetMouseButtonDown(0) && !currentlyTouching)
			{
				StartMouseDrag();
			}

			if(usingMouseInput && currentlyTouching)
			{
				if(Input.GetMouseButtonUp(0))
				{
					currentlyTouching = false;
					ThrowItem();
				}
			}

            //TODO if statement to see if object is grabbed and released
            if (rightController.IsGrabButtonPressed())
            {
                Debug.Log("Right Grip Pressed");
                isgrabbed = true;
                autoGrabScript.GetComponent<VRTK.VRTK_ObjectAutoGrab>().enabled = true;
            }

            if (isgrabbed && !rightController.IsGrabButtonPressed())
            {
                Debug.Log("Right Grip Pressed");
                autoGrabScript.GetComponent<VRTK.VRTK_ObjectAutoGrab>().enabled = false;
                isgrabbed = false;
                ThrowItem();
            }
        }
    }
	
	/// <summary>
	/// The dog just picked the item up. We set it's target to be the player
	/// </summary>
	public void PickUpItemGame()
	{
		//Make the stick kinematic and put it in the dog's mouth
		itemCol.enabled = false; //Disable the collider on the stick.
		itemRB.isKinematic = true; //Turn off physics
		item.position = dogAgent.mouthPosition.position; //Set stick position
		item.rotation = dogAgent.mouthPosition.rotation; //Set stick rotation
		item.SetParent(dogAgent.mouthPosition); //Parent the stick to the dog's mouth
		dogAgent.runningToItem = false; //We are no longer running towards the stick

		//The stick is now in the dog's mouth so we need to change
		//the dog's target to the return point
		dogAgent.target = returnPoint; //set the dog's target to the return point
		dogAgent.UpdateDirToTarget(); //update the direction vector
		dogAgent.returningItem = true; //we are not returning the stick
	}

	/// <summary>
	/// The dog just droped the stick at the position of the player
	/// </summary>
	public void DropItemGame()
	{
		itemRB.isKinematic = false; //Enable physics on the stick 
		item.parent = null; //Stick no longer parented to the dog
		itemCol.enabled = true; //Re-enable the collider on the stick
		dogAgent.returningItem = false; //We are done returning the stick
        canThrow = true; //Dog has dropped the stick. We can now throw it again
	}
	
	/// <summary>
	/// Triggered when the player throws the item.
	/// </summary>
	/// <returns></returns>
	 public IEnumerator GoGetItemGame()
    {   
		if(Application.isEditor)
		{
			print("STARTING GoGetItemGame()"); //debug
		}

        //GO GET THE STICK
        dogAgent.target = item; //Set the target to the stick.
        dogAgent.runningToItem = true; //We are now running towards the stick

        //Wait till we are in range of the stick
        while(dogAgent.dirToTarget.sqrMagnitude > 1f) //wait until we are close
        {
            yield return null;
        }
		//Since we are in proximity to the stick
		//We should pick up the stick
        PickUpItemGame();

        //Wait till we are in range of the return point
        while(dogAgent.dirToTarget.sqrMagnitude > 1f) //wait until we are close
        {
            yield return null;
        }

		//Since we are back at the return point
		//We should drop the stick
        DropItemGame();

		if(Application.isEditor)
		{
			print("ENDING GoGetItemGame()"); //debug
		}
    }
	
}
