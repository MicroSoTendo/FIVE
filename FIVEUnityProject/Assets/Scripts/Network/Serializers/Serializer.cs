using System;
using System.Collections.Generic;
using static FIVE.Util;
namespace FIVE.Network.Serializers
{
    public abstract class Serializer 
    {
        private static readonly Dictionary<Type, Serializer> Serializers = new Dictionary<Type, Serializer>();
        static Serializer()
        {
            foreach (Type type in GetDerived(typeof(Serializer)))
            {
                Type componentType = type.BaseType.GenericTypeArguments[0];
                Serializers.Add(componentType, type.GetConstructor(Type.EmptyTypes).Invoke(null) as Serializer);
            }
        }

        public static Serializer<T> Get<T>()
        {
            return (Serializer<T>)Serializers[typeof(T)];
        }
        
    }

    public abstract class Serializer<T> : Serializer
    {
        public abstract void Serialize(in T obj, out byte[] bytes);
        public abstract void Deserialize(in byte[] bytes, T obj);
        
    }
}
