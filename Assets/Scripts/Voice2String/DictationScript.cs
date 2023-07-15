using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using TMPro;

/*
参照元:
https://docs.unity3d.com/ja/current/ScriptReference/Windows.Speech.DictationRecognizer.html
ほとんどコピペだから，説明もそんなに入れられない．
*/

public class FlagWord{
    public string keyword = "";
    public bool isExist = false;

    public FlagWord(string _keyword){
        this.keyword = _keyword;
        this.isExist = false;
    }
}

public class DictationScript : MonoBehaviour{
    [SerializeField]
    private TextMeshProUGUI m_Recognitions;

    // フラグメント出力用
    [SerializeField]
    private TextMeshProUGUI m_Hypotheses;
    private bool is_HypothesesUGUI_active = false;

    private DictationRecognizer m_DictationRecognizer;

    // 形態素解析用ライブラリNMeCabの自作ライブラリ
    NMeCab_Script mecab = null;

    // NMeCab解析結果のスコアを格納する
    // 値域はint,0<=score
    // -1のときはまだスコアが出ていない状況
    private int score;
    public int getScore(){
        int _temp = score;
        score = -1;
        return _temp;
    }

    // フラグになるキーワードを持つか
    [SerializeField]
    private List<FlagWord> flagWords = null;

    void Start(){
        // NMeCabのライブラリのコンストラクタを走らせる
        mecab = new NMeCab_Script();

        // フラグメント用TMP GUIがアタッチされてないなら，初期設定を省略
        if(m_Hypotheses == null){
            is_HypothesesUGUI_active = false;
            Debug.Log("フラグメント出力用のTMP GUIがm_Hypothesesにアタッチされていません\n必要であればアタッチしてください");
        }else{
            // 表示器のinit
            m_Hypotheses.text = "";
        }

        if(m_Recognitions == null){
            Debug.LogError("認識結果の表示先が存在しません");
        }else{
            // 表示器のinit
            m_Recognitions.text = "";
        }

        // scoreのinit
        score = -1;

        // フラグ用のinit
        flagWords = new List<FlagWord>();

        // init
        // Start()内じゃないといけないらしい
        this.m_DictationRecognizer = new DictationRecognizer();

        // 各結果を得た後の動作設定？
        // フレーズが特定の認識精度で認識されたことを示すイベント
        // これが出力する結果が聞き取った文字列
        this.m_DictationRecognizer.DictationResult += (text, confidence) =>{
            Debug.LogFormat("Dictation Result: {0}", text);
            //m_Recognitions.text += text + "\n";
            m_Recognitions.text = text;
            if(is_HypothesesUGUI_active){
                m_Hypotheses.text = "";
            }
            List<string[]> _tokenizedSentence = mecab.Parse(text);
            this.score = mecab.CalculateScoreFromTokenizedSentence(_tokenizedSentence);
            FlagCheck(_tokenizedSentence);
        };

        // Recognizerが現在のフラグメントに対して仮説変更をするときにトリガされるイベント
        // ざっくり言うと，今わかってるとこを出力するようにしてる
        this.m_DictationRecognizer.DictationHypothesis += (text) =>{
            Debug.LogFormat("Dictation hypothesis: {0}", text);
            if(is_HypothesesUGUI_active){
                m_Hypotheses.text += text + "\n";
            }
        };

        // 音声認識セッションを終了した時にトリガされるイベント
        // セッションタイムアウトもここに来る
        this.m_DictationRecognizer.DictationComplete += (completionCause) =>{
            if(completionCause != DictationCompletionCause.Complete){
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}", completionCause);
                this.m_DictationRecognizer.Start();
            }
            // とりあえずタイムアウトだったらセッションの再起動
            if(completionCause == DictationCompletionCause.TimeoutExceeded){
                this.m_DictationRecognizer.Start();
            }
        };

        // 音声認識セッションにエラーが発生したときにトリガされるイベント
        this.m_DictationRecognizer.DictationError += (error, hresult) =>{
            Debug.LogErrorFormat("Dictation ERROR: {0}; HResult = {1}", error, hresult);
        };

