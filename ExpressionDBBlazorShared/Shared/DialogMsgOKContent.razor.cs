using Microsoft.AspNetCore.Components;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// OKボタンダイアログ画面
/// </summary>
public partial class DialogMsgOKContent : ComponentBase
{
    [Inject]
    private DialogService Service { get; set; } = null!;

    [Parameter]
    public string? MessageContent { get; set; } = "確認";
    [Parameter]
    public string? ButtonOKText { get; set; } = "OK";

    private IDictionary<string, object> AttributesFuncButton { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        AttributesFuncButton = new Dictionary<string, object>(){
            { "button1text", ButtonOKText },
            { "button2text", string.Empty },
            { "button3text", string.Empty },
            { "button4text", string.Empty },
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
    }

    private void OnClickResultF1(object? sender)
    {
        Service.Close(true);
    }
}
