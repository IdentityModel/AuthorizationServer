using System;
using System.Diagnostics;

namespace Thinktecture.AuthorizationServer
{
    public class TracingScope : IDisposable
    {
        string _activityName;

        Guid _id;


        public TracingScope(string activityName)
        {
            _id = Trace.CorrelationManager.ActivityId;
            var newId = Guid.NewGuid();

            Tracing.Transfer(activityName, Guid.NewGuid());
            Trace.CorrelationManager.ActivityId = newId;
            

            _activityName = activityName;
            Tracing.Start(_activityName);

            //_id = Trace.CorrelationManager.ActivityId;
        }

        public void Dispose()
        {
            Tracing.Stop(_activityName);
            Tracing.Transfer(_activityName, _id);

            Trace.CorrelationManager.ActivityId = _id;
        }
    }
}
