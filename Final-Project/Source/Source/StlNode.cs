using System;


class StlNode:IAstNode
{
    IAstNode _left;
    IAstNode _right;

    public StlNode(IAstNode left, IAstNode right)
    {
        _left = left;
        _right = right;
    }

    public void CodeGenCalvin(CodeGenerator cg)
    {
        Left.CodeGenCalvin(cg);

        if (Right != null)
            Right.CodeGenCalvin(cg);
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
