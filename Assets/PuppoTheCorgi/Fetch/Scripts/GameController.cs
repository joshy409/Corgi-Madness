using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MLAgents;
public class GameController : MonoBehaviour {
	[Header("GAME UI")]
	public GameObject titlePanel;
	public GameObject backButton;
	public GameObject touchToThrowPanel;

	Throw throwController;

	[Header("CAMERAS")]

	public CinemachineVirtualCamera cameraTitle;
	public CinemachineVirtualCamera cameraGame;
	public CinemachineBrain cmBrain;

	[Header("Music & Sound Effects")]
	public AudioSource audioSourceSFX;
	public AudioClip buttonClickStartSFX;
	public AudioClip buttonClickEndSFX;

    void Awake () {
		audioSourceSFX = gameObject.AddComponent<AudioSource>();
		throwController = GetComponent<Throw>();
		cmBrain = FindObjectOfType<CinemachineBrain>();
        throwController.enabled = true;
        throwController.stickTitleScreen.SetActive(false);
        throwController.item.gameObject.SetActive(true);
        throwController.canThrow = true;
    }
	
	public void StartGame()
	{
		audioSourceSFX.PlayOneShot(buttonClickStartSFX, 1);

		titlePanel.SetActive(false);
		backButton.SetActive(true);
		cameraTitle.Priority = 1;
		cameraGame.Priority = 2;
		throwController.enabled = true;
		throwController.stickTitleScreen.SetActive(false);
		throwController.item.gameObject.SetActive(true);
		throwController.canThrow = true;
		StartCoroutine(ShowTouchToThrow());
	}

	IEnumerator ShowTouchToThrow()
	{
		touchToThrowPanel.SetActive(true);
		yield return new WaitForSeconds(2);
		touchToThrowPanel.SetActive(false);
	}

	public void EndGame()
	{
		audioSourceSFX.PlayOneShot(buttonClickEndSFX, 1);
		titlePanel.SetActive(true);
		backButton.SetActive(false);
		cameraTitle.Priority = 2;
		cameraGame.Priority = 1;
		throwController.item.gameObject.SetActive(false);
		throwController.dogAgent.target = throwController.returnPoint;
		throwController.enabled = false;
		throwController.stickTitleScreen.SetActive(true);

	}
}
