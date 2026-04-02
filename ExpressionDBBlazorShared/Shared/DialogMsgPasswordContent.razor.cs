using Blazor.DynamicJS;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Microsoft.Win32;
using Radzen.Blazor;
using Sotsera.Blazor.Toaster.Core.Models;
using System.ComponentModel;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// パスワード確認ダイアログ画面
/// </summary>
public partial class DialogMsgPasswordContent : RadzenComponent, IAsyncDisposable
{
    //Blazor.DynamicJS
    private DynamicJSRuntime? _js { get; set; }
    public async ValueTask DisposeAsync()
    {
        await JS.InvokeVoidAsync("removeLoginKeyListener");
        _ = _js?.DisposeAsync() ?? ValueTask.CompletedTask;
    }

    //EventConsole console;
    [Inject] private DialogService Service { get; set; } = null!;

    [Parameter]
    public string? MessageContent { get; set; } = "確認";

    [Parameter]
    public string? ButtonYesText { get; set; } = "はい";
    [Parameter]
    public string? ButtonNoText { get; set; } = "いいえ";

    /// <summary>
    /// Gets or sets a value indicating whether automatic complete of inputs is enabled.
    /// </summary>
    /// <value><c>true</c> if automatic complete of inputs is enabled; otherwise, <c>false</c>.</value>
    [Parameter]
    public bool AutoComplete { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating the type of built-in autocomplete
    /// the browser should use.
    /// <see cref="Blazor.AutoCompleteType" />
    /// </summary>
    /// <value>
    /// The type of built-in autocomplete.
    /// </value>
    [Parameter]
    public AutoCompleteType UserNameAutoCompleteType { get; set; } = AutoCompleteType.Username;

    /// <summary>
    /// Gets or sets a value indicating the type of built-in autocomplete
    /// the browser should use.
    /// <see cref="Blazor.AutoCompleteType" />
    /// </summary>
    /// <value>
    /// The type of built-in autocomplete.
    /// </value>
    [Parameter]
    public AutoCompleteType PasswordAutoCompleteType { get; set; } = AutoCompleteType.CurrentPassword;

    /// <summary>
    /// ログイン画面と異なり単体で完結するように変更
    /// </summary>
    private string? workername;
    private string? username;
    private string? password;

    /// <summary>
    /// Gets or sets a value indicating whether default login button is shown.
    /// </summary>
    /// <value><c>true</c> if default login button is shown; otherwise, <c>false</c>.</value>
    [Parameter]
    public bool ShowLoginButton { get; set; } = true;

    /// <summary>
    /// Gets or sets the login text.
    /// </summary>
    /// <value>The login text.</value>
    [Parameter]
    public string LoginText { get; set; } = "ログイン";

    /// <summary>
    /// Gets or sets the user text.
    /// </summary>
    /// <value>The user text.</value>
    [Parameter]
    public string WorkerText { get; set; } = "作業者名";

    /// <summary>
    /// Gets or sets the user text.
    /// </summary>
    /// <value>The user text.</value>
    [Parameter]
    public string UserText { get; set; } = "作業者コード";

    /// <summary>
    /// Gets or sets the user required text.
    /// </summary>
    /// <value>The user required text.</value>
    [Parameter]
    public string UserRequired { get; set; } = "作業者ｺｰﾄﾞは必須です。";

    /// <summary>
    /// Gets or sets the password text.
    /// </summary>
    /// <value>The password text.</value>
    [Parameter]
    public string PasswordText { get; set; } = "パスワード";

    /// <summary>
    /// Gets or sets the password required.
    /// </summary>
    /// <value>The password required.</value>
    [Parameter]
    public string PasswordRequired { get; set; } = "ﾊﾟｽﾜｰﾄﾞは必須です。";

    /// <summary>
    /// //ログインフォームのタイトルと入力欄のColumn値
    /// </summary>
    [Parameter]
    public int LoginFormTitleColumnSize { get; set; } = 5;
    [Parameter]
    public int LoginFormTextBoxColumnSize { get; set; } = 7;

    /// <summary>
    /// チェックを解除できる権限レベル(レベル以上のユーザーをOKとする)
    /// 初期値はユーザー管理者 = 5以上の権限が必要
    /// </summary>
    [Parameter]
    public int UserAuthorityLevel { get; set; } = 5;

    private RadzenTextBox? txtWorkername;
    private RadzenTextBox? txtUsername;
    private RadzenPassword? txtPassword;

    public string LoginFontSize { get; set; } = "120%";
    public string LoginFontWeight { get; set; } = "bold";
    public string LoginFontSizeBtn { get; set; } = "150%";
    public string LoginFontWeightBtn { get; set; } = "bold";

    private bool isBusyLoginButton = false;

    private string Id(string name)
    {
        //return $"{GetId()}-{name}";
        return $"{name}-{GetId()}";
    }

    private IDictionary<string, object> AttributesFuncButton { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// ログインユーザー情報
    /// </summary>
    LoginInfo[]? allUser = null;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        AttributesFuncButton = new Dictionary<string, object>(){
            { "button1text", ButtonYesText },
            { "button2text", string.Empty },
            { "button3text", string.Empty },
            { "button4text", ButtonNoText },
            { "button5text", string.Empty },
            { "button6text", string.Empty },
            { "button7text", string.Empty },
            { "button8text", string.Empty },
            { "button9text", string.Empty },
            { "button10text", string.Empty },
            { "button11text", string.Empty },
            { "button12text", string.Empty},
            { "IsBusyDialog", false }
        };

        await base.OnInitializedAsync();

        SystemParameterService sysParams = SystemParamService;

        LoginFontSize = sysParams.LoginFontSize;
        LoginFontWeight = sysParams.LoginFontWeight;
        LoginFontSizeBtn = sysParams.LoginFontSizeBtn;
        LoginFontWeightBtn = sysParams.LoginFontWeightBtn;

        LoginFormTitleColumnSize = sysParams.LoginFormTitleColumnSize;
        LoginFormTextBoxColumnSize = sysParams.LoginFormTextBoxColumnSize;

        // ユーザ情報を取得する
        _ = InvokeAsync(async () =>
        {
            allUser = await ComService.GetLoginInfoAsync();
        });
    }

    /// <inheritdoc />
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (!firstRender)
        {
            return;
        }

        _js = await JS.CreateDymaicRuntimeAsync();

        DotNetObjectReference<DialogMsgPasswordContent> dotNetReference = DotNetObjectReference.Create(this);
        await JS.InvokeVoidAsync("initializeLoginKeyListener", dotNetReference);

        // 画面表示時はユーザーIDにフォーカスを合わせる
        _js.GetWindow().focus();
        await txtUsername.Element.FocusAsync();
    }

