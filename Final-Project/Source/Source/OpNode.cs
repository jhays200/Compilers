using System;


class OpNode: IAstNode
{
    private IAstNode _left;
    private IAstNode _right;
    private StateSymbol op;
    const string TMP_LOC = 

    public OpNode(IAstNode left, StateSymbol op, IAstNode right)
    {
        _left = left;
        this.op = op;
        _right = right;
    }



    public void CodeGenCalvin(CodeGenerator cg)
    {
        leftAddr = Left.CodeGenCalvin(cg);
        cg.WriteLine("store left");
        rightAddr = Right.CodeGenCalvin(cg);

        cg.WriteLine("add right");
    }

    IAstNode Left
    {
        get
        {
            return _left;
        }
    }
    IAstNode Right
    {
        get
        {
            return _right;
        }
    }
}
