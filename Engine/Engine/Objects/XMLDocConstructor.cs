﻿using System.IO;
using System.Xml;
using Jurassic;
using Jurassic.Library;

namespace Engine.Objects
{
    class XMLDocConstructor : ClrFunction
    {
        public XMLDocConstructor(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "XmlFile", new XMLDocInstance(engine.Object.InstancePrototype))
        {
        }

        [JSConstructorFunction]
        public XMLDocInstance Create(string filename)
        {
            return new XMLDocInstance(InstancePrototype, Program.ParseSpherePath(filename, ""));
        }
    }

    // TODO: read and write objects!
    class XMLDocInstance : ObjectInstance
    {
        string _path = "";
        XmlDocument _doc;

        public XMLDocInstance(ObjectInstance prototype)
            : base(prototype)
        {
            PopulateFunctions();
        }

        public XMLDocInstance(ObjectInstance prototype, string path)
            : this(prototype)
        {
            _doc = new XmlDocument();
            _path = path;
            _doc.AppendChild(_doc.CreateXmlDeclaration("1.0", null, null));
            _doc.AppendChild(_doc.CreateComment("Generated by Sphere SFML XML Content Serializer v1.0"));
        }

        [JSFunction(Name = "write")]
        public void Write(string name, ObjectInstance obj)
        {
            XmlElement element = _doc.CreateElement(name);
            foreach (PropertyNameAndValue pv in obj.Properties)
            {
                if (pv.Value is FunctionInstance) continue;

                if (pv.Value is ArrayInstance)
                    WriteArray(element, "a_" + pv.Name, pv.Value as ArrayInstance);
                else if (pv.Value is ObjectInstance)
                    WriteObject(element, "o_" + pv.Name, pv.Value as ObjectInstance);
                else
                    WritePrimitive(element, pv.Name, pv.Value);
            }
            _doc.AppendChild(element);
        }

        [JSFunction(Name = "close")]
        public void Close()
        {
            _doc.Save(_path);
        }

        private void WriteObject(XmlElement parent, string name, ObjectInstance obj)
        {
            XmlElement element = _doc.CreateElement(name);
            System.Console.WriteLine("writing: {0}", name);
            foreach (PropertyNameAndValue pv in obj.Properties)
            {
                if (pv.Value is FunctionInstance) continue;

                if (pv.Value is ArrayInstance)
                    WriteArray(element, "a_" + pv.Name, pv.Value as ArrayInstance);
                else if (pv.Value is ObjectInstance)
                    WriteObject(element, "o_" + pv.Name, pv.Value as ObjectInstance);
                else
                    WritePrimitive(element, pv.Name, pv.Value);
            }
            parent.AppendChild(element);
        }

        private void WriteArray(XmlElement parent, string name, ArrayInstance array)
        {
            XmlElement element = _doc.CreateElement(name);
            System.Console.WriteLine("writing {0} elements", array.Length);
            for (int i = 0; i < array.Length; ++i)
            {
                var obj = array[i];
                if (obj is FunctionInstance) continue;

                if (obj is ObjectInstance)
                    WriteObject(element, "o_" + i, obj as ObjectInstance);
                else if (obj is ArrayInstance)
                    WriteArray(element, "a_" + i, obj as ArrayInstance);
                else
                    WritePrimitive(element, i.ToString(), obj);
            }
            parent.AppendChild(element);
        }

        private void WritePrimitive(XmlElement parent, string name, object obj)
        {
            string prefix = "s_";
            if (obj is NumberInstance) prefix = "n_";
            if (obj is BooleanInstance) prefix = "b_";
            if (obj is FunctionInstance) prefix = "f_";
            XmlAttribute attribute = _doc.CreateAttribute(prefix + name);
            attribute.Value = obj.ToString();
            parent.Attributes.Append(attribute);
        }
    }
}
