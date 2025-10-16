using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

	public  static  T instance
	{
		get
		{
			return  _instance;
		}
	}

	public  virtual  void  Awake ()
	{
		Debug.Log("Singleton Awake called");

		if (_instance  ==  null) {
			_instance  =  this  as T;
			DontDestroyOnLoad (this.gameObject);
		} else {
			Destroy (gameObject);
		}
	}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
