using Microsoft.AspNetCore.Components;
using SharedModels;
using System.Timers;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 摘取ピック(倉庫配送先別)/パレットNo.読取
    /// </summary>
    public partial class StepItemPickingItemByDeliveryPallet : StepItemBase
    {
        private StepItemPickingItemByDeliveryViewModel? model;

        /// <summary>
        /// 画面自動更新用タイマー
        /// </summary>
        private System.Timers.Timer? timeMonitorPickScheduleReflesh;

        /// <summary>
        /// ピッキング予定リフレッシュ間隔[ミリ秒]
        /// </summary>
        private int MonitorPickScheduleRefleshInterval { get; set; } = 30000;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "MPalletNo";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemPickingItemByDeliveryViewModel?)PageVm;

            // 初期処理呼び出し
            await InitProcAsync();

            if (null != _sysParams)
            {
                MonitorPickScheduleRefleshInterval = _sysParams.MonitorPickScheduleRefleshInterval;
            }
            // ピッキング予定リフレッシュタイマー起動
            StopMonitorPickScheduleRefleshTimer();
            StartMonitorPickScheduleRefleshTimer(MonitorPickScheduleRefleshInterval);
        }

        /// <summary>
        /// 破棄
        /// </summary>
        protected override void Dispose()
        {
            // タイマー停止
            StopMonitorPickScheduleRefleshTimer();

            base.Dispose();
        }

        /// <summary>
        /// 確定前チェック
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)
        {
            //予定一覧に存在するパレットNoの場合は確定処理を実行する
            bool isExist = _gridData.Any(_ => _.ContainsKey("ﾊﾟﾚｯﾄNo") && _["ﾊﾟﾚｯﾄNo"].ToString() == model!.MPalletNo);
            if (!isExist)
            {
                await ComService.DialogShowOK($"ﾋﾟｯｸ予定一覧にないﾊﾟﾚｯﾄです。", pageName);
                SetElementIdFocus("MPalletNo");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F1画面遷移(ComponentProgramInfo info)
        {
            (int, string) ret = await GetHikiateZumiZaikoDelivery(model!.MPalletNo, model!.DeliveryCd);
            if (ret.Item1 == 0)
            {
                await ComService.DialogShowOK("引当済在庫が存在しません。");
                SetElementIdFocus("MPalletNo");
                return;
            }

            if (ret.Item1 == 1)
            {
                // 出荷予定集約IDをセットして確定ステップへ
                model!.ShippingAggrId = ret.Item2;
                if (StepsExtend is not null)
                {
                    await StepsExtend!.SetStep(2);
                }
            }
            else if (ret.Item1 > 1)
            {
                await 次ステップへ(info);
            }
        }

        /// <summary>
        /// 切出
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F2画面遷移(ComponentProgramInfo info)
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.MPalletNo);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_NM, model!.DeliveryName);
            // 切出搬送画面に遷移
            NavigationManager.NavigateTo("move_complete");
        }
        /// <summary>
        /// コーナー
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F3画面遷移(ComponentProgramInfo info)
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.MPalletNo);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_NM, model!.DeliveryName);
            // コーナー搬送画面に遷移
            NavigationManager.NavigateTo("move_complete_corner");
        }

        /// <summary>
        /// 戻る
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F4画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID, model!.AreaCd);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID, model!.ZoneCd);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_ID, model!.DeliveryCd);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_NM, model!.DeliveryName);
            // ピッキング【倉庫配送先別】メニューに戻る
            NavigationManager.NavigateTo("picking_target_select_item_by_delivery");
        }

        /// <summary>
        /// スキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            string value = scanData.strStringData;

            if (IsPalletBarcode(value))
            {
                model!.MPalletNo = value;
                //予定一覧に存在するパレットNoの場合は確定処理を実行する
                bool isExist = _gridData.Any(_ => _.ContainsKey("ﾊﾟﾚｯﾄNo") && _["ﾊﾟﾚｯﾄNo"].ToString() == model.MPalletNo);
                if (isExist)
                {
                    await ContainerMainLayout.ButtonClickF1();
                }
            }
            StateHasChanged();
        }

        #endregion

        #region Event

        /// <summary>
        /// OnCellRenderのCallBack
        /// </summary>
        /// <param name="args"></param>
        private void CellRender(DataGridCellRenderEventArgs<IDictionary<string, object>> args)
        {
            try
            {
                // 取得した列情報の１列目のタイトルと一致するセルの背景色を変える
                if (_gridColumns.Count > 0 && _gridColumns[0].Title == args.Column.Title)
                {
                    if (args.Data.TryGetValue("緊急出荷区分", out object value))
                    {
                        if ("1" == value.ToString())
                        {
                            args.Attributes.Add("style", $"background-color: {_sysParams.EmergencyColor};");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// パレットNo変更イベント
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private async Task OnChangePalletNo(object value)
        {
            model!.MPalletNo = (string)value;

            await Task.Delay(0);
            //await LoadGridData();
        }

        #endregion

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            InitParam();

            // データの読込
            await LoadGridData();
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
        }


        /// <summary>
        /// ピッキング予定リフレッシュタイマー起動
        /// </summary>
        private void StartMonitorPickScheduleRefleshTimer(int Intartval)
        {
            timeMonitorPickScheduleReflesh = new System.Timers.Timer(Intartval);
            timeMonitorPickScheduleReflesh.Elapsed += OnMonitorPickScheduleRefleshTimedEvent;
            timeMonitorPickScheduleReflesh.AutoReset = true;
            timeMonitorPickScheduleReflesh.Enabled = true;
        }

        /// <summary>
        /// ピッキング予定リフレッシュタイマー停止
        /// </summary>
        private void StopMonitorPickScheduleRefleshTimer()
        {
            if (timeMonitorPickScheduleReflesh is not null)
            {
                timeMonitorPickScheduleReflesh.Enabled = false;
                timeMonitorPickScheduleReflesh.Elapsed -= OnMonitorPickScheduleRefleshTimedEvent;
            }
        }

        /// <summary>
        /// ピッキング予定リフレッシュタイマー通知
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private async void OnMonitorPickScheduleRefleshTimedEvent(object? source, ElapsedEventArgs e)
        {
            try
            {
                // データの読込
                await LoadGridData();
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        #endregion
    }
}