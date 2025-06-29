using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class LevelLoader : MonoBehaviour
{
    [Header("Json Files")]
    public TextAsset[] jsonFiles;

    [Header("Prefab")]
    public GameObject boxAPrefab;
    public GameObject boxBPrefab;
    public GameObject boxWinPrefab;
    public GameObject playerPrefab;
    public GameObject boxFallPrefab;

    [Header("Manager")]
    public UIManager uiManager;

    [Header("Camera")]
    public Camera mainCamera;

    private List<Tween> activeTweens = new List<Tween>();
    private int currentLevel = 0;
    private bool isLoading = false;
    private Coroutine loadCoroutine = null;


    public void PlayGame()
    {
        if (!isLoading)
            loadCoroutine = StartCoroutine(LoadLevelWithCloudEffect());
    }
    public void ResetLevel()
    {
        
        if (loadCoroutine != null)
        {
            StopCoroutine(loadCoroutine);
            loadCoroutine = null;
        }
        foreach (Tween t in activeTweens)
        {
            if (t != null && t.IsActive())
                t.Kill();
        }
        activeTweens.Clear();
        
        ClearMap();
        loadCoroutine = StartCoroutine(LoadLevelWithCloudEffect());
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
                    seq.Append(block.transform.DOMoveY(startPos.y + 2f, 0.5f).SetEase(Ease.OutSine));
                    seq.Append(block.transform.DOMoveY(finalPos.y, 0.3f).SetEase(Ease.InSine));
                    seq.Play();
                    activeTweens.Add(seq);
                    yield return new WaitForSeconds(0.035f);
                }
            }
        }

        Vector3 playerPos = level.playerPosition.ToVector3();

        if (playerPrefab != null)
        {
            GameObject player = Instantiate(playerPrefab, playerPos, Quaternion.identity, transform);
            player.transform.localScale = Vector3.zero;
            player.transform.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBack);
            PlayerController controller = player.GetComponent<PlayerController>();
            uiManager.SetPlayer(controller);
        }

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
            gm.SetMap(level.blocks.ToArray(), level.mapSize.x, level.mapSize.z);


        MoveCameraToCenter(level.mapSize);

        isLoading = false;
    }
    void MoveCameraToCenter(FlatMapSize size)
    {
        if (mainCamera == null) return;

        float centerX = size.x / 2f;
        
        Vector3 targetPos = new Vector3(centerX, 10f,-7f);     

        mainCamera.transform.DOMove(targetPos, 1f).SetEase(Ease.InOutSine);
    }

    public void ClearMap()
    {
        foreach (Tween t in activeTweens)
        {
            if (t != null && t.IsActive())
                t.Kill();
        }
        activeTweens.Clear();
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
            case 'F': return boxFallPrefab;
            default: return null;
        }
    }
}