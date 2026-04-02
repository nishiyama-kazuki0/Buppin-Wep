using Blazored.LocalStorage;
using Blazored.SessionStorage;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ExpressionDBBlazorShared.Shared;

public partial class ButtonHTMenuRadzen : IAsyncDisposable
{
    /// <summary>
    /// メニューオブジェクトリスト
    /// </summary>
    private List<MenuInfo> MenuItems = [];

    [Parameter]
    public string ClassName { get; set; } = string.Empty;

    [Parameter]
    public bool IsTopMenu { get; set; } = false;

    private readonly CompMenuButton[] menubtn = new CompMenuButton[10];

    private int countNumber { get; set; } = 0;

    public string BottonMargin { get; set; } = "1px";
    public string FontSize { get; set; } = "200%";
    public string Fontweight { get; set; } = "bold";
    public string BackGroundColorCode { get; set; } = "#30445f";
    public string ForeColorCode { get; set; } = "#ffffff";
    public string BackGroundColorCodeForFocus { get; set; } = "#ffa500";
    public string ForeColorCodeCodeForFocus { get; set; } = "#ffffff";

    [Inject]
    private ILocalStorageService? _localStorage { get; set; }
    [Inject]
    private ISessionStorageService? _sessionStorage { get; set; }
    //Blazor.DynamicJS
    [Inject]
    protected IJSRuntime? JS { get; set; }
    public async ValueTask DisposeAsync()
    {
        await JS.InvokeVoidAsync("removeTtMenuKeyListener");
    }

    /// <summary>
    /// 初期表示イベント
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        //if (null == _sessionStorage)
        //{
        //    return;
        //}

        SystemParameterService sysParams = SystemParamService;
        if (null != sysParams)
        {
            BottonMargin = sysParams.MenuBottonMargin;
            FontSize = sysParams.MenuFontSize;
            Fontweight = sysParams.MenuFontweight;
            BackGroundColorCode = sysParams.MenuBackGroundColorCode;
            ForeColorCode = sysParams.MenuForeColorCode;
            BackGroundColorCodeForFocus = sysParams.MenuBackGroundColorCodeForFocus;
            ForeColorCodeCodeForFocus = sysParams.MenuForeColorCodeCodeForFocus;
        }

        // メニュー一覧取得
        await GetMenuItems();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            return;
        }

        await JS.InvokeVoidAsync("removeTtMenuKeyListener");
        DotNetObjectReference<ButtonHTMenuRadzen> dotNetReference = DotNetObjectReference.Create(this);
        await JS.InvokeVoidAsync("initializeTtMenuKeyListener", dotNetReference);
    }

    [JSInvokable("OnKeyDown")]
    public void OnKeyDown(int keyCode)
    {
        //数値キーが割り当てられている値の最小値 //ex:0キーがKeycode 48  ,1キーがKeycode 49
        int countNumberstart = 48;

        int judgNum = keyCode - countNumberstart;
        if (judgNum > 0 && menubtn.Length > judgNum)
        {
            if (menubtn[judgNum] != null)
            {
                _ = menubtn[judgNum].ProcKeyDown();
            }
        }
    }

    /// <summary>
    /// メニュー一覧を取得する
    /// </summary>
    /// <returns></returns>
    private async Task GetMenuItems()
    {
        MenuItems = IsTopMenu ? await ComService.GetMenuInfoListHtTopAsync() : await ComService.GetMenuInfoListAtClassAsync(ClassName);
    }
}
