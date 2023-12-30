using EventBus.Base.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base.Abstraction
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

        void subscribe<T, TH>() where T : IntegrationEvent where TH : IINtegrationEventHandler<T>;
        void Unsubscribe<T, TH>() where T : IntegrationEvent where TH : IINtegrationEventHandler<T>;


    }
}
