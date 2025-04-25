//https://nekojara.city/unity-input-system-rebinding
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;

namespace Yns
{
    public class YnInputSystem : YnsSingletonMonoBehaviour<YnInputSystem>
    {
        // スワップ時のPrefix
        const string SwapPrefix = "<Gamepad>/";

        // 対象となるInputActionAsset
        [SerializeField] private InputActionAsset actionAsset;
        static private InputActionAsset _actionAsset;

        // リバインド対象のAction ※AppData.PadActionの順に定義する事
        [SerializeField] private InputActionReference[] actionRef;
        static private InputActionReference[] _actionRef;

        // リバインド対象のScheme
        [SerializeField] static private string _scheme = "Gamepad";

        // 現在のBindingのパスを表示するテキスト
        [SerializeField] static private TMP_Text _pathText;

        // リバインド中のマスク用オブジェクト
        [SerializeField] static private GameObject _mask;

        static private InputAction _action;
        static private InputActionRebindingExtensions.RebindingOperation _rebindOperation;

        static private int _actionIndex = 0;
        static public bool IsRebinding { get; private set; } = false;

        static List<string> _bindList;

        private void Awake()
        {
            _actionAsset = actionAsset;
            _actionRef = new InputActionReference[actionRef.Length];
            for (int i = 0; i < actionRef.Length; ++i)
            {
                _actionRef[i] = actionRef[i];
            }
            _bindList = new List<string>();
        }

        static public void SetMaskObject(GameObject go)
        {
            _mask = go;
        }

        // リバインドを開始する
        static public void StartRebinding(AppData.PadAction act)
        {
            if (act < 0 || _actionRef.Length <= (int)act)
            {
                Debug.LogWarning("ActionRef length over : length[" + _actionRef.Length +"] action[" + (int)act + "]");
                return;
            }

            _actionIndex = (int)act;
            _action = _actionRef[_actionIndex];

            // もしActionが設定されていなければ、何もしない
            if (_action == null) return;

            // もしリバインド中なら、強制的にキャンセル
            // Cancelメソッドを実行すると、OnCancelイベントが発火する
            _rebindOperation?.Cancel();

            // リバインド前にActionを無効化する必要がある
            _action.Disable();

            // リバインド対象のBindingIndexを取得
            int bindingIndex = _action.GetBindingIndex(
                InputBinding.MaskByGroup(_scheme)
            );

            // ブロッキング用マスクを表示
            if (_mask != null)
                _mask.SetActive(true);

            // リバインドが終了した時の処理を行うローカル関数
            void OnFinished()
            {
                // オペレーションの後処理
                CleanUpOperation();

                // 一時的に無効化したActionを有効化する
                _action.Enable();

                // ブロッキング用マスクを非表示
                if (_mask != null)
                    _mask.SetActive(false);

                IsRebinding = false;
            }

            IsRebinding = true;
            UpdateBindList();

            // リバインドのオペレーションを作成し、
            // 各種コールバックの設定を実施し、
            // 開始する
            _rebindOperation = _action
                .PerformInteractiveRebinding(bindingIndex)
                .OnComplete(_ =>
                {
                    // リバインドが完了した時の処理
                    CheckSwapBinding();
                    //RefreshDisplay(true);
                    OnFinished();
                })
                .OnCancel(_ =>
                {
                    // リバインドがキャンセルされた時の処理
                    OnFinished();
                })
                .Start(); // ここでリバインドを開始する
        }

        static void UpdateBindList()
        {
            _bindList.Clear();

            for (int i = 0; i < _actionRef.Length; ++i)
            {
                InputAction action = _actionRef[i];
                action.GetBindingDisplayString(0, out var deviceLayoutName, out var controlPath);
                _bindList.Add(controlPath);
                Debug.Log("BindList[" + i +"]:" + controlPath);
            }
        }

