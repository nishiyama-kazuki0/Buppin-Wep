using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using SharedModels;
using ZennohBlazorShared.Data;

namespace ZennohBlazorShared.Shared
{
    /// <summary>
    /// 出荷欠品確定ダイアログ
    /// </summary>
    public partial class DialogShipmentsConfirmContent : ChildPageBasePC
    {
        private const string PROPKEY_REMARKS = "備考";

        private CompItemInfo? compRemarks;
        private ButtonFuncRadzen? ButtonFuncRadzen;

        //編集ダイアログの備考の最大入力数,行,列数
        [Parameter]
        public int DialogContentMaxlength { set; get; } = 256;
        [Parameter]
        public int DialogContentCols { set; get; } = 60;
        [Parameter]
        public int DialogContentRows { set; get; } = 3;

        [Inject]
        private ISessionStorageService? _sessionStorage { get; set; }

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
        /// 明細データ
        /// </summary>
        [Parameter]
        public List<IDictionary<string, object>> DetailData { get; set; } = new List<IDictionary<string, object>>();

        #region Override

        /// <summary>
        /// 検索条件初期化
        /// </summary>
        protected override async Task InitSearchConditionAsync()
        {
            await Task.Delay(0);

            // クリア
            _searchCompItems.Clear();

            // 備考
            compRemarks = new CompItemInfo
            {
                CompType = typeof(CompTextArea),
                CompParam = new Dictionary<string, object>
                {
                    { "Title", PROPKEY_REMARKS },
                    { "Required", false },
                    { "InitialValue", string.Empty },
                    { "MaxLength", (long)DialogContentMaxlength },
                    { "Cols", DialogContentCols },
                    { "Rows", DialogContentRows },
                    //{ "RegexPattern" , SharedConst.STR_VARIDATE_FULL_WIDTH_CHAR_ONLY}　//TODO :半角交じりでも上位連携はエラーになっていないようなので、一旦コメント化。必要な場合はコメントを戻す。
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

                // 備考セット
                if (compRemarks?.CompObj?.Instance is CompTextArea compTextArea)
                {
                    _storedData["備考"] = compTextArea.InputValue;
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// ストアドデータ設定_テーブルデータ作成
        /// </summary>
        /// <returns></returns>
        public override async Task ストアドデータ設定_テーブルデータ作成(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            try
            {
                _storedTableData = new List<Dictionary<string, object>>();

                // 明細データセット
                if (DetailData is not null)
                {
                    foreach (IDictionary<string, object> rows in DetailData)
                    {
                        Dictionary<string, object> rowdata = new();
                        if (rows.TryGetValue("出荷管理ID", out object value))
                        {
                            rowdata["出荷予定集約ID"] = value;
                        }
                        _storedTableData.Add(rowdata);
                    }
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
