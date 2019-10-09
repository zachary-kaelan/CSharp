using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib.ReferentialObjects
{
    public abstract class ReferentialObject : IObservable<object>, IObserver<object>
    {
        public abstract void OnCompleted();
        public abstract void OnError(Exception error);
        public abstract void OnNext(object value);

        public IDisposable Subscribe(IObserver<object> observer)
        {
            throw new NotImplementedException();
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<object>> _subListRef;
            private IObserver<object> _observerRef;

            public Unsubscriber(List<IObserver<object>> subListRef, IObserver<object> observerRef)
            {
                this._subListRef = subListRef;
                this._observerRef = observerRef;
            }

            public void Dispose()
            {
                if (_observerRef != null && _subListRef != null && _subListRef.Contains(_observerRef))
                    _subListRef.Remove(_observerRef);
            }
        }
    }
}
