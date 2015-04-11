using System;
using System.IO;
using UnityEngine;


namespace InControl
{

	public class DeviceBindingSource : BindingSource
	{
		InputControlType control;


		internal DeviceBindingSource()
		{
		}


		public DeviceBindingSource( InputControlType control )
		{
			this.control = control;
		}


		public override float GetValue( InputDevice inputDevice )
		{
			if (inputDevice == null)
			{
				return 0.0f;
			}

			return inputDevice.GetControl( control ).Value;
		}


		public override bool GetState( InputDevice inputDevice )
		{
			if (inputDevice == null)
			{
				return false;
			}

			return inputDevice.GetControl( control ).State;
		}


		public override string Name
		{ 
			get
			{
				if (BoundTo == null)
				{
					Debug.LogError( "Cannot query property 'Name' for unbound BindingSource." );
					return null;
				}
				else
				{
					var inputDevice = BoundTo.Device;
					var inputControl = inputDevice.GetControl( control );
					if (inputControl == InputControl.Null)
					{
						return control.ToString();
					}
					return inputDevice.GetControl( control ).Handle;
				}
			}
		}


		public override string DeviceName
		{ 
			get
			{
				if (BoundTo == null)
				{
					Debug.LogError( "Cannot query property 'DeviceName' for unbound BindingSource." );
					return null;
				}
				else
				{
					var inputDevice = BoundTo.Device;
					if (inputDevice == InputDevice.Null)
					{
						return "Controller";
					}
					return inputDevice.Name;
				}
			}
		}


		public override bool Equals( BindingSource other )
		{
			if (other == null)
			{
				return false;
			}

			var bindingSource = other as DeviceBindingSource;
			if (bindingSource != null)
			{
				return control == bindingSource.control;
			}

			return false;
		}


		public override bool Equals( object other )
		{
			if (other == null)
			{
				return false;
			}

			var bindingSource = other as DeviceBindingSource;
			if (bindingSource != null)
			{
				return control == bindingSource.control;
			}

			return false;
		}


		public override int GetHashCode()
		{
			return control.GetHashCode();
		}


		internal override BindingSourceType BindingSourceType
		{
			get
			{
				return BindingSourceType.DeviceBindingSource;
			}
		}


		internal override void Save( BinaryWriter writer )
		{
			writer.Write( (int) control );
		}


		internal override void Load( BinaryReader reader )
		{
			control = (InputControlType) reader.ReadInt32();
		}


		internal override bool IsValid
		{ 
			get
			{
				if (BoundTo == null)
				{
					Debug.LogError( "Cannot query property 'IsValid' for unbound BindingSource." );
					return false;
				}
				else
				{
					return BoundTo.Device.HasControl( control );
				}
			}
		}
	}
}

