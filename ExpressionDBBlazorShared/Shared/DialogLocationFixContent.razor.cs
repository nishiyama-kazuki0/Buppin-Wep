using ExpressionDBBlazorShared.Data;
using Sotsera.Blazor.Toaster.Core.Models;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// ロケーションマスタメンテナンスダイアログ
/// </summary>
public partial class DialogLocationFixContent : DialogCommonInputContent
{
    #region private変数

    /// <summary>
    /// 倉庫コードDropDownコンポーネント
    /// </summary>
    private CompDropDown? _cmbAreaCd = null;

    /// <summary>
    /// ゾーンコードDropDownコンポーネント
    /// </summary>
    private CompDropDown? _cmbZoneCd = null;

    /// <summary>
    /// 倉庫マスタ情報
    /// </summary>
    private List<MstAreaData> _lstMstArea = [];

    /// <summary>
    /// ゾーンマスタ情報
    /// </summary>
    private List<MstZoneData> _lstMstZone = [];

    private List<MstShelf> _lstMstShelf = [];

    /// <summary>
    /// 倉庫ドロップダウンデータ
    /// </summary>
    private IList<ValueTextInfo> _dropdownArea { get; set; } = [];

    /// <summary>
    /// ゾーンドロップダウンデータ
    /// </summary>
    private IList<ValueTextInfo> _dropdownZone { get; set; } = [];

    #endregion

    #region override

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // 倉庫、ゾーン情報取得
        await InitAreaZoneData();

        // Blazor へ状態変化を通知
        StateHasChanged();

        // メンバ変数、イベント関連付け
        List<List<CompItemInfo>> listItems = Mode == enumDialogMode.Edit ? _infoItems : _inputItems;
        foreach (List<CompItemInfo> listItem in listItems)
        {
            foreach (CompItemInfo item in listItem)
            {
                if (item?.CompObj?.Instance is not CompDropDown)
                {
                    continue;
                }
                if (item.TitleLabel == "倉庫コード")
                {

                    _cmbAreaCd = (CompDropDown)item.CompObj.Instance;
                    _cmbAreaCd.ChangeSelectValue += OnChangeArea;
                }
                else if (item.TitleLabel == "ゾーンコード")
                {

                    _cmbZoneCd = (CompDropDown)item.CompObj.Instance;
                }
            }
        }

        // 倉庫Dropdown初期化
        SetDropdownArea();
        // ゾーンDropdown初期化
        SetDropdownZone();
    }

    /// <summary>
    /// 終了処理
    /// </summary>
    protected override void Dispose()
    {
        // イベント削除
        if (_cmbAreaCd != null)
        {
            _cmbAreaCd.ChangeSelectValue -= OnChangeArea;
        }
    }

    #endregion

    #region private

    /// <summary>
    /// 倉庫、ゾーン、ロケーション情報を取得
    /// </summary>
    /// <returns></returns>
    private async Task InitAreaZoneData()
    {
        try
        {
            // 倉庫
            _lstMstArea = await ComService!.GetArea(true);
            // ゾーン
            _lstMstZone = await ComService!.GetZone(true);

            _lstMstShelf = await ComService!.GetShelf(true);
        }
        catch (Exception ex)
        {
            ComService!.ShowNotifyMessege(ToastType.Error, DialogTitle, $"ﾏｽﾀ値取得に失敗しました。{ex.Message}");
        }

    }

    /// <summary>
    /// 倉庫コンボボックスの初期化
    /// </summary>
    protected void SetDropdownArea()
    {
        if (_cmbAreaCd != null)
        {
            _dropdownArea.Clear();
            foreach (MstAreaData item in _lstMstArea)
            {
                ValueTextInfo info = new()
                {
                    Value = item.AreaId,
                    Text = item.AreaName,
                };
                _dropdownArea.Add(info);
            }
            _cmbAreaCd.Data = _dropdownArea;
            _cmbAreaCd.Refresh();
        }
    }

    /// <summary>
    /// ゾーンコンボボックスの初期化
    /// 西山
    /// </summary>
    protected void SetDropdownZone()
    {
        if (_cmbAreaCd != null && _cmbZoneCd != null)
        {
            _dropdownZone.Clear();

            List<MstZoneData> lstZone = _lstMstZone.Where(_ => _.AreaId == _cmbAreaCd.InputValue).ToList();
            foreach (MstZoneData item in lstZone)
            {
                ValueTextInfo info = new()
                {
                    Value = item.棚ID,
                    Text = item.ZoneName,
                };
                _dropdownZone.Add(info);
            }
            _cmbZoneCd.Data = _dropdownZone;

            // DropDownにInputValueが存在しない場合未選択
            if (0 == lstZone.Where(_ => _.棚ID == _cmbZoneCd.InputValue).ToList().Count())
            {
                _cmbZoneCd.InputValue = string.Empty;
            }

            // 再描画
            _cmbZoneCd.Refresh();
        }
    }

    /// <summary>
    /// 倉庫の選択イベント
    /// </summary>
    /// <param name="value"></param>
    private void OnChangeArea(object? sender, object e)
    {
        SetDropdownZone();
    }

    #endregion
}