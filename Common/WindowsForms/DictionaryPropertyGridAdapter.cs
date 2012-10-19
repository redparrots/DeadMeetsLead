using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace Common.WindowsForms
{
    public class DictionaryPropertyGridAdapter : ICustomTypeDescriptor
    {
        public DictionaryPropertyGridAdapter(IDictionary dictionary)
        {
            this.dictionary = dictionary;
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return dictionary;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            ArrayList properties = new ArrayList();
            foreach (DictionaryEntry de in dictionary)
            {
                properties.Add(new DictionaryPropertyDescriptor(dictionary, de.Key));
            }

            PropertyDescriptor[] props = (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

            return new PropertyDescriptorCollection(props);
        }

        IDictionary dictionary;
    }

    class DictionaryPropertyDescriptor : PropertyDescriptor
    {

        public DictionaryPropertyDescriptor(IDictionary dictionary, object key)
            : base(key.ToString(), null)
        {
            this.dictionary = dictionary;
            this.key = key;
        }

        public override void SetValue(object component, object value)
        {
            dictionary[key] = value;
        }

        public override object GetValue(object component)
        {
            return dictionary[key];
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type PropertyType { get { return dictionary[key].GetType(); } }
        public override bool IsReadOnly { get { return false; } }
        public override Type ComponentType { get { return null; } }

        IDictionary dictionary;
        object key;
    }
}
