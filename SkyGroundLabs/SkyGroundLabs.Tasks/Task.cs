using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkyGroundLabs.Tasks
{
	public abstract class Task
	{
		protected List<string> _errors;
		public IEnumerable<string> Errors { get { return _errors; } }

		public string Name { get; protected set; }
		public double Health { get { return (_errors == null ? 100 : (100 - (_errors.Count * 2))); } }
		public string State { get { return _thread.ThreadState.ToString(); } }
		public bool IsAlive { get { return _thread.IsAlive; } }
		public bool IsBackground { get; protected set; }
		public DateTime StartDate { get; protected set; }
		public abstract bool IsRestartable { get; }
		public abstract bool IsDisposable { get; }
		public abstract bool IsTimed { get; }
		public bool HasParameters { get { return _threadStart == null && _pThreadStart != null; } }
		protected ParameterizedThreadStart _pThreadStart { get; set; }
		protected ThreadStart _threadStart { get; set; }
		protected Thread _thread { get; set; }

		public abstract void Kill();

		public void Start(params object[] parameters)
		{
			if (_thread.ThreadState.ToString().Contains("Unstarted"))
			{
				if (parameters == null)
				{
					_thread.Start();
				}
				else
				{
					_thread.Start(parameters);
				}
				StartDate = DateTime.Now;
			}
			else if (!IsAlive && IsRestartable)
			{
				if (_threadStart != null)
				{
					_thread = new Thread(_threadStart);
					_thread.Name = Name;
					_thread.IsBackground = IsBackground;
					_thread.Start();
				}
				else
				{
					_thread = new Thread(_pThreadStart);
					_thread.Name = Name;
					_thread.IsBackground = IsBackground;
					_thread.Start(parameters);
				}
				StartDate = DateTime.Now;
			}
		}

		public void Start()
		{
			Start(null);
		}
	}
}
