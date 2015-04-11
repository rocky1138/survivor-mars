using System;
using System.IO;
using UnityEngine;


namespace InControl
{
	public class MouseBindingSource : BindingSource
	{
		Mouse mouseControl;


		internal MouseBindingSource()
		{
		}


		public MouseBindingSource( Mouse mouseControl )
		{
			this.mouseControl = mouseControl;
		}


		public override float GetValue( InputDevice inputDevice )
		{
			var scale = 0.2f;

			switch (mouseControl)
			{
				case Mouse.LeftButton:
					return Input.GetMouseButton( 0 ) ? 1.0f : 0.0f;
				case Mouse.RightButton:
					return Input.GetMouseButton( 1 ) ? 1.0f : 0.0f;
				case Mouse.MiddleButton:
					return Input.GetMouseButton( 2 ) ? 1.0f : 0.0f;
				case Mouse.NegativeX:
					return -Mathf.Min( Input.GetAxisRaw( "mouse x" ) * scale, 0.0f );
				case Mouse.PositiveX:
					return Mathf.Max( 0.0f, Input.GetAxisRaw( "mouse x" ) * scale );
				case Mouse.NegativeY:
					return -Mathf.Min( Input.GetAxisRaw( "mouse y" ) * scale, 0.0f );
				case Mouse.PositiveY:
					return Mathf.Max( 0.0f, Input.GetAxisRaw( "mouse y" ) * scale );
				case Mouse.NegativeScrollWheel:
					return -Mathf.Min( Input.GetAxisRaw( "mouse z" ) * scale, 0.0f );
				case Mouse.PositiveScrollWheel:
					return Mathf.Max( 0.0f, Input.GetAxisRaw( "mouse z" ) * scale );
			}

			return 0.0f;
		}


		public override bool GetState( InputDevice inputDevice )
		{
			return !Mathf.Approximately( GetValue( inputDevice ), 0.0f );
		}


		public override string Name
		{ 
			get
			{
				return mouseControl.ToString();
			}
		}


		public override string DeviceName
		{ 
			get
			{
				return "Mouse";
			}
		}


		public override bool Equals( BindingSource other )
		{
			if (other == null)
			{
				return false;
			}

			var bindingSource = other as MouseBindingSource;
			if (bindingSource != null)
			{
				return mouseControl == bindingSource.mouseControl;
			}

			return false;
		}


		public override bool Equals( object other )
		{
			if (other == null)
			{
				return false;
			}

			var bindingSource = other as MouseBindingSource;
			if (bindingSource != null)
			{
				return mouseControl == bindingSource.mouseControl;
			}

			return false;
		}


		public override int GetHashCode()
		{
			return mouseControl.GetHashCode();
		}


		internal override BindingSourceType BindingSourceType
		{
			get
			{
				return BindingSourceType.MouseBindingSource;
			}
		}


		internal override void Save( BinaryWriter writer )
		{
			writer.Write( (int) mouseControl );
		}


		internal override void Load( BinaryReader reader )
		{
			mouseControl = (Mouse) reader.ReadInt32();
		}
	}
}

