using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;

    Player player;
    float speed;

    public void Init(GameManager inGameManager)
    {
        player = inGameManager.player;
        speed = inGameManager.gameData.enemySpeed;
    }

    void Update()
    {
        Vector2 direction = player.transform.position - transform.position;
        Vector2 normalizedDir =  direction.normalized;
        
        rb.linearVelocity = normalizedDir * speed;
    }
}
