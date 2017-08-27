using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SharpFuncExt
{
	public static class NullFunc
	{
		public static TResult IfNull<T, TResult>(this T arg, Func<TResult> ifNull, Func<T, TResult> ifNotNull)
		{
			if (arg.IsNull())
				return ifNull();
			return ifNotNull(arg);
		}

		public static T IfNull<T>(this T arg, Func<T> ifNull)
		{
			if (arg.IsNull())
				return ifNull();
			return arg;
		}

		public static TResult IfNotNull<T, TResult>(this T arg, Func<T, TResult> ifNotNull, Func<TResult> ifNull)
		{
			if (arg.NotNull())
				return ifNotNull(arg);
			return ifNull();
		}
		public static TResult IfNotNull<T, TResult>(this T arg, Func<T, TResult> ifNotNull, TResult ifNull)
		{
			if (arg.NotNull())
				return ifNotNull(arg);
			return ifNull;
		}

		public static TResult IfNull<T, TResult>(this IEnumerable<T> arg, Func<TResult> ifNull, Func<IEnumerable<T>, TResult> ifNotNull)
		{
			if (arg.IsNull())
				return ifNull();
			return ifNotNull(arg);
		}

		public static TResult IfNotNull<T, TResult>(this IEnumerable<T> arg, Func<IEnumerable<T>, TResult> ifNotNull, Func<TResult> ifNull)
		{
			if (arg.NotNull())
				return ifNotNull(arg);
			return ifNull();
		}

		public static TResult IfNull<T, TResult>(this T arg, TResult ifNull, TResult ifNotNull)
		{
			if (arg.IsNull())
				return ifNull;
			return ifNotNull;
		}

		public static TResult IfNotNull<T, TResult>(this T arg, TResult ifNotNull, TResult ifNull)
		{
			if (arg.NotNull())
				return ifNotNull;
			return ifNull;
		}

		public static TResult NotNullOrDefault<T, TResult>(this T arg, Func<T, TResult> ifNotNull)
		{
			if (arg.NotNull())
				return ifNotNull(arg);
			return default(TResult);
		}

		public static T IfNull<T, TValue>(this T arg, TValue value, Func<TValue, T> func)
		{
			if (arg.IsNull())
				return func(value);
			return arg;
		}

		public static T IfNotNull<T, TResult>(this T arg, Func<T, TResult> func)
		{
			if (arg.NotNull())
				func(arg);
			return arg;
		}
		public static T IfNotNull<T>(this T arg, Action<T> func)
		{
			if (arg.NotNull())
				func(arg);
			return arg;
		}

		public static T If<T, TValue>(this T arg, TValue value, Func<TValue, bool> checkFunc, Func<TValue, T, T> ifTrue, Func<TValue, T, T> ifFalse)
			=> checkFunc(value) ? ifTrue(value, arg) : ifFalse(value, arg);

		public static T IfNotNull<T, TValue>(this T arg, TValue value, Func<T, T> func)
		{
			if (value.NotNull())
				return func(arg);
			return arg;
		}

		public static TResult IfNotNullOrDefault<T, TResult>(this T arg, Func<T, TResult> func)
		{
			if (arg.NotNull())
				return func(arg);
			return default(TResult);
		}

		public static T HasNotNullArg<T>(this T arg, string argName) => arg.ThrowIfNull(() => new ArgumentNullException(argName));
		public static T HasNotNullArg<T, TValue>(this T arg, Expression<Func<T, TValue>> expression, string argName) => arg.ThrowIfNull(expression, () => new ArgumentNullException(argName));
		public static T HasNotNullArg<T, TValue1, TValue2>(this T arg, Expression<Func<T, TValue1>> expression1, Expression<Func<T, TValue2>> expression2, string argName) 
			=> arg.ThrowIfNull(expression1, expression2, () => new ArgumentNullException(argName));

		public static T ThrowIfNull<T, TException>(this T arg, Func<TException> func) where TException : Exception
		{
			if (arg.IsNull())
				throw func();
			return arg;
		}

		public static T ThrowIfNull<T, TException>(this T arg) where TException : Exception, new()
		{
			if (arg.IsNull())
				throw new TException();
			return arg;
		}

		public static T ThrowIfNull<T, TValue, TException>(this T arg, Expression<Func<T, TValue>> expression, Func<TException> func) where TException : Exception
		{
			if (arg.IsNull(expression))
				throw func();
			return arg;
		}

		public static T ThrowIfNull<T, TValue1, TValue2, TException>(this T arg, Expression<Func<T, TValue1>> expression1, Expression<Func<T, TValue2>> expression2, Func<TException> func) where TException : Exception
		{
			if (arg.IsNull(expression1, expression2))
				throw func();
			return arg;
		}

		public static bool IsNull<T>(this T arg) => EqualityComparer<T>.Default.Equals(arg, default(T));
		public static bool NotNull<T>(this T arg) => !EqualityComparer<T>.Default.Equals(arg, default(T));

		// TODO: кешировать Expression
		public static bool IsNull<T, TValue>(this T arg, Expression<Func<T, TValue>> expression)
			=> EqualityComparer<T>.Default.Equals(arg, default(T))
				|| EqualityComparer<TValue>.Default.Equals(expression.Compile()(arg), default(TValue)); // TODO: Добавить кеширование

		public static bool IsNull<T, TValue1, TValue2>(this T arg, Expression<Func<T, TValue1>> expression1, Expression<Func<T, TValue2>> expression2)
			=> EqualityComparer<T>.Default.Equals(arg, default(T))
				|| EqualityComparer<TValue1>.Default.Equals(expression1.Compile()(arg), default(TValue1))
				|| EqualityComparer<TValue2>.Default.Equals(expression2.Compile()(arg), default(TValue2));

		public static bool NotNull<T, TValue>(this T arg, Expression<Func<T, TValue>> expression)
			=> !EqualityComparer<T>.Default.Equals(arg, default(T))
				&& !EqualityComparer<TValue>.Default.Equals(expression.Compile()(arg), default(TValue));

		public static bool NotNull<T, TValue1, TValue2>(this T arg, Expression<Func<T, TValue1>> expression1, Expression<Func<T, TValue2>> expression2)
			=> !EqualityComparer<T>.Default.Equals(arg, default(T))
				&& !EqualityComparer<TValue1>.Default.Equals(expression1.Compile()(arg), default(TValue1))
				&& !EqualityComparer<TValue2>.Default.Equals(expression2.Compile()(arg), default(TValue2));


		public static bool IsNull<T>(this IEnumerable<T> arg) => arg == null || !arg.Any();
		public static bool NotNull<T>(this IEnumerable<T> arg) => arg != null && arg.Any();
		public static bool IsNull<T>(this T[] arg) => arg == null || !arg.Any();
		public static bool NotNull<T>(this T[] arg) => arg != null && arg.Any();

		public static bool IsNull(this object arg, Type type) => arg == null || type.GetTypeInfo().IsValueType && GetTypedNull(type).Equals(arg);

		private static Dictionary<Type, Delegate> lambdasMap = new Dictionary<Type, Delegate>();
		private static object GetTypedNull(Type type)
		{
			Delegate func;
			if (!lambdasMap.TryGetValue(type, out func))
			{
				var body = Expression.Default(type);
				var lambda = Expression.Lambda(body);
				func = lambda.Compile();
				lambdasMap[type] = func;
			}
			return func.DynamicInvoke();
		}
	}
}
