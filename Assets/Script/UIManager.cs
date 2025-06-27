using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject panelHome;
    public GameObject panelWin;
    public GameObject panelLose;
    public GameObject panelHtp;

    public TMP_Text textWinLevel;
    public TMP_Text textLoseLevel;

    public LevelLoader levelLoader;
    public GameManager gameManager;

    public Image fadeOverlay;

    private float fadeTime = 0.6f;
    private float scaleTime = 0.4f;

    private void Start()
    {
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
        StartCoroutine(FadeAndDo(() =>
        {
            HideAllPanels();
            levelLoader.ClearMap();
            gameManager.ResetGameState();
            levelLoader.PlayGame();
        }));
    }

    public void OnResetClicked()
    {
        StartCoroutine(FadeAndDo(() =>
        {
            HideAllPanels();
            levelLoader.ClearMap();
            gameManager.ResetGameState();
            levelLoader.PlayGame();
        }));
    }

    public void OnNextLevelClicked()
    {
        StartCoroutine(FadeAndDo(() =>
        {
            HideAllPanels();
            int nextLevel = PlayerPrefs.GetInt("level", 0) + 1;
            PlayerPrefs.SetInt("level", nextLevel);
            levelLoader.ClearMap();
            gameManager.ResetGameState();
            levelLoader.PlayGame();
        }));
    }

    public void OnHomeClicked()
    {
        StartCoroutine(FadeAndDo(() =>
        {
            ShowPanel(panelHome);
            levelLoader.ClearMap();
        }));
    }

    public void OnHtpClicked()
    {
        ShowPanel(panelHtp);
        panelHtp.transform.localScale = Vector3.zero;
        panelHtp.transform.DOScale(Vector3.one, scaleTime).SetEase(Ease.OutBack);
    }

    public void OnCloseHtp()
    {
        panelHtp.transform.DOScale(Vector3.zero, scaleTime).SetEase(Ease.InBack)
            .OnComplete(() => panelHtp.SetActive(false));
        foreach (Transform child in panelHome.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void ShowWinPanel()
    {
        StartCoroutine(ShowPanelWithDelay(panelWin, textWinLevel));
    }

    public void ShowLosePanel()
    {
        StartCoroutine(ShowPanelWithDelay(panelLose, textLoseLevel));
    }

    IEnumerator ShowPanelWithDelay(GameObject panel, TMP_Text levelText)
    {
        yield return new WaitForSeconds(1f);
        ShowPanel(panel);

        int level = PlayerPrefs.GetInt("level", 0) + 1;
        if (levelText != null)
            levelText.text = "Level " + level;

        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
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
}
