using DocumentFormat.OpenXml.Drawing.Diagrams;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen.Blazor;
using SharedModels;
using Sotsera.Blazor.Toaster.Core.Models;
using static SharedModels.SharedConst;

namespace ExpressionDBBlazorShared.Shared;

public enum enumDialogMode
{
    Edit,
    Add,
}

//西山
public partial class DialogCommonInputContent : IAsyncDisposable
{
    public const string STR_ATTRIBUTE_ADD_DIALOG_INITIAL_VALUE = "AttributesAddDialogInitialValue";

    [Inject]
    protected CommonService ComService { get; set; } = null!;
    
    /// <summary>
    /// 情報エリアAttributes
    /// </summary>
    protected IDictionary<string, object> AttributesInfo { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// 入力エリアAttributes
    /// </summary>
    protected IDictionary<string, object> AttributesInput { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// ファンクションボタンAttributes
    /// </summary>
    protected IDictionary<string, object> AttributesFuncButton { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// 情報エリアコンポーネント
    /// </summary>
    protected List<List<CompItemInfo>> _infoItems = [];

    /// <summary>
    /// 入力エリアコンポーネント
    /// </summary>
    protected List<List<CompItemInfo>> _inputItems = [];

    /// <summary>
    /// Argument追加登録情報
    /// </summary>
    protected IDictionary<string, object> _Arguments { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Formコンポーネント
    /// </summary>
    protected EditForm? editForm;

    /// <summary>
    /// システム設定
    /// </summary>
    protected SystemParameterService _sysParams => SystemParamService;

    #region [Parameter]定義

    /// <summary>
    /// モード
    /// </summary>
    [Parameter]
    public enumDialogMode Mode { get; set; } = 0;
    /// <summary>
    /// ダイアログタイトル
    /// </summary>
    [Parameter]
    public string DialogTitle { get; set; } = string.Empty;
    /// <summary>
    /// ラベル幅
    /// </summary>
    [Parameter]
    public string DialogLabelWidth { get; set; } = string.Empty;
    /// <summary>
    /// 情報エリア折り畳み
    /// </summary>
    [Parameter]
    public bool InfoAllowCollapse { get; set; } = false;
    /// <summary>
    /// 情報エリアタイトル
    /// </summary>
    [Parameter]
    public string InfoTitle { get; set; } = string.Empty;
    /// <summary>
    /// 情報エリアアイコン
    /// </summary>
    [Parameter]
    public string InfoIconName { get; set; } = string.Empty;
    /// <summary>
    /// 入力エリア折り畳み
    /// </summary>
    [Parameter]
    public bool InputAllowCollapse { get; set; } = false;
    /// <summary>
    /// 入力エリアタイトル
    /// </summary>
    [Parameter]
    public string InputTitle { get; set; } = string.Empty;
    /// <summary>
    /// 入力エリアアイコン
    /// </summary>
    [Parameter]
    public string InputIconName { get; set; } = string.Empty;
    /// <summary>
    /// 確定時のプログラム名
    /// </summary>
    [Parameter]
    public string ProgramName { get; set; } = string.Empty;
    /// <summary>
    /// DEFINE_COMPONENTSテーブル情報
    /// </summary>
    [Parameter]
    public IList<ComponentsInfo> ComponentsInfos { get; set; } = [];
    /// <summary>
    /// グリッドカラム定義
    /// </summary>
    [Parameter]
    public IList<ComponentColumnsInfo> Components { get; set; } = [];
    /// <summary>
    /// 初期データ
    /// </summary>
    [Parameter]
    public Dictionary<string, object> InitialData { get; set; } = [];
    /// <summary>
    /// セル選択時のカラム名
    /// </summary>
    [Parameter]
    public string? CellColumn { get; set; }
    /// <summary>
    /// F1確認時にパスワード要求をするか
    /// </summary>
    [Parameter]
    public bool PasswordCheck { get; set; } = false;
    #endregion

    [Inject]
    protected HtService? htService { get; set; } = null!;

    /// <summary>
    /// 初期化処理
    /// 西山　物品管理
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        // システム設定値を取得しておく
        //await SessionStorage.GetItemAsync<SystemParameterService>(SharedConst.KEY_SYSTEM_PARAM);

        // 情報エリア初期化
        await InitInfoItemsAsync();

        // 入力エリア初期化
        await InitInputItemsAsync();

        //TODO DBから取得する
        // AttributesFuncButton設定
        AttributesFuncButton = new Dictionary<string, object>(){
            { "button1text", "確定[F1]" },
            { "button2text", string.Empty },//サイドバーに読取機能追加（F2）
            { "button3text", string.Empty },
            { "button4text", "キャンセル[F4]" },
            { "button5text", string.Empty },
            { "button6text", string.Empty },
            { "button7text", string.Empty },
            { "button8text", string.Empty },
            { "button9text", string.Empty },
            { "button10text", string.Empty },
            { "button11text", string.Empty },
            { "button12text", string.Empty},
            { "IsBusyDialog", false},
        };
        await base.OnInitializedAsync();
    }

    /// <summary>
    /// 終了処理
    /// </summary>
    /// <returns></returns>
    public ValueTask DisposeAsync()
    {
        Dispose();

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 終了処理
    /// </summary>
    protected virtual void Dispose()
    {
    }

    /// <summary>
    /// F1ボタンクリックイベント
    /// 引数を送る？
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected virtual async Task OnClickResultF1(object? sender)//確定ボタン
    {
        // チェック
        if (false == バリデートチェック())
        {
            return;
        }

        bool retb = false;
        try
        {
            // 入力エリアロック
            ComService.SetCompItemListDisabled(_inputItems, true);

            // 結果変数
            List<ExecResult> lstResult = [];
            int notifyDuration = _sysParams is null ? SharedConst.DEFAULT_NOTIFY_DURATION : _sysParams.NotifyPopupDuration;
            string strSummary = DialogTitle.Replace("\\n", "");
            string strResultMessage = "登録しました。";

            // 確認
            //bool? retConfirm;
            //if (PasswordCheck) retConfirm = await ComService.DialogShowPassword("設定を確定しますか？", strSummary);
            //else retConfirm = await ComService.DialogShowYesNo("設定を確定しますか？", strSummary);

            //retb = retConfirm is not null && (bool)retConfirm;
            retb = await ShowConfirmationAsync("設定を確定しますか？", strSummary);
            if (!retb)
            {
                return;
            }

            // 登録
            retb = false;
            if (!string.IsNullOrEmpty(ProgramName))
            {
                // RequestValueにデータを作成する
                RequestValue rv = RequestValue.CreateRequestProgram(ProgramName);

                // 情報エリアからWebAPIに渡す値を作成する
                Dictionary<string, (object, WhereParam)> itemInfos = ComService.GetCompItemInfoValues(_infoItems);
                foreach (List<CompItemInfo> listItem in _infoItems)
                {
                    foreach (CompItemInfo item in listItem)
                    {
                        _ = itemInfos.TryGetValue(item.TitleLabel, out (object, WhereParam) value)
                            ? rv.SetArgumentValue(item.TitleLabel, value.Item1, "")
                            : rv.SetArgumentValue(item.TitleLabel, string.Empty, "");
                    }
                }

                // 入力エリアからWebAPIに渡す値を作成する
                //西山17
                Dictionary<string, (object, WhereParam)> itemInputs = ComService.GetCompItemInfoValues(_inputItems);
                foreach (List<CompItemInfo> listItem in _inputItems)
                {
                    foreach (CompItemInfo item in listItem)
                    {
                        _ = itemInputs.TryGetValue(item.TitleLabel, out (object, WhereParam) value)
                            ? rv.SetArgumentValue(item.TitleLabel, value.Item1, "")
                            : rv.SetArgumentValue(item.TitleLabel, string.Empty, "");
                    }
                }

                // 追加情報を登録する
                foreach (KeyValuePair<string, object> item in _Arguments)
                {
                    _ = rv.SetArgumentValue(item.Key, item.Value, "");
                }

                // WebAPIへアクセス
                retb = true;
                ExecResult[]? results = await WebComService.SetRequestValue(GetType().Name, rv);
                if (results == null)
                {
                    // 実行結果がnullの場合は異常
                    ComService!.ShowNotifyMessege(ToastType.Error, $"{strSummary}", "WebAPIへのアクセスが異常終了しました。ログを確認して下さい。");
                    return;
                }
                else
                {
                    // 実行結果が返った場合
                    lstResult = new List<ExecResult>(results);
                }
            }

            // 実行結果を異常・正常・確認に分ける
            List<ExecResult> lstError = lstResult.Where(_ => _.RetCode < 0).OrderBy(_ => _.ExecOrderRank).ToList();
            List<ExecResult> lstSuccess = lstResult.Where(_ => _.RetCode == 0).OrderBy(_ => _.ExecOrderRank).ToList();
            List<ExecResult> lstConfirm = lstResult.Where(_ => _.RetCode > 0).OrderBy(_ => _.ExecOrderRank).ToList();

            if (lstError.Count() > 0)
            {
                // 異常結果がある場合
                retb = false;

                // 異常メッセージを全て通知
                foreach (ExecResult result in lstError)
                {
                    ComService!.ShowNotifyMessege(ToastType.Error, $"{strSummary}", result.Message);
                }
            }
            else
            {
                // 異常結果が無い場合
                retb = true;

                // 正常結果のメッセージがある場合、全て通知
                foreach (ExecResult result in lstSuccess)
                {
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        ComService!.ShowNotifyMessege(ToastType.Success, $"{strSummary}", result.Message);
                    }
                }

                // 確認結果がある場合、全ての確認ダイアログ表示
                foreach (ExecResult result in lstConfirm)
                {
                    bool? ret = await ComService.DialogShowYesNo(result.Message);
                    retb = ret is not null && (bool)ret;
                    if (!retb)
                    {
                        break;
                    }
                }
            }

            // 正常終了で結果メッセージがある場合は通知
            if (retb)
            {
                if (!string.IsNullOrEmpty(strResultMessage))
                {
                    ComService!.ShowNotifyMessege(ToastType.Success, $"{strSummary}", strResultMessage);
                }
            }
        }
        finally
        {
            // 入力エリアロック解除
            ComService.SetCompItemListDisabled(_inputItems, false);
        }

        // 正常終了ならダイアログ閉じる
        if (retb)
        {
            DialogService.CloseSide(retb);
        }
    }


    protected virtual async Task<bool> ShowConfirmationAsync(string message, string summary)
    {
        bool? result;

        if (PasswordCheck)
            result = await ComService.DialogShowPassword(message, summary);
        else
            result = await ComService.DialogShowYesNo(message, summary);

        return result is not null && (bool)result;
    }


    /// <summary>
    /// F4ボタンクリックイベント
    /// </summary>
    /// <param name="sender"></param>
    protected virtual void OnClickResultF4(object? sender)
    {
        // 閉じる
        DialogService.CloseSide();
    }

    /// <summary>
    /// F2カメラ読取
    /// 2025/12/25 西山　 
    /// </summary>
    ///// <param name="sender"></param>
    protected virtual void  OnClickResultF2(object? sender)
    {
        _ = htService?.StartRead();
    }


    /// <summary>
    /// 情報エリア初期化
    /// </summary>
    protected virtual async Task InitInfoItemsAsync()//情報を表示させる
    {
        // クリア
        _infoItems.Clear();

        if (Mode == enumDialogMode.Edit)//編集の場合はEDIT_TYPEが「２」の場合そのデータは情報として出る
        {
            // カラム設定データからヘッダ項目のみを抽出し、並び変える
            List<ComponentColumnsInfo> listInfo = Components
                .Where(_ => _.IsEdit == true && _.EditType == 2)
                .OrderBy(_ => _.EditDialogLayoutGroup)
                .ThenBy(_ => _.EditDialogLayoutDispOrder)
                .ToList();

            // コンポーネント情報を作成
            _infoItems = await ComService.GetCompItemInfo(listInfo, InitialData, Components, ComponentsInfos, false, true);

            // 情報エリアAttributes設定
            AttributesInfo.Add("AllowCollapse", InfoAllowCollapse);
            AttributesInfo.Add("GroupTitle", InfoTitle);
            AttributesInfo.Add("IconName", InfoIconName);
            AttributesInfo.Add("CopmItems", _infoItems);
            AttributesInfo.Add("LabelWidth", DialogLabelWidth);
        }

        if (Mode == enumDialogMode.Add)
        {
            // カラム設定データからヘッダ項目のみを抽出し、並び変える
            List<ComponentColumnsInfo> listInfo = Components
                .Where(_ => _.IsEdit == true && _.EditType == 2)
                .OrderBy(_ => _.EditDialogLayoutGroup)
                .ThenBy(_ => _.EditDialogLayoutDispOrder)
                .ToList();

            // コンポーネント情報を作成
            _infoItems = await ComService.GetCompItemInfo(listInfo, InitialData, Components, ComponentsInfos, false, true);

            // 情報エリアAttributes設定
            AttributesInfo.Add("AllowCollapse", InfoAllowCollapse);
            AttributesInfo.Add("GroupTitle", InfoTitle);
            AttributesInfo.Add("IconName", InfoIconName);
            AttributesInfo.Add("CopmItems", _infoItems);
            AttributesInfo.Add("LabelWidth", DialogLabelWidth);
        }
    }

    /// <summary>
    /// 明細項目初期化
    /// </summary>
    protected virtual async Task InitInputItemsAsync()//入力コンポーネント
    {
        // クリア
        _inputItems.Clear();

        // 追加モードの場合、DEFINE_COMPONENTSの初期値を設定
        if (Mode == enumDialogMode.Add)
        {
            List<ComponentsInfo> lstInitInfo = ComponentsInfos.Where(_ => _.ComponentName == STR_ATTRIBUTE_ADD_DIALOG_INITIAL_VALUE).ToList();
            foreach (ComponentsInfo initInfo in lstInitInfo)
            {
                if (!string.IsNullOrEmpty(initInfo.Value))
                {
                    // 【注意】複数の初期値を設定できるコンポーネントは、カンマ(,)区切りで初期値が登録されている前提です
                    string[] values = initInfo.Value.Split(',');
                    InitialData[initInfo.AttributesName] = values.Length > 1 ? new List<string>(values) : values[0];
                }
            }
        }

        // カラム設定データから明細項目のみを抽出し、並び変える
        List<ComponentColumnsInfo> listInfo = Mode == enumDialogMode.Edit
            ? Components
                .Where(_ => _.IsEdit == true && _.EditType == 1)
                .OrderBy(_ => _.EditDialogLayoutGroup)
                .ThenBy(_ => _.EditDialogLayoutDispOrder)
                .ToList()
            : Components
                .Where(_ => _.IsEdit == true && (_.EditType == 1 || _.EditType == 2))
                .OrderBy(_ => _.EditDialogLayoutGroup)
                .ThenBy(_ => _.EditDialogLayoutDispOrder)
                .ToList();

        // コンポーネント情報を作成
        _inputItems = await ComService.GetCompItemInfo(listInfo, InitialData, Components, ComponentsInfos, false, false);

        // 入力エリアAttributes設定
        AttributesInput.Add("AllowCollapse", InputAllowCollapse);
        AttributesInput.Add("GroupTitle", InputTitle);
        AttributesInput.Add("IconName", InputIconName);
        AttributesInput.Add("CopmItems", _inputItems);
        AttributesInput.Add("LabelWidth", DialogLabelWidth);
    }

    /// <summary>
    /// バリデート
    /// </summary>
    /// <returns></returns>
    public bool バリデートチェック()
    {
        if (editForm is null)
        {
            return true;
        }

        //EditContextのValidate()メソッドを実行することでSubmitと同等のイベントが発火
        return editForm!.EditContext.Validate();
    }



}