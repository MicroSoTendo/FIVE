using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FIVE.AWSL;

public class AWSLTester : MonoBehaviour
{
    public string Code;

    private RuntimeContext rc;

    // Start is called before the first frame update
    private void Start()
    {
        rc = new Parser(Code).Parse();
    }
}