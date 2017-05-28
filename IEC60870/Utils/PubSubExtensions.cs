using System;

namespace IEC60870.Utils
{
    public static class PubSubExtensions
    {
        private static readonly PubSubHub hub = new PubSubHub();

        public static void Publish<T>(this object obj, string topic, T data)
        {
            hub.Publish(obj, topic, data);
        }

        public static void Subscribe<T>(this object obj, string topic, Action<T> handler)
        {
            hub.Subscribe(obj, topic, handler);
        }
    }
}
