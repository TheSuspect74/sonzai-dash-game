using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Vector2 moveDir;
    [SerializeField] TrailRenderer dashTrail;
    
    float speed;
    float dashSpeed;
    float dashCooldown;
    float dashDuration;
    
    GameManager gameManager;

    public float tDashCooldown;
    [SerializeField] float tDashDuration;
    bool isDashing;
    
    public void Init(GameManager inGameManager)
    {
        gameManager = inGameManager;

        speed = gameManager.gameData.playerSpeed;
        dashSpeed = gameManager.gameData.dashSpeed;
        dashCooldown = gameManager.gameData.dashCooldon;
        dashDuration = gameManager.gameData.dashDuration;
        
        moveDir = Vector2.zero;
    }

    public void DoUpdate(float dt)
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            moveDir.y += 1;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            moveDir.y -= 1;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            moveDir.x -= 1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            moveDir.x += 1;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            moveDir.y -= 1;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            moveDir.y += 1;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            moveDir.x += 1;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            moveDir.x -= 1;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool canDash = CanDash();
            if (canDash)
            {
                UseDash();
            }
        }

        if (tDashDuration > 0)
        {
            tDashDuration -= dt;
            if (tDashDuration <= 0)
            {
                isDashing = false;
            }
        }
        
        if (tDashCooldown > 0)
        {
            tDashCooldown -= dt;
        }
        
        Vector2 normalizedDir = moveDir.normalized;
        float dashingSpeed = isDashing ? dashSpeed : 0;
        rb.linearVelocity = normalizedDir * (speed + dashingSpeed);
    }

    public void UseDash()
    {
        isDashing = true;
        tDashDuration = dashDuration;
        tDashCooldown = dashCooldown;

        dashTrail.gameObject.SetActive(true);
        Invoke(nameof(HideDashTrail),2f);
    }

    void HideDashTrail()
    {
        dashTrail.gameObject.SetActive(false);
    }
    
    bool CanDash()
    {
        bool result = tDashCooldown <= 0;
        return result;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        bool isCollectible = other.CompareTag("Collectible");
        if (isCollectible)
        {
            gameManager.AddCollectibleCount(other.gameObject);
            return;
        }
        
        bool isEnemy = other.CompareTag("Enemy");
        if (isEnemy)
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            gameManager.OnCollideWithEnemy(enemy);
        }
    }
}
