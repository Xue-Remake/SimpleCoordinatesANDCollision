using SimpleSQLiteORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagAndShop.Informations
{
    /// <summary>
    /// 数据库加载操作的结果信息类，包含加载状态、目标数据库、查询对象等数据。
    /// 继承自 <see cref="Information"/>，用于传递数据加载过程中的成功、失败或异常信息。
    /// </summary>
    public class LoadInformation : Information
    {
        /// <summary>
        /// 表示加载过程中是否发生了意外异常。为 <c>true</c> 时应在外部捕获并抛出异常。
        /// </summary>
        public bool Unexpected { get; private set; } = false;
        /// <summary>
        /// 表示数据加载是否已完成。
        /// </summary>
        public bool LoadCompleted { get; private set; }
        /// <summary>
        /// 目标数据库实例。
        /// </summary>
        public SqliteDataBase? TargetDataBase { get; private set; }
        /// <summary>
        /// 查询时使用的原始对象，通常包含用于定位数据的主键。
        /// </summary>
        public object? OriginalObject { get; private set; }
        public LoadInformation(bool unexpected, bool loadCompleted, SqliteDataBase? targetDataBase, object? obj)
        {
            Unexpected = unexpected;
            LoadCompleted = loadCompleted;
            TargetDataBase = targetDataBase;
            OriginalObject = obj;
        }
        /// <summary>
        /// 创建一个表示发生非预期错误的 <see cref="LoadInformation"/> 实例。
        /// 通常用于在外部获取后及时抛出异常（Unexpected 为 true）。
        /// </summary>
        /// <returns>包含错误状态的 <see cref="LoadInformation"/> 实例。</returns>
        public static LoadInformation GetErrorInformation()
        {
            // Unexpected = true，用于在外部获取并及时throw Exception
            return new LoadInformation(true, false, null, null);
        }
        /// <summary>
        /// 创建一个表示数据加载成功的 <see cref="LoadInformation"/> 实例。
        /// </summary>
        /// <param name="db">目标数据库实例。</param>
        /// <param name="obj">查询时使用的原始对象。</param>
        /// <returns>包含成功状态及相关数据的 <see cref="LoadInformation"/> 实例。</returns>
        public static LoadInformation GetCompleted(SqliteDataBase db, object obj)
        {
            return new LoadInformation(false, true, db, obj);
        }
        /// <summary>
        /// 创建一个表示数据加载失败（如未找到数据）但未发生异常的 <see cref="LoadInformation"/> 实例。
        /// </summary>
        /// <param name="db">目标数据库实例。</param>
        /// <param name="obj">查询时使用的原始对象。</param>
        /// <returns>包含失败状态及相关数据的 <see cref="LoadInformation"/> 实例。</returns>
        public static LoadInformation GetFaild(SqliteDataBase db, object obj)
        {
            return new LoadInformation(false, false, db, obj);
        }
    }
}