// https://gist.github.com/SchreinerK/e05633ef87fb768c6131f3714bae0f45

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace KsWare.Threading {

	public static class AsyncHelper {

		public static Task<TResult> RunAsync<TResult>(Func<TResult> func) => Task.Run<TResult>(() => func());

		public static Task<TResult> RunAsync<TResult, T1>(Func<T1, TResult> func, T1 p1) =>
			Task.Run<TResult>(() => func(p1));

		public static Task<TResult> RunAsync<TResult, T1, T2>(Func<T1, T2, TResult> func, T1 p1, T2 p2) =>
			Task.Run<TResult>(() => func(p1, p2));

		/// <summary>
		/// Executes an async Task{T} method which has a void return value synchronously
		/// </summary>
		/// <param name="task">Task{T} method to execute</param>
		public static void RunSync(Func<Task> task) {
			var oldContext = SynchronizationContext.Current;
			var synch = new ExclusiveSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(synch);
			synch.Post(async _ => {
				try {
					await task();
				}
				catch (Exception e) {
					synch.InnerException = e;
					throw;
				}
				finally {
					synch.EndMessageLoop();
				}
			}, null);
			synch.BeginMessageLoop();

			SynchronizationContext.SetSynchronizationContext(oldContext);
		}

		/// <summary>
		/// Executes an async Task{T} method which has a T return type synchronously
		/// </summary>
		/// <typeparam name="T">Return Type</typeparam>
		/// <param name="task">Task{T} method to execute</param>
		/// <returns></returns>
		public static T RunSync<T>(Func<Task<T>> task) {
			var oldContext = SynchronizationContext.Current;
			var synch = new ExclusiveSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(synch);
			T ret = default(T);
			synch.Post(async _ => {
				try {
					ret = await task();
				}
				catch (Exception e) {
					synch.InnerException = e;
					throw;
				}
				finally {
					synch.EndMessageLoop();
				}
			}, null);
			synch.BeginMessageLoop();
			SynchronizationContext.SetSynchronizationContext(oldContext);
			return ret;
		}

		private class ExclusiveSynchronizationContext : SynchronizationContext {

			private bool _done;
			private readonly AutoResetEvent _workItemsWaiting = new AutoResetEvent(false);

			public Exception InnerException { get; set; }

			readonly Queue<Tuple<SendOrPostCallback, object>> items = new Queue<Tuple<SendOrPostCallback, object>>();

			public override void Send(SendOrPostCallback d, object state) {
				throw new NotSupportedException("We cannot send to our same thread");
			}

			public override void Post(SendOrPostCallback d, object state) {
				lock (items) {
					items.Enqueue(Tuple.Create(d, state));
				}

				_workItemsWaiting.Set();
			}

			public void EndMessageLoop() {
				Post(_ => _done = true, null);
			}

			public void BeginMessageLoop() {
				while (!_done) {
					Tuple<SendOrPostCallback, object> task = null;
					lock (items) {
						if (items.Count > 0) {
							task = items.Dequeue();
						}
					}

					if (task != null) {
						task.Item1(task.Item2);
						if (InnerException != null) // the method threw an exception
						{
							throw new AggregateException("AsyncHelper.Run method threw an exception.", InnerException);
						}
					}
					else {
						_workItemsWaiting.WaitOne();
					}
				}
			}

			public override SynchronizationContext CreateCopy() {
				return this;
			}
		}

	}

}
