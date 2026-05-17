/// <summary>
/// 关卡内仿真流程：未开始 → 进行中 → 暂停。
/// </summary>
public enum GameState
{
    BeforeStart,
    /// <summary>已点击「开始」，物理与输入按运行中处理。</summary>
    Start,
    /// <summary>已点击「暂停」。</summary>
    Stop
}
