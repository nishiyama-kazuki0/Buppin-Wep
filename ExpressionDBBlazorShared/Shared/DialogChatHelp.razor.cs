using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// チャットページ
/// </summary>
public partial class DialogChatHelp : ChildPageBaseMobile
{
    [Inject]
    private ChatService _chatService { get; set; } = null!;

    private string _inputMessage = string.Empty;

    //TODO 暫定
    public string FontSizeLabel { get; set; } = "125%";
    public string FontSizeTextBox { get; set; } = "150%";
    public string FontWeightBold { get; set; } = "bold";

    protected override void OnInitialized()
    {
        base.OnInitialized();
        // 初回時にイベントハンドラに、イベントの紐づけ
        _chatService.StateChanged -= OnChatStateChanged;
        _chatService.StateChanged += OnChatStateChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            return;
        }

    }

    protected void SetElementIdFocus(string id)
    {
        dynamic window = _js!.GetWindow();
        dynamic element = window.document.getElementById(id);
        element?.focus(); // カーソルを合わせる
    }

    protected override void Dispose()
    {
        base.Dispose();
        // 閉じられるとき、イベントのリリース
        _chatService.StateChanged -= OnChatStateChanged;
    }

    public new async ValueTask DisposeAsync()
    {
        //await JS.InvokeVoidAsync("removeHtStepKeyListener");
        await base.DisposeAsync();
    }

    private void OnChatStateChanged(object? sender, EventArgs e)
    {
        _ = InvokeAsync(StateHasChanged);
    }


    // ChartServiceを使用して、webAPIのAIから回答をもらう
    /// <summary>
    /// F1クリックイベント
    /// </summary>
    /// <param name="sender"></param>
    private async void OnClickResultF1(object? sender)
    {
        try
        {
            // 送信処理

            //リクエストを送信する
            Task<bool> bret = _chatService.SendMessageRequest(_inputMessage);
            if (await bret)
            {
                //送信に成功すれば入力を削除
                _inputMessage = string.Empty;
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }
    /// <summary>
    /// F3クリックイベント
    /// </summary>
    /// <param name="sender"></param>
    private void OnClickResultF3(object? sender)
    {
        try
        {
            // メッセージクリア
            _chatService.ClearMessages();
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }
    /// <summary>
    /// F4クリックイベント
    /// </summary>
    /// <param name="sender"></param>
    private void OnClickResultF4(object? sender)
    {
        try
        {
            // 閉じる
            DialogService.CloseSide();
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }

}
