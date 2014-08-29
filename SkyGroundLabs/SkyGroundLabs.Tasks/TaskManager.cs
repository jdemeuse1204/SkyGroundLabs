using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkyGroundLabs.Tasks
{
	public static class TaskManager
	{
		#region Properties
		private static ObservableCollection<Task> _tasks;
		public static IEnumerable<Task> Tasks { get { return _tasks; } }
		private static System.Timers.Timer _refreshTimer { get; set; }
		private static object _lock = new object();
		private const int REFRESH_INTERVAL = 5000;
		public static event NotifyUpdateEndedHandler OnUpdateEnded;
		public delegate void NotifyUpdateEndedHandler(TaskEventArgs e);
		public static event NotifyTaskStartedHandler OnTaskRun;
		public delegate void NotifyTaskStartedHandler(TaskEventArgs e);
		#endregion

		#region Constructors
		public static void AddTask(string name, ThreadStart threadStart, bool isBackground = true, bool isRestartable = false)
		{
			if (_initializeList(name))
			{
				_tasks.Add(isRestartable ?
					new RestartableTask(name, threadStart, isBackground) :
					new StaticTask(name, threadStart, isBackground));
			}

		}

		public static void AddTask(string name, ParameterizedThreadStart threadStart, bool isBackground = true, bool isRestartable = false)
		{
			if (_initializeList(name))
			{
				_tasks.Add(isRestartable ?
					new RestartableTask(name, threadStart, isBackground) :
					new StaticTask(name, threadStart, isBackground));
			}
		}

		public static void AddTimedRestartableTask(string name, ParameterizedThreadStart threadStart, int secondsInterval = 60, bool isBackground = true)
		{
			if (_initializeList(name))
			{
				_tasks.Add(new TimedRestartableTask(name, threadStart, secondsInterval, isBackground));
			}
		}

		public static void AddTimedRestartableTask(string name, ThreadStart threadStart, int secondsInterval = 60, bool isBackground = true)
		{
			if (_initializeList(name))
			{
				_tasks.Add(new TimedRestartableTask(name, threadStart, secondsInterval, isBackground));
			}
		}
		#endregion

		#region Methods
		private static bool _initializeList(string name)
		{
			if (_tasks == null)
			{
				_refreshTimer = new System.Timers.Timer(REFRESH_INTERVAL);
				_refreshTimer.Elapsed += _refreshTimer_Elapsed;
				_refreshTimer.Start();
				_tasks = new ObservableCollection<Task>();
			}

			return !_containsName(name);
		}

		private static bool _containsName(string name)
		{
			return _tasks.Where(w => w.Name == name).Count() >= 1;
		}

		private static void _refreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			lock (_lock)
			{
				// look for timed
				var timedTasks = _tasks.Where(w => w.IsTimed == true);

				foreach (TimedRestartableTask task in timedTasks)
				{
					if (task.EndDate == DateTime.MinValue)
					{
						task.EndDate = DateTime.Now;
					}
					else
					{
						TimeSpan span = DateTime.Now - task.EndDate;
						if (span.TotalSeconds > task.SecondsInterval)
						{
							task.EndDate = DateTime.MinValue;
							task.Start();
						}
					}
				}

				if (OnUpdateEnded != null)
				{
					OnUpdateEnded(new TaskEventArgs()
					{
						Action = "Internal Refresh Timer",
						TaskName = "All",
						Parameters = null
					});
				}
			}
		}

		public static void RunTask(string name, params object[] parameters)
		{
			if (OnTaskRun != null)
			{
				OnTaskRun(new TaskEventArgs()
				{
					Action = "Run Task",
					TaskName = name,
					Parameters = parameters
				});
			}

			_tasks.Where(w => w.Name == name).FirstOrDefault().Start(parameters);
		}

		public static void RunTask(string name)
		{
			if (OnTaskRun != null)
			{
				OnTaskRun(new TaskEventArgs()
				{
					Action = "Run Task",
					TaskName = name,
					Parameters = null
				});
			}

			_tasks.Where(w => w.Name == name).FirstOrDefault().Start();
		}

		public static void EndTask(string name)
		{
			_tasks.Where(w => w.Name == name).FirstOrDefault().Kill();
		}

		public static bool IsAlive(string name)
		{
			var task = _tasks.Where(w => w.Name == name).FirstOrDefault();

			if (task != null)
			{
				return task.IsAlive;
			}

			return false;
		}
		#endregion

		#region Internal Classes
		class DisposableTask : StaticTask
		{
			#region Properties
			public override bool IsRestartable { get { return this is RestartableTask; } }
			public override bool IsTimed { get { return this is TimedRestartableTask; } }
			public override bool IsDisposable { get { return this is DisposableTask; } }
			#endregion

			#region Constructor
			public DisposableTask(string name, ParameterizedThreadStart threadStart, bool isBackground = true)
				: base(name, threadStart, isBackground)
			{
				IsBackground = isBackground;
				_thread = new Thread(threadStart);
				_thread.Name = name;
				_thread.IsBackground = isBackground;
				StartDate = DateTime.MinValue;
				Name = name;
				_pThreadStart = threadStart;
			}

			public DisposableTask(string name, ThreadStart threadStart, bool isBackground = true)
				: base(name, threadStart, isBackground)
			{
				IsBackground = isBackground;
				_thread = new Thread(threadStart);
				_thread.Name = name;
				_thread.IsBackground = isBackground;
				StartDate = DateTime.MinValue;
				Name = name;
				_threadStart = threadStart;
			}
			#endregion

			#region Methods
			public override void Kill()
			{
				try
				{
					_thread.Abort();
				}
				catch (Exception)
				{

					throw;
				}
			}
			#endregion
		}

		class StaticTask : Task
		{
			#region Properties
			public override bool IsRestartable { get { return this is RestartableTask; } }
			public override bool IsTimed { get { return this is TimedRestartableTask; } }
			public override bool IsDisposable { get { return this is DisposableTask; } }
			#endregion

			#region Constructor
			public StaticTask(string name, ParameterizedThreadStart threadStart, bool isBackground = true)
			{
				IsBackground = isBackground;
				_thread = new Thread(threadStart);
				_thread.Name = name;
				_thread.IsBackground = isBackground;
				StartDate = DateTime.MinValue;
				Name = name;
				_pThreadStart = threadStart;
			}

			public StaticTask(string name, ThreadStart threadStart, bool isBackground = true)
			{
				IsBackground = isBackground;
				_thread = new Thread(threadStart);
				_thread.Name = name;
				_thread.IsBackground = isBackground;
				StartDate = DateTime.MinValue;
				Name = name;
				_threadStart = threadStart;
			}
			#endregion

			#region Methods
			public override void Kill()
			{
				try
				{
					_thread.Abort();
				}
				catch (Exception)
				{

					throw;
				}
			}
			#endregion
		}

		class RestartableTask : StaticTask
		{
			#region Properties
			public override bool IsRestartable { get { return this is RestartableTask; } }
			public override bool IsTimed { get { return this is TimedRestartableTask; } }
			public override bool IsDisposable { get { return this is DisposableTask; } }
			#endregion

			#region Constructor
			public RestartableTask(string name, ThreadStart threadStart, bool isBackground = true)
				: base(name, threadStart, isBackground) { }

			public RestartableTask(string name, ParameterizedThreadStart threadStart, bool isBackground = true)
				: base(name, threadStart, isBackground) { }
			#endregion
		}

		class TimedRestartableTask : RestartableTask
		{
			#region Properties
			public DateTime EndDate { get; set; }
			public int SecondsInterval { get; private set; }
			public override bool IsRestartable { get { return this is RestartableTask; } }
			public override bool IsTimed { get { return this is TimedRestartableTask; } }
			public override bool IsDisposable { get { return this is DisposableTask; } }
			#endregion

			#region Constructors
			public TimedRestartableTask(string name, ThreadStart threadStart, int secondsInterval = 60, bool isBackground = true)
				: base(name, threadStart, isBackground)
			{
				SecondsInterval = secondsInterval;
			}

			public TimedRestartableTask(string name, ParameterizedThreadStart threadStart, int secondsInterval = 60, bool isBackground = true)
				: base(name, threadStart, isBackground)
			{
				SecondsInterval = secondsInterval;
			}
			#endregion
		}
		#endregion
	}
}
