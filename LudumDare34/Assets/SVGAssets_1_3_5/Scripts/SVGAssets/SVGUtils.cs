/****************************************************************************
** Copyright (C) 2013-2015 Mazatech S.r.l. All rights reserved.
**
** This file is part of SVGAssets software, an SVG rendering engine.
**
** W3C (World Wide Web Consortium) and SVG are trademarks of the W3C.
** OpenGL is a registered trademark and OpenGL ES is a trademark of
** Silicon Graphics, Inc.
**
** This file is provided AS IS with NO WARRANTY OF ANY KIND, INCLUDING THE
** WARRANTY OF DESIGN, MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
**
** For any information, please contact info@mazatech.com
**
****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pair<F, S>
{
	public Pair(F first, S second)
	{
		this.m_First = first;
		this.m_Second = second;
	}
	
	public F First
	{
		get
		{
			return this.m_First;
		}
	}
	
	public S Second
	{
		get
		{
			return this.m_Second;
		}
	}
	
	public override bool Equals(object obj)
	{
		if (obj == null)
			return false;
		if (obj == this)
			return true;

		Pair<F, S> other = obj as Pair<F, S>;
		if (other == null)
			return false;
		
		return (((this.First == null)  && (other.First == null))  || ((this.First != null)  && First.Equals(other.First))) &&
			   (((this.Second == null) && (other.Second == null)) || ((this.Second != null) && Second.Equals(other.Second)));
	}
	
	public override int GetHashCode()
	{
		int hashcode = 0;

		if (this.First != null)
			hashcode += this.First.GetHashCode();
		if (this.Second != null)
			hashcode += this.Second.GetHashCode();
		
		return hashcode;
	}

	private F m_First;
	private S m_Second;
}

// A serializable dictionary template
[System.Serializable]
public class SerializableDictionary<K, V> : IEnumerable<KeyValuePair<K, V>>
{
	public V this[K key]
	{
		get
		{
			if (!m_DictionaryRestored)
				RestoreDictionary();
			return m_ValuesList[m_Dictionary[key]];
		}
		set
		{
			if (!m_DictionaryRestored)
				RestoreDictionary();
			
			int index;
			if (m_Dictionary.TryGetValue(key, out index))
				m_ValuesList[index] = value;
			else
				Add(key, value);
		}
	}
	
	public void Add(K key, V value)
	{
		m_Dictionary.Add(key, m_ValuesList.Count);
		m_KeysList.Add(key);
		m_ValuesList.Add(value);
	}
	
	public int Count
	{
		get
		{
			return m_ValuesList.Count;
		}
	}
	
#region IEnumerable<KeyValuePair<K,V>> Members
	public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
	{
		return new Enumerator(this);
	}
	
	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(this);
	}
#endregion

	public V Get(K key, V default_value)
	{
		if (!m_DictionaryRestored)
			RestoreDictionary();
		
		int index;
		if (m_Dictionary.TryGetValue(key, out index))
			return m_ValuesList[index];
		else
			return default_value;
	}
	
	public bool TryGetValue(K key, out V value)
	{
		if (!m_DictionaryRestored)
			RestoreDictionary();
		
		int index;
		if (m_Dictionary.TryGetValue(key, out index))
		{
			value = m_ValuesList[index];
			return true;
		}
		else {
			value = default(V);
			return false;
		}
	}
	
	public bool Remove(K key)
	{
		if (!m_DictionaryRestored)
			RestoreDictionary();
		
		int index;
		if (m_Dictionary.TryGetValue(key, out index))
		{
			RemoveAt(index);
			return true;
		}
		return false;
	}
	
	public void RemoveAt(int index)
	{
		if (!m_DictionaryRestored)
			RestoreDictionary();
		
		K key = m_KeysList[index];
		m_Dictionary.Remove(key);
		m_KeysList.RemoveAt(index);
		m_ValuesList.RemoveAt(index);
		
		for (int k = index; k < m_KeysList.Count; ++k)
			--m_Dictionary[m_KeysList[k]];
	}
	
	public KeyValuePair<K, V> GetAt(int index)
	{
		return new KeyValuePair<K, V>(m_KeysList[index], m_ValuesList[index]);
	}
	
	public V GetValueAt(int index)
	{
		return m_ValuesList[index];
	}
	
	public bool ContainsKey(K key)
	{
		if (!m_DictionaryRestored)
			RestoreDictionary();
		return m_Dictionary.ContainsKey(key);
	}
	
	public void Clear()
	{
		m_Dictionary.Clear();
		m_KeysList.Clear();
		m_ValuesList.Clear();
	}
	
	public List<V> Values()
	{
		return(new List<V>(this.m_ValuesList));
	}
	
	private void RestoreDictionary()
	{
		for (int i = 0 ; i < m_KeysList.Count; ++i)
			m_Dictionary[m_KeysList[i]] = i;
		m_DictionaryRestored = true;
	}
	
	private Dictionary<K, int> m_Dictionary = new Dictionary<K, int>();
	[SerializeField]
	private List<K> m_KeysList = new List<K>();
	[SerializeField]
	private List<V> m_ValuesList = new List<V>();
	[NonSerialized]
	private bool m_DictionaryRestored = false;
	
#region Nested type: Enumerator
	private class Enumerator : IEnumerator<KeyValuePair<K, V>>
	{
		public Enumerator(SerializableDictionary<K, V> dictionary)
		{
			Dictionary = dictionary;
		}
		
	#region IEnumerator<KeyValuePair<K,V>> Members
		public KeyValuePair<K, V> Current
		{
			get
			{
				return Dictionary.GetAt(current);
			}
		}
		
		public void Dispose()
		{
		}
		
		object IEnumerator.Current
		{
			get
			{
				return Dictionary.GetAt(current);
			}
		}
		
		public bool MoveNext()
		{
			++current;
			return current < Dictionary.Count;
		}
		
		public void Reset()
		{
			current = -1;
		}
	#endregion

		private readonly SerializableDictionary<K, V> Dictionary;
		private int current = -1;
	}
#endregion
}

static public class SVGUtils
{
	static public Vector2 GetGameView()
	{
	#if UNITY_EDITOR
		System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		System.Reflection.MethodInfo getSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		System.Object resolution = getSizeOfMainGameView.Invoke(null, null);
		return (Vector2)resolution;
	#else
		return Vector2.zero;
	#endif
	}
}
