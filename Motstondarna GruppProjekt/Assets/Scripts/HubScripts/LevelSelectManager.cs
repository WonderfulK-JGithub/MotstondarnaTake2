using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour//av K-J
{
    [SerializeField] Transform[] levelCameraPoints;
    [SerializeField] Renderer[] levelNumbers;
    [SerializeField] TextMeshProUGUI coinCountText;
    [SerializeField] TextMeshProUGUI[] levelCoinCountTexts;

    [SerializeField] Transform ball;
    [SerializeField] Vector3 ballOffsett;

    [SerializeField] Material finishedColor;
    [SerializeField] Material unlockedColor;
    [SerializeField] Material lockedColor;

    [SerializeField] float rotateSpeed;
    [SerializeField] float transitionWait;
    [SerializeField] float unlockTime;
    [SerializeField] ParticleSystem unlockPS;

    [SerializeField] Button exitButton;

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
        Cursor.lockState = CursorLockMode.None;

        state = LevelSelectState.Selecting;

        if (GameSaveInfo.currentLevel != -1) levelIndex = GameSaveInfo.currentLevel;//man ska börja vid siffran på banan man nyss klarade av

        ProgressCheck();

        if(GameSaveInfo.current.coinCount == 50)//tar bort text som säger att man behöver mynt för bonus banan om man inte längre behöver det
        {
            needCoinImage.enabled = false;
            needText.enabled = false;
        }

        coinCountText.text = GameSaveInfo.current.coinCount.ToString();//ändar totala coincount texten

        for (int i = 0; i < GameSaveInfo.current.coinLevelsCount.Length; i++)//uppdarerar coincount för varje bana
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
                
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) //La till så att man kan använda A och D också - Max
                {
                    levelIndex++;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                {
                    levelIndex--;
                }
                //ser till att levelIndex inte hamnar utanför "the bound of the array"
                levelIndex = Mathf.Clamp(levelIndex, 0, levelCameraPoints.Length - 1);

                cam.targetPoint = levelCameraPoints[levelIndex];//ändar vart kameran ska kolla

                levelNumbers[levelIndex].transform.localRotation *= Quaternion.Euler(0f, rotateSpeed * Time.deltaTime, 0f);//roterar siffran man kollar på just då

                ball.transform.position = levelCameraPoints[levelIndex].position - ballOffsett;//flyttar klotet

                if (Input.GetKeyDown(KeyCode.Return))//selectar level
                {
                    if (levelIndex <= GameSaveInfo.current.levelProgress)//ser till att man kan välja den banan
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
                    levelNumbers[unlockedLevel].material = unlockedColor;//ändrar färgen(materail) på siffran
                    unlockPS.transform.position = levelNumbers[unlockedLevel].transform.position + new Vector3(-1f,1.5f,0f);
                    unlockPS.Play();//fyverkerier
                    state = LevelSelectState.Selecting;//gör att man kan börja välja igen
                    levelIndex = unlockedLevel;

                    SoundManagerScript.PlaySound("CheckPoint");

                    SaveSystem.current.Save();
                }
                else
                {
                    levelNumbers[unlockedLevel].transform.localRotation *= Quaternion.Euler(0f, rotateSpeed * Time.deltaTime * 10f, 0f);//spin fast snabbare
                }
                #endregion
                break;

        }
    }

    void LevelSelected()
    {
        foreach (var item in levelNumbers)//gör siffrorna osynliga
        {
            item.enabled = false;
        }

        BallMovement ball = FindObjectOfType<BallMovement>();
        ball.state = PlayerState.Hub;
        ball.rb.velocity = new Vector3(11f, 0f, 0f);//rulla klotet

        state = LevelSelectState.Off;

        Invoke("EnterLevel", transitionWait);//börjar scenetransition efter en vis tid

        exitButton.enabled = false;//stänger av exitknappen
    }

    void EnterLevel()
    {
        SceneTransition.current.EnterScene(levelIndex + GameSaveInfo.levelStartIndex);//laddar rätt scene
    }
    
    void ProgressCheck()
    {
        //ger sifrorna rätt färg baserat på om de är låsta, upplåsta eller avklarade
        for (int i = 0; i < levelNumbers.Length; i++)
        {
            if (i < GameSaveInfo.current.levelProgress)
            {
                levelNumbers[i].material = finishedColor;
            }
            else if (i == GameSaveInfo.current.levelProgress)
            {
                levelNumbers[i].material = unlockedColor;
            }
            else
            {
                levelNumbers[i].material = lockedColor;
            }

            if(i == 5 && GameSaveInfo.current.coinCount < 50)
            {
                levelNumbers[i].material = lockedColor;
            }
        }

        
        //kollar om den banan man nyligen klarat har samma värde som ens levelprogress (då ska nästa bana unlockas)
        if (GameSaveInfo.currentLevel != -1 && GameSaveInfo.current.levelProgress == GameSaveInfo.currentLevel)
        {
            if (GameSaveInfo.currentLevel < 4)
            {
                GameSaveInfo.current.levelProgress++;

                UnlockLevel(GameSaveInfo.current.levelProgress);
            }
            else if(GameSaveInfo.currentLevel == 4)//klarade man bana 5 ska bonus banan inte unlockas direkt
            {
                GameSaveInfo.current.levelProgress = 5;
                levelNumbers[4].material = finishedColor;
            }
            
        }

        GameSaveInfo.currentLevel = -1;
    }
    void UnlockLevel(int level)
    {
        levelNumbers[level-1].material = finishedColor;//ger rätt färg på banan man nyss klara

        state = LevelSelectState.Unlocking;
        unlockedLevel = level;

        cam.targetPoint = levelCameraPoints[level];//ändrar så kameran kollar på den siffran man unlockar
        unlockTimer = unlockTime;

        SoundManagerScript.PlaySound("Spin");
    }

    public void BackToTitleScreen()
    {
        state = LevelSelectState.Off;
        SceneTransition.current.EnterScene(0);
    }
}


public enum LevelSelectState
{
    Unlocking,
    Selecting,
    Off,
}