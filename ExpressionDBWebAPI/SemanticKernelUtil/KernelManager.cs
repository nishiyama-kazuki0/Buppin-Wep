using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SharedModels;

namespace ExpressionDBWebAPI.SemanticKernelUtil;

/// <summary>
/// カーネル用のマネージャークラス。シングルトンインスタンス
/// </summary>
internal class KernelManager
{
    private readonly string endpoint;//プレイグラウンドのコードで表示時に表示されるエンドポイントURL
    private readonly string apiKey;//プレイグラウンドのコードで表示時に表示されるAPIキー
    private readonly string deploymentId; // Azure OpenAIのデプロイメントID Azure AI Studioのデプロイ名に相当。

    /// <summary>
    /// AIアシスタントのシステムロール。リクエストの最初に必ずつけて、会話の流れを失わないようにする。
    /// </summary>
    private static readonly string systemRoleMessage //TODO コンフィグで設定できるようにするか？
        = "あなたは在庫管理システムの管理者です。会話する相手に在庫管理システムの使用方法を聞かれた場合は、以下の使用方法をもとに解説してください。\r\n" +
        "・入荷作業は、入荷受付されたデータをHT入庫作業機能で、入荷検品およびパレット紐付を行うことです。パレット紐付を行った時点で在庫データが作成されます。\r\n" +
        "・出荷作業は、出荷予定をもとに引当された在庫をピッキング、切出搬送、仕分、出荷搬送を行い出荷を確定させる機能です。出荷搬送を確定した時点で在庫データが減算されます。\r\n" +
        "・在庫作業は、パレット紐付された在庫データを分割、詰め合わせ、パレット移動を行う機能です。\r\n" +
        "回答内容は、在庫管理システムや在庫管理業務、物流業務に関係のある質問内容にのみ行ってください。回答内容は文字データのみとしてください。";

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private KernelManager()
    {
        // コンストラクタをプライベートにして外部からのインスタンス生成を禁止

        // コンフィグから必要な情報を読み取るようにする
        IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        endpoint = config.GetValue<string>("AISettings:AIEndPoint") ?? throw new NullReferenceException();
        deploymentId = config.GetValue<string>("AISettings:DeploymentID") ?? throw new NullReferenceException();
        apiKey = config.GetValue<string>("AISettings:APIKey") ?? throw new NullReferenceException();

        //カーネルのセットアップ 
        IKernelBuilder builder = Kernel.CreateBuilder();
        _ = builder.AddAzureOpenAIChatCompletion(
            $"{deploymentId}",
            $"{endpoint}",
            $"{apiKey}"); //TODO コンフィグから読み込むか、引数で設定するようにする
        KernelObj = builder.Build();
    }

    /// <summary>
    /// インスタンスへのグローバルアクセスを提供
    /// </summary>
    public static KernelManager Instance { get; } = new KernelManager();
    /// <summary>
    /// カーネルオブジェクト
    /// </summary>
    public readonly Kernel KernelObj;
    /// <summary>
    /// AIモデルに渡す用のチャット履歴を作成する。
    /// </summary>
    /// <param name="chatModels"></param>
    /// <returns></returns>
    public ChatHistory CreateChatHistory(List<ChatMessageModel> chatModels)
    {
        ChatHistory chatHistory = [];
        //システムロールメッセージを追加する
        chatHistory.AddSystemMessage(systemRoleMessage);

        //これまでのやり取りをチャットヒストリーに追加
        foreach (ChatMessageModel chatModel in chatModels)
        {
            if (chatModel.Type == ChatSendAndReceiveType.AssistantInput)
            {
                chatHistory.AddAssistantMessage(chatModel.Message);
            }
            else
            {
                chatHistory.AddUserMessage(chatModel.Message);
            }
        }
        return chatHistory;
    }

}
