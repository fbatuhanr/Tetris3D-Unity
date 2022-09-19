using System;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    
    [SerializeField] private RectTransform gameoverUI, tutorialUI;

    private const string TutorialKey = "Tutorial";
    public bool IsTutorialComplete
    {
        get => PlayerPrefs.GetInt(TutorialKey, 0) == 1;
        set => PlayerPrefs.SetInt(TutorialKey, value ? 1 : 0);
    }

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
    }

    private void Start()
    {
        if(!IsTutorialComplete)
            EnableTutorialUI();
    }

    private void EnableTutorialUI()
    {
        tutorialUI.DOScale(Vector3.one, 0.2f);
    }
    public void TutorialUIComplete_Button()
    {
        tutorialUI
            .DOScale(Vector3.zero, 0.2f)
            .OnComplete(() =>
            {
                IsTutorialComplete = true;
                BlockManager.Instance.SpawnBlock();
            });
    }

    public void EnableGameoverUI()
    {
        gameoverUI.DOScale(Vector3.one, 0.2f).OnComplete(() => Time.timeScale = 0);
    }
    
    public void Restart_Button()
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
