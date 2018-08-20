using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Unity
{
    [System.Serializable]
    public class InteractableEvent
    {
        public string Name;
        public UnityEvent Event;
        public string ClassName;
        public ReceiverBase Receiver;
        public List<PropertySetting> Settings;

        public struct EventLists
        {
            public List<Type> EventTypes;
            public List<String> EventNames;
        }

        [System.Serializable]
        public struct PropertySetting
        {
            public InspectorField.FieldTypes Type;
            public string Label;
            public string Tooltip;
            public int IntValue;
            public string StringValue;
            public float FloatValue;
            public bool BoolValue;
            public GameObject GameObjectValue;
            public ScriptableObject ScriptableObjectValue;
            public UnityEngine.Object ObjectValue;
            public Material MaterialValue;
            public Texture TextureValue;
            public Color ColorValue;
            public Vector2 Vector2Value;
            public Vector3 Vector3Value;
            public Vector4 Vector4Value;
            public AnimationCurve CurveValue;
            public AudioClip AudioClipValue;
            public Quaternion QuaternionValue;
            public string[] Options;
        }

        [System.Serializable]
        public struct FieldData
        {
            public InspectorField Attributes;
            public object Value;
            public string Name;
        }

        public struct ReceiverData
        {
            public string Name;
            public List<FieldData> Fields;
        }
        
        public ReceiverData AddOnClick()
        {
            return AddReceiver(typeof(OnClickReceiver));
        }

        public ReceiverData AddReceiver(Type type)
        {
            ReceiverBase receiver = (ReceiverBase)Activator.CreateInstance(type, Event);
            // get the settings for the inspector

            List<FieldData> fields = new List<FieldData>();

            Type myType = receiver.GetType();
            int index = 0;

            ReceiverData data = new ReceiverData();

            //Debug.Log(myType + " / " + myType.GetProperties().Length + " / " + myType.GetFields().Length);
            foreach (PropertyInfo prop in myType.GetProperties())
            {
                var attrs = (InspectorField[])prop.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    fields.Add(new FieldData() { Name = prop.Name, Attributes = attr, Value = prop.GetValue(receiver, null)});
                    //Debug.Log("Props: " + prop.Name + " / " + attr.Type + " / " + attr.Label + " / " + prop.GetValue(receiver, null ));
                }

                index++;
            }

            index = 0;
            foreach (FieldInfo field in myType.GetFields())
            {
                var attrs = (InspectorField[])field.GetCustomAttributes(typeof(InspectorField), false);
                foreach (var attr in attrs)
                {
                    fields.Add(new FieldData() { Name = field.Name, Attributes = attr, Value = field.GetValue(receiver) });
                    //Debug.Log("Fields: " + field.Name + " / " + attr.Type + " / " + attr.Label + " / " + field.GetValue(receiver));
                }

                index++;
            }

            data.Fields = fields;
            data.Name = receiver.Name;

            return data;
        }

        public static EventLists GetEventTypes()
        {
            List<Type> eventTypes = new List<Type>();
            List<string> names = new List<string>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(ReceiverBase)))
                    {
                        eventTypes.Add(type);
                        names.Add(type.Name);
                    }
                }
            }

            EventLists lists = new EventLists();
            lists.EventTypes = eventTypes;
            lists.EventNames = names;
            return lists;
        }

        public static PropertySetting FieldToProperty(InspectorField attributes, object fieldValue, string fieldName)
        {
            PropertySetting setting = new PropertySetting();
            setting.Type = attributes.Type;
            setting.Tooltip = attributes.Tooltip;
            setting.Label = attributes.Label;
            setting.Options = attributes.Options;

            UpdatePropertySetting(setting, fieldValue);

            return setting;
        }

        public static PropertySetting UpdatePropertySetting(PropertySetting setting, object update)
        {
            switch (setting.Type)
            {
                case InspectorField.FieldTypes.Float:
                    setting.FloatValue = (float)update;
                    break;
                case InspectorField.FieldTypes.Int:
                    setting.IntValue = (int)update;
                    break;
                case InspectorField.FieldTypes.String:
                    setting.StringValue = (string)update;
                    break;
                case InspectorField.FieldTypes.Bool:
                    setting.BoolValue = (bool)update;
                    break;
                case InspectorField.FieldTypes.Color:
                    setting.ColorValue = (Color)update;
                    break;
                case InspectorField.FieldTypes.DropdownInt:
                    setting.IntValue = (int)update;
                    break;
                case InspectorField.FieldTypes.DropdownString:
                    setting.StringValue = (string)update;
                    break;
                case InspectorField.FieldTypes.GameObject:
                    setting.GameObjectValue = (GameObject)update;
                    break;
                case InspectorField.FieldTypes.ScriptableObject:
                    setting.ScriptableObjectValue = (ScriptableObject)update;
                    break;
                case InspectorField.FieldTypes.Object:
                    setting.ObjectValue = (UnityEngine.Object)update;
                    break;
                case InspectorField.FieldTypes.Material:
                    setting.MaterialValue = (Material)update;
                    break;
                case InspectorField.FieldTypes.Texture:
                    setting.TextureValue = (Texture)update;
                    break;
                case InspectorField.FieldTypes.Vector2:
                    setting.Vector2Value = (Vector2)update;
                    break;
                case InspectorField.FieldTypes.Vector3:
                    setting.Vector3Value = (Vector3)update;
                    break;
                case InspectorField.FieldTypes.Vector4:
                    setting.Vector4Value = (Vector4)update;
                    break;
                case InspectorField.FieldTypes.Curve:
                    setting.CurveValue = (AnimationCurve)update;
                    break;
                case InspectorField.FieldTypes.Quaternion:
                    setting.QuaternionValue = (Quaternion)update;
                    break;
                case InspectorField.FieldTypes.AudioClip:
                    setting.AudioClipValue = (AudioClip)update;
                    break;
                default:
                    break;
            }

            return setting;
        }

        public static ReceiverBase GetReceiver(InteractableEvent iEvent, EventLists lists)
        {
            int index = ReverseLookup(iEvent.ClassName, lists.EventNames.ToArray());
            Type eventType = lists.EventTypes[index];
            // apply the settings?
            return (ReceiverBase)Activator.CreateInstance(eventType, iEvent.Event);
        }

        // put somewhere it makes sense!!!!
        public static int ReverseLookup(string option, string[] options)
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == option)
                {
                    return i;
                }
            }

            return 0;
        }
    }

}
