using Blazor.DynamicJS;
using Blazored.SessionStorage;
using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ExpressionDBBlazorShared.Shared;

public partial class CompMenuButton
{
    [Parameter]
    public required string Text { get; set; }
    [Parameter]
    public required string Path { get; set; }
    [Parameter]
    public required string Icon { get; set; }
    [Parameter]
    public required string style { get; set; }

    /// <summary>
    /// ボタンの背景色コード
    /// レンダリング用
    /// </summary>
    [Parameter]
    public required string BackGroundColorCode { get; set; } = "#30445f";

    /// <summary>
    /// ボタンのテキスト色コード
    /// レンダリング用
    /// </summary>
    [Parameter]
    public required string ForeColorCode { get; set; } = "#ffffff";

    ///// <summary>
    ///// ボタンの背景色コード
    ///// 元の色保持用
    ///// </summary>
    //private string _backGroundColorCodeOriginal = null!;
    ///// <summary>
    ///// ボタンのテキスト色コード
    ///// 元の色保持用
    ///// </summary>
    //private string _foreColorCodeOriginal = null!;

    /// <summary>
    /// ボタンの背景色コード
    /// フォーカス用
    /// </summary>
    [Parameter]
    public required string BackGroundColorCodeForFocus { get; set; } = "#ffa500";

    /// <summary>
    /// ボタンのテキスト色コード
    /// フォーカス用
    /// </summary>
    [Parameter]
    public required string ForeColorCodeCodeForFocus { get; set; } = "#ffffff";

    /// <summary>
    /// レンダリングNo
    /// 別Razorでレンダリングされたボタンを区別するために使用する
    /// </summary>
    [Parameter]
    public required int RenderNum { get; set; }

    /// <summary>
    /// //HTメニューボタンの幅の大きさ
    /// </summary>
    [Parameter]
    public string HT_MenuBottonWidth { get; set; } = "100%";

    private bool isKeyDown = false;

    [Inject]
    private ISessionStorageService? _sessionStorage { get; set; }
    //Blazor.DynamicJS
    private DynamicJSRuntime? _js;
    public ValueTask DisposeAsync()
    {
        return _js?.DisposeAsync() ?? ValueTask.CompletedTask;
    }

    protected override void OnInitialized()
    {
        ////元の色を保持する
        //_backGroundColorCodeOriginal = BackGroundColorCode;
        //_foreColorCodeOriginal = ForeColorCode;

        base.OnInitialized();
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(0);


        SystemParameterService sysParams = SystemParamService;

        HT_MenuBottonWidth = sysParams.HT_MenuBottonWidth;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _js = await JS.CreateDymaicRuntimeAsync();
    }

    /// <summary>
    /// ボタンクリック用処理
    /// </summary>
    /// <param name="args"></param>
    private async Task ButtonClick(MouseEventArgs? args)
    {
        // キーダウン
        await ProcKeyDown();
    }

    /// <summary>
    /// キーダウン時用処理
    /// </summary>
    /// <param name="args"></param>
    public async Task ProcKeyDown()
    {
        if (string.IsNullOrWhiteSpace(Path))
        {
            return;
        }
        if (isKeyDown)
        {
            return;
        }

        try
        {
            isKeyDown = true;

            //ボタンの色を変更する
            BackGroundColorCode = BackGroundColorCodeForFocus;
            ForeColorCode = ForeColorCodeCodeForFocus;

            //コンポーネントを再レンダリングする
            StateHasChanged();

            //200ms待たせる
            await Task.Delay(200);

            //次画面に遷移する
            NavigationManager.NavigateTo(Path);
        }
        finally
        {
            isKeyDown = false;
        }
    }
}
