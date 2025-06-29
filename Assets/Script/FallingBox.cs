using UnityEngine;
using DG.Tweening;
using System.Collections;

public class FallingBox : MonoBehaviour
{
    private bool isTriggered = false;
    private Vector3Int myTilePos;
    private Tween fallTween;

    private void Start()
    {
        myTilePos = new Vector3Int(
            Mathf.RoundToInt(transform.position.x),
            0,
            Mathf.RoundToInt(transform.position.z));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("checkpoint"))
        {
            isTriggered = true;
            StartCoroutine(FallAfterDelay(0f));
        }
    }

    private IEnumerator FallAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Transform player = GameObject.FindGameObjectWithTag("checkpoint")?.transform;
        if (player != null)
        {
            Vector3Int playerTile = new Vector3Int(
                Mathf.RoundToInt(player.position.x),
                0,
                Mathf.RoundToInt(player.position.z));

            if (playerTile == myTilePos)
            {
                GameManager gm = FindFirstObjectByType<GameManager>();
                if (gm != null)
                    gm.GameOver();
            }
        }

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
            gameManager.SetBlockAt(myTilePos.x, myTilePos.z, '.');

        fallTween = transform.DOMoveY(-100f, 0.4f)
    .SetEase(Ease.InQuad)
    .OnComplete(() =>
    {
        Destroy(gameObject);
    });

    }

    private void OnDestroy()
    {
        if (fallTween != null && fallTween.IsActive())
            fallTween.Kill();
    }
}
