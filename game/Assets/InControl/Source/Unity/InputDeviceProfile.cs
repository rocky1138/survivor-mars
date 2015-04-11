using System;
using System.Collections.Generic;
using UnityEngine;
using InControl.TinyJSON;


namespace InControl
{
	public abstract class InputDeviceProfile
	{
		[Include]
		public string Name { get; protected set; }

		[Include]
		public string Meta { get; protected set; }

		[Include]
		public InputControlMapping[] AnalogMappings { get; protected set; }

		[Include]
		public InputControlMapping[] ButtonMappings { get; protected set; }

		[Include]
		public string[] SupportedPlatforms { get; protected set; }

		[Include]
		public VersionInfo MinUnityVersion { get; protected set; }

		[Include]
		public VersionInfo MaxUnityVersion { get; protected set; }

		static HashSet<Type> hideList = new HashSet<Type>();

		float sensitivity = 1.0f;
		float lowerDeadZone = 0.0f;
		float upperDeadZone = 1.0f;


		public InputDeviceProfile()
		{
			Name = "";
			Meta = "";

			AnalogMappings = new InputControlMapping[0];
			ButtonMappings = new InputControlMapping[0];

			SupportedPlatforms = new string[0];

			MinUnityVersion = new VersionInfo( 3 );
			MaxUnityVersion = new VersionInfo( 9 );
		}


		[Include]
		public float Sensitivity
		{ 
			get { return sensitivity; }
			protected set { sensitivity = Mathf.Clamp01( value ); }
		}


		[Include]
		public float LowerDeadZone
		{ 
			get { return lowerDeadZone; }
			protected set { lowerDeadZone = Mathf.Clamp01( value ); }
		}


		[Include]
		public float UpperDeadZone
		{ 
			get { return upperDeadZone; }
			protected set { upperDeadZone = Mathf.Clamp01( value ); }
		}


		public bool IsSupportedOnThisPlatform
		{
			get
			{
				if (!IsSupportedOnThisVersionOfUnity)
				{
					return false;
				}

				if (SupportedPlatforms == null || SupportedPlatforms.Length == 0)
				{
					return true;
				}

				foreach (var platform in SupportedPlatforms)
				{
					if (InputManager.Platform.Contains( platform.ToUpper() ))
					{
						return true;
					}
				}

				return false;
			}
		}


		bool IsSupportedOnThisVersionOfUnity
		{
			get
			{
				var unityVersion = VersionInfo.UnityVersion();
				return unityVersion >= MinUnityVersion && unityVersion <= MaxUnityVersion;
			}
		}


		public abstract bool IsKnown { get; }
		public abstract bool IsJoystick { get; }
		public abstract bool HasJoystickName( string joystickName );
		public abstract bool HasLastResortRegex( string joystickName );
		public abstract bool HasJoystickOrRegexName( string joystickName );


		public bool IsNotJoystick
		{ 
			get
			{ 
				return !IsJoystick; 
			} 
		}


		internal static void Hide( Type type )
		{
			hideList.Add( type );
		}


		internal bool IsHidden
		{
			get { return hideList.Contains( GetType() ); }
		}


		public int AnalogCount
		{
			get { return AnalogMappings.Length; }
		}


		public int ButtonCount
		{
			get { return ButtonMappings.Length; }
		}


		#region InputControlSource Helpers

		protected static InputControlSource MouseButton0 = new UnityMouseButtonSource( 0 );
		protected static InputControlSource MouseButton1 = new UnityMouseButtonSource( 1 );
		protected static InputControlSource MouseButton2 = new UnityMouseButtonSource( 2 );
		protected static InputControlSource MouseXAxis = new UnityMouseAxisSource( "x" );
		protected static InputControlSource MouseYAxis = new UnityMouseAxisSource( "y" );
		protected static InputControlSource MouseScrollWheel = new UnityMouseAxisSource( "z" );

		#endregion
	}
}

