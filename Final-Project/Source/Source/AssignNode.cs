using System;

class AssignNode: IAstNode
{
    string id;
    IAstNode node;

    public AssignNode(string idName, IAstNode opNode)
    {
        id = idName;
        node = opNode;
    }

    public void CodeGenCalvin(CodeGenerator cg)
    {
        node.CodeGenCalvin(cg);

        cg.WriteLine("STORE " + cg.GetIdAddrStr(id));
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
