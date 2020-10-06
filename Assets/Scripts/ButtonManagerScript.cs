using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManagerScript : MonoBehaviour {
	public static ButtonManagerScript Instance { set; get; }
	public int[] inventory_chessman_num;
	public int[] inventory_chessman_total_num;
	public GameObject[] inventory_chessman_go;
	private Chessman vis_chessman;
	public GameObject end_phase_canvas;
	public GameObject fight_phase_canvas;
	public GameObject start_phase_canvas;
	public GameObject tutorial_phase_canvas;
	public GameObject ShopItem_1;
	
	public int win_or_loss = 0;
	public Camera StartPhaseCam, TutorialPhaseCam;
	public Camera normalCam;

    public AudioSource[] bgm;


    private void Start() {
		Instance = this;
		inventory_chessman_num = new int[]{0, 0, 0, 0, 0, 0};
		inventory_chessman_total_num = new int[]{0, 0, 0, 0, 0, 0};

        bgm = GetComponents<AudioSource>();


    }

	private void Update() {
		// Camera check
		if(BoardManager.Instance.game_phase == -1)
		{
			StartPhaseCam.enabled = true;
			normalCam.enabled = false;
			TutorialPhaseCam.enabled = false;
			fight_phase_canvas.SetActive(false);
			start_phase_canvas.SetActive(true);
			tutorial_phase_canvas.SetActive(false);
		}
		else if(BoardManager.Instance.game_phase == -2)
		{
			StartPhaseCam.enabled = false;
			normalCam.enabled = false;
			TutorialPhaseCam.enabled = true;
			fight_phase_canvas.SetActive(false);
			start_phase_canvas.SetActive(false);
			tutorial_phase_canvas.SetActive(true);
		}
		else
		{
			StartPhaseCam.enabled = false;
			normalCam.enabled = true;
			TutorialPhaseCam.enabled = false;
			fight_phase_canvas.SetActive(true);
			start_phase_canvas.SetActive(false);
            tutorial_phase_canvas.SetActive(false);
		}

        if (BoardManager.Instance.game_phase == 0)
        {
            //bgm[1].Play();
        }


        // Update inventory number
        for (int i = 0; i < 6; i++)
		{
			inventory_chessman_go[i].GetComponent<Text>().text = inventory_chessman_num[i].ToString();
		}

		// Updata EndPhase Canvas
		if(BoardManager.Instance.game_phase == 3)
		{

			GameObject go;
			int enemy_remain_num = BoardManager.Instance.activechessman_black.Count;
			go = end_phase_canvas.transform.Find("round").gameObject;
			go.GetComponent<Text>().text = BoardManager.Instance.round.ToString() + "/" + BoardManager.Instance.total_round.ToString();
			go = end_phase_canvas.transform.Find("enemy_remain").gameObject;
			go.GetComponent<Text>().text = enemy_remain_num.ToString();
			go = end_phase_canvas.transform.Find("hp_loss").gameObject;
			go.GetComponent<Text>().text = enemy_remain_num.ToString();
			go = end_phase_canvas.transform.Find("hp_left").gameObject;
			go.GetComponent<Text>().text = (BoardManager.Instance.player_hp - enemy_remain_num).ToString();

			if((BoardManager.Instance.player_hp - enemy_remain_num) <= 0)
			{
				// Lose
				go = end_phase_canvas.transform.Find("youlose").gameObject;
				go.SetActive(true);

				go = end_phase_canvas.transform.Find("continue_button").gameObject;
				go.GetComponent<RectTransform>().localPosition = new Vector3(160, -243, 0);

				go = go.transform.Find("button_text").gameObject;
				go.GetComponent<Text>().text = "END";

				win_or_loss = -1;
			}

			else if(BoardManager.Instance.round == BoardManager.Instance.total_round)
			{
				// Win
				go = end_phase_canvas.transform.Find("youwin").gameObject;
				go.SetActive(true);

				go = end_phase_canvas.transform.Find("continue_button").gameObject;
				go.GetComponent<RectTransform>().localPosition = new Vector3(-5, -322, 0);

				go = go.transform.Find("button_text").gameObject;
				go.GetComponent<Text>().text = "Win!";

				win_or_loss = 1;
			}

			else
			{
				// continue
				go = end_phase_canvas.transform.Find("continue_button").gameObject;
				go.GetComponent<RectTransform>().localPosition = new Vector3(160, -243, 0);
				win_or_loss = 0;
			}

			BoardManager.Instance.game_phase = 4;
		}
		
        
		// Check end phase
		if(BoardManager.Instance.game_phase == 2)
		{
			StartCoroutine(SleepForAWhile(1.5f));
			BoardManager.Instance.game_phase = 3;
		}
	}

	IEnumerator SleepForAWhile(float duration)
    {
        yield return new WaitForSeconds(duration);
        end_phase_canvas.SetActive(true);
		BoardManager.Instance.player_hp = (BoardManager.Instance.player_hp - BoardManager.Instance.activechessman_black.Count);
		BoardManager.Instance.round += 1;
    }

	public int a = 0;

	public void ReadyButtonPressed()
	{
		BoardManager.Instance.game_phase = 1;
        bgm[0].Play();
        bgm[1].Stop();
    }

	public void InventoryButtonPressed(int ChessmanID)
	{
		// Show Chessman Info
		GameObject go = Instantiate(BoardManager.Instance.chessmanPrefabs[ChessmanID], Vector3.zero, Quaternion.Euler(90, 0, 0)) as GameObject;
		vis_chessman = go.GetComponent<Chessman>();
		vis_chessman.InitInfo();
		BoardManager.Instance.ShowChessmanInfoCanvas(vis_chessman);
		Destroy(go);


		Debug.Log("Inventory choose chessman id = " + ChessmanID.ToString());
		BoardManager.Instance.pressed_inventory_chessman_id = ChessmanID;	
		BoardManager.Instance.pressed_shop_chessman_id = -1;
	}

	public void ContinueButtonPressed()
	{
		RefreshShopAfterEachRound();
		BoardManager.Instance.player_money += BoardManager.Instance.round * 2 + 2;
		if(this.win_or_loss != 0)
		{
			// restart the game
			BoardManager.Instance.game_phase = -1;
			InitGame();
			BoardManager.Instance.SpawnAllChessmans();
		}
		else
		{
			InitButtonCanvas();
			BoardManager.Instance.game_phase = 0;
			for (int i = 0; i < 6; i++)
			{
				inventory_chessman_num[i] = inventory_chessman_total_num[i];
			}
			BoardManager.Instance.DestroyAllChessman();
			BoardManager.Instance.SpawnChessmanAtRound();
			
		}
        bgm[1].Play();
        bgm[0].Stop();
	}

	public void RefreshShopAfterEachRound()
	{
		ShopItem_1.GetComponent<ShopItem>().RandomLoadNewItem(BoardManager.Instance.round);
		for(int i=0; i<2; i++)
		{
			ShopItem_1.GetComponent<ShopItem>().sibling_items[i].GetComponent<ShopItem>().RandomLoadNewItem(BoardManager.Instance.round);
		}
	}

	public void InitButtonCanvas()
	{
		end_phase_canvas.SetActive(false);
		GameObject go;
		go = end_phase_canvas.transform.Find("youlose").gameObject;
		go.SetActive(false);
		go = end_phase_canvas.transform.Find("youwin").gameObject;
		go.SetActive(false);
		
	}

	public void InitGame()
	{
		InitButtonCanvas();
		for (int i = 0; i < 6; i++)
		{
			inventory_chessman_num[i] = 0;
			inventory_chessman_total_num[i] = 0;
		}
		BoardManager.Instance.DestroyAllChessman();
		win_or_loss = 0;
		BoardManager.Instance.player_hp = 20;
		BoardManager.Instance.player_money = 4;
		BoardManager.Instance.round = 1;
		RefreshShopAfterEachRound();
	}

	public void NewGameButtonPressed()
	{
		BoardManager.Instance.game_phase = 0;
		InitGame();
		BoardManager.Instance.SpawnChessmanAtRound();
        bgm[1].Play();
        bgm[0].Stop();
		
	}

	public void TutorialExitButtonPressed()
	{
		BoardManager.Instance.game_phase = -1;
		InitGame();
		BoardManager.Instance.SpawnAllChessmans();
	}

	public void TutorialButtonPressed()
	{
		BoardManager.Instance.game_phase = -2;
		TutorialScript.Instance.current_page_idx = 0;
	}

	public void StuckAndEndRound()
	{
		// BoardManager.Instance.game_phase = 3;
		// end_phase_canvas.SetActive(true);
		// BoardManager.Instance.player_hp = (BoardManager.Instance.player_hp - BoardManager.Instance.activechessman_black.Count);
		// BoardManager.Instance.round += 1;
		StartCoroutine(SleepForAWhile(0.1f));
		BoardManager.Instance.game_phase = 3;
	}

	public void RespawnButtonPressed()
	{
		if(BoardManager.Instance.game_phase == 0)
		{
			BoardManager.Instance.DestroyAllChessman();
			BoardManager.Instance.SpawnChessmanAtRound();
			for (int i = 0; i < 6; i++)
			{
				inventory_chessman_num[i] = inventory_chessman_total_num[i];
			}
		}
		
	}

	public void Tutorial_set_pages_next_or_prev(int direction)
	{

		TutorialScript.Instance.current_page_idx += direction;
		if(TutorialScript.Instance.current_page_idx < 0)
		{
			TutorialScript.Instance.current_page_idx = 0;
		}
		else if(TutorialScript.Instance.current_page_idx >= TutorialScript.Instance.total_page_num)
		{
			TutorialScript.Instance.current_page_idx = TutorialScript.Instance.total_page_num - 1;
		}
		
	}

	
}
