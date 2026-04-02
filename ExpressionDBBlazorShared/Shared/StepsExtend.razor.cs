using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// ステップ画面用UIコンポーネント
/// </summary>
public partial class StepsExtend : ComponentBase
{
    #region パラメータ
    /// <summary>
    /// ステップ画面用クラスリスト
    /// </summary>
    [Parameter]
    public List<StepItemInfo> StepItems { get; set; } = [];
    /// <summary>
    /// ステップ画面で使用するViewModel
    /// </summary>
    [Parameter]
    public BaseViewModel StepItemVm { get; set; } = new BaseViewModel();
    /// <summary>
    /// メインページ
    /// </summary>
    [Parameter]
    public required ChildPageBase Parent { get; set; }

    #endregion

    private string stepUlDisplay = string.Empty;
    private string stepTitleDisplay = string.Empty;
    private string stepNumberDisplay = string.Empty;

    /// <summary>
    /// システム設定
    /// </summary>
    protected SystemParameterService _sysParams => SystemParamService;

    // RadzenStepsのSteps
    private RenderFragment? steps;
    public required RadzenSteps radzenSteps;
    public bool InitStep = false;

    /// <summary>
    /// ステップ位置
    /// </summary>
    /// <returns></returns>
    public int SelectedIndex => radzenSteps.SelectedIndex;

    #region OnInitializedAsync
    /// <summary>
    /// 初期処理
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {

        stepUlDisplay = _sysParams?.StepUlDisplay ?? string.Empty;
        stepNumberDisplay = _sysParams?.StepNumberDisplay ?? string.Empty;
        stepTitleDisplay = _sysParams?.StepTitleDisplay ?? string.Empty;

        steps = new RenderFragment(builder =>
        {
            int seq = 0;
            for (int i = 0; StepItems.Count > i; i++)
            {
                int ii = i;
                builder.OpenComponent<RadzenStepsItem>(seq++);
                builder.AddAttribute(seq++, "Text", StepItems[i].Title);
                builder.AddAttribute(seq++, "ChildContent", new RenderFragment(builder2 =>
                {
                    builder2.OpenComponent(0, StepItems[ii].StepItem.GetType());
                    builder2.AddAttribute(1, "StepsExtend", this);
                    builder2.CloseComponent();
                }));
                builder.CloseComponent();
            }
        });

        await base.OnInitializedAsync();
    }
    #endregion

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (!InitStep && radzenSteps.StepsCollection.Count > 0)
        {
            InitStep = true;

            // 初回はSelectIndex以外のステップを無効にする
            for (int i = 0; radzenSteps.StepsCollection.Count > i; i++)
            {
                radzenSteps.StepsCollection[i].Disabled = radzenSteps.SelectedIndex != i;
            }
        }

        return base.OnAfterRenderAsync(firstRender);
    }

    #region OnChange
    private void OnChange(int stepNo)
    {
        // 表示中のステップ以外を無効にする
        for (int i = 0; radzenSteps.StepsCollection.Count > i; i++)
        {
            radzenSteps.StepsCollection[i].Disabled = radzenSteps.SelectedIndex != i;
        }
    }
    #endregion

    #region 公開メソッド
    /// <summary>
    /// 前のステップへ戻る
    /// </summary>
    /// <returns></returns>
    public async Task PrevStep()
    {
        // ステップ遷移するために一旦すべてのステップを有効にする
        for (int i = 0; radzenSteps.StepsCollection.Count > i; i++)
        {
            radzenSteps.StepsCollection[i].Disabled = false;
        }
        await radzenSteps.PrevStep();
    }

    /// <summary>
    /// 次のステップへ移動
    /// </summary>
    /// <returns></returns>
    public async Task NextStep()
    {
        // ステップ遷移するために一旦すべてのステップを有効にする
        for (int i = 0; radzenSteps.StepsCollection.Count > i; i++)
        {
            radzenSteps.StepsCollection[i].Disabled = false;
        }
        await radzenSteps.NextStep();
    }

    /// <summary>
    /// 次のステップへ移動
    /// </summary>
    /// <returns></returns>
    public async Task SetStep(int nStep)
    {
        // ステップ遷移するために一旦すべてのステップを有効にする
        for (int i = 0; radzenSteps.StepsCollection.Count > i; i++)
        {
            radzenSteps.StepsCollection[i].Disabled = false;
        }
        radzenSteps.SelectedIndex = nStep;
        await radzenSteps.Change.InvokeAsync();
    }
    #endregion
}

/// <summary>
/// ステップ用データクラス
/// </summary>
public class StepItemInfo
{
    // ステップタイトル文字列
    public string Title { get; set; } = "";
    // ステップ用UIコンポーネント
    public StepItemBase StepItem { get; set; } = new StepItemBase();
}
