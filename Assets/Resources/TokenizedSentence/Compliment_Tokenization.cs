using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NMeCab;
using System.IO;

public class Compliment_Tokenization : MonoBehaviour{
    private NMeCab_Script mecab=null;

    [SerializeField]
    List<string> untokenized_compliments_list = new List<string>();

    [SerializeField]
    List<string[]> tokenized_compliments_list = new List<string[]>();

    void Start(){
        mecab = new NMeCab_Script();
        untokenized_compliments_list = Load_untokenized_compliments_from_file("compliments.txt");
        foreach(string _compliment in untokenized_compliments_list){
            // List<string[]> compliment_list = mecab.Parse(_compliment);
            tokenized_compliments_list.AddRange(mecab.Parse(_compliment));
            //Debug.Log(_compliment);
        }
        Write_tokenized_compliments_to_csv("tokenizedCompliments.csv", tokenized_compliments_list);
    }

    List<string> Load_untokenized_compliments_from_file(string _filename){
        List<string> load_list = new List<string>();
        using(StreamReader reader = new StreamReader(@"Assets/Resources/TokenizedSentence/"+_filename,System.Text.Encoding.UTF8)){
            Debug.Log(reader);
            string _line;
            while((_line = reader.ReadLine()) != null){
                // Debug.Log(_line);
                load_list.Add(_line);
            }
        }
        return load_list;
    }

    void Write_tokenized_compliments_to_csv(string _filename, List<string[]> _tokenized_compliments){
        using(StreamWriter writer = new StreamWriter(@"Assets/Resources/TokenizedSentence/"+_filename,/*appendを許可するか*/false,System.Text.Encoding.UTF8)){
            // ヘッダの書き込み
            writer.WriteLine(string.Join(",", new string[]{"表層形","品詞","読み","原形"}));
            foreach(string[] tokenizedSentence in _tokenized_compliments){
                writer.WriteLine(string.Join(",", tokenizedSentence));
            }
            writer.Close();
        }
    }

}
