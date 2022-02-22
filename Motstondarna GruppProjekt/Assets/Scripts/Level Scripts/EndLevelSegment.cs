using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelSegment : MonoBehaviour
{
    [SerializeField] List<BaseEnemy> goldenPins;

    bool levelEnded;
    void Update()
    {
        if(!levelEnded)
        {
            for (int i = 0; i < goldenPins.Count; i++)
            {
                if (goldenPins[i].hasDied)
                {
                    goldenPins.RemoveAt(i);
                    i--;
                }
            }


            if (goldenPins.Count == 0)
            {
                levelEnded = true;
                EndLevel();
            }
        }
    }

    void EndLevel()
    {
        int i = 0;
        foreach (var item in FindObjectsOfType<CollectableCoin>())
        {
            if (item.isCollected)
            {
                if (!item.isStored) GameSaveInfo.current.coinCount++;
                item.isStored = true;

            }
            if (item.isStored) i++;

            item.isCollected = false;
        }

        GameSaveInfo.currentLevel = SceneTransition.current.GetSceneIndex() - GameSaveInfo.levelStartIndex;

        if(GameSaveInfo.currentLevel < 5)
        {
            GameSaveInfo.current.coinLevelsCount[GameSaveInfo.currentLevel] = i;
        }
        else
        {
            GameSaveInfo.current.levelProgress = 6;
        }

        SaveSystem.current.Save();

        PlayerPrefs.SetInt("progress", 0);


        //SceneTransition.current.ReLoadScene();
        SceneTransition.current.EnterScene(3);
    }
}
