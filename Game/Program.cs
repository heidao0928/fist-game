using System;
using System.Threading;

namespace FistGame;

// ============================================================
//  界面层：负责"画画面"和"收按键"。
//  游戏规则不写在这里 —— 那些在 Core/IdleGame.cs 里。
// ============================================================
internal class Program
{
    // 程序是不是跑在一个"真正的黑窗口"里。
    // 如果是被别的程序接管输出（重定向），就不能用光标定位和按键，
    // 否则会直接报错。这一步是为了让程序更结实。
    private static readonly bool HasWindow =
        !Console.IsInputRedirected && !Console.IsOutputRedirected;

    private static void Main()
    {
        var game = new IdleGame();   // 创建一个新游戏

        long lastTick = Environment.TickCount64; // 记住上一次"过了一秒"是什么时候

        if (HasWindow)
        {
            Console.CursorVisible = false;  // 藏起光标，画面更干净
            Console.Clear();
        }

        while (true)
        {
            // ---------- 第 1 步：看看玩家有没有按键 ----------
            // KeyAvailable 的意思是"现在有键按下了吗"，没有就跳过，不会卡住等
            if (HasWindow && Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;   // true = 不要把按键显示在屏幕上

                if (key == ConsoleKey.Q)      // Q 键 = 退出
                {
                    break;
                }

                if (key == ConsoleKey.D1)     // 数字 1 键 = 手动砍一刀
                {
                    game.ChopByHand();
                }
            }

            // ---------- 第 2 步：每过 1 秒，让游戏世界往前走一步 ----------
            long now = Environment.TickCount64;
            if (now - lastTick >= 1000)   // 1000 毫秒 = 1 秒
            {
                lastTick = now;
                game.Tick();
            }

            // ---------- 第 3 步：把当前状态画出来 ----------
            Draw(game);

            Thread.Sleep(50);   // 歇 50 毫秒再循环，不然会把 CPU 跑满
        }

        // ---------- 退出时收尾 ----------
        if (HasWindow)
        {
            Console.CursorVisible = true;
            Console.Clear();
        }
        Console.WriteLine("游戏已结束，再见。");
    }

    /// <summary>把游戏状态画到屏幕上</summary>
    private static void Draw(IdleGame game)
    {
        // 把"画笔"移回左上角，这样就是原地覆盖，屏幕不会闪
        if (HasWindow)
        {
            Console.SetCursorPosition(0, 0);
        }

        Console.WriteLine("==========================================");
        Console.WriteLine("            卫月放置  v0.1");
        Console.WriteLine("==========================================");
        Console.WriteLine($" 木材：{game.Wood,-8}     每秒产出：+{game.WoodPerSecond}");
        Console.WriteLine($" 等级：{game.Level,-8}     累计砍伐：{game.TotalChops} 次");
        Console.WriteLine($" 经验：{DrawBar(game.Exp, game.ExpToNextLevel)}  {game.Exp}/{game.ExpToNextLevel}");
        Console.WriteLine("------------------------------------------");
        Console.WriteLine("   [1] 手动砍树            [Q] 退出");
        Console.WriteLine();
        Console.WriteLine("   提示：什么都不按，也会自己长木材。");
    }

    /// <summary>画一条 [####--------] 形状的经验条</summary>
    /// <remarks>
    /// 这里故意只用 # 和 - 这种最简单的符号。
    /// 方块符号（比如 █）虽然好看，但在某些终端里会变成问号。
    /// </remarks>
    private static string DrawBar(int value, int max)
    {
        const int width = 12;              // 进度条总共 12 格

        int filled = 0;
        if (max > 0)
        {
            filled = (int)((long)value * width / max);   // 按比例算出该亮几格
        }
        if (filled > width) filled = width;

        return "[" + new string('#', filled) + new string('-', width - filled) + "]";
    }
}
