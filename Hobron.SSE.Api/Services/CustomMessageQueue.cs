using Hobron.SSE.Api.Dto;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Hobron.SSE.Api.Services
{
    public interface ICustomMessageQueue
    {
        bool Register(string id);
        void Unregister(string id);
        ICollection<string> Keys { get; }
        IAsyncEnumerable<string> DequeueAsync(string id, CancellationToken cancellationToken = default);
        IAsyncEnumerable<string> DequeueAsync(CancellationToken cancellationToken = default);
        Task EnqueueSync(Notification notification, CancellationToken cancellationToken = default);
    }
    public class CustomMessageQueue : ICustomMessageQueue
    {
        private ConcurrentDictionary<string, Channel<string>> _concurrentDictionary;

        public CustomMessageQueue()
        {
            _concurrentDictionary = new ConcurrentDictionary<string, Channel<string>>();
        }
        public ICollection<string> Keys => _concurrentDictionary.Keys;


        public IAsyncEnumerable<string> DequeueAsync(string id, CancellationToken cancellationToken = default)
        {
            var success = _concurrentDictionary.TryGetValue(id, out Channel<string>? channel);
            if ((success) && (channel != null))
            {
                return channel.Reader.ReadAllAsync(cancellationToken);
            }
            else
            {
                throw new ArgumentNullException($"Client with {id} is not registered");
            }
        }

        public async IAsyncEnumerable<string> DequeueAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
           // IAsyncEnumerable<string> result;
            foreach (var keyValuePair in _concurrentDictionary)
            {
                await foreach(string str in DequeueAsync(keyValuePair.Key, cancellationToken))
                {
                    yield return str;
                }
            }
        }

        public async Task EnqueueSync(Notification notification, CancellationToken cancellationToken = default)
        {
            bool success = _concurrentDictionary.TryGetValue(notification.Id, out Channel<string>? channel);
            if ((success) && (channel != null))
            {                
                await channel.Writer.WriteAsync(notification.Message, cancellationToken);
            }
            else
            {
                throw new ArgumentException($"Error occurred when adding a new message to the queue");
            }
        }

        public bool Register(string id)
        {
            return _concurrentDictionary.TryAdd(id, Channel.CreateUnbounded<string>());
        }

        public void Unregister(string id)
        {
            _concurrentDictionary.TryRemove(id, out _);
        }
    }
}
