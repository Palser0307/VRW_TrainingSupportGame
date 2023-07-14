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
    [SerializeField]
    private string status = "wait";

    void Start(){
        Debug.Log("GameController_EASY.Start()");

        // get component
        animator = this.GetComponent<Animator>();
        dict_script = this.GetComponent<DictationScript>();
        dict_script.AddFlagWord("ハジメル");
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
                    this.status = "training";
                    dict_script.DeleteFlagWord("始める");
                    dict_script.AddFlagWord("次");
                    animator.SetTrigger("Training_Start");
                }
                break;
            case "training":
                if(dict_script.CheckFlagWord("次") == true){
                    //status = "training";
                    dict_script.DeleteFlagWord("次");
                    dict_script.CheckFlagWord("次");
                    animator.SetTrigger("Next_Training");
                }
                if(animator.GetCurrentAnimatorStateInfo(0).IsTag("Training") == false){
                    Debug.Log("Training Finish");
                    this.status = "finished";
                }
                break;
            default:
                Debug.LogWarning("undefined status");
                break;
        }
    }
}
