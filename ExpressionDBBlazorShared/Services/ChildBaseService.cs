namespace ExpressionDBBlazorShared.Services;

public class ChildBaseService//西山
{
    internal bool BasePageInitilizing = false;


    internal bool IsHandy { get; set; } = false;
    /// <summary>
    /// ファンクション処理中にバーコード読取を制御するためのフラグ
    /// </summary>
    internal bool IsFuncProc { get; set; } = false;
    /// <summary>
    /// 最後にフォーカスが当たっていたElementのID
    /// </summary>
    internal string LastFocusId { get; set; } = string.Empty;

    //バリデーションサマリーメッセージのフォントサイズとフォント幅。ページごとにシステムパラメータを読みだして、ValidationSammaryにセットするような実装は避けたいので、
    //アプリシステムパラメータ読み出し直後にセットするとする
    internal string FontSizeValidationSammary { get; set; } = "100%";
    internal string FontWeightValidationSammary { get; set; } = "normal";
    internal string StyleValidationSammary => $"font-size:{FontSizeValidationSammary};font-weight:{FontWeightValidationSammary};";
    #region Event

    public event Action<object, EventArgs>? EventChangeChild;

    public event Func<object, EventArgs, Task>? EventMainLayoutF1;
    public event Func<object, EventArgs, Task>? EventMainLayoutF2;
    public event Func<object, EventArgs, Task>? EventMainLayoutF3;
    public event Func<object, EventArgs, Task>? EventMainLayoutF4;
    public event Func<object, EventArgs, Task>? EventMainLayoutF5;
    public event Func<object, EventArgs, Task>? EventMainLayoutF6;
    public event Func<object, EventArgs, Task>? EventMainLayoutF7;
    public event Func<object, EventArgs, Task>? EventMainLayoutF8;
    public event Func<object, EventArgs, Task>? EventMainLayoutF9;
    public event Func<object, EventArgs, Task>? EventMainLayoutF10;
    public event Func<object, EventArgs, Task>? EventMainLayoutF11;
    public event Func<object, EventArgs, Task>? EventMainLayoutF12;
    public event Func<object, EventArgs, Task>? EventMainLayoutHtNotify;
    public event Func<object, EventArgs, Task>? EventMainLayoutHtHomeNavigate;
    public event Func<object, EventArgs, Task>? EventMainLayoutPageUp;
    public event Func<object, EventArgs, Task>? EventMainLayoutPageDown;

    public event Func<object, EventArgs, Task>? EventMainLayoutUserSetting;

    public void EventCangeChildAsync(object sender)
    {
        EventChangeChild?.Invoke(sender, EventArgs.Empty);
    }

    public async Task EventMainLayoutF1ClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutF1?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutF2ClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutF2?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutF3ClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutF3?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutF4ClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutF4?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutF5ClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutF5?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutF6ClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutF6?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutF7ClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutF7?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutF8ClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutF8?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutF9ClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutF9?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutF10ClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutF10?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutF11ClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutF11?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutF12ClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutF12?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutHtNotifyClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutHtNotify?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutHtHomeNavigateClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutHtHomeNavigate?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutPageUpClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutPageUp?.Invoke(sender, EventArgs.Empty));
    }
    public async Task EventMainLayoutPageDownClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutPageDown?.Invoke(sender, EventArgs.Empty));
    }

    public async Task EventMainLayoutUserSettingClickAsync(object sender)
    {
        await Task.Run(() => EventMainLayoutUserSetting?.Invoke(sender, EventArgs.Empty));
    }

    #endregion
}
