using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelHome;
    public GameObject panelWin;
    public GameObject panelLose;
    public GameObject panelHtp;
    public GameObject panelInGame;

    [Header("Text")]
    public TMP_Text textWinLevel;
    public TMP_Text textLoseLevel;

    [Header("Manager")]
    public LevelLoader levelLoader;
    public GameManager gameManager;

    public Image fadeOverlay;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip[] moveSounds;


    private float fadeTime = 0.6f;
    private float scaleTime = 0.4f;

    private PlayerController playerController;

    public void SetPlayer(PlayerController controller)
    {
        playerController = controller;
    }
    void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    void PlayRandomMoveSound()
    {
        if (audioSource != null && moveSounds != null && moveSounds.Length > 0)
        {
            int index = Random.Range(0, moveSounds.Length);
            audioSource.PlayOneShot(moveSounds[index]);
        }
    }

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();

        fadeOverlay.gameObject.SetActive(true);
        fadeOverlay.color = new Color(0, 0, 0, 1);
        fadeOverlay.DOFade(0, fadeTime).SetEase(Ease.OutQuad);

        ShowPanel(panelHome);
        panelWin.SetActive(false);
        panelLose.SetActive(false);
        panelHtp.SetActive(false);
    }

    public void OnPlayClicked()
    {
        PlayButtonClickSound();
        StartCoroutine(FadeAndDo(() =>
        {
            HideAllPanels();
            panelInGame.SetActive(true);
            levelLoader.ClearMap();
            gameManager.ResetGameState();
            levelLoader.PlayGame();
        }));
    }

    public void OnResetClicked()
    {
        PlayButtonClickSound();
        StartCoroutine(FadeAndDo(() =>
        {
            HideAllPanels();
            panelInGame.SetActive(true);
            levelLoader.ResetLevel();
            if (gameManager != null)
                gameManager.ResetGameState();
        }));
    }

    public void OnNextLevelClicked()
    {
        PlayButtonClickSound();
        StartCoroutine(FadeAndDo(() =>
        {
            HideAllPanels();
            int nextLevel = PlayerPrefs.GetInt("level", 0) + 1;
            PlayerPrefs.SetInt("level", nextLevel);
            levelLoader.ClearMap();
            gameManager.ResetGameState();
            levelLoader.PlayGame();
            panelInGame.SetActive(true);
        }));
    }

    public void OnHomeClicked()
    {
        PlayButtonClickSound();
        StartCoroutine(FadeAndDo(() =>
        {
            ShowPanel(panelHome);
            levelLoader.ClearMap();
        }));
    }

    public void OnHtpClicked()
    {
        PlayButtonClickSound();
        ShowPanel(panelHtp);
        panelHtp.transform.localScale = Vector3.zero;
        panelHtp.transform.DOScale(Vector3.one, scaleTime).SetEase(Ease.OutBack);
    }

    public void OnCloseHtp()
    {
        PlayButtonClickSound();
        panelHtp.transform.DOScale(Vector3.zero, scaleTime).SetEase(Ease.InBack)
            .OnComplete(() => panelHtp.SetActive(false));
        foreach (Transform child in panelHome.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void ShowWinPanel()
    {
        StartCoroutine(DelayPanelInGameOff());
        PlaySound(winSound);
        StartCoroutine(ShowPanelWithDelay(panelWin, textWinLevel));
    }

    public void ShowLosePanel()
    {
        StartCoroutine(DelayPanelInGameOff());
        PlaySound(loseSound);
        StartCoroutine(ShowPanelWithDelay(panelLose, textLoseLevel));
    }

    IEnumerator ShowPanelWithDelay(GameObject panel, TMP_Text levelText)
    {
        yield return new WaitForSeconds(0.5f);
        ShowPanel(panel);

        int level = PlayerPrefs.GetInt("level", 0) + 1;
        if (levelText != null)
            levelText.text = "Level " + level;

        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 1.2f).SetEase(Ease.OutBack);
    }

    void ShowPanel(GameObject panel)
    {
        if (panel == panelHtp)
        {
            panelHome.SetActive(true);
            foreach (Transform child in panelHome.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        else panelHome.SetActive(false);
        panelWin.SetActive(false);
        panelLose.SetActive(false);
        panelHtp.SetActive(false);

        if (panel == panelHome)
        {
            panelHome.SetActive(true);
            StartCoroutine(AnimatePanelHome());
        }
        else if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    IEnumerator AnimatePanelHome()
    {
        float delayBetween = 0.3f;
        float duration = 0.5f;

        foreach (Transform child in panelHome.transform)
        {
            child.localScale = Vector3.zero;
        }

        yield return null;

        foreach (Transform child in panelHome.transform)
        {
            child.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(delayBetween);
        }
    }

    void HideAllPanels()
    {
        panelHome.SetActive(false);
        panelWin.SetActive(false);
        panelLose.SetActive(false);
        panelHtp.SetActive(false);
    }

    IEnumerator FadeAndDo(System.Action action)
    {
        fadeOverlay.gameObject.SetActive(true);
        yield return fadeOverlay.DOFade(1, fadeTime).SetEase(Ease.InOutQuad).WaitForCompletion();

        action?.Invoke();

        yield return new WaitForSeconds(0.1f);
        fadeOverlay.DOFade(0, fadeTime).SetEase(Ease.InOutQuad);
    }

    public void OnMoveUp()
    {
        playerController?.MoveUp();
        PlayRandomMoveSound();
    }

    public void OnMoveDown()
    {
        playerController?.MoveDown();
        PlayRandomMoveSound();
    }

    public void OnMoveLeft()
    {
        playerController?.MoveLeft();
        PlayRandomMoveSound();
    }

    public void OnMoveRight()
    {
        playerController?.MoveRight();
        PlayRandomMoveSound();
    }
    IEnumerator DelayPanelInGameOff()
    {
        yield return new WaitForSeconds(0.5f);
        panelInGame.SetActive(false);
    }
    ///////////////////////////////////////////
    public void OnResetPrefsClicked()
    {
       
        PlayerPrefs.DeleteAll(); 
        PlayerPrefs.Save();

        Debug.Log("PlayerPrefs reset!");
    }
}
