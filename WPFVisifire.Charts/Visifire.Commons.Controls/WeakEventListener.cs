using System;

namespace Visifire.Commons.Controls
{
	internal class WeakEventListener<TInstance, TSource, TEventArgs> where TInstance : class
	{
		private WeakReference _weakInstance;

		public Action<TInstance, TSource, TEventArgs> OnEventAction
		{
			get;
			set;
		}

		public Action<WeakEventListener<TInstance, TSource, TEventArgs>> OnDetachAction
		{
			get;
			set;
		}

		public WeakEventListener(TInstance instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			this._weakInstance = new WeakReference(instance);
		}

		public void OnEvent(TSource source, TEventArgs eventArgs)
		{
			TInstance tInstance = (TInstance)((object)this._weakInstance.Target);
			if (tInstance != null)
			{
				if (this.OnEventAction != null)
				{
					this.OnEventAction(tInstance, source, eventArgs);
					return;
				}
			}
			else
			{
				this.Detach();
			}
		}

		public void Detach()
		{
			if ((TSource)((object)this._weakInstance.Target) != null && this.OnDetachAction != null)
			{
				this.OnDetachAction(this);
				this.OnDetachAction = null;
			}
		}
	}
}
