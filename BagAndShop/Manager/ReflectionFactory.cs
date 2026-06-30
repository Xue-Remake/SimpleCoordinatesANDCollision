using System.Collections.Concurrent;
using System.Reflection;

namespace BagAndShop.Manager
{
    /// <summary>
    /// 静态反射工厂，提供基于注册表和基于程序集/类型名称的动态实例创建功能。
    /// </summary>
    public static class ReflectionFactory
    {
        // 使用 ConcurrentDictionary 保证类型注册表的线程安全
        private static readonly ConcurrentDictionary<string, Type> _typeRegistry = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// 手动将类型注册到工厂中。
        /// </summary>
        /// <param name="id">类型的唯一标识符</param>
        /// <param name="type">要注册的类型（可以是泛型定义）</param>
        /// <returns>注册是否成功</returns>
        public static bool RegisterType(string id, Type type)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));
            if (type == null) throw new ArgumentNullException(nameof(type));

            return _typeRegistry.TryAdd(id, type);
        }

        /// <summary>
        /// 移除已注册的类型。
        /// </summary>
        public static bool UnregisterType(string id)
        {
            return _typeRegistry.TryRemove(id, out _);
        }

        #region 方式一：通过注册表 ID 创建实例

        /// <summary>
        /// 根据注册的 ID 创建非泛型（或已封闭泛型）实例
        /// </summary>
        /// <param name="id">注册的唯一标识符</param>
        /// <param name="args">构造函数参数</param>
        public static object CreateInstanceById(string id, params object[] args)
        {
            return CreateInstanceById(id, null, args);
        }

        /// <summary>
        /// 根据注册的 ID 创建实例，支持泛型参数
        /// </summary>
        /// <param name="id">注册的唯一标识符</param>
        /// <param name="typeArguments">泛型类型参数数组（如果不是泛型，传 null 即可）</param>
        /// <param name="args">构造函数参数</param>
        public static object CreateInstanceById(string id, Type[] typeArguments, params object[] args)
        {
            if (!_typeRegistry.TryGetValue(id, out Type targetType))
            {
                throw new KeyNotFoundException($"Type with ID '{id}' was not found in the registry.");
            }

            return InstantiateType(targetType, typeArguments, args);
        }

        #endregion

        #region 方式二：通过 Assembly 和 TypeName 创建实例

        /// <summary>
        /// 根据程序集和类型全名创建非泛型实例
        /// </summary>
        /// <param name="assembly">程序集指针</param>
        /// <param name="typeName">类型全名（包含命名空间）</param>
        /// <param name="args">构造函数参数</param>
        public static object CreateInstanceByType(Assembly assembly, string typeName, params object[] args)
        {
            return CreateInstanceByType(assembly, typeName, null, args);
        }

        /// <summary>
        /// 根据程序集和类型全名创建实例，支持泛型参数
        /// </summary>
        /// <param name="assembly">程序集指针</param>
        /// <param name="typeName">类型全名（包含命名空间）。如果是泛型，名称后需带泛型参数个数，例如 "MyNamespace.MyClass`1"</param>
        /// <param name="typeArguments">泛型类型参数数组</param>
        /// <param name="args">构造函数参数</param>
        public static object CreateInstanceByType(Assembly assembly, string typeName, Type[] typeArguments, params object[] args)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (string.IsNullOrWhiteSpace(typeName)) throw new ArgumentNullException(nameof(typeName));

            Type targetType = assembly.GetType(typeName);
            if (targetType == null)
            {
                throw new TypeLoadException($"Type '{typeName}' was not found in assembly '{assembly.FullName}'.");
            }

            return InstantiateType(targetType, typeArguments, args);
        }

        #endregion

        #region 私有核心辅助方法

        /// <summary>
        /// 核心实例化逻辑，处理泛型与非泛型情况
        /// </summary>
        private static object InstantiateType(Type targetType, Type[] typeArguments, object[] args)
        {
            Type typeToConstruct = targetType;

            // 如果提供了泛型参数，且目标类型是泛型类型定义 (Open Generic Type)
            if (typeArguments != null && typeArguments.Length > 0)
            {
                if (!targetType.IsGenericTypeDefinition)
                {
                    throw new InvalidOperationException($"Type '{targetType.Name}' is not a generic type definition, but type arguments were provided.");
                }

                // 构造成封闭泛型 (Closed Generic Type)
                typeToConstruct = targetType.MakeGenericType(typeArguments);
            }
            else if (targetType.IsGenericTypeDefinition)
            {
                throw new InvalidOperationException($"Type '{targetType.Name}' is a generic type definition. Type arguments must be provided.");
            }

            // 使用 Activator 创建实例
            return Activator.CreateInstance(typeToConstruct, args);
        }

        #endregion
    }
    public static class TypeExtensions
    {
        /// <summary>
        /// 判断当前类型是否实现了指定的泛型接口定义（如 typeof(IMyInterface<,>)）
        /// </summary>
        public static bool ImplementsGenericInterface(this Type type, Type interfaceType)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));

            // 确保传入的 interfaceType 是一个泛型接口定义
            if (!interfaceType.IsInterface || !interfaceType.IsGenericTypeDefinition)
            {
                throw new ArgumentException("interfaceType 必须是一个泛型接口定义，例如 typeof(IMyInterface<,>)");
            }

            // 1. 如果当前类型本身就是该泛型接口（虽然通常我们检查的是类，但也做个容错）
            if (type.IsGenericType && type.GetGenericTypeDefinition() == interfaceType)
            {
                return true;
            }

            // 2. 遍历该类型实现的所有接口
            return type.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType
            );
        }
    }
}
