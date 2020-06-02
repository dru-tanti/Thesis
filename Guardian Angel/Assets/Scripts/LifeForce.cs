using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LifeForce : MonoBehaviour
{
    public Slider _slider;
    public Image sliderFill;
    public Gradient sliderColour;
    private void Awake() {
        _slider = GetComponent<Slider>();   
    }

    // Will be used to set the slider value.
    public void SetValue(float value) {
        _slider.value = value;
        sliderFill.color = sliderColour.Evaluate(_slider.value / _slider.maxValue);
    }
}
