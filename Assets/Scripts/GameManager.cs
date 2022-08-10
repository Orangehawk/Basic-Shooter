using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public enum State
	{
		Menu,
		Playing,
		Paused,
		GameWon,
		GameLost
	}

	public static GameManager instance;

	[SerializeField]
	State initialState = State.Paused;

	[Header("Debug")]
	[SerializeField]
	int fps = 144;

	[SerializeField]
	UIManager uiManager;

	[SerializeField]
	State currentState;

	void Awake()
	{
		if (instance != null && instance != this)
		{
			Debug.LogWarning("Duplicate GameManager!");
			Destroy(this);
		}
		else
		{
			instance = this;
		}
	}

	private void OnValidate()
	{
		Application.targetFrameRate = fps;
	}

	// Start is called before the first frame update
	void Start()
	{
		uiManager = UIManager.instance;

		SetState(initialState);

		QualitySettings.vSyncCount = 0;  // VSync must be disabled
		Application.targetFrameRate = fps;
	}

	public void TogglePausePlay()
	{
		if(GetState() == State.Playing)
		{
			SetState(State.Paused);
		}
		else if(GetState() == State.Paused)
		{
			SetState(State.Playing);
		}
	}

	public void SetState(State state)
	{
		if (currentState != state)
		{
			currentState = state;
			HandleState();
		}
	}

	void HandleState()
	{
		switch (currentState)
		{
			case State.Menu:
				MenuState();
				break;
			case State.Playing:
				PlayingState();
				break;
			case State.Paused:
				PausedState();
				break;
			case State.GameWon:
				GameWonState();
				break;
			case State.GameLost:
				GameLostState();
				break;
			default:
				Debug.LogWarning($"No function for GameManager state \"{currentState}\"");
				break;
		}
	}

	void MenuState()
	{

	}

	void PlayingState()
	{
		uiManager.SetHudLayer("Playing");
		Cursor.lockState = CursorLockMode.Locked;

		Time.timeScale = 1;
	}

	void PausedState()
	{
		Time.timeScale = 0;
		uiManager.SetHudLayer("Paused");
		Cursor.lockState = CursorLockMode.None;
	}

	void GameWonState()
	{
		Time.timeScale = 0;
		uiManager.SetHudLayer("GameWon");
	}

	void GameLostState()
	{
		Time.timeScale = 0;
		uiManager.SetHudLayer("GameLost");
	}

	public State GetState()
	{
		return currentState;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Exit()
	{
		Application.Quit();
	}
}
