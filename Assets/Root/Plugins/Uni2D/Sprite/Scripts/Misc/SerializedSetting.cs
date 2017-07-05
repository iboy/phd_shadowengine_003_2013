#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

public class SerializedSetting<T>
{
	private T m_rValue        = default( T );
	private bool m_bIsDefined = false;

	private SerializedProperty m_rSerializedProperty = null;

	public bool IsDefined
	{
		get
		{
			return m_bIsDefined;
		}
	}

	public bool HasMultipleDifferentValues
	{
		get
		{
			return m_bIsDefined == false && m_rSerializedProperty != null && m_rSerializedProperty.hasMultipleDifferentValues;
		}
	}

	public T Value
	{
		get
		{
			return m_bIsDefined ? m_rValue : this.GetSerializedValue( );
		}
		set
		{
			m_rValue     = value;
			m_bIsDefined = true;
		}
	}

	public SerializedSetting( SerializedObject a_rSerializedObject, string a_rPropertyPath )
	{
		m_rSerializedProperty = a_rSerializedObject.FindProperty( a_rPropertyPath );
		
		if( m_rSerializedProperty != null )
		{
			m_rValue = this.GetSerializedValue( );
		}
		else
		{
			this.NoSerializedPropertyAtGivenPathWarning( a_rPropertyPath );
		}
	}

	public void Apply( )
	{
		if( m_bIsDefined && m_rSerializedProperty != null )
		{
			m_bIsDefined = false;
			switch( m_rSerializedProperty.propertyType )
			{
				case SerializedPropertyType.Boolean:
				{
					m_rSerializedProperty.boolValue = (bool) System.Convert.ChangeType( m_rValue, typeof( bool ) );
				}
				break;
				case SerializedPropertyType.ArraySize:
				case SerializedPropertyType.Integer:
				{
					m_rSerializedProperty.intValue = (int) System.Convert.ChangeType( m_rValue, typeof( int ) );
				}
				break;
				case SerializedPropertyType.Float:
				{
					m_rSerializedProperty.floatValue = (float) System.Convert.ChangeType( m_rValue, typeof( float ) );
				}
				break;
				case SerializedPropertyType.String:
				{
					m_rSerializedProperty.stringValue = (string) System.Convert.ChangeType( m_rValue, typeof( string ) );
				}
				break;
				case SerializedPropertyType.Enum:
				{
					//m_rSerializedProperty.enumValueIndex = (int) System.Convert.ChangeType( m_rValue, typeof( int ) );
					m_rSerializedProperty.intValue = (int) System.Convert.ChangeType( m_rValue, typeof( int ) );
				}
				break;

				case SerializedPropertyType.Generic:
				case SerializedPropertyType.LayerMask:
				case SerializedPropertyType.ObjectReference:
				{
					//m_rSerializedProperty.objectReferenceValue = (Object) System.Convert.ChangeType( m_rValue, typeof( Object ) );
					m_rSerializedProperty.objectReferenceValue = m_rValue as Object;
				}
				break;
				case SerializedPropertyType.Vector2:
				{
					m_rSerializedProperty.vector2Value = (Vector2) System.Convert.ChangeType( m_rValue, typeof( Vector2 ) );
				}
				break;
				case SerializedPropertyType.Vector3:
				{
					m_rSerializedProperty.vector3Value = (Vector3) System.Convert.ChangeType( m_rValue, typeof( Vector3 ) );
				}
				break;
				case SerializedPropertyType.Color:
				{
					m_rSerializedProperty.colorValue = (Color) System.Convert.ChangeType( m_rValue, typeof( Color ) );
				}
				break;
				case SerializedPropertyType.Rect:
				{
					m_rSerializedProperty.rectValue = (Rect) System.Convert.ChangeType( m_rValue, typeof( Rect ) );
				}
				break;
				case SerializedPropertyType.Bounds:
				{
					m_rSerializedProperty.boundsValue = (Bounds) System.Convert.ChangeType( m_rValue, typeof( Bounds ) );
				}
				break;
				case SerializedPropertyType.AnimationCurve:
				{
					m_rSerializedProperty.animationCurveValue = (AnimationCurve) System.Convert.ChangeType( m_rValue, typeof( AnimationCurve ) );
				}
				break;
				default:
				{
					m_bIsDefined = true;
					this.UnsupportedSerializedPropertyTypeError( );
				}
				break;
			}
		}
		else if( m_rSerializedProperty == null )
		{
			this.NoSerializedPropertyAtGivenPathWarning( );
		}
	}

