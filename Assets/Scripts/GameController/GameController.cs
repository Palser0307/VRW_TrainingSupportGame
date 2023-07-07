using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour{
    //[SerializeField]
    private Animator animator = null;

    private DictationScript dict_script = null;

    private int score = 0;

    public TextMeshProUGUI score_panel = null;
    public TextMeshProUGUI status_panel = null;

    //
    // wait
    // ready
    // training
    // finish
    private string status = "ready";

    void Start(){
        Debug.Log("GameController.Start()");

        // get component
        animator = this.GetComponent<Animator>();
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

        switch(status){
            case "wait":
                if(dict_script.CheckFlagWord("始める") == true){
                    status = "ready";
                    dict_script.DeleteFlagWord("始める");
                    dict_script.AddFlagWord("始める");
                }
                break;
            case "ready":
                if(dict_script.CheckFlagWord("始める") == true){
                    status = "training";
                    dict_script.DeleteFlagWord("始める");
                    dict_script.AddFlagWord("終わり");
                    animator.SetTrigger("training_Trigger");
                }
                break;
            case "training":
                if(dict_script.CheckFlagWord("終わり") == true){
                    status = "finish";
                    dict_script.DeleteFlagWord("終わり");
                    animator.SetTrigger("wait_Trigger");
                }
                break;
            default:
                break;
        }

    }
}
