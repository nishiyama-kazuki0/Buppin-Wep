using Microsoft.AspNetCore.Components;

namespace ExpressionDBBlazorShared.Shared;

public partial class DialogBusyContent : ComponentBase
{
    //EventConsole console;
    [Inject] private DialogService Service { get; set; } = null!;

    [Parameter]
    public string? MessageContent { get; set; } = "処理中..";

    protected override async Task OnInitializedAsync()
    {

        await base.OnInitializedAsync();
    }


}