        // 待機時間の再設定 (float)
        // default: 5 second
        this.m_DictationRecognizer.InitialSilenceTimeoutSeconds = 2.0f;

        // 現セッション中待機時間 (float)
        // default: 20 second
        this.m_DictationRecognizer.AutoSilenceTimeoutSeconds = 30.0f;

        // セッション開始
        this.m_DictationRecognizer.Start();
    }

    void Update() {
        if(this.m_DictationRecognizer.Status == SpeechSystemStatus.Stopped){
            this.m_DictationRecognizer.Start();
        }
    }

    // 認識した発言をMeCabでParseした形態素のリスト_tokenizedSentenceに
    // キーワードが含まれていないか確認
    // 計算時間はO(n^2) 多分ね
    void FlagCheck(List<string[]> _tokenizedSentence){
        //this.flagWords.Count
        for(int i=0;i<this.flagWords.Count;i++){
            if(this.flagWords[i].isExist == true){
                continue;
            }
            for(int j=0;j<_tokenizedSentence.Count;j++){
                // [2]が読み（カタカナ）
                // [3]が原形（漢字含む）
                if(this.flagWords[i].keyword == _tokenizedSentence[j][3]){
                    this.flagWords[i].isExist = true;
                }
            }
        }
    }
    public void AddFlagWord(string _keyword){
        List<string[]> _tokenized_keyword_list = mecab.Parse(_keyword);
        string _tokenized_keyword = _tokenized_keyword_list[0][3];
        flagWords.Add(new FlagWord(_tokenized_keyword));
    }

    // 与えられたキーワードがisExist=Trueになっているか
    // 計算時間はO(n)ぐらいのはず
    public bool CheckFlagWord(string _keyword){
        List<string[]> _tokenized_keyword_list = mecab.Parse(_keyword);
        string _tokenized_keyword = _tokenized_keyword_list[0][3];
        if(this.flagWords.Count == 0){
            return false;
        }
        int num = 0;

        for(int i = 0; i < this.flagWords.Count; i++){
            if(this.flagWords[i].keyword == _tokenized_keyword){
                bool _temp = this.flagWords[i].isExist;
                //Debug.Log("check flag word: "+_temp+"");
                //return this.flagWords[i].isExist;
                num = i;
                break;
            }
        }
        // Debug.Log("check flag word: "+this.flagWords[num].isExist);
        return this.flagWords[num].isExist;
    }
    public bool DeleteFlagWord(string _keyword){
        List<string[]> _tokenized_keyword_list = mecab.Parse(_keyword);
        string _tokenized_keyword = _tokenized_keyword_list[0][3];
        for(int i = 0; i < this.flagWords.Count; i++){
            if(this.flagWords[i].keyword == _tokenized_keyword){
                return this.flagWords.Remove(this.flagWords[i]);
            }
        }
        return false;
    }

    /*
    null referense exceptionを返すのでとりあえずコメントアウト
    */
    // アプリ終了時実行の関数
    void OnApplicationQuit() {
        // 音声認識がまだ生きてたら殺す
        if(this.m_DictationRecognizer.Status != SpeechSystemStatus.Stopped){
            this.m_DictationRecognizer.Stop();
        }
    }

    // オブジェクト破棄時実行の関数
    private void OnDestroy() {
        // 音声認識がまだ生きてたら殺す
        if(this.m_DictationRecognizer.Status != SpeechSystemStatus.Stopped){
            this.m_DictationRecognizer.Stop();
        }
    }
    //*/

    private void OnApplicationFocus(bool focusStatus) {
        if(focusStatus == true){
            Debug.Log("アプリが選択されたんで音声認識を起動");
            if(this.m_DictationRecognizer.Status == SpeechSystemStatus.Stopped){
                this.m_DictationRecognizer.Start();
            }
        }else{
            Debug.Log("アプリが選択されなくなったんで音声認識を停止");
            if(this.m_DictationRecognizer.Status == SpeechSystemStatus.Running){
                this.m_DictationRecognizer.Stop();
            }
        }
    }
}
