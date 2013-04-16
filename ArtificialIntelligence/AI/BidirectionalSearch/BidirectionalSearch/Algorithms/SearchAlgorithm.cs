using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SearchAlgorithms.Model
{
    public class SearchAlgorithm
    {
        protected Graph graph;
        protected Vertex root;
        protected Vertex goal;

        public Action<object> searchDidFinishedWithData;

        public Thread ActionThread { get; protected set; }

        protected void ExecuteInBiggerStackThread<T>(Action<T> action, T parameterObject)
        {
            this.ActionThread = new Thread(() => action(parameterObject), 1024 * 1024);
            this.ActionThread.Start();
            this.ActionThread.Join();
        }

        public virtual void AsynchronousSearch(Graph graph, Vertex root, Vertex goal)
        {
            
        }
    }
}
