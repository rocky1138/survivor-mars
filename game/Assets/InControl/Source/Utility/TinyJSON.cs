using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System;


namespace InControl.TinyJSON
{
	// @cond nodoc
	public sealed class Decoder : IDisposable
	{
		const string WhiteSpace = " \t\n\r";
		const string WordBreak = " \t\n\r{}[],:\"";

		enum Token
		{
			None,
			OpenBrace,
			CloseBrace,
			OpenBracket,
			CloseBracket,
			Colon,
			Comma,
			String,
			Number,
			True,
			False,
			Null
		}

		StringReader json;

		Decoder( string jsonString )
		{
			json = new StringReader( jsonString );
		}

		public static Variant Decode( string jsonString )
		{
			using (var instance = new Decoder( jsonString ))
			{
				return instance.DecodeValue();
			}
		}

		public void Dispose()
		{
			json.Dispose();
			json = null;
		}

		ProxyObject DecodeObject()
		{
			var proxy = new ProxyObject();

			// Ditch opening brace.
			json.Read();

			// {
			while (true)
			{
				switch (NextToken)
				{
					case Token.None:
						return null;

					case Token.Comma:
						continue;

					case Token.CloseBrace:
						return proxy;

					default:
						// Key
						string key = DecodeString();
						if (key == null)
						{
							return null;
						}

						// :
						if (NextToken != Token.Colon)
						{
							return null;
						}
						json.Read();

						// Value
						proxy.Add( key, DecodeValue() );
						break;
				}
			}
		}

		ProxyArray DecodeArray()
		{
			ProxyArray proxy = new ProxyArray();

			// Ditch opening bracket.
			json.Read();

			// [
			var parsing = true;
			while (parsing)
			{
				Token nextToken = NextToken;

				switch (nextToken)
				{
					case Token.None:
						return null;

					case Token.Comma:
						continue;

					case Token.CloseBracket:
						parsing = false;
						break;

					default:
						proxy.Add( DecodeByToken( nextToken ) );
						break;
				}
			}

			return proxy;
		}

		Variant DecodeValue()
		{
			var nextToken = NextToken;
			return DecodeByToken( nextToken );
		}

		Variant DecodeByToken( Token token )
		{
			switch (token)
			{
				case Token.String:
					return DecodeString();

				case Token.Number:
					return DecodeNumber();

				case Token.OpenBrace:
					return DecodeObject();

				case Token.OpenBracket:
					return DecodeArray();

				case Token.True:
					return new ProxyBoolean( true );

				case Token.False:
					return new ProxyBoolean( false );

				case Token.Null:
					return null;

				default:
					return null;
			}
		}

		Variant DecodeString()
		{
			var stringBuilder = new StringBuilder();
			char c;

			// ditch opening quote
			json.Read();

			bool parsing = true;
			while (parsing)
			{
				if (json.Peek() == -1)
				{
					parsing = false;
					break;
				}

				c = NextChar;
				switch (c)
				{
					case '"':
						parsing = false;
						break;

					case '\\':
						if (json.Peek() == -1)
						{
							parsing = false;
							break;
						}

						c = NextChar;
						switch (c)
						{
							case '"':
							case '\\':
							case '/':
								stringBuilder.Append( c );
								break;

							case 'b':
								stringBuilder.Append( '\b' );
								break;

							case 'f':
								stringBuilder.Append( '\f' );
								break;

							case 'n':
								stringBuilder.Append( '\n' );
								break;

							case 'r':
								stringBuilder.Append( '\r' );
								break;

							case 't':
								stringBuilder.Append( '\t' );
								break;

							case 'u':
								var hex = new StringBuilder();

								for (int i = 0; i < 4; i++)
								{
									hex.Append( NextChar );
								}

								stringBuilder.Append( (char) Convert.ToInt32( hex.ToString(), 16 ) );
								break;
						}
						break;

					default:
						stringBuilder.Append( c );
						break;
				}
			}

			return new ProxyString( stringBuilder.ToString() );
		}

		Variant DecodeNumber()
		{
			return new ProxyNumber( NextWord );
		}

		void ConsumeWhiteSpace()
		{
			while (WhiteSpace.IndexOf( PeekChar ) != -1)
			{
				json.Read();

				if (json.Peek() == -1)
				{
					break;
				}
			}
		}

