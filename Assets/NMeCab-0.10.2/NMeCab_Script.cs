using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NMeCab;
/*
こことか
https://qiita.com/Ubermensch/items/f8320bbbf18ef227fc1e

公式とか
https://github.com/komutan/NMeCab#%E4%BD%BF%E3%81%84%E6%96%B9

から切り貼り
*/

public class NMeCab_Script : MonoBehaviour{
    private MeCabTagger tagger = null;
    public enum enum_Status{
        MISSING,
        READY,
        NULL,
    }
    public enum_Status status = enum_Status.NULL;

    // 一応実装しておくが，可能なら下のコンストラクタを使ってほしい
    // = こいつをオブジェクトにアタッチするな
    void Start(){
        tagger = MeCabTagger.Create(@"Assets/NMeCab-0.10.2/dic/ipadic");
        if(tagger != null){
            status = enum_Status.READY;
        }else{
            status = enum_Status.MISSING;
        }
    }

    // 懐かしのコンストラクタ
    // 技術試験用に一応実装しておく
    public NMeCab_Script(){
        Debug.Log("NMeCab_Scriptのコンストラクタ");
        tagger = MeCabTagger.Create(@"Assets/NMeCab-0.10.2/dic/ipadic");
        if(tagger != null){
            status = enum_Status.READY;
        }else{
            status = enum_Status.MISSING;
        }
    }

    // 与えられた日本語の文章を形態素解析する
    // 返り値がnullだったら処理失敗してるよ
    //
    // 返り値Listの中のstring[]に関して
    // [0]: 表層形 そのままの形
    // [1]: 品詞   名詞，動詞，とか
    // [2]: 読み   読み方をカタカナで
    // [3]: 原形   「住み」->「住む」
    public List<string[]> Parse(string sentense){
        // とりあえずsentenseが空だったらnullを返して終了
        if(sentense == "" || sentense == null){
            Debug.LogErrorFormat("sentense \"{0}\" is null",sentense);
            return null;
        }
        // 形態素解析
        var nodes = tagger.Parse(sentense);
        // nodesが空だったらnullを返して終了
        if(nodes == null){
            Debug.LogError("parse failed!");
            return null;
        }

        // 返り値用の変数を宣言
        List<string[]> return_temp = new List<string[]>();

        // 解析結果から必要な部分を取り出す
        foreach(var node in nodes){
            // とりあえず標準出力
            Debug.LogFormat("表層形: {0}\n品詞  : {1}\n読み  : {2}\n原形  : {3}",node.Surface,node.GetFeatureAt(0),node.GetFeatureAt(8),node.GetFeatureAt(6));

            // 返り値に格納する
            // 表層形，品詞，読み，原形 を格納
            string[] str_temp = {node.Surface, node.GetFeatureAt(0),node.GetFeatureAt(8),node.GetFeatureAt(6)};
            return_temp.Add(str_temp);
        }

        return return_temp;
    }
/*
ログ代わりの最初期版
このクラスはこいつ単体だった
勿論グローバル変数も無い
    void Start(){
        string sentence = "Unityで形態素解析";

        MeCabTagger t = MeCabTagger.Create(@"Assets/NMeCab-0.10.2/dic/ipadic");
        var nodes = t.Parse(sentence);
        foreach(var node in nodes){
            //Debug.LogFormat("表層形: {0}\nToString:{1}",node.Surface,node.ToString());
            //Debug.LogFormat("表層形: {0}\nFeature:{1}\n",node.Surface,node.Feature);
            Debug.LogFormat("表層形: {0}\n品詞  : {1}\n読み  : {2}",node.Surface,node.GetFeatureAt(0),node.GetFeatureAt(8));
        }
    }
*/

}
