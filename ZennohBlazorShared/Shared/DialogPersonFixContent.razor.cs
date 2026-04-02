namespace ZennohBlazorShared.Shared
{
    /// <summary>
    /// 作業者マスタメンテナンスダイアログ
    /// </summary>
    public partial class DialogPersonFixContent : DialogCommonInputContent
    {
        protected override async Task OnInitializedAsync()
        {
            if (Mode == enumDialogMode.Edit)
            {
                // 編集の時はパスワードを非表示にする
                Components = Components.Where(_ => _.Property != "パスワード").ToList();
            }

            await base.OnInitializedAsync();
        }
    }
}