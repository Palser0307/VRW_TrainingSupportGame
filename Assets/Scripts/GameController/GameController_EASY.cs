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

    //
    // wait = "Wait" State
    // training_1 = "Training_1" State
    // training_2 = "Training_2" State
    // training_3 = "Training_3" State
    // finish = "Exit" State
    [SerializeField]
    private string status = "wait";

    void Start(){
        Debug.Log("GameController_EASY.Start()");

        // get component
        //animator = this.GetComponent<Animator>();
        dict_script = this.GetComponent<DictationScript>();
        dict_script.AddFlagWord("始める");
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

        switch(this.status){
            case "wait":
                if(dict_script.CheckFlagWord("始める") == true){
                    this.status = "training_1";
                    dict_script.DeleteFlagWord("始める");
                    dict_script.AddFlagWord("次");
                    animator.SetTrigger("Training_Start");
                    Debug.Log("next status ->"+this.status);
                }
                break;
            case "training_1":
                if(dict_script.CheckFlagWord("次") == true){
                    this.status = "training_2";
                    dict_script.DeleteFlagWord("次");
                    dict_script.AddFlagWord("次");
                    animator.SetTrigger("Next_Training_1");
                    Debug.Log("next status ->"+this.status);
                }
                break;
            case "training_2":
                if(dict_script.CheckFlagWord("次") == true){
                    this.status = "training_3";
                    dict_script.DeleteFlagWord("次");
                    dict_script.AddFlagWord("終わり");
                    animator.SetTrigger("Next_Training_2");
                    Debug.Log("next status ->"+this.status);
                }
                break;
            case "training_3":
                if(dict_script.CheckFlagWord("終わり") == true){
                    this.status = "finish";
                    dict_script.DeleteFlagWord("終わり");
                    animator.SetTrigger("Next_Training_3");
                    Debug.Log("next status ->"+this.status);
                }
                break;
            case "finish":

                break;
            default:
                Debug.LogWarning("undefined status : "+this.status);
                break;
        }
    }
}
