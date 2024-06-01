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

                                // EventManager.instance.CustomEvent(AnalyticsType.RV, "GainReward_GetAllUpgrade");
                                break;

                            case RewardType.Revive:
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
                                    // EventManager.instance.CustomEvent(AnalyticsType.RV, "GainReward_Revive");
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

                                    EventManager.instance.CustomEvent(AnalyticsType.RV, "GainReward_TrialShip - " + GameManager.instance.currentShip.shipObjectData.shipCode);
                                    // FirebaseAnalytics.LogEvent("TrialShip", "shipName", GameManager.instance.currentShip.shipObjectData.shipCode);
                                });

                                break;

                            case RewardType.DoubleCrystal:
                                AdManager.instance.HideDoubleCrystalBtn();
                                GameManager.instance.playerStat.GetCrystalDouble();
                                AdManager.instance.useCrystalDoubleThisStage = true;
                                InterstitialAdCaller.instance.RestartIrAdsCoolTime();
                                gameObject.SetActive(false);
                                // AdManager.instance.crystalBonusRVBtn?.SetActive(false);

                                // EventManager.instance.CustomEvent(AnalyticsType.RV, "GainReward_DoubleCrystal");
                                break;

                            case RewardType.FreeModule:
                                AdManager.instance.HideDoubleCrystalBtn();
                                GameManager.instance.playerStat.GetCrystalDouble();
                                AdManager.instance.useCrystalDoubleThisStage = true;
                                InterstitialAdCaller.instance.RestartIrAdsCoolTime();
                                AdManager.instance.crystalBonusRVBtn?.SetActive(false);

                                // EventManager.instance.CustomEvent(AnalyticsType.RV, "GainReward_DoubleCrystal");
                                break;

                            case RewardType.FreeCrystal:
                                UserDataManager.instance.currentUserData.usingFreeCrystalTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                UserDataManager.instance.Save();
                                UserDataManager.instance.AddCrystalValue(AdManager.instance.crystalValue);
                                InterstitialAdCaller.instance.RestartIrAdsCoolTime();

                                // EventManager.instance.CustomEvent(AnalyticsType.RV, "GainReward_FreeCrystal");
                                break;

                            case RewardType.Reroll:
                                LevelUpManager.instance.Reroll();
                                // EventManager.instance.CustomEvent(AnalyticsType.RV, "GainReward_Reroll");
                                break;


                            default:
                                Debug.LogError("선언되지 않은 보상형 광고 타입입니다.");
                                break;
                        }

                        EventManager.instance.CustomEvent(AnalyticsType.RV, "GainReward_" + type);
                    }, type.ToString());
        }
    }

}
