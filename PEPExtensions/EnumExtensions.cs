using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection.Emit;

namespace PEPExtensions
{
    /// <summary>
    /// Enumの元のToString()実装はめちゃくちゃ遅いらしいので
    /// https://qiita.com/higty/items/513296536d3b26fbd033
    /// </summary>
    public static class EnumExtensions
    {
        private static ConcurrentDictionary<Type, MulticastDelegate> _ToStringFromEnumMethods = new ConcurrentDictionary<Type, MulticastDelegate>();

        /// <summary>
        /// enumの定義名文字列を返す
        /// </summary>
        /// <typeparam name="T">列挙型</typeparam>
        /// <param name="value">値</param>
        /// <returns>定義名文字列</returns>
        public static string ToString<T>(this T value)
            where T : Enum
        {
            var tp = typeof(T);

            MulticastDelegate md = null;
            if (_ToStringFromEnumMethods.TryGetValue(tp, out md) == false)
            {
                var aa = tp.GetCustomAttributes(typeof(FlagsAttribute), false);
                if (aa.Length == 0)
                {
                    md = CreateToStringFromEnumFunc<T>();
                }
                _ToStringFromEnumMethods[tp] = md;
            }
            // Flags
            if (md == null) return value.ToString().Replace(" ", "");

            var f = (Func<T, string>)md;
            return f(value);
        }

        private static Func<T, string> CreateToStringFromEnumFunc<T>()
        {
            var tp = typeof(T);
            DynamicMethod dm = new DynamicMethod("ToStringFromEnum", typeof(string), new[] { tp });
            ILGenerator il = dm.GetILGenerator();

            var values = ((T[])Enum.GetValues(tp)).Select(el => Convert.ToInt64(el)).ToList();
            var names = Enum.GetNames(tp);

            var returnLabel = il.DefineLabel();
            //Have any value different from index number
            if (values.Where((el, i) => el != i).Any())
            {
                var result = il.DeclareLocal(typeof(string));

                for (int i = 0; i < values.Count; i++)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Conv_I8);
                    il.Emit(OpCodes.Ldc_I8, values[i]);
                    il.Emit(OpCodes.Ceq);

                    var label = il.DefineLabel();
                    il.Emit(OpCodes.Brfalse, label);

                    il.Emit(OpCodes.Ldstr, names[i]);
                    il.Emit(OpCodes.Stloc, result);
                    il.Emit(OpCodes.Br, returnLabel);

                    il.MarkLabel(label);
                }
                il.ThrowException(typeof(InvalidOperationException));

                il.MarkLabel(returnLabel);
                il.Emit(OpCodes.Ldloc, result);
                il.Emit(OpCodes.Ret);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Conv_I8);
                il.Emit(OpCodes.Ldc_I4, 0);
                il.Emit(OpCodes.Conv_I8);
                il.Emit(OpCodes.Clt);
                il.Emit(OpCodes.Brtrue, returnLabel);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Conv_I8);
                il.Emit(OpCodes.Ldc_I4, names.Length - 1);
                il.Emit(OpCodes.Conv_I8);
                il.Emit(OpCodes.Cgt);
                il.Emit(OpCodes.Brtrue, returnLabel);

                il.Emit(OpCodes.Ldarg_0);
                var caseLabels = new Label[names.Length + 1];
                for (int i = 0; i < names.Length; i++)
                {
                    caseLabels[i] = il.DefineLabel();
                }
                Label defaultCase = il.DefineLabel();
                caseLabels[names.Length] = defaultCase;
                il.Emit(OpCodes.Switch, caseLabels);
                for (int i = 0; i < names.Length; i++)
                {
                    // Case ??: return "";
                    il.MarkLabel(caseLabels[i]);
                    il.Emit(OpCodes.Ldstr, names[i]);
                    il.Emit(OpCodes.Ret);
                }
                il.MarkLabel(defaultCase);
                il.ThrowException(typeof(InvalidOperationException));

                il.MarkLabel(returnLabel);
                il.ThrowException(typeof(InvalidOperationException));
            }

            var f = typeof(Func<,>);
            var gf = f.MakeGenericType(tp, typeof(string));
            return (Func<T, string>)dm.CreateDelegate(gf);
        }
    }
}
