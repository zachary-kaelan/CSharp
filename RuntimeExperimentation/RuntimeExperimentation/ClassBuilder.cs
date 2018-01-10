using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace RuntimeExperimentation
{
    public static class ClassBuilder
    {
        private static Type obj = null;
        public static void CreateNewObject(string name, KeyValuePair<string, Type>[] fields = null)
        {
            if (obj == null)
                obj = CompileResultType(fields);
            var myObject = Activator.CreateInstance(obj);
        }

        public static Type CompileResultType(KeyValuePair<string, Type>[] fields)
        {
            TypeBuilder tb = GetTypeBuilder();
            ConstructorBuilder constructor =
                tb.DefineDefaultConstructor(
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.RTSpecialName
                );

            foreach (var field in fields)
                CreateProperty(tb, field.Key, field.Value);

            return tb.CreateType();
        }

        public static TypeBuilder GetTypeBuilder()
        {
            string typeSignature = "Location";
            AssemblyName an = new AssemblyName(typeSignature);
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder mb = ab.DefineDynamicModule("FileFormatSkeleton");
            TypeBuilder tb = mb.DefineType(typeSignature,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                null
            );

            return tb;
        }

        private static void CreateProperty(TypeBuilder tb, string name, Type type)
        {
            FieldBuilder fb = tb.DefineField(
                name, type, FieldAttributes.Public);

            PropertyBuilder pb = tb.DefineProperty(
                name, PropertyAttributes.HasDefault, type, null);
            MethodBuilder getPropMB = tb.DefineMethod("get_" + name,
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig,
                type, Type.EmptyTypes
            );
            ILGenerator getIl = getPropMB.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fb);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMB =
                tb.DefineMethod("set_" + name,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    null, new[] { type })
                );

            ILGenerator setIl = setPropMB.GetILGenerator();
            Label modify = setIl.DefineLabel();
            Label exit = setIl.DefineLabel();

            setIl.MarkLabel(modify);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fb);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exit);
            setIl.Emit(OpCodes.Ret);

            pb.SetGetMethod(getPropMB);
            pb.SetSetMethod(setPropMB);
        }
    }
}