    [JSInvokable("OnKeyDown")]
    public async Task OnKeyDownAsync(int keyCode, string activeId)
    {
        switch (keyCode)
        {
            case 13:
                if (!DeviceInfo.IsHandy() && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !isBusyLoginButton)
                {
                    // PCの場合ログイン画面でEnterキーが押下された場合、ユーザーとパスワードが両方入力されていればログイン処理を実行する。
                    await Login();
                }
                else
                {
                    // ログイン画面でEnterキーが押下された場合、最後にフォーカスが当たっていたElementにフォーカスを戻す
                    if (!string.IsNullOrEmpty(ChildBaseService.LastFocusId) && !activeId.Contains(ChildBaseService.LastFocusId))
                    {
                        if (_js != null)
                        {
                            dynamic window = _js!.GetWindow();
                            dynamic element = window.document.getElementById(ChildBaseService.LastFocusId);
                            element?.focus();
                        }
                    }
                }
                break;
            case 112:
                await Login();
                break;
        }
    }

    public async void UsernameEnter(KeyboardEventArgs e)
    {
        if (e.Key is "Enter")
        {
            await txtPassword.Element.FocusAsync();
        }
    }

    public async void PasswordEnter(KeyboardEventArgs e)
    {
        if (e.Key is "Enter")
        {
            await txtUsername.Element.FocusAsync();
        }
    }

    private async void OnUsernameChanged(string value)
    {
        await GetWorkerName();
    }

    /// <summary>
    /// UsernameのOnFocusイベント
    /// </summary>
    /// <param name="e"></param>
    private async void OnUsernameFocus(FocusEventArgs e)
    {
        await Task.Delay(0);
        // 最終フォーカスID更新
        ChildBaseService.LastFocusId = Id("username");

        // _jsを使用する方法では、画面遷移時初回は_jsがnullのためjavascriptコードを呼び出す方法とする
        await JS.InvokeVoidAsync("selectText", Id("username"));
    }

    /// <summary>
    /// PasswordのOnFocusイベント
    /// </summary>
    /// <param name="e"></param>
    private async void OnPasswordFocus(FocusEventArgs e)
    {
        await Task.Delay(0);
        // 最終フォーカスID更新
        ChildBaseService.LastFocusId = Id("password");

        // _jsを使用する方法では、画面遷移時初回は_jsがnullのためjavascriptコードを呼び出す方法とする
        await JS.InvokeVoidAsync("selectText", Id("password"));
    }

    protected　async Task GetWorkerName()
    {
        IEnumerable<LoginInfo>? getUser = allUser?.Where(_ =>
            _.Id == username
            && _.AuthorityLevel >= UserAuthorityLevel
        );
        string? userName = getUser is not null && getUser.Any() ? getUser.First().UserName : string.Empty;
        workername = userName;
    }

    /// <summary>
    /// ログイン処理
    /// </summary>
    /// <returns></returns>
    protected async Task Login()
    {
        if (allUser == null || allUser.Count() <= 0)
        {
            allUser = await ComService.GetLoginInfoAsync();
        }

        LoginInfo? login = allUser.FirstOrDefault(_ =>
            _.Id == username
            && _.Password == password
            && _.AuthorityLevel >= UserAuthorityLevel
        );
        if (login is null)
        {
            // ユーザーが取得できなかったためNG
            ComService!.ShowNotifyMessege(ToastType.Error, "ログイン失敗", "ユーザー取得に失敗しました。");
            return;
        }

        Service.Close(true);
    }

    private async void OnClickResultF1(object? sender)
    {
        await Login();
    }

    private void OnClickResultF4(object? sender)
    {
        Service.Close(false);
    }
}