		char PeekChar
		{
			get
			{
				var peek = json.Peek();
				return peek == -1 ? '\0' : Convert.ToChar( peek );
			}
		}

		char NextChar
		{
			get
			{
				return Convert.ToChar( json.Read() );
			}
		}

		string NextWord
		{
			get
			{
				StringBuilder word = new StringBuilder();

				while (WordBreak.IndexOf( PeekChar ) == -1)
				{
					word.Append( NextChar );

					if (json.Peek() == -1)
					{
						break;
					}
				}

				return word.ToString();
			}
		}

		Token NextToken
		{
			get
			{
				ConsumeWhiteSpace();

				if (json.Peek() == -1)
				{
					return Token.None;
				}

				switch (PeekChar)
				{
					case '{':
						return Token.OpenBrace;

					case '}':
						json.Read();
						return Token.CloseBrace;

					case '[':
						return Token.OpenBracket;

					case ']':
						json.Read();
						return Token.CloseBracket;

					case ',':
						json.Read();
						return Token.Comma;

					case '"':
						return Token.String;

					case ':':
						return Token.Colon;

					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
					case '-':
						return Token.Number;
				}

				switch (NextWord)
				{
					case "false":
						return Token.False;

					case "true":
						return Token.True;

					case "null":
						return Token.Null;
				}

				return Token.None;
			}
		}
	}


	[Flags]
	public enum EncodeOptions
	{
		None = 0,
		PrettyPrint = 1,
		NoTypeHints = 2
	}


	public sealed class Encoder
	{
		static readonly Type includeAttrType = typeof(Include);
		static readonly Type excludeAttrType = typeof(Exclude);
		static readonly Type typeHintAttrType = typeof(TypeHint);

		StringBuilder builder;
		EncodeOptions options;
		int indent;

		Encoder( EncodeOptions options )
		{
			this.options = options;
			builder = new StringBuilder();
			indent = 0;
		}

		public static string Encode( object obj, EncodeOptions options = EncodeOptions.None )
		{
			var instance = new Encoder( options );
			instance.EncodeValue( obj, false );
			return instance.builder.ToString();
		}

		bool PrettyPrintEnabled
		{
			get
			{
				return ((options & EncodeOptions.PrettyPrint) == EncodeOptions.PrettyPrint);
			}
		}

		bool TypeHintsEnabled
		{
			get
			{
				return ((options & EncodeOptions.NoTypeHints) != EncodeOptions.NoTypeHints);
			}
		}

		void EncodeValue( object value, bool forceTypeHint )
		{
			Array asArray;
			IList asList;
			IDictionary asDict;
			string asString;

			if (value == null)
			{
				builder.Append( "null" );
			}
			else
			if ((asString = value as string) != null)
			{
				EncodeString( asString );
			}
			else
			if (value is bool)
			{
				builder.Append( value.ToString().ToLower() );
			}
			else
			if (value is Enum)
			{
				EncodeString( value.ToString() );
			}
			else
			if ((asArray = value as Array) != null)
			{
				EncodeArray( asArray, forceTypeHint );
			}
			else
			if ((asList = value as IList) != null)
			{
				EncodeList( asList, forceTypeHint );
			}
			else
			if ((asDict = value as IDictionary) != null)
			{
				EncodeDictionary( asDict, forceTypeHint );
			}
			else
			if (value is char)
			{
				EncodeString( value.ToString() );
			}
			else
			{
				EncodeOther( value, forceTypeHint );
			}
		}

