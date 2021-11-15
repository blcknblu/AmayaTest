using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StartOptions : MonoBehaviour
{
    const string TASK_MAIN_TEXT = "Find ";

    [Header("Уровни и ячейки")]
    [SerializeField] LevelInfo[] levels;
    [SerializeField] GameObject cellPrefab;

    [Header("Вспомогательный UI")]
    [SerializeField] TextMeshProUGUI task;
    [SerializeField] GameObject playArea;
    [SerializeField] GameObject restartPanel;
    [SerializeField] CanvasGroup fadeCanvas;

    int currentLevel = 0;

    VerticalLayoutGroup baseVerticalLayoutGroup;
    RectTransform verticalLayoutRectTransform;
    List<HorizontalLayoutGroup> childrenHorizontalLayoutGroups = new List<HorizontalLayoutGroup>();
    float spacingBetweenCells;
    int columnsNumber;     

    string correctAnswerCellName;
    SessionGameplayData sgd;
    VisualEffectsController visualEffects;
    Image restartPanelImage;
    Fader fader;

    private void Awake()
    {
        baseVerticalLayoutGroup = playArea.GetComponent<VerticalLayoutGroup>();       
        verticalLayoutRectTransform = baseVerticalLayoutGroup.GetComponent<RectTransform>();
        visualEffects = FindObjectOfType<VisualEffectsController>();
        restartPanelImage = restartPanel.GetComponent<Image>();
        fader = fadeCanvas.GetComponent<Fader>();
    }

    private void Start()
    {
        spacingBetweenCells = baseVerticalLayoutGroup.spacing;
        sgd = new SessionGameplayData();
        if (levels.Length > 0)
            StartLevel(0);
    }

    public void StartLevel(int levelIndex)
    {
        LevelInfo thisLevel = levels[levelIndex];

        ResetTable();

        ModifyPlayAreaSize(thisLevel.rows, thisLevel.columns);
        List<CellInfo> generatedCells = GenerateCells(thisLevel);
        DisplayCells(generatedCells);
        visualEffects.TransformBounce(baseVerticalLayoutGroup.transform, new Vector3(0f, -50f, 0f), 0.5f, DG.Tweening.Ease.InSine, 6);
        visualEffects.TextFade(task, 1f, 2f);
    }  

    private void ModifyPlayAreaSize(int rows, int columns)
    {
        // Настраиваем размер главного объекта с компонентом VerticalLayoutGroup
        float xVerticalLayoutTransform = columns * cellPrefab.GetComponent<RectTransform>().sizeDelta.x;
        float yVerticalLayoutTransform = rows * cellPrefab.GetComponent<RectTransform>().sizeDelta.y;

        //verticalLayoutRectTransform.sizeDelta = new Vector2(xVerticalLayoutTransform, yVerticalLayoutTransform);
        verticalLayoutRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, xVerticalLayoutTransform);
        verticalLayoutRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, yVerticalLayoutTransform);

        // Добавляем ряды и настраиваем их размер (с компонентом HorizontalLayoutGroup)
        for (int i = 0; i < rows; i++)
        {
            GameObject newRow = new GameObject();
            newRow.transform.parent = baseVerticalLayoutGroup.transform;

            RectTransform horizontalLayoutGroupRectTransform = newRow.AddComponent<RectTransform>();
            horizontalLayoutGroupRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, xVerticalLayoutTransform);
            horizontalLayoutGroupRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rows * cellPrefab.GetComponent<RectTransform>().sizeDelta.y / rows);

            horizontalLayoutGroupRectTransform.localScale = Vector3.one;

            HorizontalLayoutGroup horizontalLayoutGroup = newRow.AddComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.spacing = spacingBetweenCells;
            childrenHorizontalLayoutGroups.Add(horizontalLayoutGroup);
        }

        columnsNumber = columns;
    }

    private List<CellInfo> GenerateCells(LevelInfo levelInfo)
    {
        int spritesQuantity = levelInfo.rows * levelInfo.columns;
        // Создаем пустой лист, куда будут помещаться новые ячейки для текущего уровня
        List<CellInfo> newCellsForCurrentLevel = new List<CellInfo>();
        // Создаем лист, со всеми вариантами ячеек для текущего уровня
        List<CellInfo> possibleCells = levelInfo.cellVariations.ToList();
        // Мешаем
        possibleCells.Shuffle();
      
        // Сначала выбираем рандомно ячейку с правильным ответом
        CellInfo correctAnswerCell = sgd.GenerateCorrectAnswer(possibleCells);
        // Выводим в UI задание
        DisplayTaskForCorrectAnswer(correctAnswerCell);

        newCellsForCurrentLevel.Add(correctAnswerCell);
        possibleCells.Remove(correctAnswerCell);

        // Выбираем рандомно остальные ячейки
        for (int i = 0; i < spritesQuantity - 1; i++)
        {
            CellInfo randomCell = possibleCells[UnityEngine.Random.Range(0, possibleCells.Count - 1)];
            newCellsForCurrentLevel.Add(randomCell);
            possibleCells.Remove(randomCell);
        }

        return newCellsForCurrentLevel;       
    }

    private void DisplayTaskForCorrectAnswer(CellInfo correctAnswerCell)
    {
        correctAnswerCellName = correctAnswerCell.name;
        task.text = TASK_MAIN_TEXT + correctAnswerCellName;
    }  

    private void DisplayCells(List<CellInfo> cellsForCurrentLevel)
    {
        // Еще раз помешиваем
        cellsForCurrentLevel.Shuffle();

        // Выводим в ряды с HorizontalLayoutGroup все наши ячейки (по "columnsNumber" ячейки в каждом ряду)
        foreach (var row in childrenHorizontalLayoutGroups)
        {
            int itemsQuantity = columnsNumber;
            while (itemsQuantity > 0)
            {
                GameObject newCell = Instantiate(cellPrefab, row.transform);
                newCell.GetComponent<CellSettings>().cellValue.sprite = cellsForCurrentLevel[0].sprite;

                newCell.transform.Rotate(new Vector3(0f, 0f, cellsForCurrentLevel[0].rotate));

                if (cellsForCurrentLevel[0].name == correctAnswerCellName)
                    newCell.GetComponent<CellButton>().correctAnswer = true;

                cellsForCurrentLevel.RemoveAt(0);
                itemsQuantity--;
            }
        }
    }

    public void MoveToNextLevel()
    {
        if (levels.Length - 1 <= currentLevel)
        {
            LoadRestartPanel();
            return;
        }

        currentLevel++;
        StartLevel(currentLevel);
    }

    private void ResetTable()
    {
        foreach (Transform child in baseVerticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        childrenHorizontalLayoutGroups.Clear();

        task.ChangeAlpha(0f);
    }

    private void LoadRestartPanel()
    {
        restartPanel.SetActive(true);
        visualEffects.ImageFade(restartPanelImage, 0.6f, 1f);
    }

    public void RestartGame()
    {
        StartCoroutine(RestartingGame());      
    }

    IEnumerator RestartingGame()
    {
        restartPanel.SetActive(false);
        visualEffects.ImageFade(restartPanelImage, 0f, 0.1f);

        yield return fader.FadeOut();

        sgd.ClearAllAnswers();
        currentLevel = 0;
        StartLevel(currentLevel);

        yield return fader.FadeIn();
    }
}
