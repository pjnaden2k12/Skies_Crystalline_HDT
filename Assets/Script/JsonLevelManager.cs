using DG.Tweening;
using System.Collections;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public TextAsset[] jsonFiles;
    public GameObject boxAPrefab;
    public GameObject boxBPrefab;
    public GameObject boxWinPrefab;
    public GameObject playerPrefab;

    private int currentLevel = 0;
    private bool isLoading = false;

    public void PlayGame()
    {
        if (!isLoading)
            StartCoroutine(LoadLevelWithCloudEffect());
    }

    public IEnumerator LoadLevelWithCloudEffect()
    {
        isLoading = true;

        ClearMap();

        currentLevel = PlayerPrefs.GetInt("level", 0);
        if (currentLevel < 0 || currentLevel >= jsonFiles.Length)
            currentLevel = 0;

        LevelData level = JsonUtility.FromJson<LevelData>(jsonFiles[currentLevel].text);

        for (int z = 0; z < level.mapSize.z; z++)
        {
            string row = level.blocks[z];
            for (int x = 0; x < level.mapSize.x; x++)
            {
                char c = row[x];
                GameObject prefab = GetPrefabFromChar(c);
                if (prefab != null)
                {
                    Vector3 finalPos = new Vector3(x, 0, z);
                    Vector3 startPos = finalPos + Vector3.down * 1f;

                    GameObject block = Instantiate(prefab, startPos, Quaternion.identity, transform);

                    Sequence seq = DOTween.Sequence();
                    seq.Append(block.transform.DOMoveY(startPos.y + 2f, 0.6f).SetEase(Ease.OutSine));
                    seq.Append(block.transform.DOMoveY(finalPos.y, 0.3f).SetEase(Ease.InSine));
                    seq.Play();

                    yield return new WaitForSeconds(0.05f);
                }
            }
        }

        Vector3 playerPos = level.playerPosition.ToVector3();

        if (playerPrefab != null)
        {
            GameObject player = Instantiate(playerPrefab, playerPos, Quaternion.identity, transform);
            player.transform.localScale = Vector3.zero;
            player.transform.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBack);
        }

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
            gm.SetMap(level.blocks.ToArray(), level.mapSize.x, level.mapSize.z);

        isLoading = false;
    }

    public void ClearMap()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }

    GameObject GetPrefabFromChar(char c)
    {
        switch (c)
        {
            case 'A': return boxAPrefab;
            case 'B': return boxBPrefab;
            case 'W': return boxWinPrefab;
            default: return null;
        }
    }
}