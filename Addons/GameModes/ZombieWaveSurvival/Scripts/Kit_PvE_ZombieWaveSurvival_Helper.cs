using System;
using System.Reflection;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public static class Kit_PvE_ZombieWaveSurvival_Helper
        {
            public static T AddComponent<T>(this GameObject destination, T original) where T : Component
            {
                System.Type type = original.GetType();
                Component copy = destination.AddComponent(type);
                System.Reflection.FieldInfo[] fields = type.GetFields();
                foreach (System.Reflection.FieldInfo field in fields)
                {
                    field.SetValue(copy, field.GetValue(original));
                }
                return copy as T;
            }
        }
    }
}