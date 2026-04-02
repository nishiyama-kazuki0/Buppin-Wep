using Blazor.DynamicJS;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace ExpressionDBBlazorShared.Shared.HtTitleValue
{
    public partial class CustomHTDropDown<TValue> : CustomHTDropDownBase<TValue>, IAsyncDisposable
    {
        protected DynamicJSRuntime? _js;
        public ValueTask DisposeAsync()
        {
            return _js?.DisposeAsync() ?? ValueTask.CompletedTask;
        }
        //EventCallback
        [Parameter]
        public EventCallback<Task> OnPopupClose { get; set; }
        [Parameter]
        public EventCallback<Task> OnFocusCallback { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is read only.
        /// </summary>
        /// <value><c>true</c> if is read only; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the value template.
        /// </summary>
        /// <value>The value template.</value>
        [Parameter]
        public RenderFragment<dynamic> ValueTemplate { get; set; }

        /// <summary>
        /// Gets or sets the empty template.
        /// </summary>
        /// <value>The empty template.</value>
        [Parameter]
        public RenderFragment EmptyTemplate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether popup should open on focus. Set to <c>false</c> by default.
        /// </summary>
        /// <value><c>true</c> if popup should open on focus; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool OpenOnFocus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether search field need to be cleared after selection. Set to <c>false</c> by default.
        /// </summary>
        /// <value><c>true</c> if need to be cleared; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool ClearSearchAfterSelection { get; set; }

        /// <summary>
        /// Gets or sets the filter placeholder.
        /// </summary>
        /// <value>The filter placeholder.</value>
        [Parameter]
        public string FilterPlaceholder { get; set; } = string.Empty;

        [Parameter]
        public string DivId { get; set; } = string.Empty;

        private async Task OnFocus(FocusEventArgs args)
        {
            await OnFocusCallback.InvokeAsync();
            FocusOutFlag = false;

            if (OpenOnFocus)
            {
                await OpenPopup("Enter", false);
            }
        }

        private async Task OnFocusOut(FocusEventArgs args)
        {
            //_ = WebComService.PostLogAsync($"{UniqueID}:OnFocusOut");

            dynamic window = _js.GetWindow();
            dynamic popup = window.document.getElementById($"{PopupID}");
            string display = popup.style.display;
            // フィルターが有効の場合は、ポップアップ表示時にフィルターテキストボックスにフォーカスが当たるため
            // div自身のOnFocusOutが発生するため、フィルター無しの場合のみ処理を行うように変更
            if (display != "none" && !AllowFiltering)
            {
                //_ = WebComService.PostLogAsync($"{UniqueID}:OnFocusOut ClosePopup");
                await ClosePopup();
            }
        }

        private async Task OnClick(MouseEventArgs args)
        {
            //_ = WebComService.PostLogAsync($"{UniqueID}:OnClick");
            // KeyenceHTのブラウザでEnterを押すとOnClickが発生してしまうので処理しないようにする
            // 但し、タッチしたときにポップアップを表示する必要があるのでOnTouchEndイベントで表示させる
            //await OpenPopup("ArrowDown", false, true);
            await Task.Delay(0);
        }

        protected async Task OnTouchEnd(TouchEventArgs args)
        {
            //_ = WebComService.PostLogAsync($"{UniqueID}:OnTouchEnd");
            // ポップアップを表示する
            await OpenPopup("ArrowDown", false, true);
        }

        protected override async Task<bool> ArrowDownKeyPush()
        {
            dynamic window = _js.GetWindow();
            dynamic popup = window.document.getElementById($"{PopupID}");
            string display = popup.style.display;
            if (display == "none")
            {
                FocusOutFlag = true;
                // フィルター文字をクリアする
                _view = null;
                searchText = null;
                await OnPopupClose.InvokeAsync();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Opens the popup.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="isFilter">if set to <c>true</c> [is filter].</param>
        /// <param name="isFromClick">if set to <c>true</c> [is from click].</param>
        protected override async Task OpenPopup(string key = "ArrowDown", bool isFilter = false, bool isFromClick = false)
        {
            if (Disabled)
                return;

            //_ = WebComService.PostLogAsync($"{UniqueID}:OpenPopup key:{key}");

            await JSRuntime.InvokeVoidAsync(OpenOnFocus ? "Radzen.openPopup" : "Radzen.togglePopup", Element, PopupID, true);
            await JSRuntime.InvokeVoidAsync("Radzen.focusElement", isFilter ? UniqueID : SearchID);

            if (list != null)
            {
                await JSRuntime.InvokeVoidAsync("Radzen.selectListItem", search, list, selectedIndex);
            }

            if (isFromClick)
            {
                await Task.Delay(100);
                StateHasChanged();
            }

            dynamic window = _js.GetWindow();
            dynamic popup = window.document.getElementById($"{PopupID}");
            string display = popup.style.display;
            //_ = WebComService.PostLogAsync($"{UniqueID}:OpenPopup display:{display}");
            if (display == "none")
            {
                FocusOutFlag = true;

                // Radzenのjs処理のclosePopupでポップアップを閉じた後、100ミリ秒後にフォーカスをセットする処理を行っているため
                // 100ミリ秒待機した後にクローズイベントを発火させる
                await Task.Delay(100);
                // フィルター文字をクリアする
                _view = null;
                searchText = null;
                await OnPopupClose.InvokeAsync();
            }
        }

        internal override void RenderItem(RenderTreeBuilder builder, object item)
        {
            builder.OpenComponent(0, typeof(CustomHTDropDownItem<TValue>));
            builder.AddAttribute(1, "DropDown", this);
            builder.AddAttribute(2, "Item", item);

            if (DisabledProperty != null)
            {
                builder.AddAttribute(3, "Disabled", GetItemOrValueFromProperty(item, DisabledProperty));
            }

            builder.SetKey(GetKey(item));
            builder.CloseComponent();
        }

        /// <summary>
        /// Gets or sets the number of maximum selected labels.
        /// </summary>
        /// <value>The number of maximum selected labels.</value>
        [Parameter]
        public int MaxSelectedLabels { get; set; } = 4;

        /// <summary>
        /// Gets or sets the Popup height.
        /// </summary>
        /// <value>The number Popup height.</value>
        [Parameter]
        public string PopupStyle { get; set; } = "max-height:200px;overflow-x:hidden";

        /// <summary>
        /// Gets or sets a value indicating whether the selected items will be displayed as chips. Set to <c>false</c> by default.
        /// Requires <see cref="DropDownBase{T}.Multiple" /> to be set to <c>true</c>. 
        /// </summary>
        /// <value><c>true</c> to display the selected items as chips; otherwise, <c>false</c>.</value>
        [Parameter]
        public bool Chips { get; set; }

        /// <summary>
        /// Gets or sets the selected items text.
        /// </summary>
        /// <value>The selected items text.</value>
        [Parameter]
        public string SelectedItemsText { get; set; } = "items selected";

        /// <summary>
        /// Gets or sets the select all text.
        /// </summary>
        /// <value>The select all text.</value>
        [Parameter]
        public string SelectAllText { get; set; }

        private bool visibleChanged = false;
        private bool disabledChanged = false;
        private bool firstRender = true;

        /// <inheritdoc />
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            visibleChanged = parameters.DidParameterChange(nameof(Visible), Visible);
            disabledChanged = parameters.DidParameterChange(nameof(Disabled), Disabled);

            await base.SetParametersAsync(parameters);

            if (visibleChanged && !firstRender)
            {
                if (Visible == false)
                {
                    Dispose();
                }
            }
        }

        private bool shouldReposition;

        /// <inheritdoc />
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            this.firstRender = firstRender;

            if (firstRender)
            {
                _js = await JSRuntime.CreateDymaicRuntimeAsync();
            }

            if (firstRender || visibleChanged || disabledChanged)
            {
                visibleChanged = false;
                disabledChanged = false;

                if (Visible)
                {
                    bool reload = false;
                    if (LoadData.HasDelegate && Data == null)
                    {
                        await LoadData.InvokeAsync(await GetLoadDataArgs());
                        reload = true;
                    }

                    if (!Disabled)
                    {
                        await JSRuntime.InvokeVoidAsync("Radzen.preventArrows", Element);
                    }

                    if (reload)
                    {
                        StateHasChanged();
                    }
                }
            }

            if (shouldReposition)
            {
                shouldReposition = false;

                await JSRuntime.InvokeVoidAsync("Radzen.repositionPopup", Element, PopupID);
            }
        }

        /// <summary>
        /// Called when item is selected.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="isFromKey">if set to <c>true</c> [is from key].</param>
        protected override async Task OnSelectItem(object item, bool isFromKey = false)
        {
            if (!ReadOnly)
            {
                if (!Multiple && !isFromKey)
                {
                    await JSRuntime.InvokeVoidAsync("Radzen.closePopup", PopupID);
                }

                if (ClearSearchAfterSelection)
                {
                    await JSRuntime.InvokeAsync<string>("Radzen.setInputValue", search, string.Empty);
                    searchText = null;
                    await SearchTextChanged.InvokeAsync(searchText);
                    await OnFilter(null);
                }

                await SelectItem(item);

                dynamic window = _js.GetWindow();
                dynamic popup = window.document.getElementById($"{PopupID}");
                string display = popup.style.display;
                //_ = WebComService.PostLogAsync($"{UniqueID}:OpenPopup display:{display}");
                if (display == "none")
                {
                    FocusOutFlag = true;

                    // Radzenのjs処理のclosePopupでポップアップを閉じた後、100ミリ秒後にフォーカスをセットする処理を行っているため
                    // 100ミリ秒待機した後にクローズイベントを発火させる
                    await Task.Delay(100);
                    // フィルター文字をクリアする
                    _view = null;
                    searchText = null;
                    await OnPopupClose.InvokeAsync();
                }
            }
        }

        private async Task OnChipRemove(object item)
        {
            if (!Disabled)
            {
                await OnSelectItemInternal(item);
            }
        }

        internal async Task OnSelectItemInternal(object item, bool isFromKey = false)
        {
            await OnSelectItem(item, isFromKey);

            if (Chips)
            {
                shouldReposition = true;
            }
        }

        /// <inheritdoc />
        protected override string GetComponentCssClass()
        {
            return GetClassList("rz-dropdown")
                        .Add("rz-clear", AllowClear)
                        .Add("rz-dropdown-chips", Chips && selectedItems.Count > 0)
                        .ToString();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();

            if (IsJSRuntimeAvailable)
            {
                JSRuntime.InvokeVoidAsync("Radzen.destroyPopup", PopupID);
            }
        }

        internal async Task ClosePopup()
        {
            await JSRuntime.InvokeVoidAsync("Radzen.closePopup", PopupID);

            dynamic window = _js.GetWindow();
            dynamic popup = window.document.getElementById($"{PopupID}");
            string display = popup.style.display;
            //_ = WebComService.PostLogAsync($"{UniqueID}:OpenPopup display:{display}");
            if (display == "none")
            {
                FocusOutFlag = true;

                // Radzenのjs処理のclosePopupでポップアップを閉じた後、100ミリ秒後にフォーカスをセットする処理を行っているため
                // 100ミリ秒待機した後にクローズイベントを発火させる
                await Task.Delay(100);
                // フィルター文字をクリアする
                _view = null;
                searchText = null;
                await OnPopupClose.InvokeAsync();
            }
        }
    }
}
