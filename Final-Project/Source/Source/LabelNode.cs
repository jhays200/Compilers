using System;

class LabelNode: IAstNode
{
    IAstNode _left;
    IAstNode _right;

    public LabelNode(IAstNode left, IAstNode right)
    {
        _left = left;
        _right = right;
    }

    public void CodeGenCalvin(CodeGenerator cg)
    {
        Left.CodeGenCalvin(cg);

        if (Right != null)
            Right.CodeGenCalvin(cg);
        else
            cg.WriteLine("HALT");
    }

    public IAstNode Left 
    {
        get
        {
            return _left;
        }
    }
    public IAstNode Right
    {
        get
        {
            return _right;
        }
    }
}
