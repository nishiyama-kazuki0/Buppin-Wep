using Microsoft.AspNetCore.Components;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// はい／いいえボタンダイアログ画面
/// </summary>
public partial class DialogMsgYesNoContent : ComponentBase
{
    //EventConsole console;
    [Inject] private DialogService Service { get; set; } = null!;

    [Parameter]
    public string? MessageContent { get; set; } = "確認";
    [Parameter]
    public string? ButtonYesText { get; set; } = "はい";
    [Parameter]
    public string? ButtonNoText { get; set; } = "いいえ";

    private IDictionary<string, object> AttributesFuncButton { get; set; } = new Dictionary<string, object>();

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
    }

    private void OnClickResultF1(object? sender)
    {
        Service.Close(true);
    }

    private void OnClickResultF4(object? sender)
    {
        Service.Close(false);
    }
}
