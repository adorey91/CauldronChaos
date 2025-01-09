using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class example : MonoBehaviour
{
    //called uasing
    //example.instance.TestDebug();

    private static example _instance;
    public static example instance
    {
        get { 
            if (_instance == null)
            {
                _instance = Instantiate(Resources.Load("GameManager") as GameObject).GetComponent<example>();
                _instance.name = "GameManager";
            }
            return _instance; 
        }
    }

    public void Awake() {
        if (!_instance || _instance == this)
        {
            _instance = this;
        } else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }

    public void TestDebug()
    {
        Debug.Log("Boop!");
    }
}
