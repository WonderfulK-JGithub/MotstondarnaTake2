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

        if (GameSaveInfo.currentLevel != -1) levelIndex = GameSaveInfo.currentLevel;//man ska b�rja vid siffran p� banan man nyss klarade av

        ProgressCheck();

        if(GameSaveInfo.current.coinCount == 50)//tar bort text som s�ger att man beh�ver mynt f�r bonus banan om man inte l�ngre beh�ver det
        {
            needCoinImage.enabled = false;
            needText.enabled = false;
        }

        coinCountText.text = GameSaveInfo.current.coinCount.ToString();//�ndar totala coincount texten

        for (int i = 0; i < GameSaveInfo.current.coinLevelsCount.Length; i++)//uppdarerar coincount f�r varje bana
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
                
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) //La till s� att man kan anv�nda A och D ocks� - Max
                {
                    levelIndex++;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                {
                    levelIndex--;
                }
                //ser till att levelIndex inte hamnar utanf�r "the bound of the array"
                levelIndex = Mathf.Clamp(levelIndex, 0, levelCameraPoints.Length - 1);

                cam.targetPoint = levelCameraPoints[levelIndex];//�ndar vart kameran ska kolla

                levelNumbers[levelIndex].transform.localRotation *= Quaternion.Euler(0f, rotateSpeed * Time.deltaTime, 0f);//roterar siffran man kollar p� just d�

                ball.transform.position = levelCameraPoints[levelIndex].position - ballOffsett;//flyttar klotet

                if (Input.GetKeyDown(KeyCode.Return))//selectar level
                {
                    if (levelIndex <= GameSaveInfo.current.levelProgress)//ser till att man kan v�lja den banan
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
                    levelNumbers[unlockedLevel].material = unlockedColor;//�ndrar f�rgen(materail) p� siffran
                    unlockPS.transform.position = levelNumbers[unlockedLevel].transform.position + new Vector3(-1f,1.5f,0f);
                    unlockPS.Play();//fyverkerier
                    state = LevelSelectState.Selecting;//g�r att man kan b�rja v�lja igen
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
        foreach (var item in levelNumbers)//g�r siffrorna osynliga
        {
            item.enabled = false;
        }

        BallMovement ball = FindObjectOfType<BallMovement>();
        ball.state = PlayerState.Hub;
        ball.rb.velocity = new Vector3(11f, 0f, 0f);//rulla klotet

        state = LevelSelectState.Off;

        Invoke("EnterLevel", transitionWait);//b�rjar scenetransition efter en vis tid

        exitButton.enabled = false;//st�nger av exitknappen
    }

    void EnterLevel()
    {
        SceneTransition.current.EnterScene(levelIndex + GameSaveInfo.levelStartIndex);//laddar r�tt scene
    }
    
    void ProgressCheck()
    {
        //ger sifrorna r�tt f�rg baserat p� om de �r l�sta, uppl�sta eller avklarade
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

        
        //kollar om den banan man nyligen klarat har samma v�rde som ens levelprogress (d� ska n�sta bana unlockas)
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
        levelNumbers[level-1].material = finishedColor;//ger r�tt f�rg p� banan man nyss klara

        state = LevelSelectState.Unlocking;
        unlockedLevel = level;

        cam.targetPoint = levelCameraPoints[level];//�ndrar s� kameran kollar p� den siffran man unlockar
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