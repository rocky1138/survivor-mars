#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;


namespace InControl
{
	public class XInputDevice : InputDevice
	{
		public int DeviceIndex { get; private set; }
		GamePadState state;


		public XInputDevice( int deviceIndex )
			: base( "Xbox 360 Controller (XInput)" )
		{
			DeviceIndex = deviceIndex;
			SortOrder = deviceIndex;

			Meta = "XInput Device #" + deviceIndex;

			AddControl( InputControlType.LeftStickLeft, "LeftStickLeft" );
			AddControl( InputControlType.LeftStickRight, "LeftStickRight" );
			AddControl( InputControlType.LeftStickUp, "LeftStickUp" );
			AddControl( InputControlType.LeftStickDown, "LeftStickDown" );

			AddControl( InputControlType.RightStickLeft, "RightStickLeft" );
			AddControl( InputControlType.RightStickRight, "RightStickRight" );
			AddControl( InputControlType.RightStickUp, "RightStickUp" );
			AddControl( InputControlType.RightStickDown, "RightStickDown" );

			AddControl( InputControlType.LeftTrigger, "LeftTrigger" );
			AddControl( InputControlType.RightTrigger, "RightTrigger" );

			AddControl( InputControlType.DPadUp, "DPadUp" );
			AddControl( InputControlType.DPadDown, "DPadDown" );
			AddControl( InputControlType.DPadLeft, "DPadLeft" );
			AddControl( InputControlType.DPadRight, "DPadRight" );

			AddControl( InputControlType.Action1, "Action1" );
			AddControl( InputControlType.Action2, "Action2" );
			AddControl( InputControlType.Action3, "Action3" );
			AddControl( InputControlType.Action4, "Action4" );

			AddControl( InputControlType.LeftBumper, "LeftBumper" );
			AddControl( InputControlType.RightBumper, "RightBumper" );

			AddControl( InputControlType.LeftStickButton, "LeftStickButton" );
			AddControl( InputControlType.RightStickButton, "RightStickButton" );

			AddControl( InputControlType.Start, "Start" );
			AddControl( InputControlType.Back, "Back" );

			QueryState();
		}


		public override void Update( ulong updateTick, float deltaTime )
		{
			QueryState();

			UpdateLeftStickWithValue( state.ThumbSticks.Left.Vector, updateTick, deltaTime );
			UpdateRightStickWithValue( state.ThumbSticks.Right.Vector, updateTick, deltaTime );

			UpdateWithValue( InputControlType.LeftTrigger, state.Triggers.Left, updateTick, deltaTime );
			UpdateWithValue( InputControlType.RightTrigger, state.Triggers.Right, updateTick, deltaTime );

			UpdateWithState( InputControlType.DPadUp, state.DPad.Up == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.DPadDown, state.DPad.Down == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.DPadLeft, state.DPad.Left == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.DPadRight, state.DPad.Right == ButtonState.Pressed, updateTick, deltaTime );

			UpdateWithState( InputControlType.Action1, state.Buttons.A == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.Action2, state.Buttons.B == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.Action3, state.Buttons.X == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.Action4, state.Buttons.Y == ButtonState.Pressed, updateTick, deltaTime );

			UpdateWithState( InputControlType.LeftBumper, state.Buttons.LeftShoulder == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.RightBumper, state.Buttons.RightShoulder == ButtonState.Pressed, updateTick, deltaTime );

			UpdateWithState( InputControlType.LeftStickButton, state.Buttons.LeftStick == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.RightStickButton, state.Buttons.RightStick == ButtonState.Pressed, updateTick, deltaTime );

			UpdateWithState( InputControlType.Start, state.Buttons.Start == ButtonState.Pressed, updateTick, deltaTime );
			UpdateWithState( InputControlType.Back, state.Buttons.Back == ButtonState.Pressed, updateTick, deltaTime );

			Commit( updateTick, deltaTime );
		}


		public override void Vibrate( float leftMotor, float rightMotor )
		{
			GamePad.SetVibration( (PlayerIndex) DeviceIndex, leftMotor, rightMotor );
		}


		void QueryState()
		{
			state = GamePad.GetState( (PlayerIndex) DeviceIndex, GamePadDeadZone.Circular );
		}


		public bool IsConnected
		{
			get { return state.IsConnected; }
		}
	}
}
#endif
