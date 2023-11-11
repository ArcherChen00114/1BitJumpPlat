using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static bool PlaneDead=false;
    public static bool MaJiangDead = false;
    public static bool XiangQiDead = false;
    public static bool BallDead = false;
    public static int BossLeftNum = 2;
    public static Action DefeatBossEvent;

    public static Action<AudioClip> PlaySFXEvent;

    public static void CallPlaySFXEvent(AudioClip audioClip) {
        PlaySFXEvent?.Invoke(audioClip);
    }

    public static Action<AudioClip> SwitchMusicEvent;

    public static void CallSwitchMusicEvent(AudioClip audioClip) {
        SwitchMusicEvent?.Invoke(audioClip);
    }

    public static void CallDefeatBossEvent()
    {
        DefeatBossEvent?.Invoke();
    }

    public static Action StartGameEvent;

    public static void CallStartGameEvent() {
        StartGameEvent?.Invoke();
    }



    public static Action CameraShakeEvent;
    public static void CallCameraShakeEvent() {
        CameraShakeEvent?.Invoke();
    }

    public static Action BattleEndEvent;

    public static void CallBattleEndEvent() {
        BattleEndEvent?.Invoke();
    }

    public static Action EnemyDeadEvent;
    public static void CallEnemyDeadEvent() {
        EnemyDeadEvent?.Invoke();
    }

    public static Action PlayerDeathEvent;
    public static void CallPlayerDeathEvent() {
        PlayerDeathEvent?.Invoke();
    }

    public static Action<string, Vector3> SaveEvent;

    public static void CallSaveEvent(string sceneName, Vector3 positionToGo) {
        SaveEvent?.Invoke(sceneName, positionToGo);
    }

    public static Action LoadEvent;

    public static void CallLoadEvent()
    {
        LoadEvent?.Invoke();
    }


    #region 场景转换事件

    public static Action PortalEvent;

    public static void CallPortalEvent() {
        PortalEvent?.Invoke();
    }

    public static Action<string, Vector3> TransitionEvent;

    public static void CallTransitionEvent(string transition, Vector3 position)
    {
        TransitionEvent?.Invoke(transition, position);
    }

    public static Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }
    public static Action AfterSceneLoadedEvent;
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }

    public static event Action<Vector3> moveToPosition;
    public static void CallMoveToPosition(Vector3 position)
    {
        moveToPosition?.Invoke(position);
    }
    #endregion
}
