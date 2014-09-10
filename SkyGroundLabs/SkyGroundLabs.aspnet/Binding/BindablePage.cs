using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SkyGroundLabs.Reflection;

namespace SkyGroundLabs.aspnet.Binding
{
	public partial class BindablePage : System.Web.UI.Page
	{
		#region Properties
		public static object DataContext { get; set; }
		#endregion

		#region Methods
		public void PushDataContextToPage()
		{
			if (DataContext == null)
			{
				return;
			}

			List<IBindable> controls = new List<IBindable>();

			_getControlList(Page.Controls, controls);

			// This needs to be tested
			foreach (var control in controls)
			{
				object value = ReflectionManager.GetPropertyValue(DataContext, ((IBindable)control).Path);
				value = _convertPushValue(((IBindable)control).PushConversion, value);
				ReflectionManager.SetPropertyValue(((IBindable)control), ((IBindable)control).PropertyName, value);
			}
		}

		public void ClearPage()
		{
			List<IBindable> controls = new List<IBindable>();

			_getControlList(Page.Controls, controls);

			// This needs to be tested
			foreach (var control in controls)
			{
				if (control is BindableDropDownList)
				{
					ReflectionManager.SetPropertyValue(((ListControl)control), ((IBindable)control).PropertyName, 0);
				}
				else
				{
					ReflectionManager.SetPropertyValue(((IBindable)control), ((IBindable)control).PropertyName, "");
				}
			}
		}

		private object _convertPushValue(PushConverter converter, object value)
		{
			switch (converter)
			{
				case PushConverter.DateTimeToShortDateString:
					return Convert.ToDateTime(value).ToShortDateString();
				case PushConverter.BoolToInt:
					return ((bool)value ? 1 : 0);
				case PushConverter.NumberToDecimalPrecision2:
					return Convert.ToDecimal(value).ToString("0.00");
			}

			return value;
		}

		private object _convertPullValue(PullConverter converter, object value)
		{
			switch (converter)
			{
				case PullConverter.IntToBool:
					return Convert.ToInt64(value) == 1;
			}

			return value;
		}

		private object _getDefaultFromValueType(object value)
		{
			Type type = value.GetType();

			if (type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}

		public void PullDataContextFromPage()
		{
			if (DataContext == null)
			{
				return;
			}

			List<IBindable> controls = new List<IBindable>();

			_getControlList(Page.Controls, controls);

			foreach (var control in controls)
			{
				object value = ReflectionManager.GetPropertyValue(control, ((IBindable)control).PropertyName);
				object toValue = ReflectionManager.GetPropertyValue(DataContext, ((IBindable)control).Path);
				value = _convertPullValue(((IBindable)control).PullConversion, value);

				if (value != null && toValue != null && string.IsNullOrWhiteSpace(value.ToString()))
				{
					// get the default for the value type from the data context
					value = _getDefaultFromValueType(toValue);
				}

				ReflectionManager.SetPropertyValue(DataContext, ((IBindable)control).Path, value);
			}
		}

		private void _getControlList(ControlCollection controlCollection, List<IBindable> resultCollection)
		{
			foreach (Control control in controlCollection)
			{
				if (control is BindableTextBox)
				{
					_addControlToList((BindableTextBox)control, resultCollection);
				}
				else if (control is BindableDropDownList)
				{
					_addControlToList((BindableDropDownList)control, resultCollection);
				}

				if (control.HasControls())
				{
					_getControlList(control.Controls, resultCollection);
				}
			}
		}

		private void _addControlToList<T>(T control, List<IBindable> resultCollection)
			where T : IBindable
		{
			if (!String.IsNullOrWhiteSpace(((IBindable)control).Path) &&
				!String.IsNullOrWhiteSpace(((IBindable)control).PropertyName))
			{
				resultCollection.Add(control);
			}
		}
		#endregion
	}
}
