using UnityEngine;

[CreateAssetMenu(fileName = "Human", menuName = "Guardian Angel/Human", order = 0)]
public class HumanData : ScriptableObject {
    public string description;
    public int age;
    public int lifeExpectancy;
    private int years;
    private void Awake() {
        age = Random.Range(15, 90);
        lifeExpectancy = age + Random.Range(2, 70);
        if(lifeExpectancy > 100) lifeExpectancy = 100;
        years = lifeExpectancy - age;
    }
}
