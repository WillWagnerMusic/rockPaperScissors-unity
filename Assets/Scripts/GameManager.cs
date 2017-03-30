using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GameManager for the rock paper scissors game, handles all game logic
//Patrick Ngo


public class GameManager : MonoBehaviour {

	//gameobject references neede for ui manipulations
	public GameObject timer;
	public GameObject playerChoiceImage;
	public GameObject aiChoiceImage;
	public GameObject playerScoreLabel;
	public GameObject aiScoreLabel;
	public GameObject winnerLabel;

	//ai modes
	private const int AIMODE_NORMAL = 0;
	private const int AIMODE_SUREWIN = 1;

	//choices
	private const int CHOICE_ROCK = 0;
	private const int CHOICE_PAPER = 1;
	private const int CHOICE_SCISSOR = 2;
	private const int CHOICE_NONE = 3;

	//winners
	private const int RESULT_NIL = 0;
	private const int RESULT_PLAYER = 1;
	private const int RESULT_AI = 2;

	//timer
	public float TIMER_DEFAULT = 11.0f;
	public float timeLeft = 0.0f;

	//win timer
	public float WIN_TIMER_DEFAULT = 2.0f;
	public float winTimeLeft = 0.0f;
	private bool showWinner = false;

	//selections
	private int aiSelection = 0;
	private int playerSelection = 3;

	//0 = nil, 1 = player wins, 2 = ai wins
	private int winner = 0;

	//scores
	private double aiScore = 0;
	private double playerScore = 0;

	//streaks
	private int playerWinningStreakMultiplier = 1;
	private int aiWinningStreakMultiplier = 1;

	//aimode
	private int aiMode = 0;

