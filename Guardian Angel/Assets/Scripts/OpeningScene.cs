using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Probably the least efficient way to animate text in the history of game dev
public class OpeningScene : MonoBehaviour {
    public GameObject first;
    public GameObject second;
    public GameObject third;
    public GameObject fourth;
    private void Start() {
        first.SetActive(true);
        second.SetActive(false);
        third.SetActive(false);
        fourth.SetActive(false);
    }
    private void Update() {
        if(first.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Finished")) {
            first.SetActive(false);
            second.SetActive(true);
        }
        if(second.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Finished")) {
            second.SetActive(false);
            third.SetActive(true);
        }
        if(third.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Finished")) {
            third.SetActive(false);
            fourth.SetActive(true);
        }
    }
}
