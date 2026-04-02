using Blazored.SessionStorage;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using SharedModels;
using ZennohBlazorShared.Data;

namespace ZennohBlazorShared.Shared
{
    /// <summary>
    /// 作業実績備考編集ダイアログ
    /// </summary>
    public partial class DialogProductivityDifferenceContent : ChildPageBasePC
    {
        private const string PROPKEY_REMARKS = "備考";

        private CompItemInfo? compRemarks;
        private ButtonFuncRadzen? ButtonFuncRadzen;

        //編集ダイアログの最大文字数,行,列数
        [Parameter]
        public int DialogContentMaxlength { set; get; } = 256;
        [Parameter]
        public int DialogContentCols { set; get; } = 60;
        [Parameter]
        public int DialogContentRows { set; get; } = 3;

        /// <summary>
        /// 明細データ
        /// </summary>
        [Parameter]
        public List<IDictionary<string, object>> DetailData { get; set; } = new List<IDictionary<string, object>>();

        [Inject]
        private ISessionStorageService? _sessionStorage { get; set; }

        #region Override

        protected override async Task OnInitializedAsync()
        {
            if (null == _sessionStorage)
            {
                return;
            }
            SystemParameter sysParams = await _sessionStorage.GetItemAsync<SystemParameter>(SharedConst.KEY_SYSTEM_PARAM);
            if (null != sysParams)
            {
                DialogContentMaxlength = sysParams.DialogContentMaxlength;
                DialogContentCols = sysParams.DialogContentCols;
                DialogContentRows = sysParams.DialogContentRows;
            }

            await base.OnInitializedAsync();
        }

        /// <summary>
        /// 検索条件初期化
        /// </summary>
        protected override async Task InitSearchConditionAsync()
        {
            await Task.Delay(0);

            // クリア
            _searchCompItems.Clear();

            // 初期値取得
            string strInitRemarks = string.Empty;
            if (DetailData[0].TryGetValue(PROPKEY_REMARKS, out object? value))
            {
                strInitRemarks = value.ToString();
            }

            // 備考
            compRemarks = new CompItemInfo
            {
                CompType = typeof(CompTextArea),
                CompParam = new Dictionary<string, object>
                {
                    { "Title", PROPKEY_REMARKS },
                    { "Required", false },
                    { "InitialValue", strInitRemarks },
                    { "MaxLength", (long)DialogContentMaxlength },
                    { "Cols", DialogContentCols },
                    { "Rows", DialogContentRows },
                    //{ "RegexPattern" , SharedConst.STR_VARIDATE_FULL_WIDTH_CHAR_ONLY} //TODO :半角交じりでも上位連携はエラーになっていないようなので、一旦コメント化。必要な場合はコメントを戻す。
                },
                TitleLabel = PROPKEY_REMARKS,
                DispTitleLabel = true,
                DispRequiredSuffix = false,
            };

            // コンポーネント情報追加
            _searchCompItems.Add(new List<CompItemInfo> { compRemarks });
        }

        /// <summary>
        /// ストアドデータ設定_引数データ作成
        /// </summary>
        /// <returns></returns>
        public override async Task ストアドデータ設定_引数データ作成(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            try
            {
                _storedData = new Dictionary<string, object>();

                // 管理IDセット
                if (DetailData[0].TryGetValue("管理ID", out object? value))
                {
                    _storedData["管理_ID"] = value;
                }
                // WORK_CATEGORYセット
                if (DetailData[0].TryGetValue("WORK_CATEGORY", out value))
                {
                    _storedData["WORK_CATEGORY"] = value;
                }
                // 備考セット
                if (compRemarks?.CompObj?.Instance is CompTextArea compTextArea)
                {
                    _storedData["備考"] = compTextArea.InputValue;
                }
                // 入荷Noセット
                if (DetailData[0].TryGetValue("入荷No", out value))
                {
                    _storedData["入荷No"] = value;
                }
                // 明細Noセット
                if (DetailData[0].TryGetValue("明細No", out value))
                {
                    _storedData["明細No"] = value;
                }
                // 出荷予定集約IDセット
                if (DetailData[0].TryGetValue("出荷予定集約ID", out value))
                {
                    _storedData["出荷予定集約ID"] = value;
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// 確定後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task 確定後処理(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            // ビジーダイアログを先にCloseさせる
            DialogService.Close();
            ButtonFuncRadzen.SetIsBusyDialogClose(false);

            DialogService.Close(0);
        }

        #endregion

        #region private

        /// <summary>
        /// F1ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private async Task OnClickResultF1(object? sender)
        {
            await ExecProgram();
        }

        /// <summary>
        /// F4ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        private async Task OnClickResultF4(object? sender)
        {
            await ExecProgram();
        }

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        private bool CheckInput()
        {
            if (editForm is null)
            {
                return true;
            }

            //EditContextのValidate()メソッドを実行することでSubmitと同等のイベントが発火
            return editForm!.EditContext.Validate();
        }

        #endregion
    }
}
