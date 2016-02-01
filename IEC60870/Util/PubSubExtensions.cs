using System;

namespace IEC60870.Util
{
    static public class PubSubExtensions
    {
        static private readonly PubSubHub hub = new PubSubHub();

        static public void Publish<T>(this object obj, string topic, T data)
        {
            hub.Publish(obj, topic, data);
        }

        static public void Subscribe<T>(this object obj, string topic, Action<T> handler)
        {
            hub.Subscribe(obj, topic, handler);
        }
    }
}
