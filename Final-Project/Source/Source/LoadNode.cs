using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class LoadNode: IAstNode
{
    private StateSymbol _token;

    public LoadNode(StateSymbol token)
    {
        _token = token;
    }

    public void CodeGenCalvin(CodeGenerator cg)
    {
        //make an entry into the symbol/constant table
        //load address of value
        string address;

        if (_token.type == StateSymbol.Type.Id)
        {
            address = cg.GetIdAddrStr(_token.value);
        }
        else
        {
            address = cg.GetConstAddrStr(_token.value);
        }

        cg.WriteLine("LOAD " + address);
    }

    public IAstNode Left
    {
        get
        {
            return null;
        }
    }

    public IAstNode Right 
    { 
        get
        {
            return null;
        } 
    }
}
