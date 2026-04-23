using UnityEngine;
using System.IO;
using System.Data;
using System.Collections.Generic;

public class LoadText 
{
    private static LoadText instance;

    public static LoadText Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LoadText();
            }
            return instance;
        }
    }

    public int xSize;
    public int ySize;
    public int zSize;




    // Start is called before the first frame update
}