        static private void CheckSwapBinding()
        {
            _action.GetBindingDisplayString(0, out var deviceLayoutName, out var controlPath);
            Debug.Log("CheckSwapBinding ActionIndex=" + _actionIndex + " controllPath:" + controlPath);

            int existCodeIdx = _bindList.FindIndex(x => x == controlPath);
            if (existCodeIdx == _actionIndex)
            {
                Debug.Log("変更前と同じ場所:Action[" + ((AppData.PadAction)_actionIndex).ToString() + "]");
                return;
            }
            if (existCodeIdx >= 0)
            {
                Debug.Log("別の場所に設定されている:[" + ((AppData.PadAction)existCodeIdx).ToString() + "]");
                string swapStr = _bindList[_actionIndex];
                InputAction action = _actionRef[existCodeIdx];
                action.ApplyBindingOverride(0, SwapPrefix + swapStr);
            }
            else
            {
                Debug.Log("未設定:" + controlPath);
            }
        }

        // 上書きされた情報をリセットする
        static public void ResetOverrides()
        {
            // Bindingの上書きを全て解除する
            for (int i = 0; i < _actionRef.Length; ++i)
            {
                InputAction action = _actionRef[i];
                action?.RemoveAllBindingOverrides();
            }
        }

        // 上書き情報の保存
        static public void Save()
        {
            if (_actionAsset == null) return;

            // InputActionAssetの上書き情報の保存
            AppData.PadConfig = _actionAsset.SaveBindingOverridesAsJson();
        }

        // 上書き情報の読み込み
        static public void Load()
        {
            if (_actionAsset == null) return;

            var json = AppData.PadConfig;

            if (!string.IsNullOrEmpty(json))
            {
                // InputActionAssetの上書き情報を設定
                _actionAsset.LoadBindingOverridesFromJson(json);
            }
        }

        // 現在のキーバインド表示を更新
        static private void RefreshDisplay(bool isFinished)
        {
            if (_action == null) return;
            if (isFinished)
                Debug.Log("Rebind:" + _action.GetBindingDisplayString());
        }

        // Actionに設定されているバインド文字列の取得
        static public string GetBindingDisplayString(AppData.PadAction act)
        {
            if (act >= 0 && _actionRef.Length > (int)act)
            {
                InputAction action = _actionRef[(int)act];
                action.GetBindingDisplayString(0, out var deviceLayoutName, out var controlPath);
                // 一度入力しないと取れない情報かも？
                //var bindStr = action.GetBindingDisplayString();
                return controlPath;
            }
            return string.Empty;
        }

        // リバインドオペレーションを破棄する
        static private void CleanUpOperation()
        {
            // オペレーションを作成したら、Disposeしないとメモリリークする
            _rebindOperation?.Dispose();
            _rebindOperation = null;
        }

        // 後処理
        private void OnDestroy()
        {
            // オペレーションは必ず破棄する必要がある
            CleanUpOperation();
        }

        public void OnTrigger(InputAction.CallbackContext context)
        {
            // コールバックだと2回呼ばれるので、performedのみとする
            if (context.performed)
            {
                string name = context.action.name;

                AppData.PadAction padAction = AppData.PadAction.None;

                switch (name)
                {
                    case "Decide":
                        padAction = AppData.PadAction.Decide;
                        break;
                    case "Cancel":
                        padAction = AppData.PadAction.Cancel;
                        break;
                    case "Up":
                        padAction = AppData.PadAction.Up;
                        break;
                    case "Down":
                        padAction = AppData.PadAction.Down;
                        break;
                    case "Left":
                        padAction = AppData.PadAction.Left;
                        break;
                    case "Right":
                        padAction = AppData.PadAction.Right;
                        break;
                    case "Metronome":
                        padAction = AppData.PadAction.Metronome;
                        break;
                    case "Grid":
                        padAction = AppData.PadAction.Grid;
                        break;
                    case "Flip":
                        padAction = AppData.PadAction.Flip;
                        break;
                    case "Speed":
                        padAction = AppData.PadAction.Speed;
                        break;
                    case "Headless":
                        padAction = AppData.PadAction.Headless;
                        break;
                    case "VisionSensor":
                        padAction = AppData.PadAction.VisionSensor;
                        break;
                    case "AutoTakeoffLanding":
                        padAction = AppData.PadAction.AutoTakeoffLanding;
                        break;
                }

                AppData.SetPadTrgs(padAction);
            }
        }

    }
}
