using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Singleton<T> where T : new()
{
	internal static T _instance = new T();

	public static T Instance 
	{
		get {
			if( null == _instance ) _instance = new T();
			return _instance;
		}
	}

	public static void PurgeInstance()
	{
		//_instance = null;
		_instance = new T();
	}
}