		void EncodeObject( object value, bool forceTypeHint )
		{
			var type = value.GetType();

			AppendOpenBrace();

			forceTypeHint = forceTypeHint || TypeHintsEnabled;

			var firstItem = !forceTypeHint;
			if (forceTypeHint)
			{
				if (PrettyPrintEnabled)
				{
					AppendIndent();
				}
				EncodeString( ProxyObject.TypeHintName );
				AppendColon();
				EncodeString( type.FullName );
				firstItem = false;
			}

			var fields = type.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			foreach (var field in fields)
			{
				var shouldTypeHint = false;
				var shouldEncode = field.IsPublic;
				foreach (var attribute in field.GetCustomAttributes( true ))
				{
					if (excludeAttrType.IsAssignableFrom( attribute.GetType() ))
					{
						shouldEncode = false;
					}

					if (includeAttrType.IsAssignableFrom( attribute.GetType() ))
					{
						shouldEncode = true;
					}

					if (typeHintAttrType.IsAssignableFrom( attribute.GetType() ))
					{
						shouldTypeHint = true;
					}
				}

				if (shouldEncode)
				{
					AppendComma( firstItem );
					EncodeString( field.Name );
					AppendColon();
					EncodeValue( field.GetValue( value ), shouldTypeHint );
					firstItem = false;
				}
			}

			var properties = type.GetProperties( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			foreach (var property in properties)
			{
				if (property.CanRead)
				{
					var shouldTypeHint = false;
					var shouldEncode = false;
					foreach (var attribute in property.GetCustomAttributes( true ))
					{
						if (includeAttrType.IsAssignableFrom( attribute.GetType() ))
						{
							shouldEncode = true;
						}

						if (typeHintAttrType.IsAssignableFrom( attribute.GetType() ))
						{
							shouldTypeHint = true;
						}
					}

					if (shouldEncode)
					{
						AppendComma( firstItem );
						EncodeString( property.Name );
						AppendColon();
						EncodeValue( property.GetValue( value, null ), shouldTypeHint );
						firstItem = false;
					}
				}
			}

			AppendCloseBrace();
		}

		void EncodeDictionary( IDictionary value, bool forceTypeHint )
		{
			if (value.Count == 0)
			{
				builder.Append( "{}" );
			}
			else
			{
				AppendOpenBrace();

				var firstItem = true;
				foreach (object e in value.Keys)
				{
					AppendComma( firstItem );
					EncodeString( e.ToString() );
					AppendColon();
					EncodeValue( value[e], forceTypeHint );
					firstItem = false;
				}

				AppendCloseBrace();
			}
		}

		void EncodeList( IList value, bool forceTypeHint )
		{
			if (value.Count == 0)
			{
				builder.Append( "[]" );
			}
			else
			{
				AppendOpenBracket();

				var firstItem = true;
				foreach (object obj in value)
				{
					AppendComma( firstItem );
					EncodeValue( obj, forceTypeHint );
					firstItem = false;
				}

				AppendCloseBracket();
			}
		}

		void EncodeArray( Array value, bool forceTypeHint )
		{
			if (value.Rank == 1)
			{
				EncodeList( value, forceTypeHint );
			}
			else
			{
				var indices = new int[value.Rank];
				EncodeArrayRank( value, 0, indices, forceTypeHint );
			}
		}

		void EncodeArrayRank( Array value, int rank, int[] indices, bool forceTypeHint )
		{
			AppendOpenBracket();

			var min = value.GetLowerBound( rank );
			var max = value.GetUpperBound( rank );

			if (rank == value.Rank - 1)
			{
				for (int i = min; i <= max; i++)
				{
					indices[rank] = i;
					AppendComma( i == min );
					EncodeValue( value.GetValue( indices ), forceTypeHint );
				}
			}
			else
			{
				for (int i = min; i <= max; i++)
				{
					indices[rank] = i;
					AppendComma( i == min );
					EncodeArrayRank( value, rank + 1, indices, forceTypeHint );
				}
			}

			AppendCloseBracket();
		}

		void EncodeString( string value )
		{
			builder.Append( '\"' );

			char[] charArray = value.ToCharArray();
			foreach (var c in charArray)
			{
				switch (c)
				{
					case '"':
						builder.Append( "\\\"" );
						break;

					case '\\':
						builder.Append( "\\\\" );
						break;

					case '\b':
						builder.Append( "\\b" );
						break;

					case '\f':
						builder.Append( "\\f" );
						break;

					case '\n':
						builder.Append( "\\n" );
						break;

					case '\r':
						builder.Append( "\\r" );
						break;

					case '\t':
						builder.Append( "\\t" );
						break;

					default:
						int codepoint = Convert.ToInt32( c );
						if ((codepoint >= 32) && (codepoint <= 126))
						{
							builder.Append( c );
						}
						else
						{
							builder.Append( "\\u" + Convert.ToString( codepoint, 16 ).PadLeft( 4, '0' ) );
						}
						break;
				}
			}

			builder.Append( '\"' );
		}

		void EncodeOther( object value, bool forceTypeHint )
		{
			if (value is float ||
			    value is double ||
			    value is int ||
			    value is uint ||
			    value is long ||
			    value is sbyte ||
			    value is byte ||
			    value is short ||
			    value is ushort ||
			    value is ulong ||
			    value is decimal)
			{
				builder.Append( value.ToString() );
			}
			else
			{
				EncodeObject( value, forceTypeHint );
			}
		}

		#region Helpers

		void AppendIndent()
		{
			for (int i = 0; i < indent; i++)
			{
				builder.Append( '\t' );
			}
		}

		void AppendOpenBrace()
		{
			builder.Append( '{' );

			if (PrettyPrintEnabled)
			{
				builder.Append( '\n' );
				indent++;
			}
		}

		void AppendCloseBrace()
		{
			if (PrettyPrintEnabled)
			{
				builder.Append( '\n' );
				indent--;
				AppendIndent();
			}

			builder.Append( '}' );
		}

		void AppendOpenBracket()
		{
			builder.Append( '[' );

			if (PrettyPrintEnabled)
			{
				builder.Append( '\n' );
				indent++;
			}
		}

		void AppendCloseBracket()
		{
			if (PrettyPrintEnabled)
			{
				builder.Append( '\n' );
				indent--;
				AppendIndent();
			}

			builder.Append( ']' );
		}

		void AppendComma( bool firstItem )
		{
			if (!firstItem)
			{
				builder.Append( ',' );

				if (PrettyPrintEnabled)
				{
					builder.Append( '\n' );
				}
			}

			if (PrettyPrintEnabled)
			{
				AppendIndent();
			}
		}

		void AppendColon()
		{
			builder.Append( ':' );

			if (PrettyPrintEnabled)
			{
				builder.Append( ' ' );
			}
		}

		#endregion
	}


