using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tab : MonoBehaviour
{
    public InputField emailInput;
    public InputField passInput;
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void inputfieldTab()
    {
        if (emailInput.isFocused == true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                passInput.Select();
            }
        }
    }
}
