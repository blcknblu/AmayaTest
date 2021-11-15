using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SessionGameplayData
{
    // Собираемая информация о текущей сессии. В нашем случае - это коллекция правильных ответов.
    List<string> allCorrectAnswers = new List<string>();

    public CellInfo GenerateCorrectAnswer(List<CellInfo> possibleCells)
    {
        CellInfo correctAnswerCell = possibleCells.FirstOrDefault(x => !allCorrectAnswers.Contains(x.name));
        if (correctAnswerCell == null)
        {
            ClearAllAnswers();
            correctAnswerCell = possibleCells.FirstOrDefault(x => !allCorrectAnswers.Contains(x.name));
        }

        allCorrectAnswers.Add(correctAnswerCell.name);
        return correctAnswerCell;
    }

    internal void ClearAllAnswers()
    {
        allCorrectAnswers.Clear();
    }
}
