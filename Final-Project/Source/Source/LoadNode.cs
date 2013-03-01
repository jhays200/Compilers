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

    string CodeGenCalvin(CodeGenerator cg);
    IAstNode Left { get; }
    IAstNode Right { get; }
}