	public void Revert( )
	{
		m_bIsDefined = false;

		m_rValue = GetSerializedValue( );
	}


	private T GetSerializedValue( )
	{
		if( m_rSerializedProperty == null || m_rSerializedProperty.hasMultipleDifferentValues )
		{
			return default( T );
		}
		else
		{
			switch( m_rSerializedProperty.propertyType )
			{
				case SerializedPropertyType.Boolean:
				{
					return (T) System.Convert.ChangeType( m_rSerializedProperty.boolValue, typeof( T ) );
				}
				//break;
				case SerializedPropertyType.ArraySize:
				case SerializedPropertyType.Integer:
				{
					return (T) System.Convert.ChangeType( m_rSerializedProperty.intValue, typeof( T ) );
				}
				//break;
				case SerializedPropertyType.Float:
				{
					return (T) System.Convert.ChangeType( m_rSerializedProperty.floatValue, typeof( T ) );
				}
				//break;
				case SerializedPropertyType.String:
				{
					return (T) System.Convert.ChangeType( m_rSerializedProperty.stringValue, typeof( T ) );
				}
				//break;
				case SerializedPropertyType.Enum:
				{
					return (T) System.Enum.ToObject( typeof( T ), m_rSerializedProperty.intValue );
					//return (T) System.Enum.ToObject( typeof( T ), m_rSerializedProperty.enumValueIndex );
					//return (T) System.Convert.ChangeType( m_rSerializedProperty.enumValueIndex, typeof( T ) );
				}
				//break;
				case SerializedPropertyType.LayerMask:
				case SerializedPropertyType.ObjectReference:
				case SerializedPropertyType.Generic:
				{
					return (T) System.Convert.ChangeType( m_rSerializedProperty.objectReferenceValue, typeof( T ) );
				}
				//break;
				case SerializedPropertyType.Vector2:
				{
					return (T) System.Convert.ChangeType( m_rSerializedProperty.vector2Value, typeof( T ) );
				}
				//break;
				case SerializedPropertyType.Vector3:
				{
					return (T) System.Convert.ChangeType( m_rSerializedProperty.vector3Value, typeof( T ) );
				}
				//break;
				case SerializedPropertyType.Color:
				{
					return (T) System.Convert.ChangeType( m_rSerializedProperty.colorValue, typeof( T ) );
				}
				//break;
				case SerializedPropertyType.Rect:
				{
					return (T) System.Convert.ChangeType( m_rSerializedProperty.rectValue, typeof( T ) );
				}
				//break;
				case SerializedPropertyType.Bounds:
				{
					return (T) System.Convert.ChangeType( m_rSerializedProperty.boundsValue, typeof( T ) );
				}
				//break;
				case SerializedPropertyType.AnimationCurve:
				{
					return (T) System.Convert.ChangeType( m_rSerializedProperty.animationCurveValue, typeof( T ) );
				}
				//break;
				default:
				{
					this.UnsupportedSerializedPropertyTypeError( );
					return default( T );
				}
				//break;
			}
		}
	}

	private void UnsupportedSerializedPropertyTypeError( )
	{
		Debug.LogError( "Unsupported/undocumented serialized property of type \"" + m_rSerializedProperty.type + "\" ("+ m_rSerializedProperty.propertyType.ToString( ) + ")" );
	}

	private void NoSerializedPropertyAtGivenPathWarning( string a_rPropertyPath = null )
	{
		Debug.LogWarning( "No serialized property at given path" + ( a_rPropertyPath != null ? ( " \"" + a_rPropertyPath + "\"" ) : null ) );
	}
}

#endif