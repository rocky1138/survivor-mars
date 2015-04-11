using System;


namespace InControl
{
	public class BindingListenOptions
	{
		/// <summary>
		/// Include controllers when listening for new bindings.
		/// </summary>
		public bool IncludeControllers = true;

		/// <summary>
		/// Include non-standard controls on controllers when listening for new bindings.
		/// </summary>
		public bool IncludeNonStandardControls = true;

		/// <summary>
		/// Include mouse buttons when listening for new bindings.
		/// </summary>
		public bool IncludeMouseButtons = false;

		/// <summary>
		/// Include keyboard keys when listening for new bindings.
		/// </summary>
		public bool IncludeKeys = true;

		/// <summary>
		/// Treat modifiers (Shift, Alt, Control, etc.) as first class keys instead of modifiers.
		/// </summary>
		public bool IncludeModifiersAsFirstClassKeys = false;

		/// <summary>
		/// The maximum number of bindings allowed for the action. 
		/// If a new binding is detected and would cause this number to be exceeded, 
		/// enough bindings are removed to make room before adding the new binding.
		/// If set to zero (default), no limit is applied.
		/// </summary>
		public uint MaxAllowedBindings = 0;

		/// <summary>
		/// This function is called when a binding is found but before it is added.
		/// If this function returns <code>false</code>, then the binding is ignored
		/// and listening for new bindings will continue.
		/// If set to <code>null</code> (default), it will not be called.
		/// </summary>
		public Func<PlayerAction, BindingSource, bool> OnBindingFound = null;

		/// <summary>
		/// This action is called after a binding is added.
		/// If set to <code>null</code> (default), it will not be called.
		/// </summary>
		public Action<PlayerAction, BindingSource> OnBindingAdded = null;
	}
}