	public static class Extensions
	{
		public static bool AnyOfType<TSource>( this IEnumerable<TSource> source, Type expectedType )
		{
			if (source == null)
			{
				throw new ArgumentNullException( "source" );
			}

			if (expectedType == null)
			{
				throw new ArgumentNullException( "expectedType" );
			}

			foreach (var item in source)
			{
				if (expectedType.IsAssignableFrom( item.GetType() ))
				{
					return true;
				}
			}

			return false;
		}
	}


	/// <summary>
	/// Mark members that should be included.
	/// Public fields are included by default.
	/// </summary>
	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public sealed class Include : Attribute
	{
	}

	/// <summary>
	/// Mark members that should be excluded.
	/// Private fields and all properties are excluded by default.
	/// </summary>
	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class Exclude : Attribute
	{
	}

	/// <summary>
	/// Mark methods to be called after an object is decoded.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method )]
	public class AfterDecode : Attribute
	{
	}

	[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
	public class TypeHint : Attribute
	{
	}

	[Obsolete( "Use the Exclude attribute instead." )]
	public sealed class Skip : Exclude
	{
	}

	[Obsolete( "Use the AfterDecode attribute instead." )]
	public sealed class Load : AfterDecode
	{
	}

	public sealed class DecodeException : Exception
	{
		public DecodeException( string message )
			: base( message )
		{
		}

		public DecodeException( string message, Exception innerException )
			: base( message, innerException )
		{
		}
	}

	public static class JSON
	{
		static readonly Type includeAttrType = typeof(Include);
		static readonly Type excludeAttrType = typeof(Exclude);

		public static Variant Load( string json )
		{
			if (json == null)
			{
				throw new ArgumentNullException( "json" );
			}

			return Decoder.Decode( json );
		}

		public static string Dump( object data, EncodeOptions options = EncodeOptions.None )
		{
			return Encoder.Encode( data, options );
		}

		public static void MakeInto<T>( Variant data, out T item )
		{
			item = DecodeType<T>( data );
		}

		private static Dictionary<string,Type> typeCache = new Dictionary<string,Type>();
		private static Type FindType( string fullName )
		{
			if (fullName == null)
			{
				return null;
			}

			Type type;
			if (typeCache.TryGetValue( fullName, out type ))
			{
				return type;
			}

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				type = assembly.GetType( fullName );
				if (type != null)
				{
					typeCache.Add( fullName, type );
					return type;
				}
			}

			return null;
		}

		private static T DecodeType<T>( Variant data )
		{
			if (data == null)
			{
				return default(T);
			}

			var type = typeof(T);

			if (type.IsEnum)
			{
				return (T) Enum.Parse( type, data.ToString() );
			}

			if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
			{
				return (T) Convert.ChangeType( data, type );
			}

			if (type.IsArray)
			{
				if (type.GetArrayRank() == 1)
				{
					var makeFunc = decodeArrayMethod.MakeGenericMethod( new Type[] { type.GetElementType() } );
					return (T) makeFunc.Invoke( null, new object[] { data } );
				}
				else
				{
					var arrayData = data as ProxyArray;
					var arrayRank = type.GetArrayRank();
					var rankLengths = new int[arrayRank];
					if (arrayData.CanBeMultiRankArray( rankLengths ))
					{
						var array = Array.CreateInstance( type.GetElementType(), rankLengths );
						var makeFunc = decodeMultiRankArrayMethod.MakeGenericMethod( new Type[] { type.GetElementType() } );
						try
						{
							makeFunc.Invoke( null, new object[] { arrayData, array, 1, rankLengths } );
						}
						catch (Exception e)
						{
							throw new DecodeException( "Error decoding multidimensional array. Did you try to decode into an array of incompatible rank or element type?", e );
						}
						return (T) Convert.ChangeType( array, typeof(T) );
					}
					throw new DecodeException( "Error decoding multidimensional array; JSON data doesn't seem fit this structure." );
					#pragma warning disable 0162
					return default(T);
					#pragma warning restore 0162
				}
			}

			if (typeof(IList).IsAssignableFrom( type ))
			{
				var makeFunc = decodeListMethod.MakeGenericMethod( type.GetGenericArguments() );
				return (T) makeFunc.Invoke( null, new object[] { data } );
			}

			if (typeof(IDictionary).IsAssignableFrom( type ))
			{
				var makeFunc = decodeDictionaryMethod.MakeGenericMethod( type.GetGenericArguments() );
				return (T) makeFunc.Invoke( null, new object[] { data } );
			}

			// At this point we should be dealing with a class or struct.
			T instance;
			var proxyObject = data as ProxyObject;
			if (proxyObject == null)
			{
				throw new InvalidCastException( "ProxyObject expected when decoding into '" + type.FullName + "'." );
			}

			// If there's a type hint, use it to create the instance.
			var typeHint = proxyObject.TypeHint;
			if (typeHint != null && typeHint != type.FullName)
			{
				var makeType = FindType( typeHint );
				if (makeType == null)
				{
					throw new TypeLoadException( "Could not load type '" + typeHint + "'." );
				}
				else
				{
					if (type.IsAssignableFrom( makeType ))
					{
						instance = (T) Activator.CreateInstance( makeType );
						type = makeType;
					}
					else
					{
						throw new InvalidCastException( "Cannot assign type '" + typeHint + "' to type '" + type.FullName + "'." );
					}
				}
			}
			else
			{
				// We don't have a type hint, so just instantiate the type we have.
				instance = Activator.CreateInstance<T>();
			}

			// Now decode fields and properties.
			foreach (var pair in data as ProxyObject)
			{
				var field = type.GetField( pair.Key, instanceBindingFlags );
				if (field != null)
				{
					var shouldDecode = field.IsPublic;
					foreach (var attribute in field.GetCustomAttributes( true ))
					{
						if (excludeAttrType.IsAssignableFrom( attribute.GetType() ))
						{
							shouldDecode = false;
						}

						if (includeAttrType.IsAssignableFrom( attribute.GetType() ))
						{
							shouldDecode = true;
						}
					}

					if (shouldDecode)
					{
						var makeFunc = decodeTypeMethod.MakeGenericMethod( new Type[] { field.FieldType } );
						if (type.IsValueType)
						{
							// Type is a struct.
							var instanceRef = (object) instance;
							field.SetValue( instanceRef, makeFunc.Invoke( null, new object[] { pair.Value } ) );
							instance = (T) instanceRef;
						}
						else
						{
							// Type is a class.
							field.SetValue( instance, makeFunc.Invoke( null, new object[] { pair.Value } ) );
						}
					}
				}

				var property = type.GetProperty( pair.Key, instanceBindingFlags );
				if (property != null)
				{
					if (property.CanWrite && property.GetCustomAttributes( false ).AnyOfType( includeAttrType ))
					{
						var makeFunc = decodeTypeMethod.MakeGenericMethod( new Type[] { property.PropertyType } );
						if (type.IsValueType)
						{
							// Type is a struct.
							var instanceRef = (object) instance;
							property.SetValue( instanceRef, makeFunc.Invoke( null, new object[] { pair.Value } ), null );
							instance = (T) instanceRef;
						}
						else
						{
							// Type is a class.
							property.SetValue( instance, makeFunc.Invoke( null, new object[] { pair.Value } ), null );
						}
					}
				}
			}

			// Invoke methods tagged with [AfterDecode] attribute.
			foreach (var method in type.GetMethods( instanceBindingFlags ))
			{
				if (method.GetCustomAttributes( false ).AnyOfType( typeof(AfterDecode) ))
				{
					if (method.GetParameters().Length == 0)
					{
						method.Invoke( instance, null );
					}
					else
					{
						method.Invoke( instance, new object[] { data } );
					}
				}
			}

			return instance;
		}

		private static List<T> DecodeList<T>( Variant data )
		{
			var list = new List<T>();

			foreach (var item in data as ProxyArray)
			{
				list.Add( DecodeType<T>( item ) );
			}

			return list;
		}

		private static Dictionary<K,V> DecodeDictionary<K,V>( Variant data )
		{
			var dict = new Dictionary<K,V>();

			foreach (var pair in data as ProxyObject)
			{
				var k = (K) Convert.ChangeType( pair.Key, typeof(K) );
				var v = DecodeType<V>( pair.Value );
				dict.Add( k, v );
			}

			return dict;
		}

		private static T[] DecodeArray<T>( Variant data )
		{
			var arrayData = data as ProxyArray;
			var arraySize = arrayData.Count;
			var array = new T[arraySize];

			int i = 0;
			foreach (var item in data as ProxyArray)
			{
				array[i++] = DecodeType<T>( item );
			}

			return array;
		}

		private static void DecodeMultiRankArray<T>( ProxyArray arrayData, Array array, int arrayRank, int[] indices )
		{
			var count = arrayData.Count;
			for (int i = 0; i < count; i++)
			{
				indices[arrayRank - 1] = i;
				if (arrayRank < array.Rank)
				{
					DecodeMultiRankArray<T>( arrayData[i] as ProxyArray, array, arrayRank + 1, indices );
				}
				else
				{
					array.SetValue( DecodeType<T>( arrayData[i] ), indices );
				}
			}
		}

		private static BindingFlags instanceBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		private static BindingFlags staticBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
		private static MethodInfo decodeTypeMethod = typeof(JSON).GetMethod( "DecodeType", staticBindingFlags );
		private static MethodInfo decodeListMethod = typeof(JSON).GetMethod( "DecodeList", staticBindingFlags );
		private static MethodInfo decodeDictionaryMethod = typeof(JSON).GetMethod( "DecodeDictionary", staticBindingFlags );
		private static MethodInfo decodeArrayMethod = typeof(JSON).GetMethod( "DecodeArray", staticBindingFlags );
		private static MethodInfo decodeMultiRankArrayMethod = typeof(JSON).GetMethod( "DecodeMultiRankArray", staticBindingFlags );

		private static void SupportTypeForAOT<T>()
		{
			DecodeType<T>( null );
			DecodeList<T>( null );
			DecodeArray<T>( null );
			DecodeDictionary<Int16,T>( null );
			DecodeDictionary<UInt16,T>( null );
			DecodeDictionary<Int32,T>( null );
			DecodeDictionary<UInt32,T>( null );
			DecodeDictionary<Int64,T>( null );
			DecodeDictionary<UInt64,T>( null );
			DecodeDictionary<Single,T>( null );
			DecodeDictionary<Double,T>( null );
			DecodeDictionary<Decimal,T>( null );
			DecodeDictionary<Boolean,T>( null );
			DecodeDictionary<String,T>( null );
		}

		private static void SupportValueTypesForAOT()
		{
			SupportTypeForAOT<Int16>();
			SupportTypeForAOT<UInt16>();
			SupportTypeForAOT<Int32>();
			SupportTypeForAOT<UInt32>();
			SupportTypeForAOT<Int64>();
			SupportTypeForAOT<UInt64>();
			SupportTypeForAOT<Single>();
			SupportTypeForAOT<Double>();
			SupportTypeForAOT<Decimal>();
			SupportTypeForAOT<Boolean>();
			SupportTypeForAOT<String>();
		}
	}


