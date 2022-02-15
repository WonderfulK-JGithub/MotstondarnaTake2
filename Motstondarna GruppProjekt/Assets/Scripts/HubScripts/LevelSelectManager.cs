using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField] Transform[] levelCameraPoints;
    [SerializeField] TextMeshProUGUI[] levelNumbers;
    [SerializeField] TextMeshProUGUI coinCountText;
    [SerializeField] TextMeshProUGUI[] levelCoinCountTexts;

    [SerializeField] Transform ball;
    [SerializeField] Vector3 ballOffsett;

    [SerializeField] Color finishedColor;
    [SerializeField] Color unlockedColor;
    [SerializeField] Color lockedColor;

    [SerializeField] float rotateSpeed;
    [SerializeField] float transitionWait;
    [SerializeField] float unlockTime;
    [SerializeField] ParticleSystem unlockPS;

    [Header("Bonus Level")]
    [SerializeField] TextMeshProUGUI needText;
    [SerializeField] Image needCoinImage;

    HubCamera cam;

    int levelIndex;

    int unlockedLevel = -1;

    bool hasSelected;

    float unlockTimer;

    LevelSelectState state;

    void Awake()
    {
        cam = FindObjectOfType<HubCamera>();

        
    }

    void Start()
    {
        state = LevelSelectState.Selecting;

        if (GameSaveInfo.currentLevel != -1) levelIndex = GameSaveInfo.currentLevel;

        ProgressCheck();

        if(GameSaveInfo.current.coinCount == 50)
        {
            needCoinImage.enabled = false;
            needText.enabled = false;
        }

        coinCountText.text = GameSaveInfo.current.coinCount.ToString();

        for (int i = 0; i < GameSaveInfo.current.coinLevelsCount.Length; i++)
        {
            levelCoinCountTexts[i].text = GameSaveInfo.current.coinLevelsCount[i].ToString() + "/10";
        }
    }

    void Update()
    {
        switch(state)
        {
            case LevelSelectState.Selecting:
                #region
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    levelIndex++;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    levelIndex--;
                }

                levelIndex = Mathf.Clamp(levelIndex, 0, levelCameraPoints.Length - 1);

                cam.targetPoint = levelCameraPoints[levelIndex];

                levelNumbers[levelIndex].transform.localRotation *= Quaternion.Euler(0f, rotateSpeed * Time.deltaTime, 0f);

                ball.transform.position = levelCameraPoints[levelIndex].position - ballOffsett;

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (levelIndex <= GameSaveInfo.current.levelProgress)
                    {
                        if (levelIndex == 5 && GameSaveInfo.current.coinCount < 50) return;
                        LevelSelected();
                    }
                }
                #endregion
                break;
            case LevelSelectState.Unlocking:
                #region
                unlockTimer -= Time.deltaTime;

                if(unlockTimer <= 0f)
                {
                    levelNumbers[unlockedLevel].color = unlockedColor;
                    unlockPS.transform.position = levelNumbers[unlockedLevel].transform.position;
                    unlockPS.Play();
                    state = LevelSelectState.Selecting;
                    levelIndex = unlockedLevel;

                    SaveSystem.current.Save();
                }
                else
                {
                    levelNumbers[unlockedLevel].transform.localRotation *= Quaternion.Euler(0f, rotateSpeed * Time.deltaTime * 10f, 0f);
                }
                #endregion
                break;

        }
    }

    void LevelSelected()
    {
        foreach (var item in levelNumbers)
        {
            item.enabled = false;
        }

        BallMovement ball = FindObjectOfType<BallMovement>();
        ball.state = PlayerState.Hub;
        ball.rb.velocity = new Vector3(11f, 0f, 0f);

        state = LevelSelectState.Off;

        Invoke("EnterLevel", transitionWait);
    }

    void EnterLevel()
    {
        SceneTransition.current.EnterScene(levelIndex + GameSaveInfo.levelStartIndex);
    }
    
    void ProgressCheck()
    {

        for (int i = 0; i < levelNumbers.Length; i++)
        {
            if (i < GameSaveInfo.current.levelProgress)
            {
                levelNumbers[i].color = finishedColor;
            }
            else if (i == GameSaveInfo.current.levelProgress)
            {
                levelNumbers[i].color = unlockedColor;
            }
            else
            {
                levelNumbers[i].color = lockedColor;
            }

            if(i == 5 && GameSaveInfo.current.coinCount < 50)
            {
                levelNumbers[i].color = lockedColor;
            }
        }

        

        if (GameSaveInfo.currentLevel != -1 && GameSaveInfo.current.levelProgress == GameSaveInfo.currentLevel)
        {
            GameSaveInfo.current.levelProgress++;

            UnlockLevel(GameSaveInfo.current.levelProgress);
        }

        GameSaveInfo.currentLevel = -1;
    }
    void UnlockLevel(int level)
    {
        levelNumbers[level-1].color = finishedColor;

        state = LevelSelectState.Unlocking;
        unlockedLevel = level;

        cam.targetPoint = levelCameraPoints[level];
        unlockTimer = unlockTime;
    }
}


public enum LevelSelectState
{
    Unlocking,
    Selecting,
    Off,
}