using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SharedModels;

namespace ExpressionDBWebAPI.SemanticKernelUtil;

public class AIQueryFunc
{
    /// <summary>
    /// //Semantic Kernelを使用して、Azure Open AI モデルから回答を取得するメソッド
    /// </summary>
    /// <param name="prompt">問い合わせるための全ての文字列が入っているとする</param>
    /// <returns></returns>
    public static (int retCode, string retMsg) GetAnswerFromAI(List<ChatMessageModel> prompt)
    {

        //TODO プロンプトを設定、ロールの設定、プロンプト関数なども設定できるようにする

        //chatHistoryを作成する
        ChatHistory chatHistory = KernelManager.Instance.CreateChatHistory(prompt);

        // IChatCompletionService を使って Chat Completion API を呼び出す
        IChatCompletionService chatService = KernelManager.Instance.KernelObj.GetRequiredService<IChatCompletionService>();
        // レスポンスを取得
        Task<ChatMessageContent> task = chatService.GetChatMessageContentAsync(chatHistory);
        //Task<FunctionResult> task = KernelManager.Instance.KernelObj.InvokePromptAsync(prompt);
        task.Wait(); // 非同期操作が完了するまで待機

        ChatMessageContent ret = task.Result; // 結果を取得
        KernelContent? mes = ret.Items.FirstOrDefault();
        return mes is TextContent responseText
            ? ((int retCode, string retMsg))(0, responseText.Text ?? string.Empty)
            : ((int retCode, string retMsg))(-1, string.Empty)
            ;

    }
}