	public sealed class ProxyArray : Variant, IEnumerable<Variant>
	{
		private List<Variant> list;

		public ProxyArray()
		{
			list = new List<Variant>();
		}

		IEnumerator<Variant> IEnumerable<Variant>.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public void Add( Variant item )
		{
			list.Add( item );
		}

		public override Variant this[ int index ]
		{
			get { return list[index]; }
			set { list[index] = value; }
		}

		public int Count
		{
			get { return list.Count; }
		}

		internal bool CanBeMultiRankArray( int[] rankLengths )
		{
			return CanBeMultiRankArray( 0, rankLengths );
		}

		private bool CanBeMultiRankArray( int rank, int[] rankLengths )
		{
			var count = list.Count;
			rankLengths[rank] = count;

			if (rank == rankLengths.Length - 1)
			{
				return true;
			}

			var firstItem = list[0] as ProxyArray;
			if (firstItem == null)
			{
				return false;
			}
			var firstItemCount = firstItem.Count;

			for (int i = 1; i < count; i++)
			{
				var item = list[i] as ProxyArray;

				if (item == null)
				{
					return false;
				}

				if (item.Count != firstItemCount)
				{
					return false;
				}

				if (!item.CanBeMultiRankArray( rank + 1, rankLengths ))
				{
					return false;
				}
			}

			return true;
		}
	}


