namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// 通知一覧
/// </summary>
public partial class DialogAlarmInfo : ChildPageBasePC
{
    #region private

    /// <summary>
    /// F1クリックイベント
    /// </summary>
    /// <param name="sender"></param>
    private async Task OnClickResultF1(object? sender)
    {
        await ExecProgram();
    }

    /// <summary>
    /// F2クリックイベント
    /// </summary>
    /// <param name="sender"></param>
    private async Task OnClickResultF2(object? sender)
    {
        await ExecProgram();
    }

    /// <summary>
    /// F4クリックイベント
    /// </summary>
    /// <param name="sender"></param>
    private async Task OnClickResultF4(object? sender)
    {
        await ExecProgram();
    }

    /// <summary>
    /// OnCellRenderのCallBack
    /// </summary>
    /// <param name="args"></param>
    private new void CellRender(DataGridCellRenderEventArgs<IDictionary<string, object>> args)
    {
        try
        {
            if ("区分名" == args.Column.Title)
            {
                // NOTIFY_CATEGORYの値によって区分の背景色変更
                if (args.Data.TryGetValue("NOTIFY_CATEGORY", out object? value))
                {
                    ComService.AddAttrResultCatgory(value?.ToString(), args.Attributes);
                }
            }
        }
        catch (Exception)
        {
        }
    }

    #endregion
}
