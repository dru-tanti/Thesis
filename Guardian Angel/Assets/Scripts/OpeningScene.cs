using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Probably the least efficient way to animate text in the history of game dev
public class OpeningScene : MonoBehaviour {
    public GameObject first;
    public GameObject second;
    public GameObject third;
    public GameObject fourth;
    public GameObject fifth;
    public GameObject sixth;
    public bool next = false;
    private void Start() {
        first.SetActive(true);
        second.SetActive(false);
        third.SetActive(false);
        fourth.SetActive(false);
        fifth.SetActive(false);
        sixth.SetActive(false);
        next = false;
    }
    private void Update() {
        if(Input.anyKeyDown) next = true;
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

    public void showFifth() {
        fourth.SetActive(false);
        fifth.SetActive(true);
    }

    public void showSixth() {
        fifth.SetActive(false);
        sixth.SetActive(true);
    }

    public void startGame() {
        SceneManager.LoadScene(1);
    }
}