	public sealed class ProxyBoolean : Variant
	{
		private bool value;

		public ProxyBoolean( bool value )
		{
			this.value = value;
		}

		public override bool ToBoolean( IFormatProvider provider )
		{
			return value;
		}
	}


	public sealed class ProxyNumber : Variant
	{
		private static readonly char[] floatingPointCharacters = new char[] { '.', 'e' };
		private IConvertible value;

		public ProxyNumber( IConvertible value )
		{
			if (value is string)
			{
				this.value = Parse( value as string );
			}
			else
			{
				this.value = value;
			}
		}

		private IConvertible Parse( string value )
		{
			if (value.IndexOfAny( floatingPointCharacters ) == -1)
			{
				if (value[0] == '-')
				{
					Int64 parsedValue;
					if (Int64.TryParse( value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out parsedValue ))
					{
						return parsedValue;
					}
				}
				else
				{
					UInt64 parsedValue;
					if (UInt64.TryParse( value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out parsedValue ))
					{
						return parsedValue;
					}
				}
			}

			Decimal decimalValue;
			if (Decimal.TryParse( value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out decimalValue ))
			{
				// Check for decimal underflow.
				if (decimalValue == Decimal.Zero)
				{
					Double parsedValue;
					if (Double.TryParse( value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out parsedValue ))
					{
						if (parsedValue != 0.0)
						{
							return parsedValue;
						}
					}
				}
				return decimalValue;
			}
			else
			{
				Double parsedValue;
				if (Double.TryParse( value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out parsedValue ))
				{
					return parsedValue;
				}
			}

			return 0;
		}

