using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionDBBlazorShared.Validator
{
    public abstract class FieldValidatorBase<TItem> : ComponentBase, IDisposable
    {
        private PropertyInfo? _propertyInfoCache;

        [CascadingParameter]
        protected EditContext? CurrentEditContext { get; private set; }

        private EditContext? _previousEditContext;

        [Parameter]
        [EditorRequired]
        public Expression<Func<TItem>>? For { get; set; }

        [Parameter]
        [EditorRequired]
        public string? ItemName { get; set; }


        [Parameter]
        public string? Message { get; set; } = string.Empty;

        /// <summary>
        /// エラーメッセージのフォーマットは継承先で定義する
        /// </summary>
        public abstract string? ErrorMessage { get; }
        public string? ErrorMessage2 { get; set; } = string.Empty;

        private FieldIdentifier Field { get; set; }

        private ValidationMessageStore? _messageStore;
        private bool _disposedValue;

        protected override void OnParametersSet()
        {
            // 必須パラメーターチェック
            if (CurrentEditContext is null)
            {
                throw new InvalidOperationException($"{nameof(FieldValidatorBase<TItem>)} requires a cascading parameter of type {nameof(EditContext)}.");
            }

            if (For is null)
            {
                throw new InvalidOperationException($"{nameof(For)} requires.");
            }

            if (string.IsNullOrWhiteSpace(ErrorMessage))
            {
                throw new InvalidOperationException($"{nameof(ErrorMessage)} requires.");
            }

            if (CurrentEditContext != _previousEditContext)
            {
                // バリデーションに必要なオブジェクトの初期化
                DetachEditContext();
                CurrentEditContext.OnValidationRequested += EditContext_OnValidationRequested;
                CurrentEditContext.OnFieldChanged += EditContext_OnFieldChanged;
                _messageStore = new(CurrentEditContext);

                _previousEditContext = CurrentEditContext;
            }

            // FieldIdentifier の初期化
            Field = FieldIdentifier.Create(For);
        }

        private void DetachEditContext()
        {
            // イベントハンドラー等の切り離し
            if (_previousEditContext is not null)
            {
                _previousEditContext.OnValidationRequested -= EditContext_OnValidationRequested;
                _previousEditContext.OnFieldChanged -= EditContext_OnFieldChanged;

                _messageStore = null;
            }
        }

        private void EditContext_OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            // Validation を要求されたら無条件にバリデーションを実行
            Validate();
        }

        private void EditContext_OnFieldChanged(object? sender, FieldChangedEventArgs e)
        {
            // 自分の監視対象のフィールドならバリデーションを実行
            if (e.FieldIdentifier.Model == Field.Model && e.FieldIdentifier.FieldName == Field.FieldName)
            {
                Validate();
            }
        }

        private void Validate()
        {
            ErrorMessage2 = string.Empty;

            // プロパティの値を取得してバリデーションを実行
            if (_propertyInfoCache is null || _propertyInfoCache.Name != Field.FieldName)
            {
                _propertyInfoCache = Field.Model.GetType().GetProperty(Field.FieldName);
            }

            TItem value = (TItem)_propertyInfoCache!.GetValue(Field.Model)!;
            _messageStore!.Clear(Field);
            if (ValidateValue(value) is false)
            {
                string msg = ErrorMessage!;
                if (!string.IsNullOrEmpty(Message))
                {
                    msg = Message;
                }
                if (!string.IsNullOrEmpty(ErrorMessage2))
                {
                    msg = ErrorMessage2;
                }
                _messageStore.Add(Field, msg);
            }

            CurrentEditContext!.NotifyValidationStateChanged();
        }

        /// <summary>
        ///  入力値の検証を行う。
        ///  このメソッドをオーバーライドして検証を行う。
        /// </summary>
        /// <param name="value"></param>
        /// <returns>true : 許可,false : 不許可</returns>
        protected abstract bool ValidateValue(TItem value);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    DetachEditContext();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
