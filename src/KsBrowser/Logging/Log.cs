using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KsWare.KsBrowser.CefSpecific;

namespace KsWare.KsBrowser.Logging {

	public class Log {
		private static long _lastGlobalId;
		private static readonly Dictionary<Type, int> _instanceIds = new Dictionary<Type, int>();

		private long _lastLocalId;

		public Log(object o, string className=null) {
			var isStatic = false;
			if (o is Type type) isStatic = true;
			else type = o.GetType();

			ClassName = className ?? type.Name;		
			
			if (isStatic) {
				InstanceId = 0;
			}
			else {
				lock (_instanceIds) {
					if (!_instanceIds.TryGetValue(type, out var instanceId)) {
						_instanceIds.Add(type,1); InstanceId=1;
					}
					else {
						InstanceId = ++instanceId;
						_instanceIds[type] = InstanceId;
					}
				}
						
			}


		}

		private long GetLocalId() => Interlocked.Increment(ref _lastLocalId);
		private static long GetGlobalId() => Interlocked.Increment(ref _lastGlobalId);

		public int InstanceId { get; set; }
		public string ClassName { get; set; }

		public void Method(string message=null, [CallerMemberName] string callerMemberName = null) {
			string time = $"{DateTime.Now:HH:mm:ss.fff}";
			string className = ClassName + (InstanceId > 0 ? $"${InstanceId}" : "");

			Debug.WriteLine($"{time} [{Environment.CurrentManagedThreadId,2}] {className}.{callerMemberName} {message}");
			new LogEntry {
				Log = this,
				GlobalId = GetGlobalId(),
				LocalId = GetLocalId(),
				Timestamp = DateTime.Now,
				ThreadId = Environment.CurrentManagedThreadId,
			};
		}

		public void Message(string message) {
			Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} [{Environment.CurrentManagedThreadId,2}]   {message}");
		}
	}

	public class LogEntry {
		public Log Log { get; set; }
		public long GlobalId { get; set; }
		public long LocalId { get; set; }
		public DateTime Timestamp { get; set; }
		public int ThreadId { get; set; }
		// public string ClassName { get; set; }
		public string ClassName => Log.ClassName;
		public int InstanceId => Log.InstanceId;
		public string MethodName { get; set; }
		public string Message { get; set; }
	}

}
