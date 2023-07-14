using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController_EASY : MonoBehaviour{
    //[SerializeField]
    private Animator animator = null;

    private DictationScript dict_script = null;

    private int score = 0;

    public TextMeshProUGUI score_panel = null;
    public TextMeshProUGUI status_panel = null;

    //
    // wait = "Wait" State
    // training = Tag:"Training" State
    // finish = "Exit" State
    private string status = "ready";

    void Start(){
        Debug.Log("GameController_EASY.Start()");

        // get component
        animator = this.GetComponent<Animator>();
        dict_script = this.GetComponent<DictationScript>();
        dict_script.AddFlagWord("はじめる");
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

        switch(status){
            case "wait":
                if(dict_script.CheckFlagWord("はじめる") == true){
                    status = "ready";
                    dict_script.DeleteFlagWord("はじめる");
                    dict_script.AddFlagWord("はじめる");
                }
                break;
            case "training":
                if(dict_script.CheckFlagWord("おわり") == true){
                    status = "finish";
                    dict_script.DeleteFlagWord("おわり");
                    animator.SetTrigger("wait_Trigger");
                }
                break;
            default:
                break;
        }
    }
}
