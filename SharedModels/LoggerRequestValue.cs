using static SharedModels.SharedConst;

namespace SharedModels;

public class LoggerRequestValue
{
    public string Messgae { get; set; } = string.Empty;
    public TYPE_LOGGER TypeLogger { get; set; } = TYPE_LOGGER.NONE;
}
