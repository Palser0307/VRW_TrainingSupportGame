using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// とりあえず勉強がてらAnimatorの制御用に書いたスクリプト
// Triggerでちゃんと遷移することが確認できた
// つまり，スクリプトからモーションの再生が出来たってわけだ
public class TrainingAndStop : MonoBehaviour{
    private Animator animator;
    public bool stop_training_readOnly = false;
    public int transfer_time = 15;
    void Start(){
        animator = GetComponent<Animator>();
        //stop_training_readOnly = animator.GetBool("exitFlag");
        Invoke("start_training",transfer_time);
        Invoke("stop_training",transfer_time*2);
    }
    void Update(){
        //stop_training_readOnly = animator.GetBool("exitFlag");
        //Debug.Log(animator.GetCurrentAnimatorStateInfo(0).IsName("Training"));
    }
    void start_training(){
        animator.SetTrigger("training_Trigger");
    }
    void stop_training(){
        //animator.SetBool("exitFlag",true);
        animator.SetTrigger("wait_Trigger");
    }
}
