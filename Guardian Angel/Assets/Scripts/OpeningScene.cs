using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Probably the least efficient way to animate text in the history of game dev
public class OpeningScene : MonoBehaviour {
    // Animated Text
    public GameObject first, second, third;
    // Tutorial.
    public GameObject fourth, fifth, sixth, seventh, eighth, ninth;
    private void Start() {
        first.SetActive(true);
        second.SetActive(false);
        third.SetActive(false);
        fourth.SetActive(false);
        fifth.SetActive(false);
        sixth.SetActive(false);
        AudioManager.current.Play("Opening");
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
            showFourth();
        }
    }

    public void showFourth() {
        fourth.SetActive(true);
        fifth.SetActive(false);
        sixth.SetActive(false);
        seventh.SetActive(false);
        eighth.SetActive(false);
    }

    public void showFifth() {
        fourth.SetActive(false);
        fifth.SetActive(true);
        sixth.SetActive(false);
        seventh.SetActive(false);
        eighth.SetActive(false);
    }

    public void showSixth() {
        fourth.SetActive(false);
        fifth.SetActive(false);
        sixth.SetActive(true);
        seventh.SetActive(false);
        eighth.SetActive(false);
    }

    public void showSeventh() {
        fourth.SetActive(false);
        fifth.SetActive(false);
        sixth.SetActive(false);
        seventh.SetActive(true);
        eighth.SetActive(false);
    }

    public void showEighth() {
        fourth.SetActive(false);
        fifth.SetActive(false);
        sixth.SetActive(false);
        seventh.SetActive(false);
        eighth.SetActive(true);
    }

    public void startGame() {
        SceneManager.LoadScene(1);
    }
}
