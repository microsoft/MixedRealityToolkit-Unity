using Microsoft.MixedReality.Toolkit;
using MRTK.Core;
using MRTK.StateControl;
using System;
using System.Collections;
using UnityEngine;

public class AppStateTester : MonoBehaviour
{
    public IEnumerator Start()
    {
        while (!MixedRealityToolkit.Instance.IsServiceRegistered<AppState>())
            yield return null;

        AppState appState = MixedRealityToolkit.Instance.GetService<AppState>();

        while (!appState.Initialized)
            Debug.Log("Waiting for app state to initialize...");
            yield return null;

        switch (appState.AppRole)
        {
            case AppRoleEnum.Client:
                // Tell server we're ready to synch
                appState.ReadyToSynchronize = true;
                break;

            case AppRoleEnum.Server:
                // Add a bunch of states, then say we're ready to sync
                BaseItemState baseState1 = new BaseItemState();
                baseState1.Key = 1;
                baseState1.Value = 1;

                BaseItemState baseState2 = new BaseItemState();
                baseState2.Key = 2;
                baseState1.Value = 2;

                appState.AddState<BaseItemState>(baseState1);
                appState.AddState<BaseItemState>(baseState2);

                appState.ReadyToSynchronize = true;
                break;
        }

        yield break;
    }

    private void Update()
    {
        AppState appState = MixedRealityToolkit.Instance.GetService<AppState>();
        if (appState.Initialized)
            appState.Flush();
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(AppStateTester))]
    public class AppStateTesterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
                return;

            if (!MixedRealityToolkit.Instance.IsServiceRegistered<AppState>())
            {
                UnityEditor.EditorGUILayout.LabelField("Not registered");
                return;
            }

            AppState appState = MixedRealityToolkit.Instance.GetService<AppState>();

            foreach (Type stateType in appState.ItemStateTypes)
            {
                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                UnityEditor.EditorGUILayout.LabelField(stateType.Name);
                foreach (object state in appState.GetStates(stateType))
                {
                    UnityEditor.EditorGUILayout.LabelField(StateUtils.StateToString(state));
                }
                UnityEditor.EditorGUILayout.Space();
                UnityEditor.EditorGUILayout.EndVertical();
            }
        }
    }
#endif
}

public class BaseItemStateArray : StateArray<BaseItemState>
{
    public BaseItemStateArray(IStatePipe statePipe) : base(statePipe) { }
}

public class AnotherItemStateArray : StateArray<AnotherItemState>
{
    public AnotherItemStateArray(IStatePipe statePipe) : base(statePipe) { }
}

[AppStateType]
public struct BaseItemState : IItemState, IItemStateComparer<BaseItemState>
{
    public short Key { get; set; }
    public int Value;
    
    public BaseItemState(short key)
    {
        this.Key = key;
        Value = 0;
    }

    public bool IsDifferent(BaseItemState from)
    {
        return true;
    }

    public BaseItemState Merge(BaseItemState localValue, BaseItemState remoteValue)
    {
        return remoteValue;
    }
}

[AppStateType]
public struct AnotherItemState : IItemState, IItemStateComparer<AnotherItemState>
{
    public short Key { get; set; }
    public int Value;

    public AnotherItemState(short key)
    {
        this.Key = key;
        Value = 0;
    }

    public bool IsDifferent(AnotherItemState from)
    {
        return true;
    }

    public AnotherItemState Merge(AnotherItemState localValue, AnotherItemState remoteValue)
    {
        return remoteValue;
    }
}