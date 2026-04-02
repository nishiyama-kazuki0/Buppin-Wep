using Blazor.DynamicJS;
using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen.Blazor;

namespace ExpressionDBBlazorShared.Shared;

public partial class CustomLoginForm : RadzenComponent, IAsyncDisposable
{
    //Blazor.DynamicJS
    private DynamicJSRuntime? _js { get; set; }
    public async ValueTask DisposeAsync()
    {
        await JS.InvokeVoidAsync("removeLoginKeyListener");
        _ = _js?.DisposeAsync() ?? ValueTask.CompletedTask;
    }

    /// <summary>
    /// Gets or sets a value indicating whether automatic complete of inputs is enabled.
    /// </summary>
    /// <value><c>true</c> if automatic complete of inputs is enabled; otherwise, <c>false</c>.</value>
    [Parameter]
    public bool AutoComplete { get; set; } = true;

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
    /// Gets or sets the design variant of the form field.
    /// </summary>
    /// <value>The variant of the form field.</value>
    [Parameter]
    public Variant? FormFieldVariant { get; set; }

    /// <inheritdoc />
    protected override string GetComponentCssClass()
    {
        return "rz-login";
    }

    private string? workername;
    /// <summary>
    /// Gets or sets the usercd.
    /// </summary>
    /// <value>The username.</value>
    [Parameter]
    public string? Workername { get; set; }

    private string? username;
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    /// <value>The username.</value>
    [Parameter]
    public string? Username { get; set; }

    private string? password;
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>The password.</value>
    [Parameter]
    public string? Password { get; set; }

    private bool rememberMe;
    /// <summary> Sets the initial value of the remember me switch.</summary>
    [Parameter]
    public bool RememberMe { get; set; }

    /// <summary>
    /// Gets or sets the login callback.
    /// </summary>
    /// <value>The login callback.</value>
    [Parameter]
    public EventCallback<LoginArgs> Login { get; set; }

    /// <summary>
    /// Gets or sets the register callback.
    /// </summary>
    /// <value>The register callback.</value>
    [Parameter]
    public EventCallback Register { get; set; }

    /// <summary>
    /// Gets or sets the reset password callback.
    /// </summary>
    /// <value>The reset password callback.</value>
    [Parameter]
    public EventCallback<string> ResetPassword { get; set; }

    [Parameter]
    public EventCallback<string> UserNameChanged { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether register is allowed.
    /// </summary>
    /// <value><c>true</c> if register is allowed; otherwise, <c>false</c>.</value>
    [Parameter]
    public bool AllowRegister { get; set; } = true;

    /// <summary>
    /// Asks the user whether to remember their credentials. Set to <c>false</c> by default.
    /// </summary>
    [Parameter]
    public bool AllowRememberMe { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether reset password is allowed.
    /// </summary>
    /// <value><c>true</c> if reset password is allowed; otherwise, <c>false</c>.</value>
    [Parameter]
    public bool AllowResetPassword { get; set; } = true;

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
    public string LoginText { get; set; } = "Login";

    /// <summary>
    /// Gets or sets the register text.
    /// </summary>
    /// <value>The register text.</value>
    [Parameter]
    public string RegisterText { get; set; } = "Sign up";

    /// <summary> Gets or sets the remember me text.</summary>
    [Parameter]
    public string RememberMeText { get; set; } = "Remember me";

    /// <summary>
    /// Gets or sets the register message text.
    /// </summary>
    /// <value>The register message text.</value>
    [Parameter]
    public string RegisterMessageText { get; set; } = "Don't have an account yet?";

    /// <summary>
    /// Gets or sets the reset password text.
    /// </summary>
    /// <value>The reset password text.</value>
    [Parameter]
    public string ResetPasswordText { get; set; } = "Forgot password?";

    /// <summary>
    /// Gets or sets the user text.
    /// </summary>
    /// <value>The user text.</value>
    [Parameter]
    public string WorkerText { get; set; } = "Wokername";

    /// <summary>
    /// Gets or sets the user text.
    /// </summary>
    /// <value>The user text.</value>
    [Parameter]
    public string UserText { get; set; } = "Username";

    /// <summary>
    /// Gets or sets the user required text.
    /// </summary>
    /// <value>The user required text.</value>
    [Parameter]
    public string UserRequired { get; set; } = "Username is required";

    /// <summary>
    /// Gets or sets the password text.
    /// </summary>
    /// <value>The password text.</value>
    [Parameter]
    public string PasswordText { get; set; } = "Password";

    /// <summary>
    /// Gets or sets the password required.
    /// </summary>
    /// <value>The password required.</value>
    [Parameter]
    public string PasswordRequired { get; set; } = "Password is required";

    /// <summary>
    /// //ログインフォームのタイトルと入力欄のColumn値
    /// </summary>
    [Parameter]
    public int LoginFormTitleColumnSize { get; set; } = 5;
    [Parameter]
    public int LoginFormTextBoxColumnSize { get; set; } = 7;

    private RadzenTextBox? txtWorkername;
    private RadzenTextBox? txtUsername;
    private RadzenPassword? txtPassword;

    public string LoginFontSize { get; set; } = "120%";
    public string LoginFontWeight { get; set; } = "bold";
    public string LoginFontSizeBtn { get; set; } = "150%";
    public string LoginFontWeightBtn { get; set; } = "bold";

    private bool isBusyLoginButton = false;

    /// <summary>
    /// Called when login.
    /// </summary>
    protected async Task OnLogin()
    {
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !isBusyLoginButton)
        {
            isBusyLoginButton = true;
            await Login.InvokeAsync(new LoginArgs { Username = username, Password = password, RememberMe = rememberMe });
            isBusyLoginButton = false;
        }
    }

    private string Id(string name)
    {
        //return $"{GetId()}-{name}";
        return $"{name}-{GetId()}";
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        workername = Workername;
        username = Username;
        password = Password;
        rememberMe = RememberMe;
        base.OnInitialized();
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        SystemParameterService sysParams = SystemParamService;

        LoginFontSize = sysParams.LoginFontSize;
        LoginFontWeight = sysParams.LoginFontWeight;
        LoginFontSizeBtn = sysParams.LoginFontSizeBtn;
        LoginFontWeightBtn = sysParams.LoginFontWeightBtn;

        LoginFormTitleColumnSize = sysParams.LoginFormTitleColumnSize;
        LoginFormTextBoxColumnSize = sysParams.LoginFormTextBoxColumnSize;

    }

    /// <inheritdoc />
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.DidParameterChange(nameof(Username), Username))
        {
            username = parameters.GetValueOrDefault<string>(nameof(Username));
        }

        if (parameters.DidParameterChange(nameof(Password), Password))
        {
            password = parameters.GetValueOrDefault<string>(nameof(Password));
        }

        if (parameters.DidParameterChange(nameof(RememberMe), RememberMe))
        {
            rememberMe = parameters.GetValueOrDefault<bool>(nameof(RememberMe));
        }

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

        DotNetObjectReference<CustomLoginForm> dotNetReference = DotNetObjectReference.Create(this);
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
                    await OnLogin();
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
                await OnLogin();
                break;
        }
    }

    /// <summary>
    /// Handles the <see cref="E:Reset" /> event.
    /// </summary>
    /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected async Task OnReset(EventArgs args)
    {
        await ResetPassword.InvokeAsync(username);
    }

    /// <summary>
    /// Called when register.
    /// </summary>
    protected async Task OnRegister()
    {
        await Register.InvokeAsync(EventArgs.Empty);
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
        await UserNameChanged.InvokeAsync(value);
    }

    public void SetWorkename(string _workename)
    {
        workername = _workename;
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
}

