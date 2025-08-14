using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Spawning")]
    public Transform playerLeftSpawn;
    public Transform playerRightSpawn;
    public Transform enemyLeftSpawn;
    public Transform enemyRightSpawn;

    [Header("Prefabs & UI")]
    public GameObject unitPrefab;
    public GameObject towerPrefab;
    public Text logText;
    public Image elixirBar;
    public Text elixirText;

    [Header("Gameplay")]
    public float elixirMax = 10f;
    public float elixirRegenRate = 1f; // per second

    [HideInInspector] public float elixir = 0f;

    [Header("Deck/Hand")]
    public List<CardData> allCards; // assign in inspector
    public List<CardData> playerHand = new List<CardData>();
    public List<CardData> enemyHand = new List<CardData>();
    public int handSize = 4;

    public float drawCooldown = 1.5f;

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        elixir = elixirMax * 0.5f;
        StartCoroutine(ElixirRegen());
        StartCoroutine(EnsureHands());
    }

    IEnumerator ElixirRegen()
    {
        while (true)
        {
            elixir = Mathf.Min(elixirMax, elixir + elixirRegenRate * Time.deltaTime);
            if (elixirBar) elixirBar.fillAmount = elixir / elixirMax;
            if (elixirText) elixirText.text = Mathf.FloorToInt(elixir).ToString();
            yield return null;
        }
    }

    IEnumerator EnsureHands()
    {
        // Simple initial draw
        while (playerHand.Count < handSize) DrawCardForPlayer();
        while (enemyHand.Count < handSize) DrawCardForEnemy();

        // Enemy AI loop
        StartCoroutine(EnemyAITick());

        yield break;
    }

    public void DrawCardForPlayer()
    {
        var c = allCards[Random.Range(0, allCards.Count)];
        playerHand.Add(c);
        Log("Player drew " + c.cardName);
    }

    public void DrawCardForEnemy()
    {
        var c = allCards[Random.Range(0, allCards.Count)];
        enemyHand.Add(c);
        Log("Enemy drew " + c.cardName);
    }

    public bool TrySpendElixir(int cost)
    {
        if (elixir >= cost)
        {
            elixir -= cost;
            return true;
        }
        return false;
    }

    public void SpawnUnit(CardData card, bool isPlayer, bool isLeftLane)
    {
        Transform spawn = isPlayer ? (isLeftLane ? playerLeftSpawn : playerRightSpawn) : (isLeftLane ? enemyLeftSpawn : enemyRightSpawn);
        GameObject go = Instantiate(unitPrefab, spawn.position, Quaternion.identity);
        Unit u = go.GetComponent<Unit>();
        u.Initialize(card.health, card.damage, isPlayer);
        Log((isPlayer ? "Player" : "Enemy") + " spawned " + card.cardName + " on " + (isLeftLane ? "Left" : "Right") + " lane.");
    }

    public void Log(string s)
    {
        if (logText) logText.text = s;
        Debug.Log(s);
    }

    IEnumerator EnemyAITick()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f + Random.Range(0f,1.5f));

            if (enemyHand.Count == 0) DrawCardForEnemy();

            // pick a random affordable card
            CardData chosen = null;
            foreach (var c in enemyHand)
            {
                if (c.cost <= Mathf.FloorToInt(elixir)) { chosen = c; break; }
            }
            if (chosen == null)
            {
                // try to draw
                DrawCardForEnemy();
                continue;
            }

            // spend elixir (enemy uses same elixir pool for simplicity)
            if (TrySpendElixir(chosen.cost))
            {
                // pick lane
                bool left = Random.value > 0.5f;
                SpawnUnit(chosen, false, left);
                enemyHand.Remove(chosen);
            }
        }
    }
}
