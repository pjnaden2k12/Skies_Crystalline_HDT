using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool gameOver = false;
    private bool gameWon = false;

    private string[] blockRows;
    private int mapWidth, mapDepth;
    private Transform player;
    [SerializeField] private UIManager uiManager;
    void Start()
    {
    }

    void Update()
    {
        if (gameOver || gameWon) return;

        CheckCheckpointWinCollision();

        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
                player = found.transform;
            else
                return;
        }

        foreach (Transform cube in player)
        {
            Vector3 pos = cube.position;
            int x = Mathf.RoundToInt(pos.x);
            int z = Mathf.RoundToInt(pos.z);

            if (x < 0 || x >= mapWidth || z < 0 || z >= mapDepth)
            {
                GameOver();
                return;
            }

            char tile = blockRows[z][x];
            if (tile == '.')
            {
                GameOver();
                return;
            }
        }
    }
    public void ResetGameState()
    {
        gameOver = false;
        gameWon = false;
    }
    private void CheckCheckpointWinCollision()
    {
        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("checkpoint");
        GameObject[] wins = GameObject.FindGameObjectsWithTag("win");

        foreach (GameObject cp in checkpoints)
        {
            Collider cpCollider = cp.GetComponent<Collider>();
            if (cpCollider == null) continue;

            foreach (GameObject w in wins)
            {
                Collider wCollider = w.GetComponent<Collider>();
                if (wCollider == null) continue;

                if (cpCollider.bounds.Intersects(wCollider.bounds))
                {
                    Win();
                    return;
                }
            }
        }
    }

    public void GameOver()
    {
        if (gameOver) return;
        gameOver = true;

        if (player != null)
        {
            player.DOMoveY(player.position.y - 100f, 1f).SetEase(Ease.InQuad);

            var controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.isMoving = true;
            }
        }

        uiManager.ShowLosePanel();
    }


    public void Win()
    {
        if (gameWon) return;
        gameWon = true;
        Debug.Log("VIET NAM MUON NAM");
        if (player != null)
        {
            player.DOMoveY(player.position.y + 10f, 0.5f).SetEase(Ease.OutBounce);
        }
        uiManager.ShowWinPanel();
    }

    public void SetMap(string[] rows, int width, int depth)
    {
        blockRows = rows;
        mapWidth = width;
        mapDepth = depth;
    }
}