		public override bool ToBoolean( IFormatProvider provider )
		{
			return value.ToBoolean( provider );
		}

		public override byte ToByte( IFormatProvider provider )
		{
			return value.ToByte( provider );
		}

		public override char ToChar( IFormatProvider provider )
		{
			return value.ToChar( provider );
		}

		public override decimal ToDecimal( IFormatProvider provider )
		{
			return value.ToDecimal( provider );
		}

		public override double ToDouble( IFormatProvider provider )
		{
			return value.ToDouble( provider );
		}

		public override short ToInt16( IFormatProvider provider )
		{
			return value.ToInt16( provider );
		}

		public override int ToInt32( IFormatProvider provider )
		{
			return value.ToInt32( provider );
		}

		public override long ToInt64( IFormatProvider provider )
		{
			return value.ToInt64( provider );
		}

		public override sbyte ToSByte( IFormatProvider provider )
		{
			return value.ToSByte( provider );
		}

		public override float ToSingle( IFormatProvider provider )
		{
			return value.ToSingle( provider );
		}

		public override string ToString( IFormatProvider provider )
		{
			return value.ToString( provider );
		}

		public override ushort ToUInt16( IFormatProvider provider )
		{
			return value.ToUInt16( provider );
		}

		public override uint ToUInt32( IFormatProvider provider )
		{
			return value.ToUInt32( provider );
		}

