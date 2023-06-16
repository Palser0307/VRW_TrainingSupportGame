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

public class mecab_test : MonoBehaviour{

/*
ログ代わりの最初期版
このクラスはこいつ単体だった
勿論グローバル変数も無い
*/
    void Start(){
        string sentence = "昔々あるところにおじいさんとおばあさんが住んでいました";

        MeCabTagger t = MeCabTagger.Create(@"Assets/NMeCab-0.10.2/dic/ipadic");
        var nodes = t.Parse(sentence);
        foreach(var node in nodes){
            //Debug.LogFormat("表層形: {0}\nToString:{1}",node.Surface,node.ToString());
            //Debug.LogFormat("表層形: {0}\nFeature:{1}\n",node.Surface,node.Feature);
            Debug.LogFormat("表層形: {0}\n品詞  : {1}\n読み  : {2}\n原形  : {3}",node.Surface,node.GetFeatureAt(0),node.GetFeatureAt(8),node.GetFeatureAt(6));
        }
    }
}