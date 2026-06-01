using ConsoleUILib.UILib;
using SimpleCoordinatesANDCollision.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // ========== 1. 数据准备 (AppState) ==========
            var app = new AppState();
            bool isRunning = true;
            // ========== 2. 配置输入处理与命令分发 ==========
            var inputHandler = new InputHandler
            {
                Prompt = "输入指令 > ",
                PollingIntervalMs = 30
            };
            app.InputRef = inputHandler; // 将引用传给状态机，方便提取实时输入内容
            var dispatcher = new CommandDispatcher();
            dispatcher.Separators = new[] { ' ' };
            // ========== 3. 声明控件并绑定数据 ==========
            // 标题
            var titleText = new StaticText();
            titleText.Bind(() => "【二维向量计算与转换 测试控制台】 (输入 help 查看指令)");
            // 实时状态监控区：向量A与向量B的笛卡尔与极坐标表示
            var vecAText = new StaticText();
            vecAText.Bind(() =>
                $"向量 A: {app.VectorA}  |  极坐标: {(PolarVector)app.VectorA}  |  模长: {app.VectorA.Magnitude:F2}");
            var vecBText = new StaticText();
            vecBText.Bind(() =>
                $"向量 B: {app.VectorB}  |  极坐标: {(PolarVector)app.VectorB}  |  模长: {app.VectorB.Magnitude:F2}");
            // 实时结果监控区
            var resultText = new StaticText();
            resultText.Bind(() => $"[最新计算结果] => {app.LatestResult}");
            // 历史操作日志区
            var logList = new ListView { Title = "操作日志 (最新记录在下)" };
            logList.AddStringItem("系统初始化完毕，等待指令...");
            // ★ 实时输入回显区 (满足“实时显示当前输入的指令字符串”的需求)
            var currentInputText = new StaticText();
            // 注意：这里假设 InputHandler 暴漏了 CurrentInput 或 Buffer 属性。
            // 如果你的 ConsoleUILib 库中获取实时输入的属性名不同（如 InputBuffer），请修改下方属性名。
            currentInputText.Bind(() => $"[正在输入] >>> {GetRealTimeInput(inputHandler)}");
            // ========== 4. 注册测试向量类的命令 ==========
            // 辅助方法：记录日志并更新结果
            void LogAndResult(string msg)
            {
                app.LatestResult = msg;
                logList.AddStringItem($"[{DateTime.Now:HH:mm:ss}] {msg}");
            }
            dispatcher.Register("help", _ =>
            {
                LogAndResult("可用指令: seta <x> <y>, setb <x> <y>, polar_a <r> <deg>, polar_b <r> <deg>");
                LogAndResult("可用计算: add, sub, dot, cross, dist, norma, normb, rota <deg>, rotb <deg>");
                LogAndResult("系统指令: clearlog, quit");
                return true;
            });
            dispatcher.Register("quit", _ => { isRunning = false; return true; });
            dispatcher.Register("clearlog", _ =>
            {
                // 注意：如果 ListView 支持 Clear 方法可以直接用。这里演示用重新绑定或忽略
                app.LatestResult = "日志已清空(逻辑演示)";
                return true;
            });
            // 基础设置指令
            dispatcher.Register("seta", args =>
            {
                if (args.Length == 2 && double.TryParse(args[0], out double x) && double.TryParse(args[1], out double y))
                {
                    app.VectorA = new CartesianVector(x, y);
                    LogAndResult($"设置向量 A 为 {app.VectorA}");
                }
                else LogAndResult("语法错误: seta <x> <y>");
                return true;
            });
            dispatcher.Register("setb", args =>
            {
                if (args.Length == 2 && double.TryParse(args[0], out double x) && double.TryParse(args[1], out double y))
                {
                    app.VectorB = new CartesianVector(x, y);
                    LogAndResult($"设置向量 B 为 {app.VectorB}");
                }
                else LogAndResult("语法错误: setb <x> <y>");
                return true;
            });
            dispatcher.Register("polar_a", args =>
            {
                if (args.Length == 2 && double.TryParse(args[0], out double r) && double.TryParse(args[1], out double deg))
                {
                    var p = Vector2DUtils.CreatePolarFromDegrees(r, deg);
                    app.VectorA = (CartesianVector)p; // 测试隐式/显式转换
                    LogAndResult($"极坐标设置 A: R={r}, 角度={deg}° => 转换为 {app.VectorA}");
                }
                else LogAndResult("语法错误: polar_a <r> <deg>");
                return true;
            });
            // 算术与运算测试指令
            dispatcher.Register("add", _ =>
            {
                var sum = app.VectorA + app.VectorB;
                LogAndResult($"A + B = {sum}");
                return true;
            });
            dispatcher.Register("sub", _ =>
            {
                var diff = app.VectorA - app.VectorB;
                LogAndResult($"A - B = {diff}");
                return true;
            });
            dispatcher.Register("dot", _ =>
            {
                double dot = CartesianVector.Dot(app.VectorA, app.VectorB);
                LogAndResult($"A · B (点积) = {dot}");
                return true;
            });
            dispatcher.Register("cross", _ =>
            {
                double cross = CartesianVector.Cross(app.VectorA, app.VectorB);
                LogAndResult($"A × B (二维叉积) = {cross}");
                return true;
            });
            dispatcher.Register("dist", _ =>
            {
                double dist = Vector2DUtils.Distance(app.VectorA, app.VectorB);
                LogAndResult($"A 与 B 之间的距离 = {dist:F4}");
                return true;
            });
            dispatcher.Register("norma", _ =>
            {
                var norm = Vector2DUtils.Normalize(app.VectorA);
                LogAndResult($"向量 A 归一化 = {norm}");
                return true;
            });
            dispatcher.Register("rota", args =>
            {
                if (args.Length == 1 && double.TryParse(args[0], out double deg))
                {
                    double rad = Vector2DUtils.DegreesToRadians(deg);
                    var rotated = Vector2DUtils.Rotate(app.VectorA, rad);
                    LogAndResult($"向量 A 逆时针旋转 {deg}° = {rotated}");
                }
                return true;
            });
            // 挂载提交事件
            inputHandler.OnCommandSubmitted += cmd => dispatcher.Dispatch(cmd);
            // ========== 5. 声明 Session 并添加控件 ==========
            var session = new Session(30); // 约 33 FPS 刷新率
            session.Add(titleText);
            session.Add(new DoubleDivider());
            session.Add(vecAText);
            session.Add(vecBText);
            session.Add(new Divider());
            session.Add(resultText);
            session.Add(new DoubleDivider());
            session.Add(logList);
            session.Add(new Divider());
            session.Add(currentInputText); // 放在底部，紧挨着输入区
            // ========== 6. 启动与主循环 ==========
            inputHandler.Start();
            session.Start();
            // 主循环解耦：这里的循环仅用于保持程序运行，UI由后台Session线程刷新
            while (session.IsRunning && isRunning)
            {
                Thread.Sleep(50); // 控制CPU占用
            }
            inputHandler.Stop();
            session.Stop();
            Console.Clear();
            Console.WriteLine("测试结束，程序已退出。");
        }
        /// <summary>
        /// 利用反射或公开属性获取 InputHandler 正在输入的实时文本
        /// </summary>
        static string GetRealTimeInput(InputHandler handler)
        {
            try
            {
                // 方案 A: 假设你的库中有公开的 CurrentInput 或 InputBuffer 属性
                var prop = handler.GetType().GetProperty("CurrentInput") ??
                           handler.GetType().GetProperty("InputBuffer") ??
                           handler.GetType().GetProperty("Buffer");
                if (prop != null)
                {
                    return prop.GetValue(handler)?.ToString() ?? "";
                }
                // 方案 B: 假设是一个内部私有字段（如 _buffer）
                var field = handler.GetType().GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) ??
                            handler.GetType().GetField("buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (field != null)
                {
                    return field.GetValue(handler)?.ToString() ?? "";
                }
                return "无法获取实时输入(找不到对应的属性或字段)";
            }
            catch
            {
                return "";
            }
        }
    }
    /// <summary>
    /// 应用业务状态模型
    /// </summary>
    class AppState
    {
        // 初始提供两个默认向量以供观察
        public CartesianVector VectorA { get; set; } = new CartesianVector(10, 0);
        public CartesianVector VectorB { get; set; } = new CartesianVector(0, 10);
        public string LatestResult { get; set; } = "尚未进行计算";
        public InputHandler InputRef { get; set; }
    }
}
