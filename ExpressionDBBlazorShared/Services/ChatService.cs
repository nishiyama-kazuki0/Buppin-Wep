using SharedModels;

namespace ExpressionDBBlazorShared.Services;

public class ChatService
{
    /// <summary>
    /// チャットメッセージを保持する
    /// </summary>
    private List<ChatMessageModel> _messages { get; set; } = [];

    /// <summary>
    /// 任意のイベントを実行するイベントハンドラ
    /// </summary>
    public event EventHandler StateChanged;

    /// <summary>
    /// CommonWebComService
    /// </summary>
    private readonly CommonWebComService _webComService;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ChatService(CommonWebComService commonWebComService)
    {
        _webComService = commonWebComService;
    }

    /// <summary>
    /// チャットメッセージを取得する
    /// </summary>
    /// <returns></returns>
    public List<ChatMessageModel> GetMessagesList()
    {
        return _messages;
    }

    /// <summary>
    /// チャットメッセージを追加する
    /// </summary>
    /// <param name="str"></param>
    private void AddMessage(ChatMessageModel str)
    {
        _messages.Add(str);

        // チャットメッセージが追加されたら、イベントを実行する
        StateHasChanged();
    }

    /// <summary>
    /// チャットメッセージをクリアする
    /// </summary>
    public void ClearMessages()
    {
        _messages.Clear();

        // チャットメッセージがクリアされたら、イベントを実行する
        StateHasChanged();
    }

    /// <summary>
    /// イベントを実行する
    /// </summary>
    private void StateHasChanged()
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// 現在のすべてのやり取りを一つの文字列にして返す。//現在未使用
    /// </summary>
    /// <returns></returns>
    public string GetJoinedAllMessages()
    {
        return string.Join("\n", _messages.Select(_ => _.Message));
    }
    /// <summary>
    /// メッセージを送信して受信を追加する
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task<bool> SendMessageRequest(string message)
    {
        //TODO 文字数制限(トークン制限)を超えている場合はエラーにする

        await Task.Delay(0);
        // メッセージを送信する処理を追加する
        AddMessage(new ChatMessageModel()
        {
            Message = message,
            Time = DateTime.Now,
            Type = ChatSendAndReceiveType.UserInput
        });
        //返りを受信する
        //TODO 別スレッドにしてリストへの追加はスレッドセーフに行う
        RequestValue rv
            = RequestValue.CreateRequestProgram("HelpRequest")
            .SetArgumentValue("prompt", _messages, "");//AIはすねての文脈を考慮して回答するため、すべてのメッセージ文字列を送信する
        ExecResult[]? results = await _webComService.SetRequestValue(GetType().Name, rv);
        if (results != null && results.Length > 0)
        {
            List<ExecResult> lstResult = new(results);
            //Assistantからの回答を追加する。
            AddMessage(new ChatMessageModel()
            {
                Message = lstResult[0].Message, //TODO 最初のメッセージのみの検討でよいか
                Time = DateTime.Now,
                Type = ChatSendAndReceiveType.AssistantInput
            });
        }
        else
        {
            return false;
        }

        return true;
    }

}
