using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using Andamio;
using Andamio.Collections;
using Andamio.Data;
using Andamio.Data.Access;
using Andamio.Data.Transactions;

namespace Andamio.Messaging.Sinks
{
    public class SinkPipeline : CollectionBase<IMessageSink>
    {
        #region Constructors
        internal SinkPipeline()
        {

        }

        internal SinkPipeline(IEnumerable<IMessageSink> sink)
            : base(sink)
        {

        }

        #endregion        

        #region Invocation
        internal void InvokeTransactional(RequestMessage request)
        {
            if (request == null) throw new ArgumentNullException("request");
            this.ForEach(sink => sink.Invoke(request));

            //using (TransactionWrapper transaction = TransactionWrapper.NoLock())
            //{
            //    this.ForEach(sink => sink.Invoke(request));
            //    transaction.Complete();
            //}
        }

        internal void InvokeTransactional(ReplyMessage reply)
        {
            if (reply == null) throw new ArgumentNullException("reply");
            this.ForEach(sink => sink.Invoke(reply));

            //using (TransactionWrapper transaction = TransactionWrapper.NoLock())
            //{
            //    this.ForEach(sink => sink.Invoke(reply));
            //    transaction.Complete();
            //}
        }

        #endregion

        #region Items
        public void Add(params IMessageSink[] sinks)
        {
            if (sinks != null)
            {
                AddRange(sinks);
            }
        }

        #endregion
    }
    

    public interface IMessageSink
    {
        void Invoke(RequestMessage request);
        void Invoke(ReplyMessage reply);
    }
}
