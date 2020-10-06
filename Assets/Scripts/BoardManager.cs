using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour {

    public static BoardManager Instance { set; get; }
    private bool[,] allowedMoves { set; get; }

    public Chessman[,] Chessmans { set; get; }
    public Chessman selectedChessman;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> chessmanPrefabs;
    public List<Sprite> chessmanInfoSprite;
    public List<GameObject> activechessman, activechessman_white, activechessman_black;

    private Material previousMat;
    public Material selectedMat;

    public Canvas chess_info_canvas;

    public GameObject emptyGameObjectPrefab;

    private static int idx_update_i = 0;

    public int game_phase = -1; 
    public int pressed_inventory_chessman_id = -1;
    public int pressed_shop_chessman_id = -1;
    private float[] chessman_spawn_z_height = {0.8f, 0.8f, 0.6f, 0.7f, 0.9f, 0.55f};

    public int player_money;
    public int player_hp;
    public GameObject player_hp_go, player_money_go, player_round_go;
    public int round = 1;
    public int total_round;

    private void Start()
    {
        Instance = this;
        idx_update_i = 0;
        player_hp = 100;
        player_money = 4;
        total_round = 6;
        game_phase = -1;

        SpawnAllChessmans();
        // SpawnATestChessman();
        NotShowChessmanInfoCanvas();
        
    }

    private void Update()
    {
        // Update money & hp
        player_money_go.GetComponent<Text>().text = player_money.ToString();
        player_hp_go.GetComponent<Text>().text = player_hp.ToString();
        player_round_go.GetComponent<Text>().text = round.ToString() + "/" + total_round.ToString();

        // capture mouse click to pick selected Chessman
        UpdateSelection();

        // Draw chess board
        DrawChessBoard();

        // Display chessman info
        if(selectedChessman != null)
        {
            // if select any chessman, display chessman info
            ShowChessmanInfoCanvas(selectedChessman);
        }
        else
        {
            // if not, not display info panel
            if(pressed_inventory_chessman_id < 0 && pressed_shop_chessman_id < 0)
            {
                NotShowChessmanInfoCanvas();
            }
        }

        // Detect new selection
        if (Input.GetMouseButtonDown(0))
        {
            // Within ChessBoard area
            if(selectionX >=0 && selectionY >= 0)
            {
                if (selectedChessman == null)
                {
                    // select the chessman
                    bool ret = SelectChessman(selectionX, selectionY);  
                    // if an empty space 
                    if(!ret)
                    {
                        if(this.game_phase == 0 && this.pressed_inventory_chessman_id >= 0 && selectionY < 4)
                        {
                            // inventory allows
                            if(ButtonManagerScript.Instance.inventory_chessman_num[pressed_inventory_chessman_id] > 0)
                            {
                                this.SpawnChessman(pressed_inventory_chessman_id, selectionX, selectionY, chessman_spawn_z_height[pressed_inventory_chessman_id]);
                                ButtonManagerScript.Instance.inventory_chessman_num[pressed_inventory_chessman_id] -= 1;
                                this.pressed_inventory_chessman_id = -1;
                            }
                            
                        }
                    }  
                }
                else
                {
                    // de-select the chessman
                    DeSelectChessman();
                    this.pressed_inventory_chessman_id = -1;
                    this.pressed_shop_chessman_id = -1;
                }
            }
            else //not select any thing
            {
                DeSelectChessman();
                selectedChessman = null;
            }
        }

        // gamephase = 1 : Fighting phase
        if(this.game_phase == 1 || this.game_phase == -1)
        {
            idx_update_i += 1;
            if(activechessman.Count > 0)
            {
                int i = idx_update_i % activechessman.Count;
                Chessman c = activechessman[i].GetComponent<Chessman>();
                if(c != null)
                {
                    c.AIAction();
                }
            }

            bool to_end_phase = true;
            for(int i=0; i<activechessman.Count; i++)
            {
                Chessman c = activechessman[i].GetComponent<Chessman>();
                if(!c.come_to_end_phase)
                {
                    to_end_phase = false;
                }
            }
            if(activechessman_white.Count == 0 || activechessman_black.Count == 0)
            {
                to_end_phase = true;
            }

            if(to_end_phase && this.game_phase == 1)
            {
                this.game_phase = 2;
            }
        }

        // anytime player hp < 0, end game
        if(this.game_phase == 1 && player_hp < 0)
        {
            this.game_phase = 2;
        }
        
        
            

    }

    private bool SelectChessman(int x, int y)
    {
        if (Chessmans[x, y] == null)
        {
            return false;
        }

        selectedChessman = Chessmans[x, y];

        // change render for selected chessman
        previousMat = selectedChessman.GetComponent<MeshRenderer>().material;
        selectedMat.mainTexture = previousMat.mainTexture;
        selectedChessman.GetComponent<MeshRenderer>().material = selectedMat;

        // BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves);

        return true;
    }

    public void ShowChessmanInfoCanvas(Chessman selectedChessman)
    {
        // chess_info_canvas.enabled = true;

        string chess_id = null, attack_type = null;
        int hp = -1, damage = -1, cost = -1, maxhp = -1;
        selectedChessman.GetInfo(ref chess_id, ref hp, ref maxhp, ref attack_type, ref damage, ref cost);
        string [] chess_id_list = {"King", "Queen", "Rook", "Bishop", "Knight", "Pawn"};

        Debug.Log(hp);

        GameObject num_txt_panel = chess_info_canvas.transform.Find("num_txt").gameObject;
        Debug.Log(num_txt_panel);
        num_txt_panel.SetActive(true);
        GameObject info_txt_panel = chess_info_canvas.transform.Find("info_txt").gameObject;
        info_txt_panel.SetActive(true);
        GameObject go;
        go = num_txt_panel.transform.Find("chess_id").gameObject;
        go.GetComponent<Text>().text = chess_id;
        go = num_txt_panel.transform.Find("hp").gameObject;
        go.GetComponent<Text>().text = hp.ToString() + " / " + maxhp.ToString();
        go = num_txt_panel.transform.Find("attack_type").gameObject;
        go.GetComponent<Text>().text = attack_type;
        go = num_txt_panel.transform.Find("damage").gameObject;
        go.GetComponent<Text>().text = damage.ToString();
        go = num_txt_panel.transform.Find("cost").gameObject;
        go.GetComponent<Text>().text = cost.ToString();
        go = num_txt_panel.transform.Find("Image").gameObject;
        go.GetComponent<Image>().sprite = null;
        for (int i = 0; i < 6; i++)
        {
            if(chess_id_list[i] == chess_id)
            {
                if(selectedChessman.isWhite)
                {
                    go.GetComponent<Image>().sprite = chessmanInfoSprite[i];
                }
                else
                {
                    go.GetComponent<Image>().sprite = chessmanInfoSprite[i + 6];
                }
                break;
            }
        }
        
    }

    private void NotShowChessmanInfoCanvas()
    {
        GameObject num_txt_panel = chess_info_canvas.transform.Find("num_txt").gameObject;
        num_txt_panel.SetActive(false);
        GameObject info_txt_panel = chess_info_canvas.transform.Find("info_txt").gameObject;
        info_txt_panel.SetActive(false);
        // chess_info_canvas.enabled = false;
    }

    private void DeSelectChessman()
    {
        if(selectedChessman != null)
            selectedChessman.GetComponent<MeshRenderer>().material = previousMat;
        selectedChessman = null;
    }

    private void UpdateSelection()
    {
        if(!Camera.main){
            return;
        }

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 
                           out hit, 
                           25.0f,
                           LayerMask.GetMask("ChessPlane")))
        {
            //Debug.Log(hit.point);
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    private void SpawnChessman(int index, int x, int y, float z)
    {
        Vector3 position = GetTileCenter(x, y) + Vector3.up * (z + 0.2f);
        GameObject empty = Instantiate(emptyGameObjectPrefab, position, Quaternion.Euler(0, 0, 0)) as GameObject;
        empty.transform.SetParent(transform);
        GameObject go = Instantiate(chessmanPrefabs[index], Vector3.zero, Quaternion.Euler(90, 0, 0)) as GameObject;
        go.transform.SetParent(empty.transform);

        Chessmans[x, y] = go.GetComponent<Chessman>();
        Chessmans[x, y].SetPosition(x, y);
        activechessman.Add(go);
        if(index <= 5)
        {
            activechessman_white.Add(go);
        }
        else
        {
            activechessman_black.Add(go);
        }

        Chessmans[x, y].InitInfo();
    }

    public void SpawnAllChessmans()
    {
        activechessman = new List<GameObject>();
        activechessman_white = new List<GameObject>();
        activechessman_black = new List<GameObject>();
        Chessmans = new Chessman[8, 8];

        //Spawn the white team
        SpawnChessman(0, 3, 0, 0.8f); // King
        SpawnChessman(1, 4, 0, 0.8f); // Queen
        SpawnChessman(4, 0, 0, 0.9f); // Knight
        SpawnChessman(4, 7, 0, 0.9f); 
        SpawnChessman(3, 2, 0, 0.7f); // Bishop
        SpawnChessman(3, 5, 0, 0.7f);
        SpawnChessman(4, 1, 0, 0.9f); // Knight
        SpawnChessman(4, 6, 0, 0.9f);
        for (int i = 1; i < 7; i++)
        {
            SpawnChessman(5, i, 1, 0.55f);
        }
        SpawnChessman(2, 0, 1, 0.6f); // Rook
        SpawnChessman(2, 7, 1, 0.6f); 

        //Spawn the black team
        SpawnChessman(6, 4, 7, 0.8f); // King
        SpawnChessman(7, 3, 7, 0.8f); // Queen
        SpawnChessman(10, 0, 7, 0.9f); // Rook
        SpawnChessman(10, 7, 7, 0.9f);
        SpawnChessman(9, 2, 7, 0.7f); // Bishop
        SpawnChessman(9, 5, 7, 0.7f);
        SpawnChessman(10, 1, 7, 0.9f); // Knight
        SpawnChessman(10, 6, 7, 0.9f);
        for (int i = 1; i < 7; i++)
        {
            SpawnChessman(11, i, 6, 0.55f);
        }
        SpawnChessman(8, 0, 6, 0.6f); // Rook
        SpawnChessman(8, 7, 6, 0.6f); 
    }

    public void SpawnChessmanAtRound()
    {
        activechessman = new List<GameObject>();
        activechessman_white = new List<GameObject>();
        activechessman_black = new List<GameObject>();
        Chessmans = new Chessman[8, 8];

        if(round == 1) // 2 * Pawn = 2   // 2
        {
            SpawnChessman(11, 3, 6, 0.55f);
            SpawnChessman(11, 4, 6, 0.55f);
        }

        if(round == 2) // 2 * Pawn + 2 * Kinight = 6   // 8
        {
            SpawnChessman(11, 3, 6, 0.55f); 
            SpawnChessman(11, 4, 6, 0.55f);
            SpawnChessman(10, 1, 7, 0.9f); // Knight
            SpawnChessman(10, 6, 7, 0.9f);
        }

        if(round == 3) // 2 * Pawn + 2 * Kinight + 2 * Rook = 12  // 16
        {
            SpawnChessman(8, 3, 6, 0.55f); // Rook
            SpawnChessman(8, 4, 6, 0.55f);
            SpawnChessman(11, 1, 6, 0.55f); 
            SpawnChessman(11, 6, 6, 0.55f);
            SpawnChessman(10, 1, 7, 0.9f); // Knight
            SpawnChessman(10, 6, 7, 0.9f);
            
        }

        if(round == 4) // 4 * Pawn + 2 * Kinight + 2 * Rook + 2 * Bishop = 24    // 26
        {
            SpawnChessman(11, 1, 6, 0.55f); // Pawn
            SpawnChessman(11, 6, 6, 0.55f);
            SpawnChessman(11, 2, 6, 0.55f); 
            SpawnChessman(11, 5, 6, 0.55f);
            SpawnChessman(8, 3, 6, 0.55f); // Rook
            SpawnChessman(8, 4, 6, 0.55f);
            SpawnChessman(10, 1, 7, 0.9f); // Knight
            SpawnChessman(10, 6, 7, 0.9f);
            SpawnChessman(9, 2, 7, 0.7f); // Bishop
            SpawnChessman(9, 5, 7, 0.7f);
        }

        if(round == 5) // 3 * Pawn + 4 * Kinight + 3 * Rook + 2 * Bishop + 1 * Queen = 33    // 38
        {
            SpawnChessman(11, 1, 6, 0.55f); // Pawn
            SpawnChessman(11, 6, 6, 0.55f);
            SpawnChessman(11, 2, 6, 0.55f); 
            SpawnChessman(8, 5, 6, 0.55f);
            SpawnChessman(8, 3, 6, 0.55f); // Rook
            SpawnChessman(8, 4, 6, 0.55f);
            SpawnChessman(10, 1, 7, 0.9f); // Knight
            SpawnChessman(10, 6, 7, 0.9f);
            SpawnChessman(10, 0, 7, 0.9f);
            SpawnChessman(10, 7, 7, 0.9f);
            SpawnChessman(9, 2, 7, 0.7f); // Bishop
            SpawnChessman(9, 5, 7, 0.7f);
            SpawnChessman(7, 3, 7, 0.8f); // Queen
        }

        if(round == 6) // 2 * Pawn + 4 * Kinight + 6 * Rook + 2 * Bishop + 1 * Queen + 1 * King = 47  // 52
        {
            SpawnChessman(8, 1, 6, 0.55f); // Rook
            SpawnChessman(8, 6, 6, 0.55f);
            SpawnChessman(8, 2, 6, 0.55f); 
            SpawnChessman(8, 5, 6, 0.55f);
            SpawnChessman(8, 3, 6, 0.55f); 
            SpawnChessman(8, 4, 6, 0.55f);
            SpawnChessman(11, 0, 6, 0.55f); // Pawn
            SpawnChessman(11, 7, 6, 0.55f);
            SpawnChessman(10, 1, 7, 0.9f); // Knight
            SpawnChessman(10, 6, 7, 0.9f);
            SpawnChessman(10, 0, 7, 0.9f);
            SpawnChessman(10, 7, 7, 0.9f);
            SpawnChessman(9, 2, 7, 0.7f); // Bishop
            SpawnChessman(9, 5, 7, 0.7f);
            SpawnChessman(7, 3, 7, 0.8f); // Queen
            SpawnChessman(6, 4, 7, 0.8f); // King
        }

    }


    public void DestroyAllChessman()
    {
        foreach (GameObject go in activechessman)
            Destroy(go.transform.parent.gameObject);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }

    private void DrawChessBoard () 
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for (int i = 0; i <= 8; i ++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            Vector3 start_v = Vector3.right * i;
            Debug.DrawLine(start_v, start_v + heightLine);

        }

        // Draw selection
        if(selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(
                Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
        }
    }

}