		public override ulong ToUInt64( IFormatProvider provider )
		{
			return value.ToUInt64( provider );
		}
	}


	public sealed class ProxyObject : Variant, IEnumerable<KeyValuePair<string, Variant>>
	{
		internal const string TypeHintName = "@type";
		private Dictionary<string, Variant> dict;

		public ProxyObject()
		{
			dict = new Dictionary<string, Variant>();
		}

		IEnumerator<KeyValuePair<string, Variant>> IEnumerable<KeyValuePair<string, Variant>>.GetEnumerator()
		{
			return dict.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return dict.GetEnumerator();
		}

		public void Add( string key, Variant item )
		{
			dict.Add( key, item );
		}

		public bool TryGetValue( string key, out Variant item )
		{
			return dict.TryGetValue( key, out item );
		}

		public string TypeHint
		{
			get
			{
				Variant item;
				if (TryGetValue( TypeHintName, out item ))
				{
					return item.ToString();
				}
				return null;
			}
		}

		public override Variant this[ string key ]
		{
			get { return dict[key]; }
			set { dict[key] = value; }
		}

		public int Count
		{
			get { return dict.Count; }
		}
	}


	public sealed class ProxyString : Variant
	{
		private string value;

		public ProxyString( string value )
		{
			this.value = value;
		}

		public override string ToString( IFormatProvider provider )
		{
			return value;
		}
	}


	public abstract class Variant : IConvertible
	{
		protected static IFormatProvider formatProvider = new NumberFormatInfo();

		public void Make<T>( out T item )
		{
			JSON.MakeInto<T>( this, out item );
		}

		public T Make<T>()
		{
			T item;
			JSON.MakeInto<T>( this, out item );
			return item;
		}

		public virtual TypeCode GetTypeCode()
		{
			return TypeCode.Object;
		}

		public virtual object ToType( Type conversionType, IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to " + conversionType.Name );
		}

		public virtual DateTime ToDateTime( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to DateTime" );
		}

		public virtual bool ToBoolean( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to Boolean" );
		}

		public virtual byte ToByte( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to Byte" );
		}

		public virtual char ToChar( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to Char" );
		}

		public virtual decimal ToDecimal( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to Decimal" );
		}

		public virtual double ToDouble( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to Double" );
		}

		public virtual short ToInt16( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to Int16" );
		}

		public virtual int ToInt32( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to Int32" );
		}

		public virtual long ToInt64( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to Int64" );
		}

		public virtual sbyte ToSByte( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to SByte" );
		}

		public virtual float ToSingle( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to Single" );
		}

		public virtual string ToString( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to String" );
		}

		public virtual ushort ToUInt16( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to UInt16" );
		}

		public virtual uint ToUInt32( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to UInt32" );
		}

		public virtual ulong ToUInt64( IFormatProvider provider )
		{
			throw new InvalidCastException( "Cannot convert " + this.GetType() + " to UInt64" );
		}

		public override string ToString()
		{
			return ToString( formatProvider );
		}

		public virtual Variant this[ string key ]
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		public virtual Variant this[ int index ]
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		public static implicit operator Boolean( Variant variant )
		{
			return variant.ToBoolean( formatProvider );
		}

		public static implicit operator Single( Variant variant )
		{
			return variant.ToSingle( formatProvider );
		}

		public static implicit operator Double( Variant variant )
		{
			return variant.ToDouble( formatProvider );
		}

		public static implicit operator UInt16( Variant variant )
		{
			return variant.ToUInt16( formatProvider );
		}

		public static implicit operator Int16( Variant variant )
		{
			return variant.ToInt16( formatProvider );
		}

		public static implicit operator UInt32( Variant variant )
		{
			return variant.ToUInt32( formatProvider );
		}

		public static implicit operator Int32( Variant variant )
		{
			return variant.ToInt32( formatProvider );
		}

		public static implicit operator UInt64( Variant variant )
		{
			return variant.ToUInt64( formatProvider );
		}

		public static implicit operator Int64( Variant variant )
		{
			return variant.ToInt64( formatProvider );
		}

		public static implicit operator Decimal( Variant variant )
		{
			return variant.ToDecimal( formatProvider );
		}

		public static implicit operator String( Variant variant )
		{
			return variant.ToString( formatProvider );
		}
	}
	// @endcond
}