	// Use this for initialization
	void Start () 
	{
		//init timer
		timeLeft = TIMER_DEFAULT;

		//hide images
		playerChoiceImage.SetActive (false);
		aiChoiceImage.SetActive (false);

		//hide win text
		winnerLabel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update()
	{
		if (!showWinner) {
			timeLeft -= Time.deltaTime;


			//update timer ui
			UILabel lbl = timer.GetComponent<UILabel> ();
			lbl.text = ((int)timeLeft).ToString ();

			//countdown complete
			if (timeLeft <= 0) 
			{
				SelectionMade ();
			}
		}
		//show winner 
		else
		{
			winTimeLeft -= Time.deltaTime;

			//countdown complete, finish win animation
			if (winTimeLeft <= 0) 
			{
				showWinner = false;

				//reset win text timer
				winTimeLeft = WIN_TIMER_DEFAULT;


				//hide win text
				winnerLabel.SetActive(false);

				//hide images
				playerChoiceImage.SetActive (false);
				aiChoiceImage.SetActive (false);
			}
		}
	}

	private void AiSelection()
	{
		//normal mode
		if (aiMode == AIMODE_NORMAL) {
			aiSelection = Random.Range (0, 3);
		} 
		//AI will always win
		else if (aiMode == AIMODE_SUREWIN)
		{
			switch (playerSelection)
			{
			case CHOICE_PAPER:
					aiSelection = CHOICE_SCISSOR;
					break;
				case CHOICE_ROCK:
					aiSelection = CHOICE_PAPER;
					break;
				case CHOICE_SCISSOR:
					aiSelection = CHOICE_ROCK;
					break;
				default:
					break;
			}
		}
	}

	private void ResetGame()
	{
		aiScore = 0;
		playerScore = 0;

		SelectionMade ();
	}

	private void ResetTimer()
	{
		timeLeft = TIMER_DEFAULT;
		winTimeLeft = WIN_TIMER_DEFAULT;

		//hide images
		playerChoiceImage.SetActive (false);
		aiChoiceImage.SetActive (false);
	}

	private void SelectionMade()
	{
		//reset timer
		ResetTimer ();

		//ai makes a choice
		AiSelection ();

		//determine winner
		DetermineResult ();

		//update scores
		UpdateScores ();

		//update UI
		UpdateUI ();

		//update wininng streaks
		UpdateWinningStreaks ();

		//reset player choice
		playerSelection = CHOICE_NONE;
	}

	private void UpdateScores()
	{
		//increment scores
		if (winner == RESULT_PLAYER)
		{
			playerScore = playerScore + (1 * playerWinningStreakMultiplier);
		}
		else if (winner == RESULT_AI)
		{
			aiScore = aiScore + (1 * aiWinningStreakMultiplier);
		}
	}

	private void UpdateWinningStreaks()
	{
		//update streaks
		if (winner == RESULT_PLAYER) 
		{
			playerWinningStreakMultiplier = 2 * playerWinningStreakMultiplier;
			aiWinningStreakMultiplier = 1;
		} 
		else if (winner == RESULT_AI) 
		{
			aiWinningStreakMultiplier = 2 * aiWinningStreakMultiplier;
			playerWinningStreakMultiplier = 1;
		} 
		//tie
		else 
		{
			aiWinningStreakMultiplier = 1;
			playerWinningStreakMultiplier = 1;
		}
	}


	private void UpdateUI()
	{
		//show images
		playerChoiceImage.SetActive (true);
		aiChoiceImage.SetActive (true);

		//show winner text
		winnerLabel.SetActive(true);

		//Update selection sprites
		setSelectionSprite(aiSelection, aiChoiceImage);
		setSelectionSprite(playerSelection, playerChoiceImage);

		//Update score labels
		UILabel playerLabel = playerScoreLabel.GetComponent<UILabel>();
		UILabel aiLabel = aiScoreLabel.GetComponent<UILabel>();

		playerLabel.text = playerScore.ToString();
		aiLabel.text = aiScore.ToString();

		//show winner text
		UILabel winLabel = winnerLabel.GetComponent<UILabel>();

		string winnerText = "";
		int pointsWon = 0;
		if (winner == RESULT_AI) 
		{
			winnerText = "AI";
			pointsWon = 1 * aiWinningStreakMultiplier;
			winLabel.text = winnerText + " wins! " + "+" + pointsWon;
		} 
		else if (winner == RESULT_PLAYER)
		{
			winnerText = "Player";
			pointsWon = 1 * playerWinningStreakMultiplier;
			winLabel.text = winnerText + " wins! " + "+" + pointsWon;
		}
		else
		{
			winLabel.text = "TIE!";
		}

		showWinner = true;
	}

	private void setSelectionSprite(int selection, GameObject go)
	{
		//Update selection sprites
		UISprite sprite = go.GetComponent<UISprite>();

		switch (selection)
		{
			case CHOICE_PAPER:
				sprite.spriteName = "paper";
				break;
			case CHOICE_ROCK:
				sprite.spriteName = "stone";
				break;
			case CHOICE_SCISSOR:
				sprite.spriteName = "scissor";
				break;
			//no selection
			default:
				go.SetActive (false);
				break;
		}
	}

	//logic to determine the winner
	private void DetermineResult()
	{
		if (playerSelection == CHOICE_ROCK) 
		{
			switch (aiSelection) 
			{
				case CHOICE_PAPER:
					winner = RESULT_AI;
					break;
				case CHOICE_ROCK:
					winner = RESULT_NIL;
					break;
				case CHOICE_SCISSOR:
					winner = RESULT_PLAYER;
					break;
				default:
					winner = RESULT_NIL;
					break;
			}
		}
		if (playerSelection == CHOICE_PAPER) 
		{
			switch (aiSelection) 
			{
				case CHOICE_PAPER:
					winner = RESULT_NIL;
					break;
				case CHOICE_ROCK:
					winner = RESULT_PLAYER;
					break;
				case CHOICE_SCISSOR:
					winner = RESULT_AI;
					break;
				default:
					winner = RESULT_NIL;
					break;
			}
		}
		if (playerSelection == CHOICE_SCISSOR) 
		{
			switch (aiSelection) 
			{
				case CHOICE_PAPER:
					winner = RESULT_PLAYER;
					break;
				case CHOICE_ROCK:
					winner = RESULT_AI;
					break;
				case CHOICE_SCISSOR:
					winner = RESULT_NIL;
					break;
				default:
					winner = RESULT_NIL;
					break;
			}
		}
		if (playerSelection == CHOICE_NONE) 
		{
			winner = RESULT_AI;
		}
	}





	//Button receiver methods
	public void SelectRock()
	{
		playerSelection = CHOICE_ROCK;
		SelectionMade ();
	}
	public void SelectScissors()
	{
		playerSelection = CHOICE_SCISSOR;
		SelectionMade ();
	}
	public void SelectPaper()
	{
		playerSelection = CHOICE_PAPER;
		SelectionMade ();
	}

	public void ToggleAIMode()
	{
		if (aiMode == AIMODE_NORMAL) 
		{
			aiMode = AIMODE_SUREWIN;
			Debug.Log ("Constant losing state mode activated");
		}
		else if (aiMode == AIMODE_SUREWIN) 
		{
			aiMode = AIMODE_NORMAL;
			Debug.Log ("Normal Mode");
		}
	}
}
