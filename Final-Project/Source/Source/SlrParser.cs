

using System;
using System.Collections.Generic;
using System.IO;

class SlrParser
{
    private delegate void SlrFunction(int state);
    private Dictionary<Tuple<int, StateSymbol>, Tuple<SlrFunction, int> > srTable;
    Tokenizer t;
    CodeGenerator cg;

    public SlrParser()
    {
    }

    private void Accept(int state)
    {

    }

    public void Compile(File inputFile, File outputFile)
    {
    }

    private void Reduce(int state)
    {
        switch (state)
        {
            //PROG -> BEGIN <STL> END
            case 0:
                break;
            //STL -> ST
            case 1:
                break;
            case 2:

        }
    }

    private void Shift(int state)
    {
    }

    public static int Main()
    {
        return 0;
    }


}
