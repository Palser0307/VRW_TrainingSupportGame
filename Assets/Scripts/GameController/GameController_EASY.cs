using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController_EASY : MonoBehaviour{
    [SerializeField]
    private Animator animator = null;

    private DictationScript dict_script = null;

    private int score = 0;

    public TextMeshProUGUI score_panel = null;
    public TextMeshProUGUI status_panel = null;
    public TextMeshProUGUI state_panel = null;

    //
    // wait = "Wait" State
    // training_1 = "Training_1" State
    // training_2 = "Training_2" State
    // training_3 = "Training_3" State
    // finish = "Exit" State
    [SerializeField]
    private string status = "wait";

    // 空かCLEAR!!かが入る
    [SerializeField]
    private string[] state_cleared = {/*start*/"", /*PushUp*/"", /*SitUp*/"", /*Squat*/"", /*finish*/""};
    private string nextkeyword = "";

    [SerializeField]
    private bool forceNextNode = false;

    void Start(){
        Debug.Log("GameController_EASY.Start()");

        // get component
        //animator = this.GetComponent<Animator>();
        dict_script = this.GetComponent<DictationScript>();
        dict_script.AddFlagWord("始める");
        this.nextkeyword = "始める";
    }

    // Update is called once per frame
    void Update(){
        // score加算
        int _score = dict_script.getScore();
        if(_score >= 0){
            score += _score;
            score_panel.text = "score: "+score+"";
        }

        this.status_panel.text = this.status;
        Update_StatePanel();

        switch(this.status){
            case "wait":
                if(dict_script.CheckFlagWord("始める") == true || forceNextNode){
                    forceNextNode = false;
                    this.status = "training_1";
                    dict_script.DeleteFlagWord("始める");
                    dict_script.AddFlagWord("次");
                    this.nextkeyword = "次";
                    animator.SetTrigger("Training_Start");
                    this.state_cleared[0] = "checked";
                    Debug.Log("next status ->"+this.status);
                }
                break;
            case "training_1":
                if(dict_script.CheckFlagWord("次") == true || forceNextNode){
                    forceNextNode = false;
                    this.status = "training_2";
                    dict_script.DeleteFlagWord("次");
                    dict_script.AddFlagWord("次");
                    this.nextkeyword = "次";
                    animator.SetTrigger("Next_Training_1");
                    this.state_cleared[1] = "CLEAR";
                    Debug.Log("next status ->"+this.status);
                }
                break;
            case "training_2":
                if(dict_script.CheckFlagWord("次") == true || forceNextNode){
                    forceNextNode = false;
                    this.status = "training_3";
                    dict_script.DeleteFlagWord("次");
                    dict_script.AddFlagWord("終わり");
                    this.nextkeyword = "終わり";
                    animator.SetTrigger("Next_Training_2");
                    this.state_cleared[2] = "CLEAR";
                    Debug.Log("next status ->"+this.status);
                }
                break;
            case "training_3":
                if(dict_script.CheckFlagWord("終わり") == true || forceNextNode){
                    forceNextNode = false;
                    this.status = "finish";
                    dict_script.DeleteFlagWord("終わり");
                    animator.SetTrigger("Next_Training_3");
                    this.state_cleared[3] = "CLEAR";
                    Debug.Log("next status ->"+this.status);
                }
                break;
            case "finish":
                    this.state_cleared[4] = "finish";
                break;
            default:
                Debug.LogWarning("undefined status : "+this.status);
                break;
        }
    }

    void Update_StatePanel(){
        this.state_panel.text
            = "next keyword->" + this.nextkeyword + "\n"
            + "はじめ:" + this.state_cleared[0] + "\n"
            + "腕立て伏せ:" + this.state_cleared[1] + "\n"
            + "上体起こし:" + this.state_cleared[2] + "\n"
            + "スクワット:" + this.state_cleared[3] + "\n"
            + "おわり:" + this.state_cleared[4];
    }
}
/*
save & load の参照コード

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SmartBall_ctrl : GameController_Script{

    // Cubeを出した数とその表示先
    [SerializeField]
    protected TextMeshProUGUI score_display = null;
    int score = 0;

    [SerializeField]
    protected Transform spawn_point = null;
    [SerializeField]
    protected GameObject Ball_Object = null;

    [SerializeField]
    protected int ball_stock = 10;
    [SerializeField]
    protected static int ball_stock_limit = 1000;
    [SerializeField]
    protected TextMeshProUGUI ball_stock_display = null;

    private new void Awake() {
        // ログ出力用オブジェクトがアタッチされているか
        if(LogWindow is null){
            Debug.LogError("this object \""+this.gameObject.name+"\" is not attached LOGSYSTEM Object.");
        }
    }

    // Start is called before the first frame update
    void Start(){
        // セーブデータのロード
        Load_PLData();
    }

    // Update is called once per frame
    new void Update(){
        /*if(Input.GetMouseButtonDown(0)==true){//左クリック
            Instantiate(Cube, SpawnPoint.transform.position, SpawnPoint.transform.rotation);
        }* /
        _temp_time += Time.deltaTime;
        if(_temp_time >= save_interval_time){
            _temp_time = 0.0f;
            Save_PLData();
            this.ball_stock++;
            OnPressedSpawnButton();

        }
        score_display.text = score.ToString();
        //ball_stock_display.text = ball_stock.ToString();
    }

    // スポーン数の保存
    protected new void Save_PLData(){
        PlayerPrefs.SetInt(Savedata_Key.sb_score, score);
        PlayerPrefs.SetInt(Savedata_Key.sb_ball, ball_stock);
        PlayerPrefs.Save();

        LogWindow.AddLogText(LogSystem.LogType.Event, "saved");
    }

    // スポーン数の読み取り
    protected new void Load_PLData(){
        score = PlayerPrefs.GetInt(Savedata_Key.sb_score, 0);
        ball_stock = PlayerPrefs.GetInt(Savedata_Key.sb_ball, 100);

        LogWindow.AddLogText(LogSystem.LogType.Event, "savedata loaded");
    }

    // データのリセット
    protected new void Reset_PLData(){
        // 各値を初期化＋ディスプレイも更新
        score = 0;
        score_display.text = score.ToString();
        ball_stock = 0;
        ball_stock_display.text = ball_stock.ToString();
        // 上書き保存
        Save_PLData();

        LogWindow.AddLogText(LogSystem.LogType.Event, "RESET");
    }

    public void AddScore(int _add_score){
        score += _add_score;
        score_display.text = score.ToString();
    }

    public void AddBall(int _num){
        ball_stock += _num;
        // 一応上限を
        if(ball_stock > ball_stock_limit){
            ball_stock = ball_stock_limit;
        }
        ball_stock_display.text = ball_stock.ToString();
    }

    /*---------------------------------
    |        Button Function          |
    ---------------------------------* /

    // RESETボタンを押された時の挙動
    //  - spawn_numに0を代入
    //  - spawn_numの表示も再更新
    //  - セーブ
    public void OnPressedRESETButton(){
        Reset_PLData();
    }

    // Spawnボタンを押された時の挙動
    public void OnPressedSpawnButton(){
        // LogWindow.AddLogText(LogSystem.LogType.Event, "OnPressedSpawnButton");
        if(this.ball_stock <= 0){
            ball_stock_display.text = "Empty!!";
            return;
        }
        this.ball_stock--;
        ball_stock_display.text = ball_stock.ToString();
        float randPower = 11f+(Random.value * 3f - 1.3f);
        GameObject _ball_Obj = Instantiate(Ball_Object, spawn_point.position, spawn_point.rotation);
        _ball_Obj.GetComponent<Rigidbody>().AddForce(Vector3.up * randPower, ForceMode.Impulse);
    }
}

*/