using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagAndShop.Informations
{
    /// <summary>
    /// 信息处理器类，提供多种统一的处理和报错机制。
    /// 外部可以灵活选择是直接抛出异常，还是通过传入 Action (如 UI 弹窗) 来响应不同的状态。
    /// </summary>
    public static class InformationProcessor
    {
        /// <summary>
        /// 通用处理方法：
        /// 对于非预期(Unexpected)的严重错误直接抛出异常；
        /// 对于普通操作失败，通过传入的 Action 委托进行反馈（例如弹出警告框、打印Log等）；
        /// 对于操作成功，可选择性执行成功的 Action。
        /// </summary>
        /// <param name="info">需要处理的信息对象</param>
        /// <param name="onFailure">操作失败时的回调（如弹窗：msg => MyUI.ShowAlert(msg)）</param>
        /// <param name="onSuccess">操作成功时的可选回调</param>
        public static void Handle(Information info, Action<string> onFailure, Action<string>? onSuccess = null)
        {
            if (info == null) return;

            // 1. 针对 LoadInformation 类型的特化处理
            if (info is LoadInformation loadInfo)
            {
                if (loadInfo.Unexpected)
                {
                    throw new InvalidOperationException($"[Load Error] A critical database error occurred. Message: {loadInfo.message ?? "Unexpected error"}");
                }

                if (!loadInfo.LoadCompleted)
                {
                    onFailure?.Invoke(loadInfo.message ?? "Data load failed: Record not found.");
                }
                else
                {
                    onSuccess?.Invoke(loadInfo.message ?? "Data loaded successfully.");
                }
                return;
            }

            // 2. 针对 SlotInformation 类型的特化处理
            if (info is SlotInformation slotInfo)
            {
                if (!slotInfo.Completed)
                {
                    string failMsg = $"{slotInfo.message} (Item: {slotInfo.Item?.Name ?? "Unknown"}, Remaining Count: {slotInfo.RemainCount})";
                    onFailure?.Invoke(failMsg);
                }
                else
                {
                    onSuccess?.Invoke(slotInfo.message ?? "Slot operation completed.");
                }
                return;
            }

            // 3. 基础 Information 的兜底处理（如果 message 不为空则当作失败处理）
            if (!string.IsNullOrEmpty(info.message))
            {
                onFailure?.Invoke(info.message);
            }
        }

        /// <summary>
        /// 严格抛出异常处理方法：
        /// 只要操作不成功/不完整，或者遇到 Unexpected，均直接抛出对应异常。
        /// 适合用在底层的、不允许失败的静默数据处理中。
        /// </summary>
        public static void ThrowIfFailed(Information info)
        {
            if (info == null) return;

            if (info is LoadInformation loadInfo)
            {
                if (loadInfo.Unexpected)
                {
                    throw new Exception($"Critical loading error: {loadInfo.message ?? "Unexpected database exception"}");
                }
                if (!loadInfo.LoadCompleted)
                {
                    throw new InvalidOperationException($"Loading failed: {loadInfo.message ?? "No data loaded"}");
                }
            }
            else if (info is SlotInformation slotInfo)
            {
                if (!slotInfo.Completed)
                {
                    throw new InvalidOperationException($"Slot operation failed: {slotInfo.message}. Remaining quantity: {slotInfo.RemainCount}");
                }
            }
            else if (!string.IsNullOrEmpty(info.message))
            {
                throw new Exception($"Operation failed: {info.message}");
            }
        }

        /// <summary>
        /// 检查信息是否代表“完全成功”。
        /// 方便在 if 条件中快速判断。
        /// </summary>
        public static bool IsSuccess(Information info)
        {
            if (info == null) return false;

            if (info is LoadInformation loadInfo)
            {
                return loadInfo.LoadCompleted && !loadInfo.Unexpected;
            }
            if (info is SlotInformation slotInfo)
            {
                return slotInfo.Completed;
            }

            // 默认基类没有明确成功/失败状态，通常只要没有 message 或者 message 没报错即为 true
            return true;
        }
    }
}
