using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Guardian Angel/Level", order = 0)]
public class LevelSettings : ScriptableObject {
    public Texture2D map;
    public int humans;
    public int turns;
    public int maxActionPoints;
    public int maxHazards;
}