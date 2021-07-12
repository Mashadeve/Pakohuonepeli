﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CheckAnswerScript : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> slot = new List<TMP_Text>();
    [SerializeField] private List<TMP_Text> answer = new List<TMP_Text>();
    [Space]
    [Space]
    [SerializeField] private Button nextSlot;
    [SerializeField] private Button previousSlot;
    [SerializeField] private Button sendAnswer;
    [SerializeField] private Button goBack;

    private CursorScrip cursorManager;

    [SerializeField] private int slotInt, currentSlotInt;

    private int answersGiven, slotCount;
    private Color slotDefaultColor;
    private bool okAll;
    
    [SerializeField] private int rightAnswers;

    private void Start()
    {
        if (PlayerData.lockerScene != true)
        {
            for (int i = 0; i < answer.Count; i++)
            {
                answer[i].text = Random.Range(0, 35).ToString();
                PlayerData.tempAnswers.Add(answer[i]);

            }
            PlayerData.lockerScene = true;
        }
        else
        {
            for (int i = 0; i < PlayerData.tempAnswers.Count; i++)
            {
                answer[i].text = PlayerData.tempAnswers[i].text;

            }
        }

        slotDefaultColor = slot[slotInt].color;
        currentSlotInt = slotInt;
        cursorManager = GameObject.Find("näyttö").GetComponent<CursorScrip>();

        slotInt = 0;
        slotCount = slot.Count;

        nextSlot.onClick.AddListener(GoNext);
        previousSlot.onClick.AddListener(GoBack);
        sendAnswer.onClick.AddListener(CheckAnswer2);
        goBack.onClick.AddListener(GoBackGameScene);

        sendAnswer.gameObject.SetActive(false);

    }

    private void GoBackGameScene()
    {
        SceneManager.LoadScene("GameView");
    }

    private void Update()
    {
        AllOkay();

        if (!okAll)
        {
            switch (currentSlotInt)
            {
                case 0:
                    slot[slotInt].color = Color.yellow;
                    slot[1].color = slotDefaultColor;
                    slot[4].color = slotDefaultColor;
                    break;
                case 1:
                    slot[0].color = slotDefaultColor;
                    slot[slotInt].color = Color.yellow;
                    slot[2].color = slotDefaultColor;
                    break;
                case 2:
                    slot[1].color = slotDefaultColor;
                    slot[slotInt].color = Color.yellow;
                    slot[3].color = slotDefaultColor;
                    break;
                case 3:
                    slot[2].color = slotDefaultColor;
                    slot[slotInt].color = Color.yellow;
                    slot[4].color = slotDefaultColor;
                    break;
                case 4:
                    slot[3].color = slotDefaultColor;
                    slot[slotInt].color = Color.yellow;
                    break;
            }
        }


        currentSlotInt = slotInt;
        if (slot.Count > 0)
        {
            LockWheelNumber();
        }

        if (answersGiven >= 4)
        {
            sendAnswer.gameObject.SetActive(true);
        }
    }

    public void GoNext()
    {
        if (slotInt < slot.Count - 1)
        {
            slotInt += 1;
            answersGiven += 1;
        }
    }
    public void GoBack()
    {
        if (slotInt > 0)
        {
            slotInt -= 1;
        }
    }

    private void LockWheelNumber()
    {
        slot[slotInt].text = cursorManager.currentValue.ToString();
    }

    private void CheckAnswer2()
    {
        
        for (int i = 0; i < answer.Count; i++)
        {
            if (slot[i].text == answer[i].text)
            {
                slot[i].color = Color.green;
                rightAnswers++;
            }
            else
            {
                ResetAnswers();
                rightAnswers = 0;
            }
        }
    }

    private void AllOkay()
    {
        while (rightAnswers > answersGiven)
        {
            okAll = true;
            PlayerData.lockerTaskDone = true;
            StartCoroutine(Delay());
            break;
        }
    }

    private void ResetAnswers()
    {
        slotInt = 0;
        answersGiven = 0;
        sendAnswer.gameObject.SetActive(false);
        cursorManager.currentValue = 0;

        for (int t = 0; t < slot.Count; t++)
        {
            slot[t].text = " 0 ";
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("GameView");
        PlayerData.lockerTaskDone = true;
    }
}
