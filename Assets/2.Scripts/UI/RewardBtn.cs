using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using DG.Tweening;

public class RewardBtn : MonoBehaviour
{
    public RewardType type;

    

    public void OnClick()
    {
        print(AdManager.instance.IsTimeLimitRewardReady(type));
        if (AdManager.instance.IsTimeLimitRewardReady(type))
        {
            AdManager.instance.ShowReward(() =>
                    {

                        switch (type)
                        {
                            case RewardType.GetAll:
                                LevelUpManager.instance.GetAllCurrentUpgrade();
                                break;

                            case RewardType.Revive:
                                print(1);
                                if (GameManager.instance.reviveAnimationSequence != null)
                                    GameManager.instance.reviveAnimationSequence.Kill();

                                AdManager.instance.touchProjectPanel.SetActive(true);

                                GameManager.instance.TaskDelay(0.3f, () =>
                                {
                                    AdManager.instance.touchProjectPanel.SetActive(false);
                                    GameManager.instance.revivedThisGame = true;
                                    GameManager.instance.gameStart = true;
                                    GameManager.instance.Resurrection();
                                    GameManager.instance.StartTimer();
                                    EnemyGenerator.instance.StartSpawnEnemy();
                                    GameManager.instance.playerStat.Resurrection();
                                    InterstitialAdCaller.instance.RestartIrAdsCoolTime();
                                    // FirebaseAnalytics.LogEvent("ADS_RvAdsComplete_Revive");
                                });

                                break;

                            case RewardType.TrialShip:
                                UserDataManager.instance.currentUserData.usingShipTrialTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                UserDataManager.instance.Save();

                                AdManager.instance.touchProjectPanel.SetActive(true);

                                GameManager.instance.TaskDelay(0.3f, () =>
                                {
                                    // AdManager.instance.shipTrialButton.onClick.Invoke();
                                    GameManager.instance.StartTrialShip();
                                    AdManager.instance.touchProjectPanel.SetActive(false);
                                    InterstitialAdCaller.instance.RestartIrAdsCoolTime();

                                    // FirebaseAnalytics.LogEvent("ADS_RvAdsComplete_TrialShip");
                                    // FirebaseAnalytics.LogEvent("TrialShip", "shipName", GameManager.instance.currentShip.shipObjectData.shipCode);
                                });

                                break;

                            case RewardType.DoubleCrystal:
                                AdManager.instance.HideDoubleCrystalBtn();
                                GameManager.instance.playerStat.GetCrystalDouble();
                                AdManager.instance.useCrystalDoubleThisStage = true;
                                InterstitialAdCaller.instance.RestartIrAdsCoolTime();
                                AdManager.instance.crystalBonusRVBtn?.SetActive(false);

                                // FirebaseAnalytics.LogEvent("ADS_RvAdsComplete_DoubleCrystal");
                                break;

                            case RewardType.FreeModule:
                                AdManager.instance.HideDoubleCrystalBtn();
                                GameManager.instance.playerStat.GetCrystalDouble();
                                AdManager.instance.useCrystalDoubleThisStage = true;
                                InterstitialAdCaller.instance.RestartIrAdsCoolTime();
                                AdManager.instance.crystalBonusRVBtn?.SetActive(false);

                                // FirebaseAnalytics.LogEvent("ADS_RvAdsComplete_DoubleCrystal");
                                break;

                            case RewardType.FreeCrystal:
                                UserDataManager.instance.currentUserData.usingFreeCrystalTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                UserDataManager.instance.Save();
                                UserDataManager.instance.AddCrystalValue(AdManager.instance.crystalValue);
                                InterstitialAdCaller.instance.RestartIrAdsCoolTime();

                                // FirebaseAnalytics.LogEvent("ADS_RvAdsComplete_FreeCrystal");
                                break;


                            default:
                                Debug.LogError("선언되지 않은 보상형 광고 타입입니다.");
                                break;
                        }

                    }, type.ToString());
        }
    }

}
