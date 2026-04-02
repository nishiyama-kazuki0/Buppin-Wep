using ExpressionDBBlazorShared.Data;
using Microsoft.AspNetCore.Components;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// ユーザー設定ダイアログ
/// </summary>
public partial class DialogUserSetting : ChildPageBasePC
{
    public const string STR_IS_VISIBLE_SAVE_SEARCH = "IsVisibleSaveSearch";
    public const string STR_SETTINGS_CLASS_NAME = "SettingClassName";
    public const string STR_SETTINGS_INFO = "SettingsInfo";

    private CompTextBox? txtPasswordCur;
    private CompTextBox? txtPasswordNew;
    private CompTextBox? txtPasswordNewConf;

    [Parameter]
    public bool IsVisibleSaveSearch { get; set; } = false;

    [Parameter]
    public string SettingClassName { get; set; } = string.Empty;

    [Parameter]
    public List<UserComponentSettingsInfo> SettingsInfo { get; set; } = [];

    [Parameter]
    public string RequiredDisplaySuffix { get; set; } = "＊";

    #region スタイル関連

    /// <summary>
    /// タイトルのフォントサイズ
    /// </summary>
    private string TitleFontSize { get; set; } = "100%";
    /// <summary>
    /// タイトルのフォント幅
    /// </summary>
    private string TitleFontWeight { get; set; } = "bold";
    /// <summary>
    /// ラベルのフォントサイズ
    /// </summary>
    private string LabelFontSize { get; set; } = "100%";
    /// <summary>
    /// ラベルのフォント幅
    /// </summary>
    private string LabelFontWeight { get; set; } = "bold";
    /// <summary>
    /// ラベル幅
    /// </summary>
    private string LabelWidth { get; set; } = "160";

    #endregion

    #region override

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (null != _sysParams)
        {
            RequiredDisplaySuffix = _sysParams.RequiredDisplaySuffixPC;
            TitleFontSize = _sysParams.PC_GroupFieldTitleFontSize;
            TitleFontWeight = _sysParams.PC_GroupFieldTitleFontWeight;
            LabelFontSize = _sysParams.PC_GroupFieldLabelFontSize;
            LabelFontWeight = _sysParams.PC_GroupFieldLabelFontWeight;
        }
    }

    /// <summary>
    /// コンポーネント情報初期化
    /// </summary>
    /// <returns></returns>
    protected override async Task InitComponentsAsync()
    {
        await base.InitComponentsAsync();

        // ラベル幅取得
        ComponentsInfo info = _componentsInfo.Where(_ => _.ComponentName == STR_ATTRIBUTE_SEARCH && _.AttributesName == "LabelWidth").FirstOrDefault();
        if (null != info)
        {
            LabelWidth = info.Value;
        }
    }

    /// <summary>
    /// 初期化後処理
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public override async Task AfterInitializedAsync(ComponentProgramInfo info)
    {
        try
        {
            // レンダリング抑制解除
            ChildBaseService.BasePageInitilizing = false;

            //Blazor へ状態変化を通知
            StateHasChanged();

            // コンポーネント初期化
            if (txtPasswordCur != null)
            {
                txtPasswordCur.Title = "現在のパスワード";
                txtPasswordCur.MaxLength = 10;
                txtPasswordCur.Required = true;
                txtPasswordCur.ClearValue();
            }
            if (txtPasswordNew != null)
            {
                txtPasswordNew.Title = "新しいパスワード";
                txtPasswordNew.MaxLength = 10;
                txtPasswordNew.Required = true;
                txtPasswordNew.ClearValue();
            }
            if (txtPasswordNewConf != null)
            {
                txtPasswordNewConf.Title = "新しいパスワード(確認)";
                txtPasswordNewConf.MaxLength = 10;
                txtPasswordNewConf.Required = true;
                txtPasswordNewConf.ClearValue();
            }
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }

    /// <summary>
    /// 確定前処理
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public override async Task<bool> 確定前チェック(ComponentProgramInfo info)
    {
        try
        {
            // パスワード設定
            if (txtPasswordNew.InputValue != txtPasswordNewConf.InputValue)
            {
                // ダイアログ表示
                await ComService.DialogShowOK($"新しいパスワードと新しいパスワード(確認)が異なります。", pageName);
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// ストアドデータ設定_選択データ
    /// </summary>
    /// <returns></returns>
    public override async Task ストアドデータ設定_引数データ作成(ComponentProgramInfo info)
    {
        try
        {
            _storedData = info.CurrentMethodName switch
            {
                // パスワード設定
                "OnClickSettingPassword" => new Dictionary<string, object>
                {
                    ["PASSWORD_CUR"] = txtPasswordCur == null ? string.Empty : txtPasswordCur.InputValue,
                    ["PASSWORD_NEW"] = txtPasswordNew == null ? string.Empty : txtPasswordNew.InputValue,
                },
                // 検索条件保存
                "OnClickSettingSearch" => new Dictionary<string, object>
                {
                    ["CALL_CLASS_NAME"] = SettingClassName,
                },
                // 検索条件初期化
                "OnClickInitializeSearch" => new Dictionary<string, object>
                {
                    ["CALL_CLASS_NAME"] = SettingClassName,
                },
                _ => [],
            };
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }

    /// <summary>
    /// ストアドデータ設定_テーブルデータ作成
    /// </summary>
    /// <returns></returns>
    public override async Task ストアドデータ設定_テーブルデータ作成(ComponentProgramInfo info)
    {
        try
        {
            _storedTableData = [];
            if (SettingsInfo is not null && SettingsInfo.Count > 0)
            {
                foreach (UserComponentSettingsInfo settingsInfo in SettingsInfo)
                {
                    Dictionary<string, object> rowdata = new()
                    {
                        ["COMPONENT_NAME"] = settingsInfo.ComponentName,
                        ["VIEW_NAME"] = settingsInfo.ViewName,
                        ["PROPERTY_KEY"] = settingsInfo.PropertyKey,
                        ["VALUE_KEY_ID"] = settingsInfo.ValueKeyId,
                        ["VALUE"] = settingsInfo.Value,
                        ["VALUE_DATA_TYPE"] = settingsInfo.ValueDataType
                    };
                    _storedTableData.Add(rowdata);
                }
            }
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }
    #endregion

    #region private
    /// <summary>
    /// パスワード設定
    /// </summary>
    /// <returns></returns>
    private async Task OnClickSettingPassword()
    {
        await ExecProgram();
    }

    /// <summary>
    /// パスワードクリア
    /// </summary>
    /// <returns></returns>
    private async Task OnClickClearPassword()
    {
        // 画面のクリア処理
        bool? ret = await ComService.DialogShowYesNo("入力内容をクリアしますか？", pageName);
        bool retb = ret is not null && (bool)ret;
        if (retb)
        {
            txtPasswordCur?.ClearValue();
            txtPasswordNew?.ClearValue();
            txtPasswordNewConf?.ClearValue();
        }
        await Task.Delay(0);
    }

    /// <summary>
    /// 検索条件保存
    /// </summary>
    /// <returns></returns>
    private async Task OnClickSettingSearch()
    {
        await ExecProgram();
    }

    /// <summary>
    /// 検索条件初期化
    /// </summary>
    /// <returns></returns>
    private async Task OnClickInitializeSearch()
    {
        await ExecProgram();
    }
    #endregion
}
