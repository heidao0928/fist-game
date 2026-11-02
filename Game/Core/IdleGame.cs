namespace FistGame;

// ============================================================
//  游戏规则层：这里只算数字，完全不管"怎么显示"。
//  好处：以后换成窗口界面、甚至放进 FF14，这个文件一行都不用改。
// ============================================================
public class IdleGame
{
    // ---------- 游戏里的数据 ----------

    /// <summary>当前拥有的木材</summary>
    public int Wood { get; private set; }

    /// <summary>伐木等级</summary>
    public int Level { get; private set; } = 1;

    /// <summary>当前等级已攒的经验</summary>
    public int Exp { get; private set; }

    /// <summary>一共砍了多少刀（统计用）</summary>
    public int TotalChops { get; private set; }

    // ---------- 由上面的数据算出来的东西 ----------

    /// <summary>升到下一级需要多少经验，等级越高越多</summary>
    public int ExpToNextLevel => 10 * Level;

    /// <summary>每秒自动产出多少木材，等级越高越多</summary>
    public int WoodPerSecond => Level;

    // ---------- 三个动作 ----------

    /// <summary>时间往前走 1 秒（自动砍树）</summary>
    public void Tick()
    {
        Wood += WoodPerSecond;
        TotalChops += WoodPerSecond;
        AddExp(1);
    }

    /// <summary>玩家按了 [1]，亲手砍一刀</summary>
    public void ChopByHand()
    {
        Wood += 1;
        TotalChops += 1;
        AddExp(1);
    }

    /// <summary>加经验，攒够了就升级（一次可能连升好几级）</summary>
    private void AddExp(int amount)
    {
        Exp += amount;

        while (Exp >= ExpToNextLevel)
        {
            Exp -= ExpToNextLevel; // 扣掉这一级要的经验
            Level++;               // 等级 +1
        }
    }
}
