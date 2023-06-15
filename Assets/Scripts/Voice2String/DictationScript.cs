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

public class DictationScript : MonoBehaviour{
    [SerializeField]
    private TextMeshProUGUI m_Recognitions;

    [SerializeField]
    private TextMeshProUGUI m_Hypotheses;

    private DictationRecognizer m_DictationRecognizer;

    void Start(){
        // 表示器のinit
        m_Recognitions.text = "";
        m_Hypotheses.text = "";

        // init
        // Start()内じゃないといけないらしい
        m_DictationRecognizer = new DictationRecognizer();

        // 各結果を得た後の動作設定？
        // フレーズが特定の認識精度で認識されたことを示すイベント
        m_DictationRecognizer.DictationResult += (text, confidence) =>{
            Debug.LogFormat("Dictation Result: {0}", text);
            m_Recognitions.text += text + "\n";
            m_Hypotheses.text = "";
        };

        // Recognizerが現在のフラグメントに対して仮説変更をするときにトリガされるイベント
        // ざっくり言うと，今わかってるとこを出力するようにしてる
        m_DictationRecognizer.DictationHypothesis += (text) =>{
            Debug.LogFormat("Dictation hypothesis: {0}", text);
            m_Hypotheses.text += text + "\n";
        };

        // 音声認識セッションを終了した時にトリガされるイベント
        // セッションタイムアウトもここに来る
        m_DictationRecognizer.DictationComplete += (completionCause) =>{
            if(completionCause != DictationCompletionCause.Complete){
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}", completionCause);
            }
            // とりあえずタイムアウトだったらセッションの再起動
            if(completionCause == DictationCompletionCause.TimeoutExceeded){
                m_DictationRecognizer.Start();
            }
        };

        // 音声認識セッションにエラーが発生したときにトリガされるイベント
        m_DictationRecognizer.DictationError += (error, hresult) =>{
            Debug.LogErrorFormat("Dictation ERROR: {0}; HResult = {1}", error, hresult);
        };

        // 待機時間の再設定 (float)
        // default: 5 second
        m_DictationRecognizer.InitialSilenceTimeoutSeconds = 2.0f;

        // 現セッション中待機時間 (float)
        // default: 20 second
        m_DictationRecognizer.AutoSilenceTimeoutSeconds = 30.0f;

        // セッション開始
        m_DictationRecognizer.Start();
    }

    // アプリ終了時実行の関数
    void OnApplicationQuit() {
        // 音声認識がまだ生きてたら殺す
        if(m_DictationRecognizer.Status != SpeechSystemStatus.Stopped){
            m_DictationRecognizer.Stop();
        }
    }

    // オブジェクト破棄時実行の関数
    private void OnDestroy() {
        // 音声認識がまだ生きてたら殺す
        if(m_DictationRecognizer.Status != SpeechSystemStatus.Stopped){
            m_DictationRecognizer.Stop();
        }
    }
}
