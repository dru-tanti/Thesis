using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityAtoms.BaseAtoms;
public class CanvasManger : MonoBehaviour {
    public GameObject turnCounter;
    public GameObject yearsCounter;
    public IntVariable currentTurn;
    public IntVariable currentLevel;
    public IntVariable yearsCollected;
    private TextMeshProUGUI _turns;
    private TextMeshProUGUI _years;

    private void Start() {
        _turns = turnCounter.GetComponent<TextMeshProUGUI>();
        _years = yearsCounter.GetComponent<TextMeshProUGUI>();
        printTurns();
    }

    public void printTurns() {
        _turns.SetText("Turns Remaining: {0}", GameManager.current.level[currentLevel.Value].turns - currentTurn.Value);
    }
}
