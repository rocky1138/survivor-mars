using System;
using System.IO;
using UnityEngine;


namespace InControl
{
	public class KeyBindingSource : BindingSource
	{
		KeyCombo keyCombo;


		internal KeyBindingSource()
		{
		}


		public KeyBindingSource( KeyCombo keyCombo )
		{
			this.keyCombo = keyCombo;
		}


		public KeyBindingSource( params Key[] keys )
		{
			this.keyCombo = new KeyCombo( keys );
		}


		public override float GetValue( InputDevice inputDevice )
		{
			return GetState( inputDevice ) ? 1.0f : 0.0f;
		}


		public override bool GetState( InputDevice inputDevice )
		{
			return keyCombo.IsPressed;
		}


		public override string Name
		{ 
			get
			{
				return keyCombo.ToString();
			}
		}


		public override string DeviceName
		{ 
			get
			{
				return "Keyboard";
			}
		}


		public override bool Equals( BindingSource other )
		{
			if (other == null)
			{
				return false;
			}

			var bindingSource = other as KeyBindingSource;
			if (bindingSource != null)
			{
				return keyCombo == bindingSource.keyCombo;
			}

			return false;
		}


		public override bool Equals( object other )
		{
			if (other == null)
			{
				return false;
			}

			var bindingSource = other as KeyBindingSource;
			if (bindingSource != null)
			{
				return keyCombo == bindingSource.keyCombo;
			}

			return false;
		}


		public override int GetHashCode()
		{
			return keyCombo.GetHashCode();
		}


		internal override BindingSourceType BindingSourceType
		{
			get
			{
				return BindingSourceType.KeyBindingSource;
			}
		}


		internal override void Load( BinaryReader reader )
		{
			keyCombo.Load( reader );
		}


		internal override void Save( BinaryWriter writer )
		{
			keyCombo.Save( writer );
		}
	}
}

