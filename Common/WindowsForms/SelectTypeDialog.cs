using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Common.WindowsForms
{
    public class SelectTypeDialog : SelectDialog
    {
        public void LoadTypes(Assembly assembly, Type baseType)
        {
            LoadTypes(assembly, baseType, false);
        }
        public void LoadTypes(Assembly assembly, Type baseType, bool includeBaseType)
        {
            List<Type> t = new List<Type>();
            foreach (var v in assembly.GetTypes())
                if (baseType.IsAssignableFrom(v) && (includeBaseType || v != baseType))
                    t.Add(v);
            t.Sort(new Comparison<Type>((a, b) => a.Name.CompareTo(b.Name)));
            AvailableTypes = t;
        }
        public List<Type> AvailableTypes
        {
            set
            {
                selectComboBox.Items.Clear();
                selectComboBox.Items.AddRange(value.ToArray());
            }
        }
    }


    public class InstanceSelectTypeEditor<BaseType> : System.Drawing.Design.UITypeEditor
    {
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Common.WindowsForms.SelectTypeDialog t = new Common.WindowsForms.SelectTypeDialog();
            t.LoadTypes(typeof(BaseType).Assembly, typeof(BaseType));
            if (t.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return base.EditValue(context, provider, value);

            if (t.SelectedObject == null)
                return null;

            return Activator.CreateInstance((Type)t.SelectedObject);
        }
    }

    public class TypeSelectTypeEditor<BaseType> : System.Drawing.Design.UITypeEditor
    {
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Common.WindowsForms.SelectTypeDialog t = new Common.WindowsForms.SelectTypeDialog();
            t.LoadTypes(typeof(BaseType).Assembly, typeof(BaseType));
            if (t.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return base.EditValue(context, provider, value);

            return (Type)t.SelectedObject;
        }
    }
}
