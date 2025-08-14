using UnityEngine;

[CreateAssetMenu(menuName = "Arena/Card")]
public class CardData : ScriptableObject
{
    public string cardName = "Soldier";
    public int cost = 3;
    public int health = 10;
    public int damage = 2;
    public GameObject unitPrefab; // optional override prefab
    // Add range, speed, special behavior later
}
